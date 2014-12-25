using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;
using LeagueSharp;
using LeagueSharp.Common;

namespace Oracle.Extensions
{
    internal static class Summoners
    {
        private static Menu MainMenu;
        private static Menu MenuConfig;
        private static string[] HeroSummoners;

        // Smite spellslots
        private static string[] SmiteSlots =
        {
            "itemsmiteaoe", 
            "summonersmite", 
            "s5_summonersmitequick", 
            "s5_summonersmiteduel",
            "s5_summonersmiteplayerganker"
        };

        // Set local player (me)
        private static readonly Obj_AI_Hero Me = ObjectManager.Player;

        // Create smite list
        private static readonly List<OracleChamp> SmiteList = new List<OracleChamp>();

        public static void Initialize(Menu root)
        {           
            // GameOnGameUpdate Event
            Game.OnGameUpdate += Game_OnGameUpdate;

            // Smite Drawings
            Drawing.OnDraw += Drawing_OnDraw;

            // Get summoners
            HeroSummoners = new[] {Oracle.Summoner1, Oracle.Summoner2};

            // Create menu
            MainMenu = new Menu("Summoners", "summoners");

            foreach (var x in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.Team != Me.Team))
                MenuConfig.AddItem(new MenuItem("oson" + x.SkinName, "Use for " + x.SkinName)).SetValue(true);
            MainMenu.AddSubMenu(MenuConfig);

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
            SmiteList.Add(new OracleChamp("Vi", 125f, SpellSlot.E, "onlycast"));
            SmiteList.Add(new OracleChamp("Warwick", 200f, SpellSlot.Q, "targetspell"));
            SmiteList.Add(new OracleChamp("MasterYi", 600f, SpellSlot.Q, "targetspell"));
            SmiteList.Add(new OracleChamp("Kayle", 650f, SpellSlot.Q, "targetspell"));
            SmiteList.Add(new OracleChamp("Khazix", 125f, SpellSlot.Q, "targetspell"));
            SmiteList.Add(new OracleChamp("MonkeyKing", 300f, SpellSlot.Q, "targetspell"));
            SmiteList.Add(new OracleChamp("Elise", 425f, SpellSlot.Q, "targetspell"));
            SmiteList.Add(new OracleChamp("Olaf", 325f, SpellSlot.E, "targetspell"));
            SmiteList.Add(new OracleChamp("Nunu", 125f, SpellSlot.Q, "targetspell"));
            SmiteList.Add(new OracleChamp("LeeSin", 1100f, SpellSlot.Q, "onlycast", 1));
            SmiteList.Add(new OracleChamp("Malphite", 200f, SpellSlot.E, "onlycast"));
            SmiteList.Add(new OracleChamp("Riven", 175f, SpellSlot.W, "onlycast"));
            SmiteList.Add(new OracleChamp("Nasus", 125f, SpellSlot.Q, "targetspell"));
            SmiteList.Add(new OracleChamp("Poppy", 125f, SpellSlot.Q, "targetspell"));
            SmiteList.Add(new OracleChamp("JarvanIV", 700f, SpellSlot.Q, "vectorspell"));
            SmiteList.Add(new OracleChamp("Gangplank", 625f, SpellSlot.Q, "targetspell"));
            SmiteList.Add(new OracleChamp("Jayce", 600f, SpellSlot.Q, "targetspell"));
            SmiteList.Add(new OracleChamp("Aatrox", 1000f, SpellSlot.E, "vectorspell"));
            SmiteList.Add(new OracleChamp("Amumu", 350f, SpellSlot.E, "onlycast"));
            SmiteList.Add(new OracleChamp("Chogath", 125f, SpellSlot.R, "targetspell"));
            SmiteList.Add(new OracleChamp("Nidalee", 300f, SpellSlot.E, "vectorspell"));

            Oracle.Logger(Oracle.LogType.Info, "Oracle: Smite -- Initialized", true);
            #endregion

            if (Oracle.HeroTarget == null)
                Oracle.Logger(Oracle.LogType.Error, "HeroTarget is null!", true);
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            #region Smite : Drawings

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
            #region Summoners : Smite

