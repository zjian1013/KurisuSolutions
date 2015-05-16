using System;
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
                if (hero.Player.NetworkId != sender.NetworkId)
                    return;

                if (args.Buff.Name == "RegenerationPotion")
                    hero.UsingHealthPot = true;

                if (args.Buff.Name == "FlaskOfCrystalWater")
                    hero.UsingManaPot = true;


                if (args.Buff.Name == "ItemCrystalFlask" ||
                    args.Buff.Name == "ItemMiniRegenPotion")
                {
                    hero.UsingMixedPot = true;
                }
                

                foreach (var buff in spelldebuff.debuffs)
                {
                    if (buff.Name != args.Buff.Name.ToLower()) 
                        continue;

                    if (buff.Evade)
                    {
                        spelldata.mypells.ForEach(x => Game.OnUpdate += x.OnTick);
                        Utility.DelayAction.Add(buff.EvadeTimer, delegate
                        {
                            hero.IncomeDamage = 1;
                            hero.HitTypes.Add(HitType.Ultimate);
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
                    hero.QSSBuffCount = hero.QSSBuffCount + 1;

                    if (args.Buff.EndTime - args.Buff.StartTime > hero.QSSHighestBuffTime)
                        hero.QSSHighestBuffTime = (int)(Math.Ceiling(args.Buff.EndTime - args.Buff.StartTime));
                }
            }
        }

        private static void Obj_AI_Base_OnBuffRemove(Obj_AI_Base sender, Obj_AI_BaseBuffRemoveEventArgs args)
        {
            foreach (var hero in champion.Heroes)
            {
                if (hero.Player.NetworkId == sender.NetworkId)
                {
                    if (sender.NetworkId == Activator.Player.NetworkId)
                    {
                        if (args.Buff.Name == "RegenerationPotion")
                            hero.UsingHealthPot = false;

                        if (args.Buff.Name == "FlaskOfCrystalWater")
                            hero.UsingManaPot = false;

                        if (args.Buff.Name == "ItemCrystalFlask" ||
                            args.Buff.Name == "ItemMiniRegenPotion")
                        {
                            hero.UsingMixedPot = false;
                        }
                    }

                    if (hero.QSSBuffCount == 0)
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
