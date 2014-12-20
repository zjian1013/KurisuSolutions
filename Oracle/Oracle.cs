using System;
using System.IO;
using System.Linq;
using Oracle.Core;
using Oracle.Extensions;
using LeagueSharp;
using LeagueSharp.Common;

namespace Oracle
{

    // Oracle is not ready
    throwError
    
    internal class Oracle 
    {
        //  _____             _     
        // |     |___ ___ ___| |___ 
        // |  |  |  _| .'|  _| | -_|
        // |_____|_| |__,|___|_|___|
        // Copyright © Kurisu Solutions 2014

        public static Menu MainMenu;
        public static Obj_AI_Hero HeroUnit;
        public static Obj_AI_Hero HeroTarget;
        public const string Revision = "1.0.0.0";
        public static readonly Obj_AI_Hero Me = ObjectManager.Player;
   
        public static CurrentMap Map;
        internal enum CurrentMap { SR = 0, CS = 1, HA = 2, TT = 3, };
        internal enum LogType { Error = 0, Warning = 1, Info = 2, }

        public static string FileName;
        public static float HeroDamage;
        public static string Summoner1;
        public static string Summoner2;
      
        public Oracle()
        {
            Logger(LogType.Info, "Oracle is laoding...");
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }
      
        private static void Game_OnGameLoad(EventArgs args)
        {
            FileName = DateTime.Now.ToString("yy.MM.dd") +
                      " " + DateTime.Now.ToString("h.mm.ss") + ".txt";

            Logger(LogType.Info, "Oracle r." + Revision + " started.");
            Logger(LogType.Info,  "Current Champion: " + Me.ChampionName);
            Logger(LogType.Info, "Current Ping: " + Game.Ping + "ms");

            foreach (var obj in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.Team == Me.Team))
                Logger(LogType.Info, "Found Champion: " + obj.ChampionName + " -- Friendly.");
            foreach (var obj in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.Team != Me.Team))
                Logger(LogType.Info, "Found Champion: " + obj.ChampionName + " -- Hostile.");

            // OnGameUpdate Event
            Game.OnGameUpdate += Game_OnGameUpdate;

            // OnProcessSpellCast Event
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;

            // Create Root Menu
            MainMenu = new Menu("Oracle", "oracle", true);

            Summoners.Initialize(MainMenu);
            Logger(LogType.Info, "Summoners extension loaded!");

            var DebugMenu = new Menu("Oracle Debug", "odebugger");
            DebugMenu.AddItem(new MenuItem("enabledebug", "Enable Debug")).SetValue(true);
            DebugMenu.AddItem(new MenuItem("debugwarning", "Print Warnings")).SetValue(false);
            DebugMenu.AddItem(new MenuItem("debugerror", "Print Errors")).SetValue(false);
            DebugMenu.AddItem(new MenuItem("debuginfo", "Print Info")).SetValue(false);
            MainMenu.AddSubMenu(DebugMenu);

            if (!MainMenu.Item("enabledebug").GetValue<bool>())
                Logger(LogType.Warning, "Oracle debugging is disabled");

            MainMenu.AddToMainMenu();

            // Set Current Map
            switch (Game.MapId)
            {
                case (GameMapId)11:
                    Map = CurrentMap.SR;
                    break;
                case GameMapId.CrystalScar:
                    Map = CurrentMap.CS;
                    break;
                case GameMapId.HowlingAbyss:
                    Map = CurrentMap.HA;
                    break;
                case GameMapId.TwistedTreeline:
                    Map = CurrentMap.TT;
                    break;
            }

            // Set Summoners
            var d = Me.
                Spellbook.
                    GetSpell(SpellSlot.Summoner1);
            var f = Me.
                Spellbook.
                    GetSpell(SpellSlot.Summoner2);

            Summoner1 = d.Name;
            Summoner2 = f.Name;

