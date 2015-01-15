using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using Oracle.Extensions;
using Oracle.Core.Skillshots;
using Oracle.Core.Targeted;
using SpellSlot = LeagueSharp.SpellSlot;

namespace Oracle
{
    //  _____             _     
    // |     |___ ___ ___| |___ 
    // |  |  |  _| .'|  _| | -_|
    // |_____|_| |__,|___|_|___|
    // Copyright © Kurisu Solutions 2014

    internal static class Program
    {
        public const string Revision = "202";
        public static Menu Origin;
        public static string ChampionName;
        public static Obj_AI_Hero Sender;
        public static Obj_AI_Hero AggroTarget;
        public static Obj_AI_Hero CurrentTarget;
        public static float IncomeDamage, MinionDamage;
        private static readonly Obj_AI_Hero Me = ObjectManager.Player;

        private static void Main(string[] args)
        {
            Console.WriteLine("Oracle is loading...");
            CustomEvents.Game.OnGameLoad += OnGameLoad;
        }

        private static void OnGameLoad(EventArgs args)
        {
            Game.OnGameUpdate += Game_OnGameUpdate;

            ChampionName = Me.ChampionName;
            Origin = new Menu("Oracle", "oracle", true);

            //Cleansers.Initialize(Origin);
            Defensives.Initialize(Origin);
            Summoners.Initialize(Origin);
            Offensives.Initialize(Origin);
            Consumables.Initialize(Origin);
            AutoSpells.Initialize(Origin);

            var Config = new Menu("Oracle Config", "oracleconfig");
            var DangerMenu = new Menu("Dangerous Config", "dangerconfig");

            foreach (var i in ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.Team != Me.Team))
            {
                var menu = new Menu(i.SkinName, i.SkinName + "cccmenu");
                foreach (
                    var spell in
                        TargetSpellDatabase.Spells.Where(spell => spell.ChampionName == i.ChampionName.ToLower()))
                {
                    var danger = spell.Spellslot.ToString() == "R" ||
                                 spell.CcType != CcType.No && (spell.Type == SpellType.Skillshot || spell.Type == SpellType.Targeted);

                    menu.AddItem(new MenuItem(spell.Name + "ccc", spell.Name + " | " + spell.Spellslot)).SetValue(danger);
                }

                DangerMenu.AddSubMenu(menu);
            }

            Config.AddSubMenu(DangerMenu);
          
            var DebugMenu = new Menu("Debugging", "debugmenu");
            DebugMenu.AddItem(new MenuItem("dbool", "Enable Console Debugging")).SetValue(false);
            Config.AddSubMenu(DebugMenu);


            Origin.AddSubMenu(Config);

            Origin.AddItem(
                new MenuItem("ComboKey", "Combo (Active)")
                    .SetValue(new KeyBind(32, KeyBindType.Press)));

            Origin.AddToMainMenu();
            HeroCheck();

            // Events
            GameObject.OnCreate += GameObject_OnCreate;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;

            Game.PrintChat("<font color=\"#1FFF8F\">Oracle r." + Revision + " -</font> by Kurisu");
        }

        public static bool Spell;
        public static bool Stealth;
        public static bool Danger;
        public static bool DangerCC;
        public static bool DangerUlt;
        public static bool CanManamune;

        private static GameObj _satchel;
        private static GameObj _miasma;
        private static GameObj _minesield;
        private static GameObj _chaosstorm;
        private static GameObj _glacialstorm;
        private static GameObj _crowstorm;
        private static GameObj _lightStrike;
        private static GameObj _equinox;
        private static GameObj _tormentsoil;

        private static Obj_AI_Hero _viktor;
        private static Obj_AI_Hero _fiddle;
        private static Obj_AI_Hero _anivia;
        private static Obj_AI_Hero _ziggs;
        private static Obj_AI_Hero _cass;
        private static Obj_AI_Hero _lux;
        private static Obj_AI_Hero _morgana;
        private static Obj_AI_Hero _soraka;
        private static Obj_AI_Hero _vayne;

