using LeagueSharp;
using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KurisuNami
{
    class Nami
    {
        public const string CharName = "Nami";
        public static Menu _mymenu;
        public static Orbwalking.Orbwalker _orbwalker;

        public Nami()
        {
            if (ObjectManager.Player.BaseSkinName != CharName)
                return;
            CustomEvents.Game.OnGameLoad += onLoad;
            
        }

        private static void  onLoad(EventArgs args)
        {
            NamiLogic.Q.SetSkillshot(0.50f, 200f, 1700f, false, SkillshotType.SkillshotCircle);
            try
            {
                
                
                _mymenu = new Menu("Nami", "nami", true);

                _mymenu.AddSubMenu(new Menu("Orbwalker", "orbwalker"));
                _orbwalker = new Orbwalking.Orbwalker(_mymenu.SubMenu("orbwalker"));
                //Logger.InitLog("Nami orbwalker loaded!");;


                var _tsmenu = new Menu("Target Selector", "target selecter");
                SimpleTs.AddToMenu(_tsmenu);
                _mymenu.AddSubMenu(_tsmenu);
                //Logger.InitLog("Nami target selector loaded!");



                _mymenu.AddSubMenu(new Menu("Combo", "combo"));

                _mymenu.SubMenu("combo").AddItem(new MenuItem("useQ", "Use Q")).SetValue(true);
                _mymenu.SubMenu("combo").AddItem(new MenuItem("useE", "Use E")).SetValue(true);


                _mymenu.AddSubMenu(new Menu("Harass", "harass"));
                _mymenu.SubMenu("harass").AddItem(new MenuItem("useQ2", "Use Q")).SetValue(false);
                _mymenu.SubMenu("harass").AddItem(new MenuItem("useE2", "Use E")).SetValue(true);

                _mymenu.AddSubMenu(new Menu("Extra", "extra"));

                _mymenu.AddToMainMenu();

                Game.OnGameUpdate += onUpdate;

            }
            catch (Exception e)
            {
                //Logger.FailLog("Something went wrong with Tristana routine :(");
                Console.WriteLine(e.ToString());
            }
        }

        private static void onUpdate(EventArgs args)
        { 
            

            
        }



    }
}
