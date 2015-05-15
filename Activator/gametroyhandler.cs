using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

namespace Activator
{
    public class gametroyhandler
    {
        public static void Load()
        {
            GameObject.OnCreate += GameObject_OnCreate;
            GameObject.OnDelete += GameObject_OnDelete;
        }

        private static void GameObject_OnCreate(GameObject obj, EventArgs args)
        {
            if (!Activator.TroysInGame)
                return;

            foreach (var troy in gametroy.Troys)
            {
                if (troy.Included)
                    return;

                if (obj.Name.Contains(troy.Name))
                {
                    troy.Included = true;
                    troy.Obj = obj;
                    troy.Start = Environment.TickCount;
                    Game.OnUpdate += Game_OnUpdate;
                }
            }
        }

        private static void GameObject_OnDelete(GameObject obj, EventArgs args)
        {
            foreach (var hero in champion.Heroes)
            {
                if (!Activator.TroysInGame)
                    return;

                foreach (var troy in gametroy.Troys)
                {
                    if (!troy.Included)
                        return;

                    if (obj.Name.Contains(troy.Name))
                    {
                        troy.Included = false;
                        troy.Start = 0;
                        hero.Attacker = null;
                        hero.IncomeDamage = 0f;
                        hero.HitTypes.Clear();
                        Game.OnUpdate -= Game_OnUpdate;
                    }
                }
            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            foreach (var hero in champion.Heroes)
            {
                if (!Activator.TroysInGame)
                    return;

                foreach (var troy in gametroy.Troys)
                {
                    if (!troy.Included || !troy.Owner.IsEnemy)
                        continue;

                    if (troy.Obj.IsValid && hero.Player.Distance(troy.Obj.Position) <= troy.Obj.BoundingRadius)
                    {
                        // start the event on damamge
                        spelldata.mypells.ForEach(x => Game.OnUpdate += x.OnTick);

                        hero.Attacker = troy.Owner;
                        hero.IncomeDamage = (float) troy.Owner.GetSpellDamage(hero.Player, troy.Slot);

                        // debug stuff
                        Console.WriteLine("Debug: Income Damage - " + hero.IncomeDamage);
                        Console.WriteLine("Debug: In Troy - " + troy.Name);

                        foreach (var item in spelldata.troydata)
                        {
                            if (troy.Name == item.Name)
                            {
                                if (item.HitType.Any(x => x == HitType.Danger))
                                    hero.HitTypes.Add(HitType.Danger);

                                if (item.HitType.Any(x => x == HitType.Ultimate))
                                    hero.HitTypes.Add(HitType.Ultimate);

                                if (item.HitType.Any(x => x == HitType.CrowdControl))
                                    hero.HitTypes.Add(HitType.CrowdControl);
                            }
                        }
                    }
                }
            }
        }
    }
}