            Logger(LogType.Info, "Current Map: " + Map);
            Logger(LogType.Info, "Summoners : " + Summoner1 + " " + Summoner2);
           
        }

        public static void Logger(LogType type, string msg, bool ingame = false)
        {
            if (!MainMenu.Item("enabledebug").GetValue<bool>())
                return;

            var prefix = "[" + DateTime.Now.ToString("T") + " " + type + "] ";
            var drive = Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.System));

            using (
                var file =
                    new StreamWriter(@"" + drive + "\\Users\\" + FileName, true)) 
            {
                file.WriteLine(prefix + msg);
                file.Close();
            }

            switch (type)
            {
                case LogType.Error:
                    if (!MainMenu.Item("debugerror").GetValue<bool>())
                        return;
                    Logger(LogType.Error, msg, true);

                    break;
                case LogType.Warning:
                    if (!MainMenu.Item("debugwarning").GetValue<bool>())
                        return;
                    Logger(LogType.Warning, msg, true);
                    break;
                case LogType.Info:
                    if (!MainMenu.Item("debuginfo").GetValue<bool>())
                        return;
                    Logger(LogType.Info , msg, true);
                    break;
            }
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            try
            {
                foreach (
                    var hero in
                        ObjectManager.Get<Obj_AI_Hero>()
                            .Where(x => x.IsAlly && x.IsValidTarget(900, false))
                            .OrderByDescending(o => o.Health/o.MaxHealth*100))
                {
                    HeroUnit = hero;
                }
            }

            catch (Exception e)
            {
                Logger(LogType.Error, e.Message);
            }
      
        }

        private static void Null(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            foreach (var i in SkillshotDatabase.Spells.Where(x => x.SpellName == args.SData.Name))
            {
                var data =
                    new SkillshotData(
                        i.ChampionName, i.SpellName, i.Slot,
                        i.Type, i.Delay, i.Range, i.Radius,
                        i.MissileSpeed, i.AddHitbox, i.FixedRange, i.DangerValue);

                var spell = 
                    new Skillshot(
                        DetectionType.ProcessSpell, data, Environment.TickCount, 
                        args.Start.To2D(), args.End.To2D(), sender);

                if (spell.IsAboutToHit(2, ObjectManager.Player))
                {
                    
                }

            }
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            HeroDamage = 0f;
            HeroTarget =
                ObjectManager.Get<Obj_AI_Hero>()
                    .Where(
                        hero =>
                            hero.NetworkId == args.Target.NetworkId || args.End.Distance(hero.ServerPosition, true) <= 300*300)
                    .OrderBy(x => args.End.Distance(x.ServerPosition))
                    .First();

            var HeroPercent = (int) HeroTarget.Health/HeroTarget.MaxHealth*100;

            if (sender.IsEnemy && sender.Type == GameObjectType.obj_AI_Turret)
            {
                var Turret = ObjectManager.Get<Obj_AI_Turret>().First(x => x.NetworkId == sender.NetworkId);
                if (args.Target.NetworkId == HeroTarget.NetworkId && args.Target.Type == GameObjectType.obj_AI_Hero)
                {
                    if (Turret.Distance(ObjectManager.Player.Position) <= 900)
                    {
                        HeroDamage =
                            (float)
                                Turret.CalcDamage(HeroTarget, Damage.DamageType.Physical,
                                    Turret.BaseAttackDamage + Turret.FlatPhysicalDamageMod);
                        Logger(LogType.Info,
                            "Turret has hit " + HeroTarget.ChampionName + " (" + HeroPercent + "%)  for " + HeroDamage);
                    }
                }
            }

            if (sender.IsEnemy && sender.Type == GameObjectType.obj_AI_Minion)
            {
                var Minion = ObjectManager.Get<Obj_AI_Minion>().First(x => x.NetworkId == sender.NetworkId);
                if (args.Target.NetworkId == HeroTarget.NetworkId && args.Target.Type == GameObjectType.obj_AI_Hero)
                {                 
                    HeroDamage =
                        (float)
                            Minion.CalcDamage(HeroTarget, Damage.DamageType.Physical,
                                Minion.BaseAttackDamage + Minion.FlatPhysicalDamageMod);
                    Logger(LogType.Info,
                        "Minion ( " + Minion.Name + ") has hit " + HeroTarget.ChampionName + " (" + HeroPercent + "%) for " + HeroDamage);
                }                
            }

            if (sender.IsEnemy && sender.Type == GameObjectType.obj_AI_Hero)
            {
                var Hero = ObjectManager.Get<Obj_AI_Hero>().First(x => x.NetworkId == sender.NetworkId);
                if (HeroTarget == null)
                {
                    Logger(LogType.Warning, "HeroTarget is null");
                    return;
                }

                // Get slot by spell data name
                var HeroSlot = Hero.GetSpellSlot(args.SData.Name);
                if (HeroSlot == SpellSlot.Q)
                    HeroDamage = (float) Hero.GetSpellDamage(HeroTarget, SpellSlot.Q);
                if (HeroSlot == SpellSlot.W)
                    HeroDamage = (float) Hero.GetSpellDamage(HeroTarget, SpellSlot.W);
                if (HeroSlot == SpellSlot.E)
                    HeroDamage = (float) Hero.GetSpellDamage(HeroTarget, SpellSlot.E);
                if (HeroSlot == SpellSlot.R)
                    HeroDamage = (float) Hero.GetSpellDamage(HeroTarget, SpellSlot.R);
                if (HeroSlot == SpellSlot.Unknown)
                    HeroDamage = (float) Hero.GetAutoAttackDamage(HeroTarget);

                Logger(LogType.Info,
                    "Hero (" + Hero.ChampionName + ") has hit " + HeroTarget.ChampionName + " (" + HeroPercent + "%)  for " + HeroDamage);
            }
        }
    }
}