            if (HeroSummoners.Any() == SmiteSlots.Any())
            {
                // Minion names
                string[] epicminions = {"SRU_Baron", "SRU_Dragon", "TT_Spiderboss"};
                string[] largeminions = {"SRU_Blue", "SRU_Red", "TT_NWraith", "TT_NGolem", "TT_NWolf"};
                string[] smallminions = {"SRU_Murkwolf", "SRU_Razorbeak", "SRU_Krug", "SRU_Gromp", "Sru_Crab"};

                var smite = Me.GetSpellSlot("summonersmite");
                if (Me.Spellbook.CanUseSpell(smite) != SpellState.Ready ||
                    !MainMenu.Item("useSmite").GetValue<KeyBind>().Active)
                    return;

                // todo: check jayce/nidalee forms
                foreach (var minion in 
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
                            switch (type)
                            {
                                case "targetspell":
                                    Me.Spellbook.CastSpell(slot, minion);
                                    Oracle.Logger(Oracle.LogType.Info,
                                        "Smite Casting (small-target) spell on " + minion.Name + " (" + minion.Health + " HP)");
                                    break;
                                case "vectorspell":
                                    Me.Spellbook.CastSpell(slot, minion.ServerPosition);
                                    Oracle.Logger(Oracle.LogType.Info,
                                        "Smite Casting (small - vector) spell on " + minion.Name + " (" + minion.Health + " HP)");
                                    break;
                                case "onlycast":
                                    Me.Spellbook.CastSpell(slot);
                                    Oracle.Logger(Oracle.LogType.Info,
                                        "Smite Casting (small - self) spell on " + minion.Name + " (" + minion.Health + " HP)");
                                    break;
                            }
                        }

                        if (minion.Health <= damage && MainMenu.Item("smiteSmall").GetValue<bool>())
                        {
                            // Lee check
                            if (inst != null && (Me.SkinName == "LeeSin" && inst.Name != "blindmonkqtwo" && 
                                                    !minion.HasBuff("BlindMonkSonicWave", true)))
                                return;
                              
                            Me.Spellbook.CastSpell(smite, minion);
                            Oracle.Logger(Oracle.LogType.Info,
                                "Casting smite on small " + minion.Name + " (" + minion.Health + " HP)");
                        }
                    }
             

                    else if (largeminions.Any(name => minion.Name.StartsWith(name) && !minion.Name.Contains("Mini")))
                    {
                        if (minion.Health <= damage + admg && MainMenu.Item("smiteLarge").GetValue<bool>())
                        {
                            switch (type)
                            {
                                case "targetspell":
                                    Me.Spellbook.CastSpell(slot, minion);
                                    Oracle.Logger(Oracle.LogType.Info,
                                        "Smite Casting (large - target) spell on " + minion.Name + " (" + minion.Health + " HP)");
                                    break;
                                case "vectorspell":
                                    Me.Spellbook.CastSpell(slot, minion.ServerPosition);
                                    Oracle.Logger(Oracle.LogType.Info,
                                        "Smite Casting (large - vector) spell on " + minion.Name + " (" + minion.Health + " HP)");
                                    break;
                                case "onlycast":
                                    Me.Spellbook.CastSpell(slot);
                                    Oracle.Logger(Oracle.LogType.Info,
                                        "Smite Casting (large - self) spell on " + minion.Name + " (" + minion.Health + " HP)");
                                    break;
                            }
                                
                        }

                        if (minion.Health <= damage && MainMenu.Item("smiteLarge").GetValue<bool>())
                        {
                            // Lee check
                            if (inst != null && (Me.SkinName == "LeeSin" && inst.Name != "blindmonkqtwo" &&
                                                    !minion.HasBuff("BlindMonkSonicWave", true)))
                                return;

                            Me.Spellbook.CastSpell(smite, minion);
                            Oracle.Logger(Oracle.LogType.Info,
                                "Casting smite on large " + minion.Name + " (" + minion.Health + " HP)");
                        }
                    }

