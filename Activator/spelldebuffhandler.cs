#region Copyright © 2015 Kurisu Solutions
// All rights are reserved. Transmission or reproduction in part or whole,
// any form or by any means, mechanical, electronical or otherwise, is prohibited
// without the prior written consent of the copyright owner.
// 
// Document:	activator/spelldebuffhandler.cs
// Date:		06/06/2015
// Author:		Robin Kurisu
#endregion

using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

namespace Activator
{
    public class spelldebuffhandler
    {
        public static void Load()
        {
            Obj_AI_Base.OnBuffAdd += Obj_AI_Base_OnBuffAdd;
            Obj_AI_Base.OnBuffRemove += Obj_AI_Base_OnBuffRemove;
        }

        private static void Obj_AI_Base_OnBuffAdd(Obj_AI_Base sender, Obj_AI_BaseBuffAddEventArgs args)
        {
            foreach (var hero in champion.Heroes)
            {
                if (hero.Player.NetworkId == sender.NetworkId)
                {
                    if (spelldebuff.excludedbuffs.Any(buff => args.Buff.Name.ToLower() == buff))
                        continue;

                    if (args.Buff.Name.ToLower() == "summonerdot")
                    {
                        hero.HitTypes.Add(HitType.Danger);
                        var attacker = args.Buff.Caster as Obj_AI_Hero;
                        if (attacker != null)
                            hero.IncomeDamage = (50 + (20 * attacker.Level));
                    }

                    if (args.Buff.Type == BuffType.SpellImmunity ||
                        args.Buff.Type == BuffType.Invulnerability)
                    {
                        hero.Immunity = true;
                    }

                    foreach (var buff in spelldebuff.debuffs)
                    {
                        if (buff.Name != args.Buff.Name.ToLower())
                            continue;

                        if (buff.Evade)
                        {
                            Utility.DelayAction.Add(buff.EvadeTimer, delegate
                            {
                                if (hero.Player.HasBuff(buff.Name, true))
                                {
                                    hero.IncomeDamage = 1;
                                    hero.HitTypes.Add(HitType.Ultimate);
                                }
                            });
                        }

                        if (buff.Cleanse)
                        {
                            Utility.DelayAction.Add(buff.CleanseTimer, delegate
                            {
                                hero.IncomeDamage = 1;
                                hero.ForceQSS = true;
                            });
                        }
                    }

                    if (args.Buff.Type == BuffType.Snare && Activator.Origin.Item("csnare").GetValue<bool>() ||
                        args.Buff.Type == BuffType.Charm && Activator.Origin.Item("ccharm").GetValue<bool>() ||
                        args.Buff.Type == BuffType.Taunt && Activator.Origin.Item("ctaunt").GetValue<bool>() ||
                        args.Buff.Type == BuffType.Stun && Activator.Origin.Item("cstun").GetValue<bool>() ||
                        args.Buff.Type == BuffType.Fear && Activator.Origin.Item("cfear").GetValue<bool>() ||
                        args.Buff.Type == BuffType.Flee && Activator.Origin.Item("cflee").GetValue<bool>() ||
                        args.Buff.Type == BuffType.Polymorph && Activator.Origin.Item("cpolymorph").GetValue<bool>() ||
                        args.Buff.Type == BuffType.Blind && Activator.Origin.Item("cblind").GetValue<bool>() ||
                        args.Buff.Type == BuffType.Suppression && Activator.Origin.Item("csupp").GetValue<bool>() ||
                        args.Buff.Type == BuffType.Poison && Activator.Origin.Item("cpoison").GetValue<bool>() ||
                        args.Buff.Type == BuffType.Slow && Activator.Origin.Item("cslow").GetValue<bool>() ||
                        args.Buff.Name == "summonerexhaust" && Activator.Origin.Item("cexhaust").GetValue<bool>())
                    {
                        hero.IncomeDamage = 1;
                        hero.QSSBuffCount = hero.QSSBuffCount + 1;
                        if (Math.Ceiling(args.Buff.EndTime - args.Buff.StartTime) > hero.QSSHighestBuffTime)
                            hero.QSSHighestBuffTime = (int) (Math.Ceiling(args.Buff.EndTime - args.Buff.StartTime));
                    }
                }
            }
        }

        private static void Obj_AI_Base_OnBuffRemove(Obj_AI_Base sender, Obj_AI_BaseBuffRemoveEventArgs args)
        {
            foreach (var hero in champion.Heroes)
            {
                if (hero.Player.NetworkId == sender.NetworkId)
                {
                    if (spelldebuff.excludedbuffs.Any(buff => args.Buff.Name.ToLower() == buff))
                        continue;

                    if (args.Buff.Type == BuffType.SpellImmunity ||
                        args.Buff.Type == BuffType.Invulnerability)
                    {
                        hero.Immunity = false;
                    }

                    if (args.Buff.Name.ToLower() == "summonerdot")
                    {
                        hero.IncomeDamage = 0;
                        hero.HitTypes.Clear();
                    }

                    if (hero.QSSBuffCount <= 1)
                        hero.QSSHighestBuffTime = 0;

                    foreach (var buff in spelldebuff.debuffs)
                    {
                        if (buff.Name == args.Buff.Name.ToLower())
                        {
                            if (buff.Evade)
                            {
                                Utility.DelayAction.Add(buff.EvadeTimer + 100, delegate
                                {
                                    hero.IncomeDamage = 0;
                                    hero.HitTypes.Clear();
                                });
                            }

                            if (buff.Cleanse)
                            {
                                Utility.DelayAction.Add(buff.CleanseTimer + 100, delegate
                                {
                                    hero.IncomeDamage = 0;
                                    hero.ForceQSS = false;
                                });
                            }
                        }
                    }

                    if (args.Buff.Type == BuffType.Snare && Activator.Origin.Item("csnare").GetValue<bool>() ||
                        args.Buff.Type == BuffType.Charm && Activator.Origin.Item("ccharm").GetValue<bool>() ||
                        args.Buff.Type == BuffType.Taunt && Activator.Origin.Item("ctaunt").GetValue<bool>() ||
                        args.Buff.Type == BuffType.Stun && Activator.Origin.Item("cstun").GetValue<bool>() ||
                        args.Buff.Type == BuffType.Fear && Activator.Origin.Item("cfear").GetValue<bool>() ||
                        args.Buff.Type == BuffType.Flee && Activator.Origin.Item("cflee").GetValue<bool>() ||
                        args.Buff.Type == BuffType.Silence && Activator.Origin.Item("csilence").GetValue<bool>() ||
                        args.Buff.Type == BuffType.Polymorph && Activator.Origin.Item("cpolymorph").GetValue<bool>() ||
                        args.Buff.Type == BuffType.Blind && Activator.Origin.Item("cblind").GetValue<bool>() ||
                        args.Buff.Type == BuffType.Suppression && Activator.Origin.Item("csupp").GetValue<bool>() ||
                        args.Buff.Type == BuffType.Poison && Activator.Origin.Item("cpoison").GetValue<bool>() ||
                        args.Buff.Type == BuffType.Slow && Activator.Origin.Item("cslow").GetValue<bool>() ||
                        args.Buff.Name == "summonerexhaust" && Activator.Origin.Item("cexhaust").GetValue<bool>())
                    {
                        hero.IncomeDamage = 0;
                        if (hero.QSSBuffCount > 0) 
                            hero.QSSBuffCount = hero.QSSBuffCount - 1;
                        else 
                            hero.QSSBuffCount = 0;
                    }
                }
            }
        }

    }
}
