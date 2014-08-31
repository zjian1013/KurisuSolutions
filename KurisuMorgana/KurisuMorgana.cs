using LeagueSharp;
using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = System.Drawing.Color;

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

        #region OnLoad
        private static void onLoad(EventArgs args)
        {
            try
            {
                Config = new Menu("Kurisu: Morgana", "morgana", true);

                #region Tidy : Orbwalker
                Config.AddSubMenu(new Menu("Orbwalker", "orbwalker"));
                Morgana.orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("orbwalker"));

                #endregion

                #region Tidy: ts Menu
                var _tsmenu = new Menu("Target Selector", "target selecter");
                SimpleTs.AddToMenu(_tsmenu);
                Config.AddSubMenu(_tsmenu);

                #endregion

                #region Tidy: Combo
                Config.AddSubMenu(new Menu("Combo", "combo"));
                Config.SubMenu("combo").AddItem(new MenuItem("useQ", "Use Q")).SetValue(true);
                Config.SubMenu("combo").AddItem(new MenuItem("useW", "Use W")).SetValue(true);
                Config.SubMenu("combo").AddItem(new MenuItem("useWif", "Use W only if Q Hits")).SetValue(true);
                Config.SubMenu("combo").AddItem(new MenuItem("usecombo", "Combo active").SetValue(new KeyBind(32, KeyBindType.Press)));

                #endregion

                #region Tidy: Harass
                Config.AddSubMenu(new Menu("Harass", "harass"));
                Config.SubMenu("harass").AddItem(new MenuItem("useW2", "Use W")).SetValue(true);
                Config.SubMenu("harass").AddItem(new MenuItem("useharass", "Harass active").SetValue(new KeyBind('C', KeyBindType.Press)));
                Config.SubMenu("harass").AddItem(new MenuItem("harassrPct", "Minimum mana % for Harass").SetValue(new Slider(40, 1, 100)));

                #endregion

                #region Tidy: Lanceclear
                Config.AddSubMenu(new Menu("Laneclear", "laneclear"));
                Config.SubMenu("laneclear").AddItem(new MenuItem("wclearNum", "Minion hit number for W").SetValue(new Slider(3, 1, 10)));
                Config.SubMenu("laneclear").AddItem(new MenuItem("wclear", "Use W")).SetValue(true);
                Config.SubMenu("laneclear").AddItem(new MenuItem("usewclear", "Laneclear active").SetValue(new KeyBind('V', KeyBindType.Press)));
                Config.SubMenu("laneclear").AddItem(new MenuItem("wclearPct", "Minimum mana % for Laneclear").SetValue(new Slider(40, 1, 100)));

                #endregion

                #region Drawings
                Config.AddSubMenu(new Menu("Drawings", "drawings"));
                Config.SubMenu("drawings").AddItem(new MenuItem("drawQ", "Draw Q")).SetValue(new Circle(true, Color.FromArgb(150, Color.DeepSkyBlue)));
                Config.SubMenu("drawings").AddItem(new MenuItem("drawW", "Draw W")).SetValue(new Circle(false, Color.FromArgb(150, Color.DodgerBlue)));
                Config.SubMenu("drawings").AddItem(new MenuItem("drawE", "Draw E")).SetValue(new Circle(false, Color.FromArgb(150, Color.LemonChiffon)));
                Config.SubMenu("drawings").AddItem(new MenuItem("drawR", "Draw R")).SetValue(new Circle(true, Color.FromArgb(150, Color.DeepSkyBlue)));
                
                #endregion

                #region Tidy: Extras
                Config.AddSubMenu(new Menu("Extra", "extra"));
                Config.SubMenu("extra").AddItem(new MenuItem("autoW", "Auto W on Stunned")).SetValue(true);
                Config.SubMenu("extra").AddItem(new MenuItem("autoQ", "Auto Q on Stunned")).SetValue(true);
                Config.SubMenu("extra").AddItem(new MenuItem("autoQGC", "Auto Q on Gapcloser")).SetValue(true);

                #endregion

                Config.AddToMainMenu();

            }
            catch (Exception e)
            {
                //Logger.FailLog("Something wfent wrong with Nami script :(");
                Console.WriteLine(e.ToString());
            }

            #region Callbacks
            Morgana.SetSkills();


            // Leaguesharp Stuff
            Drawing.OnDraw += OnDraw;
            Game.OnGameUpdate += OnGameUpdate;          
            GameObject.OnCreate += OnCreateObject;
            GameObject.OnDelete += OnDeleteObject;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpell;
            AntiGapcloser.OnEnemyGapcloser += OnGapcloser;

            #endregion
  
        }

        #endregion

        #region OnGapCloser
        /// <summary>
        /// Gapcloser AutoQ / Not tested!
        /// </summary>
        /// <param name="gapcloser"></param>
        private static void OnGapcloser(ActiveGapcloser gapcloser)
        {
            if (Config.SubMenu("extra").Item("autoQGC").GetValue<bool>())
                Morgana.q.Cast(gapcloser.Sender, true);
        }

        #endregion

        #region OnDraw
        private static void OnDraw(EventArgs args)
        {
            foreach (var spell in Extensions.SpellList)
            {
                var circle = Config.SubMenu("drawings").Item("draw" + spell.Slot.ToString()).GetValue<Circle>();
                if (circle.Active)
                    Utility.DrawCircle(me.Position, spell.Range, circle.Color);
            }
        }

        #endregion

        #region OnGameUpdate
        private static void OnGameUpdate(EventArgs args)
        {
            if (Config.SubMenu("extra").Item("autoW").GetValue<bool>())
                Morgana.CastAutoW();
            if (Config.SubMenu("extra").Item("autoQ").GetValue<bool>())
                Morgana.CastAutoQ();

            if (Config.SubMenu("combo").Item("usecombo").GetValue<KeyBind>().Active)
            {
                unit = SimpleTs.GetTarget(1175, SimpleTs.DamageType.Magical);
                Morgana.CastCombo(unit);
            }

            if (Config.SubMenu("harass").Item("useharass").GetValue<KeyBind>().Active)
            {
                unit = SimpleTs.GetTarget(1175, SimpleTs.DamageType.Magical);
                Morgana.CastHarass(unit);
            }

            if (Config.SubMenu("laneclear").Item("usewclear").GetValue<KeyBind>().Active)
            {
                Morgana.Laneclear();
            }

        }
        #endregion

        #region OnProcessSPell
        private static void OnProcessSpell(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            //throw new NotImplementedExceptions();
        }

        #endregion

        #region OnDeleteObject
        private static void OnDeleteObject(GameObject sender, EventArgs args)
        {
            //throw new NotImplementedExceptions();
        }

        #endregion

        #region OnCreateObject
        private static void OnCreateObject(GameObject sender, EventArgs args)
        {
            //throw new NotImplementedException();
        }

        #endregion

    }
}
