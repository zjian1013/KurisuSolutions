using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

namespace Oracle.Extensions
{
    internal class Cleansers
    {
        private static Menu mainmenu;
        private static Menu menuconfig;
        private static string[] herosummoners;

        // Set local player (me)
        private static readonly Obj_AI_Hero me = ObjectManager.Player;

        private static void Initialize(Menu root)
        {
            // GameOnGameUpdate Event
            Game.OnGameUpdate += Game_OnGameUpdate;

            // OnProcessPacker Event
            Game.OnGameProcessPacket += Game_OnGameProcessPacket;

            // Set summoners
            herosummoners = new[] {Oracle.Summoner1, Oracle.Summoner2};

            // Create menu
            mainmenu = new Menu("Cleansers", "cleansers");
            menuconfig = new Menu("Cleansers Config", "cconfig");

            foreach (var x in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.Team != me.Team))
                menuconfig.AddItem(new MenuItem("cleanseon" + x.SkinName, "Use for " + x.SkinName)).SetValue(true);

            menuconfig.AddItem(new MenuItem("sep1", "=== Buff Types"));
            menuconfig.AddItem(new MenuItem("stun", "Stuns")).SetValue(true);
            menuconfig.AddItem(new MenuItem("charm", "Charms")).SetValue(true);
            menuconfig.AddItem(new MenuItem("taunt", "Taunts")).SetValue(true);
            menuconfig.AddItem(new MenuItem("fear", "Fears")).SetValue(true);
            menuconfig.AddItem(new MenuItem("snare", "Snares")).SetValue(true);
            menuconfig.AddItem(new MenuItem("silence", "Silences")).SetValue(true);
            menuconfig.AddItem(new MenuItem("supression", "Supression")).SetValue(true);
            menuconfig.AddItem(new MenuItem("polymorph", "Polymorphs")).SetValue(true);
            menuconfig.AddItem(new MenuItem("blind", "Blinds")).SetValue(false);
            menuconfig.AddItem(new MenuItem("slow", "Slows")).SetValue(false);
            menuconfig.AddItem(new MenuItem("poison", "Poisons")).SetValue(false);
            mainmenu.AddSubMenu(menuconfig);

            // Check cleanse summoner
            if (herosummoners.Any(x => x == "summonercleanse"))
            {
                var menu = new Menu("Cleanse", "mcleanse");
                menu.AddItem(new MenuItem("useCleanse", "Enable Cleanse")).SetValue(true);
                mainmenu.AddSubMenu(menu);
            }

            CreateMenuItem("Quicksilver Sash", "Quicksilver", 1);
            CreateMenuItem("Dervish Blade", "Dervish", 1);
            CreateMenuItem("Mercurial Scimitar", "Mercurial", 1);
            CreateMenuItem("Mikael's Crucible", "Mikaels", 1);

            mainmenu.AddItem(
                new MenuItem("cleanseMode", "QSS Mode: "))
                    .SetValue(new StringList(new[] { "Always", "Combo" }));

            root.AddSubMenu(mainmenu);
        }

        private static void Game_OnGameProcessPacket(GamePacketEventArgs args)
        {
            throw new NotImplementedException();
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            throw new NotImplementedException();
        }

        private static void UseMenuItem()
        {
            
        }

        private static void CreateMenuItem(string displayname, string name, int count)
        {
            var menuName = new Menu(displayname, name);
            menuName.AddItem(new MenuItem("use" + name, "Use " + name)).SetValue(true);
            menuName.AddItem(new MenuItem(name + "Count", "Min spells to use")).SetValue(new Slider(2, 1, 5));
            menuName.AddItem(new MenuItem(name + "Duration", "Buff duration to use")).SetValue(new Slider(2, 1, 5));
            mainmenu.AddSubMenu(menuName);           
        }
    }
}
