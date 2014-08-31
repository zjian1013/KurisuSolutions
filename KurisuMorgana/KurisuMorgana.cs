using LeagueSharp;
using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KurisuMorgana
{
    class KurisuMorgana
    {
        public static Menu Config;
        public static Obj_AI_Hero unit;
        public static Obj_AI_Hero me = ObjectManager.Player;

        public KurisuMorgana()
        {

            if (me.BaseSkinName != "Morgana")
                return;
            CustomEvents.Game.OnGameLoad += onLoad;
        }

        private static void onLoad(EventArgs args)
        {
           

            try
            {
    
                Config = new Menu("Morgana", "morgana", true);

                Config.AddSubMenu(new Menu("Orbwalker", "orbwalker"));
                Morgana.orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("orbwalker"));
                //Logger.InitLog("Nami orbwalker loaded!");;


                var _tsmenu = new Menu("Target Selector", "target selecter");
                SimpleTs.AddToMenu(_tsmenu);
                Config.AddSubMenu(_tsmenu);
                //Logger.InitLog("Nami target selector loaded!");


                Config.AddSubMenu(new Menu("Combo", "combo"));
                Config.SubMenu("combo").AddItem(new MenuItem("useQ", "Use Q")).SetValue(true);
                Config.SubMenu("combo").AddItem(new MenuItem("useW", "Use W")).SetValue(true);


                Config.AddSubMenu(new Menu("Harass", "harass"));
                Config.SubMenu("harass").AddItem(new MenuItem("useW2", "Use W")).SetValue(true);

                Config.AddSubMenu(new Menu("Extra", "extra"));

                Config.AddToMainMenu();

                Drawing.OnDraw += OnDraw;
                Game.OnGameUpdate += OnGameUpdate; 
                Console.WriteLine("Debug: Menu Created");

            }
            catch (Exception e)
            {
                //Logger.FailLog("Something went wrong with Nami script :(");
                Console.WriteLine(e.ToString());
            }


            

            GameObject.OnCreate += OnCreateObject;
            GameObject.OnDelete += OnDeleteObject;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpell;
            AntiGapcloser.OnEnemyGapcloser += OnGapcloser;

            Morgana.SetSkills();
        }

        private static void OnGapcloser(ActiveGapcloser gapcloser)
        {
            //throw new NotImplementedException();
        }

        private static void OnDraw(EventArgs args)
        {
            //throw new NotImplementedException();
        }

        private static void OnGameUpdate(EventArgs args)
        {   

            if (Morgana.orbwalker.ActiveMode.ToString() == "Combo")
            {
                unit = SimpleTs.GetTarget(1175, SimpleTs.DamageType.Magical);
                Morgana.CastCombo(unit);
            }

            if (Morgana.orbwalker.ActiveMode.ToString() == "Mixed")
            {
                unit = SimpleTs.GetTarget(1175, SimpleTs.DamageType.Magical);
                Morgana.CastHarass(unit);
            }

            if (Morgana.orbwalker.ActiveMode.ToString() == "LaneClear")
            {

            }

        }

        private static void OnProcessSpell(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            //throw new NotImplementedExceptions();
        }

        private static void OnDeleteObject(GameObject sender, EventArgs args)
        {
            //throw new NotImplementedExceptions();
        }

        private static void OnCreateObject(GameObject sender, EventArgs args)
        {
            //throw new NotImplementedException();
        }

    }
}
