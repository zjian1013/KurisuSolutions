using LeagueSharp;
using LeagueSharp.Common;
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace K4Nami
{
    internal class Namirino
    {
        public const string CharName = "Nami";
        public static Menu Config;
        public static Obj_AI_Hero target;

        public Namirino()
        {
            CustomEvents.Game.OnGameLoad += init;
        }

        #region Nami Menu
        private static void init(EventArgs args)
        {
            Game.PrintChat("Nami - the Tidecaller by K44");

            try
            {
                Config = new Menu("Nami - the Tidecaller", "Nami", true);

                Config.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
                Nami.orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalker"));

                Config.AddSubMenu(new Menu("Nami Combo", "combo"));
                Config.SubMenu("combo").AddItem(new MenuItem("useQ", "Use Q")).SetValue(true);
                Config.SubMenu("combo").AddItem(new MenuItem("useW", "Use W")).SetValue(false);
                Config.SubMenu("combo").AddItem(new MenuItem("useE", "Use E")).SetValue(true);

                Config.AddSubMenu(new Menu("Nami Harass", "harass"));
                Config.SubMenu("harass").AddItem(new MenuItem("useQ", "Use Q")).SetValue(true);
                Config.SubMenu("harass").AddItem(new MenuItem("useW", "Use W")).SetValue(false);
                Config.SubMenu("harass").AddItem(new MenuItem("useE", "Use E")).SetValue(false);

            }
            catch
            {
                Game.PrintChat("Error loading script, contact dev!");
            }
        }
        #endregion

        #region OnGameUpdate

        private static void OnGameUpdate(EventArgs args)
        {
            if (Nami.orbwalker.ActiveMode.ToString() == "Combo")
            {
                target = SimpleTs.GetTarget(1150, SimpleTs.DamageType.Magical);
                Nami.CastCombo(target);
            }

            if (Nami.orbwalker.ActiveMode.ToString() == "Mixed")
            {

            }

            if (Nami.orbwalker.ActiveMode.ToString() == "LaneClear")
            {

            }

        }

        #endregion






    }
}
