using System;
using System.Linq;
using System.Collections.Generic;
using LeagueSharp;
using LeagueSharp.Common;
using OC = Oracle.Oracle;

namespace Oracle.Extensions
{
    // Create our contructor/class
    internal class SmiteChamp
    {
        public string Name;
        public float Range;
        public SpellSlot Slot;
        public string Type;
        public int Stage;

        public SmiteChamp(string skinname, float range, SpellSlot slot, string type, int stage = 0)
        {
            Name = skinname;
            Range = range;
            Slot = slot;
            Type = type;
            Stage = stage;
        }
    }

    internal static class Summoners
    {
        private static Menu MainMenu;
        private static Obj_AI_Hero HeroUnit;
        private static Obj_AI_Hero HeroTarget;
        private static string[] HeroSummoners;
        private static string[] SmiteSlots =
        {
            "summonersmite", "itemsmiteaoe", "s5_summonersmitequick", "s5_summonersmiteduel",
            "s5_summonersmiteplayerganker"
        };

        // Set local player (me)
        private static readonly Obj_AI_Hero Me = ObjectManager.Player;

        // Create smite list
        private static readonly List<SmiteChamp> SmiteList = new List<SmiteChamp>();

        public static void Initialize(Menu root)
        {           
            // GameOnGameUpdate Event
            Game.OnGameUpdate += Game_OnGameUpdate;

            // Smite Drawings
            Drawing.OnDraw += Drawing_OnDraw;

            // Get summoners
            HeroSummoners = new[] {OC.Summoner1, OC.Summoner2};

            // Create menu
            MainMenu = new Menu("Summoners", "summoners");

            // Validate summoner spell slots
            if (HeroSummoners.Any(x => x == "summonerdot"))
            {
                var menu = new Menu("Ignite", "mignite");
                menu.AddItem(new MenuItem("useIgnite", "Enable Ignite")).SetValue(true);
                menu.AddItem(new MenuItem("dotMode", "Mode: ")).SetValue(new StringList(new[] { "KSMode", "Combo" }, 1));
                MainMenu.AddSubMenu(menu);
            }

            if (HeroSummoners.Any(x => x == "summonerheal"))
            {
                var menu = new Menu("Heal", "mheal");
                menu.AddItem(new MenuItem("useHeal", "Enable Heal")).SetValue(true);
                menu.AddItem(new MenuItem("useHealPct", "Heal on min HP % ")).SetValue(new Slider(35, 1));
                menu.AddItem(new MenuItem("useHealDmg", "Heal on damage %")).SetValue(new Slider(40, 1));
                MainMenu.AddSubMenu(menu);
            }

            if (HeroSummoners.Any(x => x == "summonerexhaust"))
            {
                var menu = new Menu("Exhaust", "mexhaust");
                menu.AddItem(new MenuItem("useExhaust", "Enable Exhaust")).SetValue(true);
                menu.AddItem(new MenuItem("exDanger", "Use on Dangerous")).SetValue(true);
                menu.AddItem(new MenuItem("aExhaustPct", "Exhaust on ally HP %")).SetValue(new Slider(35));
                menu.AddItem(new MenuItem("eExhaustPct", "Exhaust on enemy HP %")).SetValue(new Slider(35));
                menu.AddItem(new MenuItem("exhaustMode", "Mode: ")).SetValue(new StringList(new[] { "Always", "Combo" }));
                MainMenu.AddSubMenu(menu);
            }

            if (HeroSummoners.Any(x => x == "summonerbarrier"))
            {
                var menu = new Menu("Barrier", "mbarrier");
                menu.AddItem(new MenuItem("useBarrier", "Enable Barrier")).SetValue(true);
                menu.AddItem(new MenuItem("useBarrierPct", "Barrior on min HP % ")).SetValue(new Slider(35, 1));
                menu.AddItem(new MenuItem("useBarrierDmg", "Barrier on damage %")).SetValue(new Slider(40, 1));
                MainMenu.AddSubMenu(menu);
            }

            if (HeroSummoners.Any(x => x == "summonermana"))
            {
                var menu = new Menu("Clarity", "mclarity");
                menu.AddItem(new MenuItem("useClarity", "Enable Clarity")).SetValue(true);
                menu.AddItem(new MenuItem("useClarityPct", "Clarity on Mana % ")).SetValue(new Slider(40, 1));
                MainMenu.AddSubMenu(menu);
            }

            if (HeroSummoners.Any() == SmiteSlots.Any())
            {
                var menu = new Menu("Smite", "msmite");
                menu.AddItem(new MenuItem("useSmite", "Use Smite")).SetValue(new KeyBind(77, KeyBindType.Toggle, true));
                menu.AddItem(new MenuItem("smiteSpell", "Use Smite + Ability")).SetValue(true);
                menu.AddItem(new MenuItem("drawSmite", "Draw smite range")).SetValue(true);
                menu.AddItem(new MenuItem("smiteSmall", "Smite small camps")).SetValue(true);
                menu.AddItem(new MenuItem("smiteLarge", "Smite large camps")).SetValue(true);
                menu.AddItem(new MenuItem("smiteEpic", "Smite epic camps")).SetValue(true);
                MainMenu.AddSubMenu(menu);
            }

            // Add menu to root
            root.AddSubMenu(MainMenu);

            #region SmiteList

            // Populate smite champ list
            SmiteList.Add(new SmiteChamp("Vi", 125f, SpellSlot.E, "onlycast"));
            SmiteList.Add(new SmiteChamp("Warwick", 200f, SpellSlot.Q, "targetspell"));
            SmiteList.Add(new SmiteChamp("MasterYi", 600f, SpellSlot.Q, "targetspell"));
            SmiteList.Add(new SmiteChamp("Kayle", 650f, SpellSlot.Q, "targetspell"));
            SmiteList.Add(new SmiteChamp("Khazix", 125f, SpellSlot.Q, "targetspell"));
            SmiteList.Add(new SmiteChamp("MonkeyKing", 300f, SpellSlot.Q, "targetspell"));
            SmiteList.Add(new SmiteChamp("Elise", 425f, SpellSlot.Q, "targetspell"));
            SmiteList.Add(new SmiteChamp("Olaf", 325f, SpellSlot.E, "targetspell"));
            SmiteList.Add(new SmiteChamp("Nunu", 125f, SpellSlot.Q, "targetspell"));
            SmiteList.Add(new SmiteChamp("LeeSin", 1100f, SpellSlot.Q, "onlycast", 1));
            SmiteList.Add(new SmiteChamp("Malphite", 200f, SpellSlot.E, "onlycast"));
            SmiteList.Add(new SmiteChamp("Riven", 175f, SpellSlot.W, "onlycast"));
            SmiteList.Add(new SmiteChamp("Nasus", 125f, SpellSlot.Q, "targetspell"));
            SmiteList.Add(new SmiteChamp("Poppy", 125f, SpellSlot.Q, "targetspell"));
            SmiteList.Add(new SmiteChamp("JarvanIV", 700f, SpellSlot.Q, "vectorspell"));
            SmiteList.Add(new SmiteChamp("Gangplank", 625f, SpellSlot.Q, "targetspell"));
            SmiteList.Add(new SmiteChamp("Jayce", 600f, SpellSlot.Q, "targetspell"));
            SmiteList.Add(new SmiteChamp("Aatrox", 1000f, SpellSlot.E, "vectorspell"));
            SmiteList.Add(new SmiteChamp("Amumu", 350f, SpellSlot.E, "onlycast"));
            SmiteList.Add(new SmiteChamp("Chogath", 125f, SpellSlot.R, "targetspell"));
            SmiteList.Add(new SmiteChamp("Nidalee", 300f, SpellSlot.E, "vectorspell"));
            #endregion
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            # region Smite : Drawings

            if (HeroSummoners.Any() == SmiteSlots.Any())
            {
                if (!MainMenu.Item("useSmite").GetValue<KeyBind>().Active || Me.IsDead)
                    return;

                // Minion names
                string[] epicminions = { "SRU_Baron", "SRU_Dragon", "TT_Spiderboss" };
                string[] largeminions = { "SRU_Blue", "SRU_Red", "TT_NWraith", "TT_NGolem", "TT_NWolf" };
                string[] smallminions = { "SRU_Murkwolf", "SRU_Razorbeak", "SRU_Krug", "SRU_Gromp", "Sru_Crab" };

                foreach (var minion in MinionManager.GetMinions(Me.Position, 760f, MinionTypes.All, MinionTeam.Neutral))
                {
                    bool valid;
                    if (Utility.Map.GetMap()._MapType.Equals(Utility.Map.MapType.TwistedTreeline))
                    {
                        valid = minion.IsHPBarRendered && !minion.IsDead &&
                                (largeminions.Any(
                                    x =>
                                        minion.Name.Substring(0, minion.Name.Length - 5).Equals(x) ||
                                        epicminions.Any(
                                            y => minion.Name.Substring(0, minion.Name.Length - 5).Equals(y))));
                    }

                    else
                    {
                        valid = minion.IsHPBarRendered && !minion.IsDead &&
                                (!minion.Name.Contains("Mini") &&
                                 (smallminions.Any(x => minion.Name.StartsWith(x)) ||
                                  largeminions.Any(y => minion.Name.StartsWith(y)) ||
                                  epicminions.Any(z => minion.Name.StartsWith(z))));
                    }

                    if (valid)
                    {
                        var hpBarPos = minion.HPBarPosition;
                            hpBarPos.X += 35;
                            hpBarPos.Y += 18;

                        var smiteDmg = (int) Me.GetSummonerSpellDamage(minion, Damage.SummonerSpell.Smite);
                        var damagePercent = smiteDmg / minion.MaxHealth;
                        var hpXPos = hpBarPos.X + (63 * damagePercent);

                        Drawing.DrawLine(hpXPos, hpBarPos.Y, hpXPos, hpBarPos.Y + 5, 2,
                            smiteDmg > minion.Health ? System.Drawing.Color.Lime : System.Drawing.Color.White);
                    }
                }
            }

            #endregion
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            HeroTarget = OC.HeroTarget;
            HeroUnit = OC.HeroUnit;

            Console.WriteLine("Summoners.cs Debug: " + HeroUnit.SkinName);


            #region Summoners : Smite

            if (HeroSummoners.Any() == SmiteSlots.Any())
            {
                // Minion names
                string[] epicminions = {"SRU_Baron", "SRU_Dragon", "TT_Spiderboss"};
                string[] largeminions = {"SRU_Blue", "SRU_Red", "TT_NWraith", "TT_NGolem", "TT_NWolf"};
                string[] smallminions = {"SRU_Murkwolf", "SRU_Razorbeak", "SRU_Krug", "SRU_Gromp", "Sru_Crab"};

                var smite = Me.GetSpellSlot("summonersmite");
                if (Me.Spellbook.CanUseSpell(smite) == SpellState.Ready && MainMenu.Item("useSmite").GetValue<KeyBind>().Active)
                {
                    // todo: check jayce/nidalee forms
                    foreach (
                        var minion in 
                            MinionManager.GetMinions(Me.Position, 760f, MinionTypes.All, MinionTeam.Neutral))
                    {
                        var admg = 0f;
                        var type = string.Empty;
                        var slot = new SpellSlot();

                        // get spell data (may be useful)
                        SpellDataInst inst = null;

                        var damage = (float)Me.GetSummonerSpellDamage(minion, Damage.SummonerSpell.Smite);

                        if (MainMenu.Item("smiteSpell").GetValue<bool>())
                        {
                            foreach (var i in SmiteList.Where(x => x.Name == Me.SkinName))
                            {
                                admg = (float) (Me.Spellbook.CanUseSpell(i.Slot) == SpellState.Ready
                                           ? Me.GetSpellDamage(minion, i.Slot)
                                           : 0);
                                type = i.Type;
                                slot = i.Slot;
                                inst = Me.Spellbook.GetSpell(slot);
                            }
                        }
                         
                        if (smallminions.Any(name => minion.Name.StartsWith(name) && !minion.Name.Contains("Mini")))
                        {
                            if (minion.Health <= damage + admg && MainMenu.Item("smiteSmall").GetValue<bool>())
                            {
                                if (Me.Spellbook.CanUseSpell(slot) == SpellState.Ready)
                                {
                                    switch (type)
                                    {
                                        case "targetspell":
                                            Me.Spellbook.CastSpell(slot, minion);
                                            break;
                                        case "vectorspell":
                                            Me.Spellbook.CastSpell(slot, minion.ServerPosition);
                                            break;
                                        case "onlycast":
                                            Me.Spellbook.CastSpell(slot);
                                            break;
                                    }
                                }
                            }

                            if (minion.Health <= damage && MainMenu.Item("smiteSmall").GetValue<bool>())
                            {
                                // Lee check
                                if (inst != null && (Me.SkinName == "LeeSin" && inst.Name != "blindmonkqtwo" && 
                                                     !minion.HasBuff("BlindMonkSonicWave", true)))
                                    return;
                              
                                Me.Spellbook.CastSpell(smite, minion);
                            }

                        }

                        else if (largeminions.Any(name => minion.Name.StartsWith(name) && !minion.Name.Contains("Mini")))
                        {
                            if (minion.Health <= damage + admg && MainMenu.Item("smiteSmall").GetValue<bool>())
                            {
                                if (Me.Spellbook.CanUseSpell(slot) == SpellState.Ready)
                                {
                                    switch (type)
                                    {
                                        case "targetspell":
                                            Me.Spellbook.CastSpell(slot, minion);
                                            break;
                                        case "vectorspell":
                                            Me.Spellbook.CastSpell(slot, minion.ServerPosition);
                                            break;
                                        case "onlycast":
                                            Me.Spellbook.CastSpell(slot);
                                            break;
                                    }
                                }
                            }

                            if (minion.Health <= damage && MainMenu.Item("smiteLarge").GetValue<bool>())
                            {
                                // Lee check
                                if (inst != null && (Me.SkinName == "LeeSin" && inst.Name != "blindmonkqtwo" &&
                                                     !minion.HasBuff("BlindMonkSonicWave", true)))
                                    return;

                                Me.Spellbook.CastSpell(smite, minion);
                            }
                        }

                        else if (epicminions.Any(name => minion.Name.StartsWith(name) && !minion.Name.Contains("Mini")))
                        {
                            if (minion.Health <= damage + admg && MainMenu.Item("smiteSmall").GetValue<bool>())
                            {
                                if (Me.Spellbook.CanUseSpell(slot) == SpellState.Ready)
                                {
                                    switch (type)
                                    {
                                        case "targetspell":
                                            Me.Spellbook.CastSpell(slot, minion);
                                            break;
                                        case "vectorspell":
                                            Me.Spellbook.CastSpell(slot, minion.ServerPosition);
                                            break;
                                        case "onlycast":
                                            Me.Spellbook.CastSpell(slot);
                                            break;
                                    }
                                }
                            }

                            if (minion.Health <= damage && MainMenu.Item("smiteEpic").GetValue<bool>())
                            {
                                // Lee check
                                if (inst != null && (Me.SkinName == "LeeSin" && inst.Name != "blindmonkqtwo" &&
                                                     !minion.HasBuff("BlindMonkSonicWave", true)))
                                    return;

                                Me.Spellbook.CastSpell(smite, minion);
                            }
                        }
                    }
                }
            }

            #endregion

            #region Summoners : Ignite

            if (HeroSummoners.Any(x => x == "summonerdot"))
            {
                var ignite = Me.GetSpellSlot("summonerdot");
                if (Me.Spellbook.CanUseSpell(ignite) == SpellState.Ready)
                {
                    switch (MainMenu.Item("dotMode").GetValue<StringList>().SelectedIndex)
                    {
                        case 0:

                            // KS Ignite
                            foreach ( var target in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsValidTarget(600)))
                            {
                                if (target.Health <= Me.GetSummonerSpellDamage(target, Damage.SummonerSpell.Smite))
                                {
                                    if (!target.HasBuff("summonerdot", true))
                                    {
                                        Me.Spellbook.CastSpell(ignite, target);
                                    }
                                }
                            }
                            break;
                        case 1:

                            // Combo ignite
                            if (OC.MainMenu.Item("usecombo").GetValue<KeyBind>().Active)
                            {
                                var damage = 0f;
                                var aa = Me.FlatPhysicalDamageMod + Me.BaseAttackDamage;

                                if (Me.AttackSpeedMod < 0.8f)
                                    damage = aa*3;
                                else if (Me.AttackSpeedMod > 1f && Me.AttackSpeedMod < 1.3f)
                                    damage = Me.FlatPhysicalDamageMod * 5;
                                else if (Me.AttackSpeedMod > 1.3f && Me.AttackSpeedMod < 1.5f)
                                    damage = Me.FlatPhysicalDamageMod * 7;
                                else if (Me.AttackSpeedMod > 1.5f && Me.AttackSpeedMod < 1.7f)
                                    damage = Me.FlatPhysicalDamageMod * 9;
                                else if (Me.AttackSpeedMod > 2.0f)
                                    damage = Me.FlatPhysicalDamageMod * 11;

                                var target = SimpleTs.GetTarget(900f, SimpleTs.DamageType.Physical);
                                if (target.IsValidTarget(600))
                                {
                                    var dmg = (Me.Level*20) + 50;
                                    var regen = (target.FlatHPRegenMod + (target.HPRegenRate*target.Level));
                                    var dmgafter = (dmg - ((regen*5)/2));

                                    if (target.Health <= dmgafter + damage && !target.HasBuff("summonerdot", true))
                                        Me.Spellbook.CastSpell(ignite, target);
                                }
                            }
                            break;
                    }
                }
            }
            #endregion

