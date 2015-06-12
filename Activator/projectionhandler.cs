#region Copyright © 2015 Kurisu Solutions
// All rights are reserved. Transmission or reproduction in part or whole,
// any form or by any means, mechanical, electronical or otherwise, is prohibited
// without the prior written consent of the copyright owner.
// 
// Document:	activator/projectionhandler.cs
// Date:		06/06/2015
// Author:		Robin Kurisu
#endregion

using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

namespace Activator
{
    public class projectionhandler
    {
        public static Obj_AI_Hero Target;
        public static int Last;

        public static void Load()
        {
            GameObject.OnCreate += GameObject_OnCreate;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast; 
        }

        private static void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            var missile = sender as Obj_SpellMissile;
            if (missile == null || !missile.IsValid)
                return;

            var caster = missile.SpellCaster as Obj_AI_Hero;
            if (caster == null || !caster.IsValid)
                return;

            if (caster.Team == Activator.Player.Team)
                return;

            var startPos = missile.StartPosition.To2D();
            var endPos = missile.EndPosition.To2D();

            var data = spelldata.GetByMissileName(missile.SData.Name.ToLower());
            if (data == null)
                return;

            var direction = (endPos - startPos).Normalized();
            if (startPos.Distance(endPos) > data.CastRange)
                endPos = startPos + direction*data.CastRange;

