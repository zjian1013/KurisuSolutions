#region Copyright © 2015 Kurisu Solutions
// All rights are reserved. Transmission or reproduction in part or whole,
// any form or by any means, mechanical, electronical or otherwise, is prohibited
// without the prior written consent of the copyright owner.
// 
// Document:	activator/gametroyhandler.cs
// Date:		01/07/2015
// Author:		Robin Kurisu
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

namespace Activator
{
    public class gametroyhandler
    {
        public static void init()
        {
            GameObject.OnCreate += GameObject_OnCreate;
            GameObject.OnDelete += GameObject_OnDelete;
        }

        public static Dictionary<int, Obj_AI_Base> objectcache = new Dictionary<int, Obj_AI_Base>(); 

        private static void GameObject_OnCreate(GameObject obj, EventArgs args)
        {
            if (obj.IsValid<Obj_AI_Base>())
            {
                var unit = obj as Obj_AI_Base;

                if (!objectcache.ContainsKey(unit.NetworkId))
                     objectcache.Add(unit.NetworkId, unit);
            }

            if (!Activator.TroysInGame)
                return;

            foreach (var troy in gametroy.Troys)
            {
                // include the troy and start ticking 
                if (!troy.Included && obj.Name.Contains(troy.Name))
                {
                    troy.Included = true;
                    troy.Obj = obj;
                    troy.Start = Utils.GameTimeTickCount;
                    Game.OnUpdate += Game_OnUpdate;
                    Console.WriteLine("[A]: " + troy.Name + "object created.");
                }
            }
        }

        private static void GameObject_OnDelete(GameObject obj, EventArgs args)
        {
            if (obj.IsValid<Obj_AI_Base>())
            {
                var unit = obj as Obj_AI_Base;
                objectcache.Remove(unit.NetworkId);
            }

            foreach (var hero in Activator.ChampionPriority())
            {
                if (!Activator.TroysInGame)
                    return;

                foreach (var troy in gametroy.Troys)
                {
                    // delete the troy and stop ticking
                    if (troy.Included && obj.Name.Contains(troy.Name))
                    {
                        troy.Included = false;
                        troy.Start = 0;
                        hero.Attacker = null;
                        hero.IncomeDamage = 0f;
                        hero.HitTypes.Clear();
                        Game.OnUpdate -= Game_OnUpdate;
                        Console.WriteLine("[A]: " + troy.Name + "object deleted.");
                    }
                }
            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            foreach (var hero in Activator.ChampionPriority())
            {
                if (!Activator.TroysInGame)
                    return;

                foreach (var troy in gametroy.Troys)
                {
                    // check if troy is included and is enemy
                    if (!troy.Included || !troy.Owner.IsEnemy)
                        continue;

                    // detect danger/cc/ultimates from our db
                    foreach (var item in gametroydata.troydata)
                    {
                        var radius = troy.Obj.BoundingRadius == null ? item.Radius : troy.Obj.BoundingRadius; 
                        if (troy.Obj.IsValid && hero.Player.Distance(troy.Obj.Position) <= radius)
                        {
                            if (troy.Name != item.Name || 
                                Utils.GameTimeTickCount - troy.Start < item.DelayFromStart)
                                continue;

                            hero.Attacker = troy.Owner;
                            hero.IncomeDamage = (float) troy.Owner.GetSpellDamage(hero.Player, troy.Slot);

                            // spell is important or lethal
                            if (item.HitType.Contains(HitType.Ultimate))
                                hero.HitTypes.Add(HitType.Ultimate);

                            // spell is important but not as fatal
                            if (item.HitType.Contains(HitType.Danger))
                                hero.HitTypes.Add(HitType.Danger);

                            // spell has a crowd control effect
                            if (item.HitType.Contains(HitType.CrowdControl))
                                hero.HitTypes.Add(HitType.CrowdControl);
                        }
                    }
                }
            }
        }
    }
}