            #region Summoners : Clarity

            if (HeroSummoners.Any(x => x == "summonermana"))
            {
                var clarity = Me.GetSpellSlot("summonermana");
                if (Me.Spellbook.CanUseSpell(clarity) == SpellState.Ready)
                {
                    if (MainMenu.Item("useClarity").GetValue<bool>() && !Utility.InFountain())
                    {
                        var manaPercent = HeroUnit.Mana/HeroUnit.MaxMana*100;
                        if (manaPercent <= MainMenu.Item("useClarityPct").GetValue<Slider>().Value)
                        {
                            Me.Spellbook.CastSpell(clarity);
                        }
                    }
                }
            }

            #endregion

            #region Summoners : Barrier

            if (HeroSummoners.Any(x => x == "summonerbarrier"))
            {
                var barrier = Me.GetSpellSlot("summonerbarrier");
                if (Me.Spellbook.CanUseSpell(barrier) == SpellState.Ready)
                {
                    var healthPercent = HeroUnit.Health/HeroUnit.MaxHealth*100;
                    if (healthPercent <= MainMenu.Item("useBarrierPct").GetValue<Slider>().Value)
                    {
                        Me.Spellbook.CastSpell(barrier);
                    }

                    // todo : check pct damage taken
                }
            }

            #endregion

