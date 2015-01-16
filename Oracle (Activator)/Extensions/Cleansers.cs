﻿using System;
﻿using System.Linq;
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
            _menuConfig.AddItem(new MenuItem("sep1", " "));

            foreach (var a in ObjectManager.Get<Obj_AI_Hero>().Where(a => a.Team == Me.Team))
                _menuConfig.AddItem(new MenuItem("ccon" + a.SkinName, "Use for " + a.SkinName)).SetValue(true);
            _mainMenu.AddSubMenu(_menuConfig);

            CreateMenuItem("Dervish Blade", "Dervish", 1);
            CreateMenuItem("Quicksilver Sash", "Quicksilver", 1);
            CreateMenuItem("Mercurial Scimitar", "Mercurial", 1);
            CreateMenuItem("Mikael's Crucible", "Mikaels", 1);

            _mainMenu.AddItem(
                new MenuItem("cleanseMode", "Cleanse Mode: "))
                .SetValue(new StringList(new[] {"Always", "Combo"}));

            root.AddSubMenu(_mainMenu);
        }

        public static void Game_OnGameUpdate(EventArgs args)
        {
            if (!_mainMenu.Item("ComboKey").GetValue<KeyBind>().Active &&
                _mainMenu.Item("cleanseMode").GetValue<StringList>().SelectedIndex == 1)
            {
                return;
            }

            UseItem("Mikaels", 3222, 600f);
            UseItem("Quicksilver", 3140);
            UseItem("Mercurial", 3139);
            UseItem("Dervish", 3137);
        }

        private static void UseItem(string name, int itemId, float range = float.MaxValue)
        {
            if (!Items.CanUseItem(itemId) || !Items.HasItem(itemId))
                return;

            if (!_mainMenu.Item("use" + name).GetValue<bool>())
                return;

            var target = range > 5000 ? Me : OC.FriendlyTarget();
            if (_mainMenu.Item("ccon" + target.SkinName).GetValue<bool>())
            {
                if (target.Distance(Me.ServerPosition, true) <= range * range)
                {
                    foreach (var buff in GameBuff.Buffs.Where(aura => target.HasBuff(aura.BuffName, true)))
                    {
                        Utility.DelayAction.Add(500 + buff.Delay, () => Items.UseItem(itemId, target));
                    }

                    if (_mainMenu.Item("slow").GetValue<bool>() && target.HasBuffOfType(BuffType.Slow))
                        Utility.DelayAction.Add(500, () => Items.UseItem(itemId, target));

                    if (_mainMenu.Item("stun").GetValue<bool>() && target.HasBuffOfType(BuffType.Stun))
                        Utility.DelayAction.Add(500, () => Items.UseItem(itemId, target));

                    if (_mainMenu.Item("charm").GetValue<bool>() && target.HasBuffOfType(BuffType.Charm))
                        Utility.DelayAction.Add(500, () => Items.UseItem(itemId, target));

                    if (_mainMenu.Item("taunt").GetValue<bool>() && target.HasBuffOfType(BuffType.Taunt))
                        Utility.DelayAction.Add(500, () => Items.UseItem(itemId, target));

                    if (_mainMenu.Item("fear").GetValue<bool>() && target.HasBuffOfType(BuffType.Fear))
                        Utility.DelayAction.Add(500, () => Items.UseItem(itemId, target));

                    if (_mainMenu.Item("snare").GetValue<bool>() && target.HasBuffOfType(BuffType.Snare))
                        Utility.DelayAction.Add(500, () => Items.UseItem(itemId, target));

                    if (_mainMenu.Item("silence").GetValue<bool>() && target.HasBuffOfType(BuffType.Silence))
                        Utility.DelayAction.Add(500, () => Items.UseItem(itemId, target));

                    if (_mainMenu.Item("suppression").GetValue<bool>() && target.HasBuffOfType(BuffType.Suppression))
                        Utility.DelayAction.Add(500, () => Items.UseItem(itemId, target));

                    if (_mainMenu.Item("polymorph").GetValue<bool>() && target.HasBuffOfType(BuffType.Polymorph))
                        Utility.DelayAction.Add(500, () => Items.UseItem(itemId, target));

                    if (_mainMenu.Item("blind").GetValue<bool>() && target.HasBuffOfType(BuffType.Blind))
                        Utility.DelayAction.Add(500, () => Items.UseItem(itemId, target));

                    if (_mainMenu.Item("poison").GetValue<bool>() && target.HasBuffOfType(BuffType.Poison))
                        Utility.DelayAction.Add(500, () => Items.UseItem(itemId, target));
                }
            }
        }

        private static void CreateMenuItem(string displayname, string name, int ccvalue)
        {
            var menuName = new Menu(displayname, name);
            menuName.AddItem(new MenuItem("use" + name, "Use " + name)).SetValue(true);
            menuName.AddItem(new MenuItem(name + "Count", "Min spells to use"));//.SetValue(new Slider(ccvalue, 1, 5));
            menuName.AddItem(new MenuItem(name + "Duration", "Buff duration to use"));//.SetValue(new Slider(2, 1, 5));
            _mainMenu.AddSubMenu(menuName);
        }
    }
}