using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using OC = Oracle.Program;

namespace Oracle.Extensions
{
    internal static class Consumables
    {
        private static Menu _mainMenu;
        private static readonly Obj_AI_Hero Me = ObjectManager.Player;

        public static void Initialize(Menu root)
        {
            Game.OnGameUpdate += Game_OnGameUpdate;

            _mainMenu = new Menu("Consumables", "imenu");

            CreateMenuItem("Mana Potion", "Mana", 40, 0);
            CreateMenuItem("Biscuit", "BiscuitHealthMana", 40, 25);
            CreateMenuItem("Crystaline Flask", "FlaskHealthMana", 40, 35);
            CreateMenuItem("Health Potion", "Health", 40, 25);

            root.AddSubMenu(_mainMenu);
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            UseItem("FlaskOfCrystalWater", 2004, "Mana");
            UseItem("ItemMiniRegenPotion", 2010, "BiscuitHealthMana", OC.IncomeDamage, OC.MinionDamage);
            UseItem("ItemCrystalFlask", 2041, "FlaskHealthMana", OC.IncomeDamage, OC.MinionDamage);
            UseItem("RegenerationPotion", 2003, "Health", OC.IncomeDamage, OC.MinionDamage);
        }

        private static void UseItem(string name, int itemId, string menuvar, float incdmg = 0, double mindmg = 0)
        {
            if (!Items.HasItem(itemId) || !Items.CanUseItem(itemId))
                return;

            if (Me.HasBuff(name, true) || !Me.NotRecalling() || Me.InFountain())
                return;

            if (!_mainMenu.Item("use" + menuvar).GetValue<bool>())
                return;

            var aManaPercent = (int)((Me.Mana / Me.MaxMana) * 100);
            var aHealthPercent = (int) ((Me.Health/Me.MaxHealth)*100);

            var iDamagePercent = (int) ((incdmg/Me.MaxHealth)*100);
            var mDamagePercent = (int) ((mindmg/Me.MaxHealth)*100);

            if (menuvar.Contains("Mana") && aManaPercent <= _mainMenu.Item("use" + menuvar + "Mana").GetValue<Slider>().Value)
            {
                if (Me.Mana != 0)
                    Items.UseItem(itemId);
            }

            if (menuvar.Contains("Health") && aHealthPercent <= _mainMenu.Item("use" + menuvar + "Pct").GetValue<Slider>().Value)
            {
                if (iDamagePercent >= 1 || incdmg >= Me.Health || Me.HasBuff("summonerdot", true) ||
                    mDamagePercent >= 1 || mindmg >= Me.Health || Me.HasBuffOfType(BuffType.Damage))
                {
                    if (OC.AggroTarget.NetworkId == Me.NetworkId)
                        Items.UseItem(itemId);
                }
                else if (iDamagePercent >= _mainMenu.Item("use" + menuvar + "Dmg").GetValue<Slider>().Value)
                {
                    if (OC.AggroTarget.NetworkId == Me.NetworkId)
                        Items.UseItem(itemId);
                }
            }
        }

        private static void CreateMenuItem(string name, string menuvar, int dvalue, int dmgvalue)
        {
            var menuName = new Menu(name, "m" + menuvar);
            menuName.AddItem(new MenuItem("use" + menuvar, "Use " + name)).SetValue(true);
            if (menuvar.Contains("Health"))
            {
                menuName.AddItem(new MenuItem("use" + menuvar + "Pct", "Use on HP &")).SetValue(new Slider(dvalue));
                menuName.AddItem(new MenuItem("use" + menuvar + "Dmg", "Use on Dmg %")).SetValue(new Slider(dmgvalue));
            }

            if (menuvar.Contains("Mana"))
                menuName.AddItem(new MenuItem("use" + menuvar + "Mana", "Use on Mana %")).SetValue(new Slider(40));
            _mainMenu.AddSubMenu(menuName);
        }
    }
}