                    else if (epicminions.Any(name => minion.Name.StartsWith(name) && !minion.Name.Contains("Mini")))
                    {
                        if (minion.Health <= damage + admg && MainMenu.Item("smiteEpic").GetValue<bool>())
                        {
                            switch (type)
                            {
                                case "targetspell":
                                    Me.Spellbook.CastSpell(slot, minion);
                                    Oracle.Logger(Oracle.LogType.Info,
                                        "Smite Casting (epic - self) spell on " + minion.Name + " (" + minion.Health + " HP)");
                                    break;
                                case "vectorspell":
                                    Me.Spellbook.CastSpell(slot, minion.ServerPosition);
                                    Oracle.Logger(Oracle.LogType.Info,
                                        "Smite Casting (epic - self) spell on " + minion.Name + " (" + minion.Health + " HP)");
                                    break;
                                case "onlycast":
                                    Me.Spellbook.CastSpell(slot);
                                    Oracle.Logger(Oracle.LogType.Info,
                                        "Smite Casting (epic - self) spell on " + minion.Name + " (" + minion.Health + " HP)");
                                    break;
                            }                           
                        }

                        if (minion.Health <= damage && MainMenu.Item("smiteEpic").GetValue<bool>())
                        {
                            // Lee check
                            if (inst != null && (Me.SkinName == "LeeSin" && inst.Name != "blindmonkqtwo" &&
                                                    !minion.HasBuff("BlindMonkSonicWave", true)))
                                return;

                            Me.Spellbook.CastSpell(smite, minion);
                            Oracle.Logger(Oracle.LogType.Info,
                                        "Casting smite on " + minion.Name + " (" + minion.Health + " HP)");
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
                                    var targetPercent = target.Health / target.MaxHealth * 100;
                                    if (!target.HasBuff("summonerdot", true))
                                    {
                                        Me.Spellbook.CastSpell(ignite, target);
                                        Oracle.Logger(Oracle.LogType.Info,
                                            "Casting Ignite (KSMode) on " + target.ChampionName + " (" + targetPercent + "%)");
                                    }
                                }
                            }
                            break;
                        case 1:

                            // Combo ignite
                            if (Oracle.MainMenu.Item("usecombo").GetValue<KeyBind>().Active)
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

                                    var targetPercent = target.Health/target.MaxHealth*100;
                                    if (target.Health <= dmgafter + damage && !target.HasBuff("summonerdot", true))
                                    {
                                        Me.Spellbook.CastSpell(ignite, target);
                                        Oracle.Logger(Oracle.LogType.Info,
                                            "Casting Ignite (Combo) on " + target.ChampionName + " (" + targetPercent + "%)");
                                    }
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
                if (Me.Spellbook.CanUseSpell(clarity) != SpellState.Ready)
                    return;

                if (MainMenu.Item("useClarity").GetValue<bool>() && !Utility.InFountain())
                {
                    if (Oracle.HeroUnit.Distance(Me.ServerPosition, true) <= 700f) // todo: verify clarity range
                    {
                        var manaPercent = Oracle.HeroUnit.Mana/Oracle.HeroUnit.MaxMana*100;
                        if (manaPercent <= MainMenu.Item("useClarityPct").GetValue<Slider>().Value)
                        {
                            Me.Spellbook.CastSpell(clarity);
                            Oracle.Logger(Oracle.LogType.Info,
                                "Casting clarity on " + Oracle.HeroUnit.ChampionName + " (" + manaPercent + "%)");
                        }
                    }
                }             
            }

            #endregion

            #region Summoners : Barrier

            if (HeroSummoners.Any(x => x == "summonerbarrier"))
            {
                var barrier = Me.GetSpellSlot("summonerbarrier");
                if (Me.Spellbook.CanUseSpell(barrier) != SpellState.Ready)
                    return;

                var incomePercent = Oracle.HeroDamage/Oracle.HeroUnit.MaxHealth*100;
                var aHealthPercent = Me.Health / Me.MaxHealth * 100;

                if (aHealthPercent <= MainMenu.Item("useBarrierPct").GetValue<Slider>().Value)
                {
                    if (incomePercent >= 1 || Oracle.HeroDamage >= Oracle.HeroUnit.Health)
                    {
                        Me.Spellbook.CastSpell(barrier);
                        Oracle.Logger(Oracle.LogType.Info,
                            "Casting barrier on " + Oracle.HeroUnit.ChampionName + " (" + aHealthPercent + "%)");
                    }

                    var incomeSetting = MainMenu.Item("useBarrierDmg").GetValue<Slider>().Value;
                    if (incomePercent >= MainMenu.Item("useBarrierDmg").GetValue<Slider>().Value)
                    {
                        Me.Spellbook.CastSpell(barrier);
                        Oracle.Logger(Oracle.LogType.Info,
                            "Casting barrier because incomedamage percent is above our setting (" + incomeSetting  + "%)");
                    }
                }                  
                
            }

