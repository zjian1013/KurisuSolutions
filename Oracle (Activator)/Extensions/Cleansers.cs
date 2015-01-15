﻿using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using OC = Oracle.Program;

namespace Oracle.Extensions
{
    internal static class Cleansers
    {
        private static Menu _menuConfig, _mainMenu;
        private static readonly Obj_AI_Hero Me = ObjectManager.Player;

        public static void Initialize(Menu root)
        {
            Game.OnGameUpdate += Game_OnGameUpdate;

            _mainMenu = new Menu("Cleansers", "cmenu");
            _menuConfig = new Menu("Cleanse Config", "cconfig");

            foreach (var a in ObjectManager.Get<Obj_AI_Hero>().Where(a => a.Team == Me.Team))
                _menuConfig.AddItem(new MenuItem("cuseon" + a.SkinName, "Use for " + a.SkinName)).SetValue(true);

            _menuConfig.AddItem(new MenuItem("sep1", "=== Buff Types"));
            _menuConfig.AddItem(new MenuItem("stun", "Stuns")).SetValue(true);
            _menuConfig.AddItem(new MenuItem("charm", "Charms")).SetValue(true);
            _menuConfig.AddItem(new MenuItem("taunt", "Taunts")).SetValue(true);
            _menuConfig.AddItem(new MenuItem("fear", "Fears")).SetValue(true);
            _menuConfig.AddItem(new MenuItem("snare", "Snares")).SetValue(true);
            _menuConfig.AddItem(new MenuItem("silence", "Silences")).SetValue(true);
            _menuConfig.AddItem(new MenuItem("supression", "Supression")).SetValue(true);
            _menuConfig.AddItem(new MenuItem("polymorph", "Polymorphs")).SetValue(true);
            _menuConfig.AddItem(new MenuItem("blind", "Blinds")).SetValue(false);
            _menuConfig.AddItem(new MenuItem("slow", "Slows")).SetValue(false);
            _menuConfig.AddItem(new MenuItem("poison", "Poisons")).SetValue(false);
            _mainMenu.AddSubMenu(_menuConfig);

            CreateMenuItem("Quicksilver Sash", "Quicksilver", 1);
            CreateMenuItem("Dervish Blade", "Dervish", 1);
            CreateMenuItem("Mercurial Scimitar", "Mercurial", 1);
            CreateMenuItem("Mikael's Crucible", "Mikaels", 1);

            _mainMenu.AddItem(
                new MenuItem("cleanseMode", "Cleanse Mode: "))
                .SetValue(new StringList(new[] {"Always", "Combo"}));

            root.AddSubMenu(_mainMenu);
        }

        public static void Game_OnGameUpdate(EventArgs args)
        {
            foreach (var i in Me.InventoryItems)
                Console.WriteLine(i.Id + " id " + " - " + i.Name);

            if (Items.HasItem("Quicksilver_Sash"))
                Console.WriteLine("dd");

            //UseItem("Mikaels", 3222, 600f);
            //UseItem("Quicksilver", 3140);
            //UseItem("Mercurial", 3139);
            //UseItem("Dervish", 3137);
        }

        private static void UseItem(string name, int itemId, float itemRange = float.MaxValue)
        {

        }


        private static void CreateMenuItem(string displayname, string name, int ccvalue)
        {
            var menuName = new Menu(displayname, name);
            menuName.AddItem(new MenuItem("use" + name, "Use " + name)).SetValue(true);
            //menuName.AddItem(new MenuItem(name + "Count", "Min spells to use")).SetValue(new Slider(ccvalue, 1, 5));
            //menuName.AddItem(new MenuItem(name + "Duration", "Buff duration to use")).SetValue(new Slider(2, 1, 5));
            _mainMenu.AddSubMenu(menuName);
        }
    }
}