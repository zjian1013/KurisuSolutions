using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace Oracle.Extensions
{
    class Consumables
    {
        private static Menu mainmenu;
        private static readonly Obj_AI_Hero me = ObjectManager.Player;

        public static void Initialize(Menu root)
        {
            // OnGameUpdate Event
            Game.OnGameUpdate += Game_OnGameUpdate;

            // Create menu
            mainmenu = new Menu("Consumables", "consumables");

            // todo: menu items
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            throw new NotImplementedException();
        }

        private static void UseItem(string name, int itemId, string menuvar)
        {
            if (!Items.HasItem(itemId) || !Items.CanUseItem(itemId))
                return;
        }

        private static void CreateMenuItem(string name, string menuvar, int v1, int v2)
        {
            var menuName = new Menu(name, "m" + menuvar);
            menuName.AddItem(new MenuItem("use" + menuvar, "Use " + name)).SetValue(true);
            if (menuvar.Contains("health"))
                menuName.AddItem(new MenuItem("use" + menuvar + "Pct", "Use on HP &")).SetValue(new Slider(v1));
            if (menuvar.Contains("health"))
                menuName.AddItem(new MenuItem("use" + menuvar + "Dmg", "Use on Dmg %")).SetValue(new Slider(v2));
            if (menuvar.Contains("mana"))
                menuName.AddItem(new MenuItem("use" + menuvar + "Mana", "Use on Mana %")).SetValue(new Slider(40));
            mainmenu.AddSubMenu(menuName);          
        }
    }
}
