using System;
using LeagueSharp;
using LeagueSharp.Common;
using OC = Oracle.Oracle;

namespace Oracle.Extensions
{
    class Offensives
    {
        private static Menu MainMenu;
        private static Obj_AI_Hero HeroUnit = OC.HeroUnit;
        private static readonly Obj_AI_Hero Me = ObjectManager.Player;

        public static void Initialize(Menu root)
        {
            // OnGameUpdate Event
            Game.OnGameUpdate += Game_OnGameUpdate;

            MainMenu = new Menu("Offensives", "offensives");
            // TODO: Champ config;

            CreateMenuItem("Muramana", "Muramana", 90, 30);
            CreateMenuItem("Tiamat/Hydra", "Hydra", 90, 30);
            CreateMenuItem("Deathfire Grasp", "DFG", 100, 30);
            CreateMenuItem("Youmuu's Ghostblade", "Youmuus", 90, 30);
            CreateMenuItem("Bilgewater's Cutlass", "Cutlass", 90, 30);
            CreateMenuItem("Hextech Gunblade", "Hextech", 90, 30);
            CreateMenuItem("Blade of the Ruined King", "Botrk", 70, 70);
            CreateMenuItem("Frost Queen's Claim", "Frostclaim", 90, 30);
            CreateMenuItem("Sword of Divine", "Divine", 90, 30);

            // Add menu to root
            root.AddSubMenu(MainMenu);

        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            throw new NotImplementedException();
        }

        private static void CreateMenuItem(string displayname, string name, int evalue, int avalue)
        {
            var menuName = new Menu(displayname, name.ToLower());
            menuName.AddItem(new MenuItem("use" + name, "Use " + name)).SetValue(true);
            menuName.AddItem(new MenuItem("use" + name + "Pct", "Use on enemy HP %")).SetValue(new Slider(evalue));
            if (!name.Contains("mana"))
                menuName.AddItem(new MenuItem("use" + name + "Me", "Use on my HP %")).SetValue(new Slider(avalue));
            if (name.Contains("mana"))
                menuName.AddItem(new MenuItem("use" + name + "Mana", "Minimum mana % to use")).SetValue(new Slider(35));
            MainMenu.AddSubMenu(menuName);           
        }

    }
}
