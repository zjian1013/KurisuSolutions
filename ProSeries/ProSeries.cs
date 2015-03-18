using System;
using LeagueSharp;
using LeagueSharp.Common;
using ProSeries.Utils;
using ProSeries.Utils.Drawings;

namespace ProSeries
{
    internal static class ProSeries
    {
        internal static Menu Config;
        internal static Orbwalking.Orbwalker Orbwalker;
        internal static Obj_AI_Hero Player;

        internal static void Load()
        {
            try
            {
                Player = ObjectManager.Player;

                //Print the welcome message
                Game.PrintChat("Pro Series Loaded!");

                //Load the menu.
                Config = new Menu("ProSeries", "ProSeries", true);

                //Add the target selector.
                TargetSelector.AddToMenu(Config.SubMenu("Selector"));

                //Add the orbwalking.
                Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalker"));

                //Load the crosshair
                Crosshair.Load();

                //Add ADC items usage.
                //ItemManager.Load();

                //Check if the champion is supported
                try
                {
                    Type.GetType("ProSeries.Champions." + Player.ChampionName).GetMethod("Load").Invoke(null, null);
                }
                catch (NullReferenceException)
                {
                    Game.PrintChat(Player.ChampionName + " is not supported yet! however the orbwalking will work");
                }

                //Add the menu as main menu.
                Config.AddToMainMenu();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        internal static bool CanCombo()
        {
            // "usecombo" keybind required
            // "combomana" slider required
            return Config.Item("usecombo").GetValue<KeyBind>().Active &&
                   Player.Mana / Player.MaxMana * 100 > Config.Item("combomana").GetValue<Slider>().Value;
        }

        internal static bool CanHarass()

        {   // "harasscombo" keybind required
            // "harassmana" slider required
            return Config.Item("useharass").GetValue<KeyBind>().Active &&
                  Player.Mana / Player.MaxMana * 100 > Config.Item("harassmana").GetValue<Slider>().Value;           
        }

        internal static bool CanClear()
        {            
            // "clearcombo" keybind required
            // "clearmana" slider required
            return Config.Item("useclear").GetValue<KeyBind>().Active &&
                  Player.Mana / Player.MaxMana * 100 > Config.Item("clearmana").GetValue<Slider>().Value;               
        }

        internal static string[] Creeps =
        {
            "SRU_Razorbeak", "SRU_Krug", "Sru_Crab",
            "SRU_Baron", "SRU_Dragon", "SRU_Blue", "SRU_Red", "SRU_Murkwolf", "SRU_Gromp"
        };
    }
}