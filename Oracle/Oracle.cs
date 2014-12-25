using System;
using System.IO;
using System.Linq;
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

        internal enum LogType { Error = 0, Warning = 1, Info = 2, Damage = 3};

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
            Logger(LogType.Info, "Summoners Extension Loaded!");

            var DebugMenu = new Menu("Oracle Debug", "odebugger");
            DebugMenu.AddItem(new MenuItem("debugwarning", "Print Warnings")).SetValue(false);
            DebugMenu.AddItem(new MenuItem("debugerror", "Print Errors")).SetValue(false);
            DebugMenu.AddItem(new MenuItem("debuginfo", "Print Info")).SetValue(false);
            MainMenu.AddSubMenu(DebugMenu);

            MainMenu.AddToMainMenu();

            if (!MainMenu.Item("enabledebug").GetValue<bool>())
                Game.PrintChat("Oracle: Debugger Disabled, it is reccomended that you leave it enabled!");

            try
            {
                var d = Me.
                    Spellbook.
                    GetSpell(SpellSlot.Summoner1);
                var f = Me.
                    Spellbook.
                    GetSpell(SpellSlot.Summoner2);

                Summoner1 = d.Name;
                Summoner2 = f.Name;
            }
            catch (Exception e)
            {
                Logger(LogType.Error, e.Message);
            }

            Logger(LogType.Info, "Current Map: " + Game.MapId);
            Logger(LogType.Info, "Summoners : " + Summoner1 + " " + Summoner2);
           
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            foreach (var hero in ObjectManager.Get<Obj_AI_Hero>()
                        .Where(x => x.IsAlly && x.IsValidTarget(900, false))
                        .OrderByDescending(o => o.Health/o.MaxHealth*100))
            {
                HeroUnit = hero;
            }             
        }

        public static void Logger(LogType type, string msg, bool ingame = false)
        {
            var user = Environment.UserName;
            var prefix = "[" + DateTime.Now.ToString("T") + " " + type + "] ";
            var drive = Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.System));

            using (
                var file = 
                    new StreamWriter( @"" + drive + "\\Users\\" + user + "\\AppData\\Roaming\\LeagueSharp\\Oracle\\" + FileName, true)) 
            {
                file.WriteLine(prefix + msg);
                file.Close();
            }

            if (ingame)
                Game.PrintChat("Oracle: " + msg);
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
                        Logger(LogType.Damage,
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
                    Logger(LogType.Damage,
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

                Logger(LogType.Damage,
                    "Hero (" + Hero.ChampionName + ") has hit " + HeroTarget.ChampionName + " (" + HeroPercent + "%)  for " + HeroDamage);
            }
        }
    }
}