            #region Summoners : Heal

            if (HeroSummoners.Any(x => x == "summonerheal"))
            {
                var heal = Me.GetSpellSlot("summonerheal");
                if (Me.Spellbook.CanUseSpell(heal) == SpellState.Ready)
                {
                    var healthPercent = HeroUnit.Health / HeroUnit.MaxHealth * 100;
                    if (healthPercent <= MainMenu.Item("useHealrPct").GetValue<Slider>().Value)
                    {
                        Me.Spellbook.CastSpell(heal);
                    }

                    // todo : check pct damage taken
                }
            }

            #endregion

            #region Summoners : Exhaust

            if (HeroSummoners.Any(x => x == "summonerexhaust"))
            {
                var exhaust = Me.GetSpellSlot("summonerexhaust");
                if (Me.Spellbook.CanUseSpell(exhaust) == SpellState.Ready)
                {
                    if (!OC.MainMenu.Item("combokey").GetValue<KeyBind>().Active &&
                        MainMenu.Item("exhaustMode").GetValue<StringList>().SelectedIndex == 1)
                        return;

                    foreach (
                        var enemy in
                            ObjectManager.Get<Obj_AI_Hero>()
                                .Where(hero => hero.IsValidTarget(650))
                                .OrderByDescending(hero => hero.BaseAttackDamage + hero.FlatPhysicalDamageMod))
                    {
                        // todo: check team and shit

                        var healthPercent = Me.Health/Me.MaxHealth*100;
                        if (healthPercent <= MainMenu.Item("aExhaustPct").GetValue<Slider>().Value)
                        {
                            if (enemy.IsFacing(Me))
                                Me.Spellbook.CastSpell(exhaust, enemy);
                        }
                    }
                }
            }
            #endregion

            #region Summoners : Cleanse // Incomplete




            #endregion
        }
    }
}
