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

        private static readonly string[] cleansers = { "Mercurial", "Quicksliver", "Dervish", "Mikaels" };

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
                            if (spelldata.summoners.Any(x => x.Name == "summonerboost") ||
                                spelldata.items.Any(x => cleansers.Any(y => x.Name == y)))
                            {
                                Utility.DelayAction.Add(buff.CleanseTimer, delegate
                                {
                                    hero.IncomeDamage = 1;
                                    hero.ForceQSS = true;
                                });
                            }
                        }
                    }

                    #region Boost

                    if (spelldata.summoners.Any(x => x.Name == "summonerboost"))
                    {
                        if (args.Buff.Type == BuffType.Snare &&
                            Activator.Origin.Item("summonerboostcsnare").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Charm &&
                            Activator.Origin.Item("summonerboostccharm").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Taunt &&
                            Activator.Origin.Item("summonerboostctaunt").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Stun &&
                            Activator.Origin.Item("summonerboostcstun").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Fear &&
                            Activator.Origin.Item("summonerboostcfear").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Flee &&
                            Activator.Origin.Item("summonerboostcflee").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Polymorph &&
                            Activator.Origin.Item("summonerboostcpolymorph").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Blind &&
                            Activator.Origin.Item("summonerboostcblind").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Suppression &&
                            Activator.Origin.Item("summonerboostcsupp").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Poison &&
                            Activator.Origin.Item("summonerboostcpoison").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Slow &&
                            Activator.Origin.Item("summonerboostcslow").GetValue<bool>() ||
                            args.Buff.Name == "summonerexhaust" && 
                            Activator.Origin.Item("summonerboostcexhaust").GetValue<bool>() ||
                            args.Buff.Name == "summonerdot")
                        {
                            hero.IncomeDamage = 1;
                            hero.CleanseBuffCount = hero.CleanseBuffCount + 1;

                            if (Math.Ceiling(args.Buff.EndTime - args.Buff.StartTime) > hero.CleanseHighestBuffTime)
                                hero.CleanseHighestBuffTime =
                                    (int)(Math.Ceiling(args.Buff.EndTime - args.Buff.StartTime));
                        }
                    }

                    #endregion

                    #region Dervish

                    if (spelldata.items.Any(x => x.Name == "Dervish"))
                    {
                        if (args.Buff.Type == BuffType.Snare &&
                            Activator.Origin.Item("Dervishcsnare").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Charm &&
                            Activator.Origin.Item("Dervishccharm").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Taunt &&
                            Activator.Origin.Item("Dervishctaunt").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Stun &&
                            Activator.Origin.Item("Dervishcstun").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Fear &&
                            Activator.Origin.Item("Dervishcfear").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Flee &&
                            Activator.Origin.Item("Dervishcflee").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Polymorph &&
                            Activator.Origin.Item("Dervishcpolymorph").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Blind &&
                            Activator.Origin.Item("Dervishcblind").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Suppression &&
                            Activator.Origin.Item("Dervishcsupp").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Poison &&
                            Activator.Origin.Item("Dervishcpoison").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Slow &&
                            Activator.Origin.Item("Dervishcslow").GetValue<bool>() ||
                            args.Buff.Name == "summonerexhaust" &&
                            Activator.Origin.Item("Dervishcexhaust").GetValue<bool>() ||
                            args.Buff.Name == "summonerdot" &&
                            Activator.Origin.Item("Dervishcignote").GetValue<bool>())
                        {
                            hero.IncomeDamage = 1;    
                            hero.DervishBuffCount = hero.DervishBuffCount + 1;

                            if (Math.Ceiling(args.Buff.EndTime - args.Buff.StartTime) >
                                hero.DervishHighestBuffTime)
                                hero.DervishBuffCount =
                                    (int) (Math.Ceiling(args.Buff.EndTime - args.Buff.StartTime));

                        }
                    }

                    #endregion

                    #region Quicksilver

                    if (spelldata.items.Any(x => x.Name == "Quicksilver"))
                    {
                        if (args.Buff.Type == BuffType.Snare &&
                            Activator.Origin.Item("Quicksilvercsnare").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Charm &&
                            Activator.Origin.Item("Quicksilverccharm").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Taunt &&
                            Activator.Origin.Item("Quicksilverctaunt").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Stun &&
                            Activator.Origin.Item("Quicksilvercstun").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Fear &&
                            Activator.Origin.Item("Quicksilvercfear").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Flee &&
                            Activator.Origin.Item("Quicksilvercflee").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Polymorph &&
                            Activator.Origin.Item("Quicksilvercpolymorph").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Blind &&
                            Activator.Origin.Item("Quicksilvercblind").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Suppression &&
                            Activator.Origin.Item("Quicksilvercsupp").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Poison &&
                            Activator.Origin.Item("Quicksilvercpoison").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Slow &&
                            Activator.Origin.Item("Quicksilvercslow").GetValue<bool>() ||
                            args.Buff.Name == "summonerexhaust" &&
                            Activator.Origin.Item("Quicksilvercexhaust").GetValue<bool>() ||
                            args.Buff.Name == "summonerdot" &&
                            Activator.Origin.Item("Quicksilvercignote").GetValue<bool>())
                        {
                            hero.IncomeDamage = 1;
                            hero.QSSBuffCount  = hero.QSSBuffCount + 1;

                            if (Math.Ceiling(args.Buff.EndTime - args.Buff.StartTime) >
                                hero.QSSHighestBuffTime)
                                hero.QSSHighestBuffTime =
                                    (int)(Math.Ceiling(args.Buff.EndTime - args.Buff.StartTime));

                        }
                    }

                    #endregion

                    #region Mercurial

                    if (spelldata.items.Any(x => x.Name == "Mercurial"))
                    {
                        if (args.Buff.Type == BuffType.Snare &&
                            Activator.Origin.Item("Mercurialcsnare").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Charm &&
                            Activator.Origin.Item("Mercurialccharm").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Taunt &&
                            Activator.Origin.Item("Mercurialctaunt").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Stun &&
                            Activator.Origin.Item("Mercurialcstun").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Fear &&
                            Activator.Origin.Item("Mercurialcfear").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Flee &&
                            Activator.Origin.Item("Mercurialcflee").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Polymorph &&
                            Activator.Origin.Item("Mercurialcpolymorph").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Blind &&
                            Activator.Origin.Item("Mercurialcblind").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Suppression &&
                            Activator.Origin.Item("Mercurialcsupp").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Poison &&
                            Activator.Origin.Item("Mercurialcpoison").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Slow &&
                            Activator.Origin.Item("Mercurialcslow").GetValue<bool>() ||
                            args.Buff.Name == "summonerexhaust" &&
                            Activator.Origin.Item("Mercurialcexhaust").GetValue<bool>() ||
                            args.Buff.Name == "summonerdot" &&
                            Activator.Origin.Item("Mercurialcignote").GetValue<bool>())
                        {
                            hero.IncomeDamage = 1;
                            hero.MercurialBuffCount = hero.MercurialBuffCount + 1;

                            if (Math.Ceiling(args.Buff.EndTime - args.Buff.StartTime) >
                                hero.MercurialHighestBuffTime)
                                hero.MercurialHighestBuffTime =
                                    (int)(Math.Ceiling(args.Buff.EndTime - args.Buff.StartTime));
                        }
                    }

                    #endregion

                    #region Mikaels

                    if (spelldata.items.Any(x => x.Name == "Mikaels"))
                    {
                        if (args.Buff.Type == BuffType.Snare &&
                            Activator.Origin.Item("Mikaelscsnare").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Charm &&
                            Activator.Origin.Item("Mikaelsccharm").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Taunt &&
                            Activator.Origin.Item("Mikaelsctaunt").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Stun &&
                            Activator.Origin.Item("Mikaelscstun").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Fear &&
                            Activator.Origin.Item("Mikaelscfear").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Flee &&
                            Activator.Origin.Item("Mikaelscflee").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Polymorph &&
                            Activator.Origin.Item("Mikaelscpolymorph").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Blind &&
                            Activator.Origin.Item("Mikaelscblind").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Suppression &&
                            Activator.Origin.Item("Mikaelscsupp").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Poison &&
                            Activator.Origin.Item("Mikaelscpoison").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Slow &&
                            Activator.Origin.Item("Mikaelscslow").GetValue<bool>() ||
                            args.Buff.Name == "summonerexhaust" &&
                            Activator.Origin.Item("Mikaelscexhaust").GetValue<bool>() ||
                            args.Buff.Name == "summonerdot" &&
                            Activator.Origin.Item("Mikaelscignote").GetValue<bool>())
                        {
                            hero.IncomeDamage = 1;
                            hero.MikaelsBuffCount = hero.MikaelsBuffCount + 1;

                            if (Math.Ceiling(args.Buff.EndTime - args.Buff.StartTime) >
                                hero.MikaelsHighestBuffTime)
                                hero.MikaelsHighestBuffTime =
                                    (int) (Math.Ceiling(args.Buff.EndTime - args.Buff.StartTime));
                        }
                    }

                    #endregion
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


                    #region Boost

                    if (spelldata.summoners.Any(x => x.Name == "summonerboost"))
                    {
                        if (args.Buff.Type == BuffType.Snare &&
                            Activator.Origin.Item("summonerboostcsnare").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Charm &&
                            Activator.Origin.Item("summonerboostccharm").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Taunt &&
                            Activator.Origin.Item("summonerboostctaunt").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Stun &&
                            Activator.Origin.Item("summonerboostcstun").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Fear &&
                            Activator.Origin.Item("summonerboostcfear").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Flee &&
                            Activator.Origin.Item("summonerboostcflee").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Polymorph &&
                            Activator.Origin.Item("summonerboostcpolymorph").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Blind &&
                            Activator.Origin.Item("summonerboostcblind").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Suppression &&
                            Activator.Origin.Item("summonerboostcsupp").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Poison &&
                            Activator.Origin.Item("summonerboostcpoison").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Slow &&
                            Activator.Origin.Item("summonerboostcslow").GetValue<bool>() ||
                            args.Buff.Name == "summonerexhaust" &&
                            Activator.Origin.Item("summonerboostcexhaust").GetValue<bool>() ||
                            args.Buff.Name == "summonerdot" &&
                            Activator.Origin.Item("summonerboostcignote").GetValue<bool>())
                        {
                            hero.IncomeDamage = 0;
                            if (hero.CleanseBuffCount > 0)
                                hero.CleanseBuffCount = hero.CleanseBuffCount - 1;
                            else
                                hero.CleanseBuffCount = 0;
                        }
                    }

                    #endregion

                    #region Dervish

                    if (spelldata.items.Any(x => x.Name == "Dervish"))
                    {
                        if (args.Buff.Type == BuffType.Snare &&
                            Activator.Origin.Item("Dervishcsnare").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Charm &&
                            Activator.Origin.Item("Dervishccharm").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Taunt &&
                            Activator.Origin.Item("Dervishctaunt").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Stun &&
                            Activator.Origin.Item("Dervishcstun").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Fear &&
                            Activator.Origin.Item("Dervishcfear").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Flee &&
                            Activator.Origin.Item("Dervishcflee").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Polymorph &&
                            Activator.Origin.Item("Dervishcpolymorph").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Blind &&
                            Activator.Origin.Item("Dervishcblind").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Suppression &&
                            Activator.Origin.Item("Dervishcsupp").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Poison &&
                            Activator.Origin.Item("Dervishcpoison").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Slow &&
                            Activator.Origin.Item("Dervishcslow").GetValue<bool>() ||
                            args.Buff.Name == "summonerexhaust" &&
                            Activator.Origin.Item("Dervishcexhaust").GetValue<bool>() ||
                            args.Buff.Name == "summonerdot" &&
                            Activator.Origin.Item("Dervishcignote").GetValue<bool>())
                        {
                            hero.IncomeDamage = 0;
                            if (hero.DervishBuffCount > 0)
                                hero.DervishBuffCount = hero.DervishBuffCount - 1;
                            else
                                hero.DervishBuffCount = 0;
                        }
                    }

                    #endregion

                    #region Quicksilver

                    if (spelldata.items.Any(x => x.Name == "Quicksilver"))
                    {
                        if (args.Buff.Type == BuffType.Snare &&
                            Activator.Origin.Item("Quicksilvercsnare").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Charm &&
                            Activator.Origin.Item("Quicksilverccharm").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Taunt &&
                            Activator.Origin.Item("Quicksilverctaunt").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Stun &&
                            Activator.Origin.Item("Quicksilvercstun").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Fear &&
                            Activator.Origin.Item("Quicksilvercfear").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Flee &&
                            Activator.Origin.Item("Quicksilvercflee").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Polymorph &&
                            Activator.Origin.Item("Quicksilvercpolymorph").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Blind &&
                            Activator.Origin.Item("Quicksilvercblind").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Suppression &&
                            Activator.Origin.Item("Quicksilvercsupp").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Poison &&
                            Activator.Origin.Item("Quicksilvercpoison").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Slow &&
                            Activator.Origin.Item("Quicksilvercslow").GetValue<bool>() ||
                            args.Buff.Name == "summonerexhaust" &&
                            Activator.Origin.Item("Quicksilvercexhaust").GetValue<bool>() ||
                            args.Buff.Name == "summonerdot" &&
                            Activator.Origin.Item("Quicksilvercignote").GetValue<bool>())
                        {
                            hero.IncomeDamage = 0;
                            if (hero.QSSBuffCount > 0)
                                hero.QSSBuffCount = hero.QSSBuffCount - 1;
                            else
                                hero.QSSBuffCount = 0;
                        }
                    }

                    #endregion

                    #region Mercurial

                    if (spelldata.items.Any(x => x.Name == "Mercurial"))
                    {
                        if (args.Buff.Type == BuffType.Snare &&
                            Activator.Origin.Item("Mercurialcsnare").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Charm &&
                            Activator.Origin.Item("Mercurialccharm").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Taunt &&
                            Activator.Origin.Item("Mercurialctaunt").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Stun &&
                            Activator.Origin.Item("Mercurialcstun").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Fear &&
                            Activator.Origin.Item("Mercurialcfear").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Flee &&
                            Activator.Origin.Item("Mercurialcflee").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Polymorph &&
                            Activator.Origin.Item("Mercurialcpolymorph").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Blind &&
                            Activator.Origin.Item("Mercurialcblind").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Suppression &&
                            Activator.Origin.Item("Mercurialcsupp").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Poison &&
                            Activator.Origin.Item("Mercurialcpoison").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Slow &&
                            Activator.Origin.Item("Mercurialcslow").GetValue<bool>() ||
                            args.Buff.Name == "summonerexhaust" &&
                            Activator.Origin.Item("Mercurialcexhaust").GetValue<bool>() ||
                            args.Buff.Name == "summonerdot" &&
                            Activator.Origin.Item("Mercurialcignote").GetValue<bool>())
                        {
                            hero.IncomeDamage = 0;
                            if (hero.MercurialBuffCount > 0)
                                hero.MercurialBuffCount = hero.MercurialBuffCount - 1;
                            else
                                hero.MercurialBuffCount = 0;
                        }
                    }

                    #endregion

                    #region Mikaels

                    if (spelldata.items.Any(x => x.Name == "Mikaels"))
                    {
                        if (args.Buff.Type == BuffType.Snare &&
                            Activator.Origin.Item("Mikaelscsnare").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Charm &&
                            Activator.Origin.Item("Mikaelsccharm").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Taunt &&
                            Activator.Origin.Item("Mikaelsctaunt").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Stun &&
                            Activator.Origin.Item("Mikaelscstun").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Fear &&
                            Activator.Origin.Item("Mikaelscfear").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Flee &&
                            Activator.Origin.Item("Mikaelscflee").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Polymorph &&
                            Activator.Origin.Item("Mikaelscpolymorph").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Blind &&
                            Activator.Origin.Item("Mikaelscblind").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Suppression &&
                            Activator.Origin.Item("Mikaelscsupp").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Poison &&
                            Activator.Origin.Item("Mikaelscpoison").GetValue<bool>() ||
                            args.Buff.Type == BuffType.Slow &&
                            Activator.Origin.Item("Mikaelscslow").GetValue<bool>() ||
                            args.Buff.Name == "summonerexhaust" &&
                            Activator.Origin.Item("Mikaelscexhaust").GetValue<bool>() ||
                            args.Buff.Name == "summonerdot" &&
                            Activator.Origin.Item("Mikaelscignote").GetValue<bool>())
                        {
                            hero.IncomeDamage = 0;
                            if (hero.MikaelsBuffCount > 0)
                                hero.MikaelsBuffCount = hero.MikaelsBuffCount - 1;
                            else
                                hero.MikaelsBuffCount = 0;
                        }
                    }

                    #endregion

                }
            }
        }

    }
}
