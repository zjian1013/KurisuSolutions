using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace Activator
{
    public class projectionhandler
    {
        public static Obj_AI_Hero Target;

        public static void Load()
        {
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsEnemy && sender.Type == GameObjectType.obj_AI_Hero)
            {
                var start = Environment.TickCount;
                foreach (var hero in champion.Heroes)
                {
                    // auto attack dectection
                    if (args.SData.IsAutoAttack())
                    {
                        var woop = (int) (hero.Player.Distance(sender.ServerPosition)/
                                          sender.BasicAttack.MissileSpeed);

                        var endtime = (int) (sender.AttackCastDelay*1000) - 100 + Game.Ping/2 +
                                      1000*woop;

                        // delay a little bit before missile endtime
                        Utility.DelayAction.Add((int) (endtime - (endtime*0.5)), delegate
                        {
                            hero.Attacker = sender;
                            hero.HitTypes.Add(HitType.AutoAttack);
                            hero.IncomeDamage +=
                                (float) Math.Abs(sender.GetAutoAttackDamage(hero.Player, true));

                            // lazy reset
                            Utility.DelayAction.Add((endtime*2), delegate
                            {
                                hero.Attacker = null;
                                hero.IncomeDamage = 0;
                                hero.HitTypes.Clear();
                            });
                        });
                    }

                    foreach (var data in spelldata.spells)
                    {
                        if (data.SDataName != args.SData.Name.ToLower())
                            return;

                        // self/selfaoe spell detection
                        if (args.SData.TargettingType == SpellDataTargetType.Self ||
                            args.SData.TargettingType == SpellDataTargetType.SelfAoe)
                        {
                            if (hero.Player.Distance(sender.ServerPosition) <= data.CastRange)
                            {
                                // delay the spell a bit before missile endtime
                                Utility.DelayAction.Add((int) (data.Delay - (data.Delay*0.5)), delegate
                                {
                                    hero.Attacker = sender;
                                    hero.HitTypes.Add(HitType.Spell);
                                    hero.IncomeDamage +=
                                        (float) Math.Abs(sender.GetSpellDamage(hero.Player, args.SData.Name));

                                    if (args.SData.HaveHitEffect)
                                        hero.HitTypes.Add(HitType.AutoAttack);

                                    // detect danger/cc/ultimates from our db
                                    foreach (var item in spelldata.spells)
                                    {
                                        if (item.SDataName != args.SData.Name.ToLower())
                                            continue;

                                        // spell is important or lethal!
                                        if (item.HitType.Contains(HitType.Ultimate))
                                            hero.HitTypes.Add(HitType.Ultimate);

                                        // spell is important but not as fatal
                                        if (item.HitType.Contains(HitType.Danger))
                                            hero.HitTypes.Add(HitType.Danger);

                                        // spell has a crowd control effect
                                        if (item.HitType.Contains(HitType.CrowdControl))
                                            hero.HitTypes.Add(HitType.CrowdControl);
                                    }

                                    // lazy safe reset
                                    Utility.DelayAction.Add((int) (data.Delay*2), delegate
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
                            if (hero.Player.Distance(sender.ServerPosition) <= data.CastRange)
                            {
                                // important spelldata shit (hope sdata is accurate)
                                var distance =
                                    (int) (1000*(sender.Distance(hero.Player.ServerPosition)/data.MissileSpeed));
                                var endtime = data.Delay - 100 + Game.Ping/2 + distance -
                                              (Environment.TickCount - start);

                                // get the real end position normalized
                                var direction = (args.End.To2D() - sender.ServerPosition.To2D()).Normalized();
                                var endpos = sender.ServerPosition.To2D() + direction*data.CastRange;

                                // setup projection
                                var proj = hero.Player.ServerPosition.To2D()
                                    .ProjectOn(sender.ServerPosition.To2D(), endpos);
                                var projdist = hero.Player.ServerPosition.To2D().Distance(proj.SegmentPoint);

                                // get the evade time 
                                var evadetime =
                                    (int)
                                        (1000*(args.SData.LineWidth - projdist + hero.Player.BoundingRadius)/
                                         hero.Player.MoveSpeed);

                                if (args.SData.LineWidth + hero.Player.BoundingRadius > projdist)
                                {
                                    // ignore if can evade and using an evade assembly
                                    if (hero.Player.NetworkId == Activator.Player.NetworkId)
                                    {
                                        if (hero.Player.CanMove && evadetime < endtime)
                                        {
                                            if (Activator.Origin.Item("evadeon").GetValue<bool>())
                                            {
                                                return;
                                            }
                                        }
                                    }

                                    // delay the action a little bit before endtime
                                    Utility.DelayAction.Add((int) (endtime - (endtime*0.5)), delegate
                                    {
                                        hero.Attacker = sender;
                                        hero.HitTypes.Add(HitType.Spell);
                                        hero.IncomeDamage +=
                                            (float) Math.Abs(sender.GetSpellDamage(hero.Player, args.SData.Name));

                                        if (args.SData.HaveHitEffect)
                                            hero.HitTypes.Add(HitType.AutoAttack);

                                        // detect danger/cc/ultimates from our db
                                        foreach (var item in spelldata.spells)
                                        {
                                            if (item.SDataName != args.SData.Name.ToLower())
                                                continue;

                                            // spell is important or lethal!
                                            if (item.HitType.Contains(HitType.Ultimate))
                                                hero.HitTypes.Add(HitType.Ultimate);

                                            // spell is important but not as fatal
                                            if (item.HitType.Contains(HitType.Danger))
                                                hero.HitTypes.Add(HitType.Danger);

                                            // spell has a crowd control effect
                                            if (item.HitType.Contains(HitType.CrowdControl))
                                                hero.HitTypes.Add(HitType.CrowdControl);

                                        }

                                        // lazy safe reset
                                        Utility.DelayAction.Add((int) (endtime*2), delegate
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
                                                  (Environment.TickCount - start);

                                    Utility.DelayAction.Add((int) (endtime - (endtime*0.5)), delegate
                                    {
                                        hero.Attacker = sender;
                                        hero.HitTypes.Add(HitType.Spell);
                                        hero.IncomeDamage +=
                                            (float) Math.Abs(sender.GetSpellDamage(hero.Player, args.SData.Name));

                                        // detect danger/cc/ultimates from our db
                                        foreach (var item in spelldata.spells)
                                        {
                                            if (item.SDataName != args.SData.Name.ToLower())
                                                continue;

                                            Utility.DelayAction.Add(Game.Ping + 10, delegate
                                            {
                                                // spell is important or lethal!
                                                if (item.HitType.Contains(HitType.Ultimate))
                                                    hero.HitTypes.Add(HitType.Ultimate);

                                                // spell is important but not as fatal
                                                if (item.HitType.Contains(HitType.Danger))
                                                    hero.HitTypes.Add(HitType.Danger);

                                                // spell has a crowd control effect
                                                if (item.HitType.Contains(HitType.CrowdControl))
                                                    hero.HitTypes.Add(HitType.CrowdControl);

                                            });

                                            // lazy reset
                                            Utility.DelayAction.Add((int) (endtime*2), delegate
                                            {
                                                hero.Attacker = null;
                                                hero.IncomeDamage = 0;
                                                hero.HitTypes.Clear();
                                            });
                                        }
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
                    if (args.Target.NetworkId == hero.Player.NetworkId)
                    {
                        if (sender.Distance(hero.Player.ServerPosition) <= 900 &&
                            Activator.Player.Distance(hero.Player.ServerPosition) <= 1000)
                        {
                            Utility.DelayAction.Add(500, delegate
                            {
                                hero.HitTypes.Add(HitType.TurretAttack);
                                hero.IncomeDamage =
                                    (float) Math.Abs(sender.CalcDamage(hero.Player, Damage.DamageType.Physical,
                                        sender.BaseAttackDamage + sender.FlatPhysicalDamageMod));

                                // lazy reset
                                Utility.DelayAction.Add(1000, delegate
                                {
                                    hero.Attacker = null;
                                    hero.IncomeDamage = 0;
                                    hero.HitTypes.Clear();
                                });
                            });
                        }
                    }
                }
            }

            if (sender.IsEnemy && sender.Type == GameObjectType.obj_AI_Minion)
            {
                foreach (var hero in champion.Heroes)
                {
                    if (hero.Player.NetworkId == args.Target.NetworkId)
                    {
                        if (hero.Player.Distance(sender.ServerPosition) <= 750 &&
                            Activator.Player.Distance(hero.Player.ServerPosition) <= 1000)
                        {
                            hero.Attacker = sender;
                            hero.HitTypes.Add(HitType.MinionAttack);
                            hero.IncomeDamage =
                                (float)
                                    Math.Abs(sender.CalcDamage(hero.Player, Damage.DamageType.Physical,
                                        sender.BaseAttackDamage + sender.FlatPhysicalDamageMod));

                            // lazy reset
                            Utility.DelayAction.Add(1000, delegate
                            {
                                hero.Attacker = null;
                                hero.IncomeDamage = 0;
                                hero.HitTypes.Clear();
                            });
                        }
                    }
                }
            }
        }
    }
}