        private static void GameObject_OnCreate(GameObject obj, EventArgs args)
        {
            if (obj.Name.Contains("Crowstorm_red") && _fiddle != null)
            {
                var dmg = (float)_fiddle.GetSpellDamage(FriendlyTarget(), SpellSlot.R);
                _crowstorm = new GameObj(obj.Name, obj, true, dmg);
            }

            else if (obj.Name.Contains("LuxLightstrike_tar_red") && _lux != null)
            {
                var dmg = (float)_lux.GetSpellDamage(FriendlyTarget(), SpellSlot.E);
                _lightStrike = new GameObj(obj.Name, obj, true, dmg);
            }

            else if (obj.Name.Contains("Viktor_ChaosStorm_red") && _viktor != null)
            {
                var dmg = (float)_viktor.GetSpellDamage(FriendlyTarget(), SpellSlot.R);
                _chaosstorm = new GameObj(obj.Name, obj, true, dmg);
            }

            else if (obj.Name.Contains("cryo_storm_red") && _anivia != null)
            {
                var dmg = (float)_anivia.GetSpellDamage(FriendlyTarget(), SpellSlot.R);
                _glacialstorm = new GameObj(obj.Name, obj, true, dmg);
            }

            else if (obj.Name.Contains("ZiggsE_red") && _ziggs != null)
            {
                var dmg = (float)_ziggs.GetSpellDamage(FriendlyTarget(), SpellSlot.E);
                _minesield = new GameObj(obj.Name, obj, true, dmg);
            }

            else if (obj.Name.Contains("ZiggsWRingRed") && _ziggs != null)
            {
                var dmg = (float)_ziggs.GetSpellDamage(FriendlyTarget(), SpellSlot.W);
                _satchel = new GameObj(obj.Name, obj, true, dmg);
            }

            else if (obj.Name.Contains("CassMiasma_tar_red") && _cass != null)
            {
                var dmg = (float)_cass.GetSpellDamage(FriendlyTarget(), SpellSlot.W);
                _miasma = new GameObj(obj.Name, obj, true, dmg);
            }

            else if (obj.Name.Contains("Soraka_Base_E_rune_RED") && _soraka != null)
            {
                var dmg = (float)_soraka.GetSpellDamage(FriendlyTarget(), SpellSlot.E);
                _equinox = new GameObj(obj.Name, obj, true, dmg);
            }

            else if (obj.Name.Contains("Morgana_Base_W_Tar_red") && _morgana != null)
            {
                var dmg = (float) _morgana.GetSpellDamage(FriendlyTarget(), SpellSlot.W);
                _tormentsoil = new GameObj(obj.Name, obj, true, dmg);
            }
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (IncomeDamage >= 1)
                Utility.DelayAction.Add(Game.Ping + 50, () => IncomeDamage = 0);
            if (MinionDamage >= 1)
                Utility.DelayAction.Add(Game.Ping + 50, () => MinionDamage = 0);

            // Get current target near mouse cursor.
            foreach (
                var targ in
                    ObjectManager.Get<Obj_AI_Hero>()
                        .Where(hero => hero.IsValidTarget(2000))
                        .OrderByDescending(hero => hero.Distance(Game.CursorPos)))
            {
                CurrentTarget = targ;
            }

            // Get ground object damage update
            if (_glacialstorm.Included)
            {
                if (_glacialstorm.Obj.IsValid && FriendlyTarget().Distance(_glacialstorm.Obj.Position, true) <= 400*400 && _anivia != null)
                {
                    Sender = _anivia;
                    AggroTarget = FriendlyTarget();
                    IncomeDamage = _glacialstorm.Damage;

                    if (Origin.Item("dbool").GetValue<bool>())
                        Console.WriteLine(
                            AggroTarget.SkinName + " is in Glacialstorm (Ground Object) for: " + IncomeDamage);
                }
            }

            if (_chaosstorm.Included)
            {
                if (_chaosstorm.Obj.IsValid && FriendlyTarget().Distance(_chaosstorm.Obj.Position, true) <= 400*400 && _viktor != null)
                {
                    Sender = _viktor;
                    AggroTarget = FriendlyTarget();
                    IncomeDamage = _chaosstorm.Damage;

                    if (AggroTarget.NetworkId == FriendlyTarget().NetworkId && Origin.Item("viktorchaosstormccc").GetValue<bool>())
                    {
                        if (FriendlyTarget().CountHerosInRange(true, 1000) >=
                            FriendlyTarget().CountHerosInRange(false, 1000) || IncomeDamage >= FriendlyTarget().Health)
                        {
                            Danger = true;
                            DangerUlt = true;
                        }
                    }

                    if (Origin.Item("dbool").GetValue<bool>())
                        Console.WriteLine(
                            AggroTarget.SkinName + " is in Chaostorm (Ground Object) for: " + IncomeDamage);
                }
            }

            if (_crowstorm.Included)
            {
                // 575 Fear Range
                if (_crowstorm.Obj.IsValid && FriendlyTarget().Distance(_crowstorm.Obj.Position, true) <= 575*575 && _fiddle != null)
                {
                    Sender = _fiddle;
                    AggroTarget = FriendlyTarget();
                    IncomeDamage = _chaosstorm.Damage;

                    if (AggroTarget.NetworkId == FriendlyTarget().NetworkId && Origin.Item("crowstormccc").GetValue<bool>())
                    {
                        if (FriendlyTarget().CountHerosInRange(true, 1000) >=
                            FriendlyTarget().CountHerosInRange(false, 1000) || IncomeDamage >= FriendlyTarget().Health)
                        {
                            Danger = true;
                            DangerUlt = true;
                            Sender = _fiddle;
                        }
                    }

                    if (Origin.Item("dbool").GetValue<bool>())
                        Console.WriteLine(
                            AggroTarget.SkinName + " is in Crowstorm (Ground Object) for: " + IncomeDamage);
                }
            }

            if (_minesield.Included)
            {
                if (_minesield.Obj.IsValid && FriendlyTarget().Distance(_minesield.Obj.Position, true) <= 300*300 && _ziggs != null)
                {
                    Sender = _ziggs;
                    AggroTarget = FriendlyTarget();
                    IncomeDamage = _minesield.Damage;

                    if (Origin.Item("dbool").GetValue<bool>())
                        Console.WriteLine(
                            AggroTarget.SkinName + " is in Minefield (Ground Object) for: " + IncomeDamage);
                }
            }

            if (_satchel.Included)
            {
                if (_satchel.Obj.IsValid && FriendlyTarget().Distance(_satchel.Obj.Position, true) <= 300*300 && _ziggs != null)
                {
                    Sender = _ziggs;
                    AggroTarget = FriendlyTarget();
                    IncomeDamage = _satchel.Damage;

                    if (Origin.Item("dbool").GetValue<bool>())
                        Console.WriteLine(
                            AggroTarget.SkinName + " is in Satchel (Ground Object) for: " + IncomeDamage);
                }
            }

            if (_tormentsoil.Included)
            {
                if (_satchel.Obj.IsValid && FriendlyTarget().Distance(_tormentsoil.Obj.Position, true) <= 300*300 && _morgana != null)
                {
                    Sender = _morgana;
                    AggroTarget = FriendlyTarget();
                    IncomeDamage = _tormentsoil.Damage;

                    if (Origin.Item("dbool").GetValue<bool>())
                        Console.WriteLine(
                            AggroTarget.SkinName + " is in Torment Soil (Ground Object) for: " + IncomeDamage);
                }
            }

            if (_miasma.Included)
            {
                if (_miasma.Obj.IsValid && FriendlyTarget().Distance(_miasma.Obj.Position, true) <= 300*300 && _cass != null)
                {
                    Sender = _cass;
                    AggroTarget = FriendlyTarget();
                    IncomeDamage = _satchel.Damage;

                    if (Origin.Item("dbool").GetValue<bool>())
                        Console.WriteLine(
                            AggroTarget.SkinName + " is in Miasma (Ground Object) for: " + IncomeDamage);
                }
            }

            if (_lightStrike.Included)
            {
                if (_lightStrike.Obj.IsValid && FriendlyTarget().Distance(_lightStrike.Obj.Position, true) <= 300*300 && _lux != null)
                {
                    Sender = _lux;
                    AggroTarget = FriendlyTarget();
                    IncomeDamage = _lightStrike.Damage;

                    if (Origin.Item("dbool").GetValue<bool>())
                        Console.WriteLine(
                            AggroTarget.SkinName + " is in Lightstrike (Ground Object) for: " + IncomeDamage);
                }
            }

            if (_equinox.Included)
            {
                if (_equinox.Obj.IsValid && FriendlyTarget().Distance(_equinox.Obj.Position, true) <= 300*300 && _soraka != null)
                {
                    Sender = _lux;
                    AggroTarget = FriendlyTarget();
                    IncomeDamage = _equinox.Damage;

                    if (Origin.Item("dbool").GetValue<bool>())
                        Console.WriteLine(
                            AggroTarget.SkinName + " is in Equinox (Ground Object) for: " + IncomeDamage);
                }
            }

            if (Danger)
                Utility.DelayAction.Add(Game.Ping + 130, () => Danger = false);
            if (DangerCC)
                Utility.DelayAction.Add(Game.Ping + 130, () => DangerCC = false);
            if (DangerUlt)
                Utility.DelayAction.Add(Game.Ping + 130, () => DangerUlt = false);
            if (Spell)
                Utility.DelayAction.Add(Game.Ping + 130, () => Spell = false);

        }