            foreach (var hero in champion.Heroes)
            {
                if (hero.Player.IsDead || hero.Player.IsZombie)
                {
                    hero.IncomeDamage = 0;
                    hero.HitTypes.Clear();
                    continue;
                }

                var distance = (1000 * (startPos.Distance(hero.Player.ServerPosition) / data.MissileSpeed));
                var endtime = - 100 + Game.Ping/2 + distance;

                // setup projection
                var proj = hero.Player.ServerPosition.To2D().ProjectOn(startPos, endPos);
                var projdist = hero.Player.ServerPosition.To2D().Distance(proj.SegmentPoint);

                // get the evade time 
                var evadetime =
                    (int)
                        (1000 * (missile.SData.LineWidth - projdist + hero.Player.BoundingRadius) /
                         hero.Player.MoveSpeed);

                // check if hero on segment
                if (missile.SData.LineWidth + hero.Player.BoundingRadius > projdist)
                {
                    // ignore if can evade and using an evade assembly
                    if (hero.Player.NetworkId == Activator.Player.NetworkId)
                    {
                        if (hero.Player.CanMove && evadetime < endtime)
                        {
                            if (Activator.Origin.Item("evadefow").GetValue<bool>() &&
                               !Activator.Origin.Item("usecombo").GetValue<KeyBind>().Active)
                            {
                                // check next player
                                continue;
                            }
                        }
                    }

                    hero.Attacker = caster;
                    hero.IncomeDamage = (float)Math.Abs(caster.GetSpellDamage(hero.Player, data.SDataName));
                    hero.HitTypes.Add(HitType.Spell);

                    // spell is important or lethal!
                    if (data.HitType.Contains(HitType.Ultimate))
                        hero.HitTypes.Add(HitType.Ultimate);

                    // spell is important but not as fatal
                    if (data.HitType.Contains(HitType.Danger))
                        hero.HitTypes.Add(HitType.Danger);

                    // spell has a crowd control effect
                    if (data.HitType.Contains(HitType.CrowdControl))
                        hero.HitTypes.Add(HitType.CrowdControl);

                    Utility.DelayAction.Add(1200, () =>
                    {
                        hero.Attacker = null;
                        hero.IncomeDamage = 0;
                        hero.HitTypes.Clear();
                    });
                }
            }
        }


        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsEnemy && sender.Type == GameObjectType.obj_AI_Hero)
            {
                foreach (var hero in champion.Heroes)
                {
                    if (hero.Player.IsDead || hero.Player.IsZombie)
                    {
                        hero.IncomeDamage = 0;
                        hero.HitTypes.Clear();
                        continue;
                    }

                    // auto attack dectection
                    if (args.SData.IsAutoAttack() && args.Target.NetworkId == hero.Player.NetworkId)
                    {
                        // delay a little bit before missile endtime
                        Utility.DelayAction.Add(250, () =>
                        {
                            hero.Attacker = sender;
                            hero.HitTypes.Add(HitType.AutoAttack);
                            hero.IncomeDamage = (float) Math.Abs(sender.GetAutoAttackDamage(hero.Player));

                            // lazy reset
                            Utility.DelayAction.Add(1000, delegate
                            {
                                hero.Attacker = null;
                                hero.HitTypes.Remove(HitType.AutoAttack);
                                hero.IncomeDamage = 0;
                            });
                        });
                    }

                    foreach (var data in spelldata.spells.Where(x => x.SDataName == args.SData.Name.ToLower()))
                    {
                        Last = Utils.GameTimeTickCount;

                        // self/selfaoe spell detection
                        if (args.SData.TargettingType == SpellDataTargetType.Self ||
                            args.SData.TargettingType == SpellDataTargetType.SelfAoe)
                        {
                            var fromObj =
                                ObjectManager.Get<GameObject>()
                                    .FirstOrDefault(
                                        x => data.FromObject != null && data.FromObject.Any(y => x.Name.Contains(y)));

                            var correctpos = fromObj != null ? fromObj.Position : sender.ServerPosition;

                            if (hero.Player.Distance(correctpos) <= data.CastRange)
                            {
                                // delay the spell a bit before missile endtime
                                Utility.DelayAction.Add((int) (data.Delay - (data.Delay*0.3)), () =>
                                {
                                    hero.Attacker = sender;
                                    hero.HitTypes.Add(HitType.Spell);
                                    hero.IncomeDamage =
                                        (float) Math.Abs(sender.GetSpellDamage(hero.Player, args.SData.Name));

                                    // spell is important or lethal!
                                    if (data.HitType.Contains(HitType.Ultimate))
                                        hero.HitTypes.Add(HitType.Ultimate);

                                    // spell is important but not as fatal
                                    if (data.HitType.Contains(HitType.Danger))
                                        hero.HitTypes.Add(HitType.Danger);

                                    // spell has a crowd control effect
                                    if (data.HitType.Contains(HitType.CrowdControl))
                                        hero.HitTypes.Add(HitType.CrowdControl);

                                    // lazy safe reset
                                    Utility.DelayAction.Add((int) (data.Delay + 1200), () =>
                                    {
                                        hero.Attacker = null;
                                        hero.IncomeDamage = 0;
                                        hero.HitTypes.Clear();
                                    });
                                });
                            }
                        }

                        // skillshot detection completerino
                        if (args.SData.TargettingType == SpellDataTargetType.Cone ||
                            args.SData.TargettingType.ToString().Contains("Location"))
                        {
                            var fromObj =
                                ObjectManager.Get<GameObject>()
                                    .FirstOrDefault(
                                        x => data.FromObject != null && data.FromObject.Any(y => x.Name.Contains(y)));

                            bool islineskillshot = args.SData.LineWidth != 0;

                            var correctpos = fromObj != null ? fromObj.Position : sender.ServerPosition;
                            var correctwidth = !islineskillshot ? args.SData.CastRadius : args.SData.LineWidth;

                            if (hero.Player.Distance(correctpos) <= data.CastRange)
                            {
                                var distance = (int)(1000 * (correctpos.Distance(hero.Player.ServerPosition) / data.MissileSpeed));
                                var endtime = data.Delay - 100 + Game.Ping/2 + distance - (Utils.GameTimeTickCount - Last);

                                // get the real end position normalized
                                var direction = (args.End.To2D() - correctpos.To2D()).Normalized();
                                var endpos = correctpos.To2D() + direction * data.CastRange;

                                // setup projection
                                var proj = hero.Player.ServerPosition.To2D().ProjectOn(correctpos.To2D(), endpos);
                                var projdist = hero.Player.ServerPosition.To2D().Distance(proj.SegmentPoint);

                                int evadetime;

                                // get the evade time 
                                if (islineskillshot)
                                {
                                    evadetime =
                                        (int)
                                            (1000 * (correctwidth - projdist + hero.Player.BoundingRadius) /
                                             hero.Player.MoveSpeed);
                                }

                                else
                                {
                                    evadetime =
                                        (int)
                                            (1000 *
                                             (correctwidth - hero.Player.Distance(args.End) / hero.Player.MoveSpeed));
                                }

                                if (!islineskillshot && hero.Player.Distance(args.End) <= correctwidth ||
                                     islineskillshot && correctwidth + hero.Player.BoundingRadius > projdist)
                                {
                                    // ignore if can evade and using an evade assembly
                                    if (hero.Player.NetworkId == Activator.Player.NetworkId)
                                    {
                                        if (hero.Player.CanMove && evadetime < endtime && correctwidth <= 250)
                                        {
                                            if (Activator.Origin.Item("evadeon").GetValue<bool>() &&
                                               !Activator.Origin.Item("usecombo").GetValue<KeyBind>().Active)
                                            {
                                                // check next player
                                                continue;
                                            }
                                        }
                                    }

                                    // delay the action a little bit before endtime
                                    Utility.DelayAction.Add((int) (endtime - (endtime*0.3)), () =>
                                    {
                                        hero.Attacker = sender;
                                        hero.HitTypes.Add(HitType.Spell);
                                        hero.IncomeDamage =
                                            (float) Math.Abs(sender.GetSpellDamage(hero.Player, args.SData.Name));

                                        // spell is important or lethal!
                                        if (data.HitType.Contains(HitType.Ultimate))
                                            hero.HitTypes.Add(HitType.Ultimate);

                                        // spell is important but not as fatal
                                        if (data.HitType.Contains(HitType.Danger))
                                            hero.HitTypes.Add(HitType.Danger);

                                        // spell has a crowd control effect
                                        if (data.HitType.Contains(HitType.CrowdControl))
                                            hero.HitTypes.Add(HitType.CrowdControl);

                                        // lazy safe reset
                                        Utility.DelayAction.Add((int) (endtime + 1200), () =>
                                        {
                                            hero.Attacker = null;
                                            hero.IncomeDamage = 0;
                                            hero.HitTypes.Clear();
                                        });
                                    });
                                }
                            }
                        }

                        // unit type detection
                        if (args.SData.TargettingType == SpellDataTargetType.Unit)
                        {
                            // check if is targeteting the hero on our table
                            if (hero.Player.NetworkId == args.Target.NetworkId)
                            {
                                // target spell dectection
                                if (hero.Player.Distance(sender.ServerPosition) <= data.CastRange)
                                {
                                    // important spelldata shit (hope sdata is accurate)
                                    var distance =
                                        (int) (1000*(sender.Distance(hero.Player.ServerPosition)/data.MissileSpeed));
                                    var endtime = data.Delay - 100 + Game.Ping/2 + distance -
                                                  (Utils.GameTimeTickCount - Last);

                                    Utility.DelayAction.Add((int) (endtime - (endtime*0.3)), () =>
                                    {
                                        hero.Attacker = sender;
                                        hero.HitTypes.Add(HitType.Spell);
                                        hero.IncomeDamage =
                                            (float) Math.Abs(sender.GetSpellDamage(hero.Player, args.SData.Name));

                                        // spell is important or lethal!
                                        if (data.HitType.Contains(HitType.Ultimate))
                                            hero.HitTypes.Add(HitType.Ultimate);

                                        // spell is important but not as fatal
                                        if (data.HitType.Contains(HitType.Danger))
                                            hero.HitTypes.Add(HitType.Danger);

                                        // spell has a crowd control effect
                                        if (data.HitType.Contains(HitType.CrowdControl))
                                            hero.HitTypes.Add(HitType.CrowdControl);

                                        // lazy reset
                                        Utility.DelayAction.Add((int) (endtime + 1200), () =>
                                        {
                                            hero.Attacker = null;
                                            hero.HitTypes.Remove(HitType.Spell);
                                            hero.IncomeDamage = 0;
                                            hero.HitTypes.Clear();
                                        });
                                    });
                                }
                            }
                        }
                    }
                }
            }

            if (sender.IsEnemy && sender.Type == GameObjectType.obj_AI_Turret)
            {
                foreach (var hero in champion.Heroes)
                {
                    if (args.Target.NetworkId != hero.Player.NetworkId) 
                        continue;

                    if (sender.Distance(hero.Player.ServerPosition) <= 900 &&
                        Activator.Player.Distance(hero.Player.ServerPosition) <= 1000)
                    {
                        Utility.DelayAction.Add(500, () =>
                        {
                            hero.HitTypes.Add(HitType.TurretAttack);
                            hero.IncomeDamage =
                                (float) Math.Abs(sender.CalcDamage(hero.Player, Damage.DamageType.Physical,
                                    sender.BaseAttackDamage + sender.FlatPhysicalDamageMod));

                            // lazy reset
                            Utility.DelayAction.Add(1200, () =>
                            {
                                hero.Attacker = null;
                                hero.IncomeDamage = 0;
                                hero.HitTypes.Clear();
                            });
                        });
                    }
                }
            }

            if (sender.IsEnemy && sender.Type == GameObjectType.obj_AI_Minion)
            {
                foreach (var hero in champion.Heroes)
                {
                    if (hero.Player.NetworkId != args.Target.NetworkId) 
                        continue;

                    if (hero.Player.Distance(sender.ServerPosition) <= 750 &&
                        Activator.Player.Distance(hero.Player.ServerPosition) <= 1000)
                    {
                        hero.MinionDamage =
                            (float) Math.Abs(sender.CalcDamage(hero.Player, Damage.DamageType.Physical,
                                sender.BaseAttackDamage + sender.FlatPhysicalDamageMod));

                        // lazy reset
                        Utility.DelayAction.Add(1000, () =>
                        {
                            hero.MinionDamage = 0;
                        });
                    }
                }
            }
        }
    }
}