            #endregion

            #region Summoners : Heal

            if (HeroSummoners.Any(x => x == "summonerheal"))
            {
                var heal = Me.GetSpellSlot("summonerheal");
                if (Me.Spellbook.CanUseSpell(heal) != SpellState.Ready)
                    return;

                // Check if unit is in heal range
                if (Oracle.HeroUnit.Distance(Me.ServerPosition, true) <= 750 * 750) // todo : verify exhaust range
                {
                    var iDamagePercent = Oracle.HeroDamage/Oracle.HeroUnit.MaxHealth*100;
                    var aHealthPercent = Oracle.HeroUnit.Health/Oracle.HeroUnit.MaxHealth*100;

                    if (aHealthPercent <= MainMenu.Item("useHealrPct").GetValue<Slider>().Value)
                    {
                        if (iDamagePercent >= 1 || Oracle.HeroDamage >= Oracle.HeroUnit.Health)
                            Me.Spellbook.CastSpell(heal);
                    }

                    if (iDamagePercent >= MainMenu.Item("useHealDmg").GetValue<Slider>().Value)
                        Me.Spellbook.CastSpell(heal);
                }
            }

            #endregion

            #region Summoners : Exhaust

            if (HeroSummoners.Any(x => x == "summonerexhaust"))
            {
                var exhaust = Me.GetSpellSlot("summonerexhaust");
                if (Me.Spellbook.CanUseSpell(exhaust) != SpellState.Ready) 
                    return;

                if (!Oracle.MainMenu.Item("combokey").GetValue<KeyBind>().Active &&
                    MainMenu.Item("exhaustMode").GetValue<StringList>().SelectedIndex == 1)
                    return;

                foreach (
                    var enemy in
                        ObjectManager.Get<Obj_AI_Hero>()
                            .Where(hero => hero.IsValidTarget(650)) // todo : verify exhaust range
                            .OrderByDescending(hero => hero.BaseAttackDamage + hero.FlatPhysicalDamageMod)) 
                {
                    var eHealthPercent = enemy.Health/enemy.MaxHealth*100;
                    var aHealthPercent = Oracle.HeroUnit.Health/Oracle.HeroUnit.MaxHealth*100;

                    if (eHealthPercent <= MainMenu.Item("eExhaustPct").GetValue<Slider>().Value && 
                        !enemy.IsFacing(Oracle.HeroUnit))
                    {
                        Me.Spellbook.CastSpell(exhaust, enemy);
                        Oracle.Logger(Oracle.LogType.Info,
                            "Casting exhaust on " + enemy.ChampionName + " (" + eHealthPercent + "%)");
                    }

                    if (aHealthPercent <= MainMenu.Item("aExhaustPct").GetValue<Slider>().Value &&
                        enemy.IsFacing(Oracle.HeroUnit))
                    {
                        Me.Spellbook.CastSpell(exhaust, enemy);
                        Oracle.Logger(Oracle.LogType.Info,
                            "Casting exhaust because herounit health percent was at (" + aHealthPercent + "%)");
                    }
                }
            }

            #endregion

            #region Summoners : Cleanse // Incomplete




            #endregion
        }
    }

    internal class OracleChamp
    {
        public string Name;
        public float Range;
        public SpellSlot Slot;
        public string Type;
        public int Stage;

        public OracleChamp(string skinname, float range, SpellSlot slot, string type, int stage = 0)
        {
            Name = skinname;
            Range = range;
            Slot = slot;
            Type = type;
            Stage = stage;
        }
    }
}