        private static void HeroCheck()
        {
            // Todo: get vayne R buff to detect stealth
            foreach (var hero in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.Team != ObjectManager.Player.Team))
            {
                switch (hero.SkinName)
                {
                    case "Viktor":
                        _viktor = hero;
                        break;
                    case "FiddleSticks":
                        _fiddle = hero;
                        break;
                    case "Anivia":
                        _anivia = hero;
                        break;
                    case "Ziggs":
                        _ziggs = hero;
                        break;
                    case "Cassiopeia":
                        _cass = hero;
                        break;
                    case "Lux":
                        _lux = hero;
                        break;
                    case "Soraka":
                        _soraka = hero;
                        break;
                    case "Vayne":
                        _vayne = hero;
                        break;
                    case "Morgana":
                        _morgana = hero;
                        break;
                }
            }
        }

        public static Obj_AI_Hero FriendlyTarget()
        {
            Obj_AI_Hero target = null;

            foreach (
                var unit in
                    ObjectManager.Get<Obj_AI_Hero>()
                        .Where(x => x.IsAlly && x.IsValidTarget(900, false))
                        .OrderByDescending(xe => xe.Health / xe.MaxHealth * 100))
            {
                target = unit;
            }

            return target;
        }

        public static int CountHerosInRange(this Obj_AI_Hero target, bool enemy = true, float range = float.MaxValue)
        {
            int count;
            var objListTeam =
                ObjectManager.Get<Obj_AI_Hero>()
                .Where(x => x.NetworkId != target.NetworkId && x.IsValidTarget(range, enemy) && enemy ? x.IsEnemy : x.IsAlly);

            var objAiHeroes = objListTeam as Obj_AI_Hero[] ?? objListTeam.ToArray();
            count = enemy ? objAiHeroes.Count() - 1: objAiHeroes.Count();

            return count;
        }

        public static bool NotRecalling(this Obj_AI_Hero target)
        {
            return !target.HasBuff("Recall") && !target.HasBuff("RecallImproved") && !target.HasBuff("OdinRecall") &&
                   !target.HasBuff("OdinRecallImproved");
        }

        public static float GetComboDamage(Obj_AI_Hero player, Obj_AI_Base target)
        {
            // todo: need to get states for e.g nidalee
            var ignite = player.GetSpellSlot("summonerdot");

            var qready = player.Spellbook.CanUseSpell(SpellSlot.Q) == SpellState.Ready;
            var wready = player.Spellbook.CanUseSpell(SpellSlot.W) == SpellState.Ready;
            var eready = player.Spellbook.CanUseSpell(SpellSlot.E) == SpellState.Ready;
            var rready = player.Spellbook.CanUseSpell(SpellSlot.R) == SpellState.Ready;
            var iready = player.Spellbook.CanUseSpell(ignite) == SpellState.Ready;

            var tmt = Items.HasItem(3077) && Items.CanUseItem(3077)
                ? Me.GetItemDamage(target, Damage.DamageItems.Tiamat)
                : 0;

            var hyd = Items.HasItem(3074) && Items.CanUseItem(3074)
                ? Me.GetItemDamage(target, Damage.DamageItems.Hydra)
                : 0;

            var bwc = Items.HasItem(3144) && Items.CanUseItem(3144)
                ? Me.GetItemDamage(target, Damage.DamageItems.Bilgewater)
                : 0;

            var brk = Items.HasItem(3153) && Items.CanUseItem(3153)
                ? Me.GetItemDamage(target, Damage.DamageItems.Botrk)
                : 0;

            var dfg = Items.HasItem(3128) && Items.CanUseItem(3128)
                ? Me.GetItemDamage(target, Damage.DamageItems.Dfg)
                : 0;

            var guise = Items.HasItem(3151)
                ? Me.GetItemDamage(target, Damage.DamageItems.LiandrysTorment)
                : 0;

            var torch = Items.HasItem(3188) && Items.CanUseItem(3188)
                ? Me.GetItemDamage(target, Damage.DamageItems.Dfg)
                : 0;

            var items = tmt + hyd + bwc + brk + dfg + torch + guise;

            var aa = player.GetAutoAttackDamage(target);
            var qq = qready ? player.GetSpellDamage(target, SpellSlot.Q) : 0;
            var ww = wready ? player.GetSpellDamage(target, SpellSlot.W) : 0;
            var ee = eready ? player.GetSpellDamage(target, SpellSlot.E) : 0;
            var rr = rready ? player.GetSpellDamage(target, SpellSlot.R) : 0;
            var ii = iready ? player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite) : 0;

            var damage = aa + qq + ww + ee + rr + ii + items;

            return (float)damage;
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                foreach (var o in SkillshotDatabase.Spells.Where(x => x.SpellName == args.SData.Name))
                {
                    foreach (var i in Damage.Spells
                        .Where(d => d.Key == o.ChampionName)
                        .SelectMany(item => item.Value).Where(i => i.Slot == (SpellSlot) o.Slot))
                    {
                        if (i.DamageType == Damage.DamageType.Physical)
                            CanManamune = true;
                    }
                }

                foreach (var o in TargetSpellDatabase.Spells.Where(x => x.Name == args.SData.Name.ToLower()))
                {
                    foreach (var i in Damage.Spells.Where(d => d.Key == o.ChampionName)
                        .SelectMany(item => item.Value).Where(i => i.Slot == (SpellSlot) o.Spellslot))
                    {
                        if (i.DamageType == Damage.DamageType.Physical)
                            CanManamune = true;
                    }
                }

                if (Me.GetSpellSlot(args.SData.Name) == SpellSlot.Unknown &&
                    (Origin.Item("ComboKey").GetValue<KeyBind>().Active || args.Target.Type == Me.Type)) 
                {
                    CanManamune = true;
                }
                else
                {
                    Utility.DelayAction.Add(400, () => CanManamune = false);
                }
            }

            Sender = null;
            if (sender.Type == GameObjectType.obj_AI_Hero && sender.IsEnemy)
            {
                var HeroSender = ObjectManager.Get<Obj_AI_Hero>().First(x => x.NetworkId == sender.NetworkId);
                if (HeroSender.GetSpellSlot(args.SData.Name) == SpellSlot.Unknown && args.Target.Type == Me.Type)
                {
                    Danger = false; DangerCC = false; DangerUlt = false;
                    AggroTarget = ObjectManager.GetUnitByNetworkId<Obj_AI_Hero>(args.Target.NetworkId);

                    IncomeDamage = (float)HeroSender.GetAutoAttackDamage(AggroTarget);

                    if (Origin.Item("dbool").GetValue<bool>())
                        Console.WriteLine(HeroSender.SkinName + " hit (Auto Attack) " + AggroTarget.SkinName + " for: " + IncomeDamage);
                }

                Sender = HeroSender;
                foreach (var o in TargetSpellDatabase.Spells.Where(x => x.Name == args.SData.Name.ToLower()))
                {
                    Spell = true;
                    Stealth = o.Stealth;
                    DangerCC = o.CcType != CcType.No && o.Type != SpellType.AutoAttack;

                    if (o.Type == SpellType.Skillshot)
                    {
                        continue;
                    }

                    if (o.Type == SpellType.Self)
                    {
                        Utility.DelayAction.Add((int)(o.Delay), delegate
                        {
                            AggroTarget =
                                ObjectManager.Get<Obj_AI_Hero>()
                                    .OrderBy(x => x.Distance(HeroSender.ServerPosition))
                                    .FirstOrDefault(x => x.IsAlly);

                            if (AggroTarget != null && AggroTarget.Distance(HeroSender.ServerPosition, true) <= o.Range*o.Range)
                            {
                                Danger = Origin.Item(o.Name.ToLower() + "ccc").GetValue<bool>();
                                DangerUlt = Origin.Item(o.Name.ToLower() + "ccc").GetValue<bool>() && o.Spellslot.ToString() == "R";
                                IncomeDamage = (float)HeroSender.GetSpellDamage(AggroTarget, (SpellSlot)o.Spellslot);

                                if (Origin.Item("dbool").GetValue<bool>())
                                    Console.WriteLine("Danger (Self): " + Danger);

                                if (Origin.Item("dbool").GetValue<bool>())
                                    Console.WriteLine(HeroSender.SkinName + " hit (Self Spell) " + AggroTarget.SkinName + " for: " + IncomeDamage);
                            }
                        });
                    }

                    if (o.Type == SpellType.Targeted && args.Target.Type == Me.Type)
                    {
                        Utility.DelayAction.Add((int)(o.Delay), delegate
                        {
                            AggroTarget =
                                ObjectManager.GetUnitByNetworkId<Obj_AI_Hero>(args.Target.NetworkId);

                            Danger = Origin.Item(o.Name.ToLower() + "ccc").GetValue<bool>();
                            DangerUlt = Origin.Item(o.Name.ToLower() + "ccc").GetValue<bool>() && o.Spellslot.ToString() == "R";
                            IncomeDamage = (float)HeroSender.GetSpellDamage(AggroTarget, (SpellSlot)o.Spellslot);

                            if (Origin.Item("dbool").GetValue<bool>())
                            {
                                Console.WriteLine("Dangerous (Targetd Spells): " + Danger);
                                Console.WriteLine(HeroSender.SkinName + " hit (Target Spell) " + AggroTarget.SkinName +
                                                  " for: " + IncomeDamage);
                            }
                        });
                    }
                }

                foreach (var o in SkillshotDatabase.Spells.Where(x => x.SpellName == args.SData.Name))
                {
                    var skillData =
                        new SkillshotData(o.ChampionName, o.SpellName, o.Slot, o.Type, o.Delay, o.Range,
                                          o.Radius, o.MissileSpeed, o.AddHitbox, o.FixedRange, o.DangerValue);

                    var endPosition = args.Start.To2D() +
                                      o.Range * (args.End.To2D() - HeroSender.ServerPosition.To2D()).Normalized();

                    var skillShot = new Skillshot(DetectionType.ProcessSpell, skillData, Environment.TickCount,
                        HeroSender.ServerPosition.To2D(), endPosition, HeroSender);

                    var castTime = (o.DontAddExtraDuration ? 0 : o.ExtraDuration) + o.Delay +
                                   (int)(1000 * HeroSender.Distance(FriendlyTarget().ServerPosition) / o.MissileSpeed) -
                                   (Environment.TickCount - skillShot.StartTick);

                    AggroTarget =
                        ObjectManager.Get<Obj_AI_Hero>()
                            .FirstOrDefault(x => !skillShot.IsSafe(x.ServerPosition.To2D()));

                    if (AggroTarget != null)
                    {
                        Utility.DelayAction.Add(castTime - 400, delegate
                        {
                            Danger = Origin.Item(o.SpellName.ToLower() + "ccc").GetValue<bool>();
                            DangerUlt = Origin.Item(o.SpellName.ToLower() + "ccc").GetValue<bool>() && o.Slot.ToString() == "R";
                            IncomeDamage = (float)HeroSender.GetSpellDamage(AggroTarget, (SpellSlot)skillShot.SkillshotData.Slot);

                            if (Origin.Item("dbool").GetValue<bool>())
                            {
                                Console.WriteLine("Dangerous (Skillshot Spells): " + Danger);
                                Console.WriteLine(HeroSender.SkinName + " may hit (SkillShot) " + AggroTarget.SkinName +
                                                  " for: " + IncomeDamage);
                            }
                        });
                    }
                }
            }

            if (sender.Type == GameObjectType.obj_AI_Minion && sender.IsEnemy)
            {
                if (args.Target.Type == Me.Type)
                {
                    Danger = false; DangerCC = false; DangerUlt = false;
                    AggroTarget = ObjectManager.GetUnitByNetworkId<Obj_AI_Hero>(args.Target.NetworkId);

                    MinionDamage =
                        (float)sender.CalcDamage(AggroTarget, Damage.DamageType.Physical,
                            sender.BaseAttackDamage + sender.FlatPhysicalDamageMod);
                }
            }

            if (sender.Type == GameObjectType.obj_AI_Turret && sender.IsEnemy)
            {
                if (args.Target.Type == Me.Type)
                {
                    Danger = false; DangerCC = false; DangerUlt = false;
                    if (sender.Distance(FriendlyTarget().ServerPosition, true) <= 900*900)
                    {
                        AggroTarget = ObjectManager.GetUnitByNetworkId<Obj_AI_Hero>(args.Target.NetworkId);

                        IncomeDamage =
                            (float)sender.CalcDamage(AggroTarget, Damage.DamageType.Physical,
                                sender.BaseAttackDamage + sender.FlatPhysicalDamageMod);

                        if (Origin.Item("dbool").GetValue<bool>())
                            Console.WriteLine("A turret hit (Turret Attack) " + AggroTarget.SkinName + " for: " + IncomeDamage);
                    }
                }
            }
        }
    }
}