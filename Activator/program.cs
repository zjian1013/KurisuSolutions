﻿#region Copyright © 2015 Kurisu Solutions
// All rights are reserved. Transmission or reproduction in part or whole,
// any form or by any means, mechanical, electronical or otherwise, is prohibited
// without the prior written consent of the copyright owner.
// 
// Document:	activator/program.cs
// Date:		01/07/2015
// Author:		Robin Kurisu
#endregion

using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;
using Activator.Items;
using Activator.Spells;
using Activator.Summoners;
using LeagueSharp;
using LeagueSharp.Common;

namespace Activator
{
    internal class Activator
    {
        internal static Menu Origin;
        internal static Obj_AI_Hero Player;

        internal static int MapId;
        internal static int LastUsedTimeStamp;
        internal static int LastUsedDuration;

        internal static SpellSlot Smite;
        internal static bool SmiteInGame;
        internal static bool TroysInGame;

        public static List<champion> Heroes = new List<champion>(); 

        private static void Main(string[] args)
        {
            Console.WriteLine("[A]: Loading Activator#..");
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            Player = ObjectManager.Player;
            MapId = (int) Utility.Map.GetMap().Type;

            Console.WriteLine("[A]: Initializing Activator#..");

            GetSmiteSlot();
            GetTroysInGame();
            GetHeroesInGame();
            GetSlotDamage();

            Origin = new Menu("Activator", "activator", true);
            var cmenu = new Menu("Cleansers", "cleansers");
            if (Game.Version.Contains("5.13"))
            {
                SubMenu(cmenu, false);
                GetItemGroup("Items.Cleansers").ForEach(t => NewItem((item) NewInstance(t), cmenu));        
            }

            Origin.AddSubMenu(cmenu);

            var dmenu = new Menu("Defensives", "dmenu");
            SubMenu(dmenu, false);
            GetItemGroup("Items.Defensives").ForEach(t => NewItem((item) NewInstance(t), dmenu));
            Origin.AddSubMenu(dmenu);

            var smenu = new Menu("Summoners", "smenu");
            SubMenuEx(smenu);
            GetItemGroup("Summoners").ForEach(t => NewSummoner((summoner) NewInstance(t), smenu));
            Origin.AddSubMenu(smenu);

            var omenu = new Menu("Offensives", "omenu");
            SubMenu(omenu, true);
            GetItemGroup("Items.Offensives").ForEach(t => NewItem((item) NewInstance(t), omenu));
            Origin.AddSubMenu(omenu);

            var imenu = new Menu("Consumables", "imenu");
            GetItemGroup("Items.Consumables").ForEach(t => NewItem((item) NewInstance(t), imenu));
            Origin.AddSubMenu(imenu);

            var amenu = new Menu("Auto Spells", "amenu");
            SubMenu(amenu, false);
            amenu.AddItem(new MenuItem("al6", "Auto Lvl 6")).SetValue(true);
            GetItemGroup("Spells.Evaders").ForEach(t => NewSpell((spell) NewInstance(t), amenu));
            GetItemGroup("Spells.Shields").ForEach(t => NewSpell((spell) NewInstance(t), amenu));
            GetItemGroup("Spells.Health").ForEach(t => NewSpell((spell) NewInstance(t), amenu));
            GetItemGroup("Spells.Slows").ForEach(t => NewSpell((spell) NewInstance(t), amenu));
            GetItemGroup("Spells.Heals").ForEach(t => NewSpell((spell) NewInstance(t), amenu));
            Origin.AddSubMenu(amenu);

            var zmenu = new Menu("Misc/Settings", "settings");

            if (SmiteInGame)
            {
                var ddmenu = new Menu("Drawings", "drawings");
                ddmenu.AddItem(new MenuItem("drawfill", "Draw Smite Fill")).SetValue(true);
                ddmenu.AddItem(new MenuItem("drawtext", "Draw Smite Text")).SetValue(true);
                ddmenu.AddItem(new MenuItem("drawsmite", "Draw Smite Range")).SetValue(true);
                zmenu.AddSubMenu(ddmenu);
            }

            var vmenu = new Menu("Info (Changelog/Updates)", "info");
            vmenu.AddItem(new MenuItem("aa", "0.9.5.6: (Paypal xrobinsong@gmail.com)"));
            vmenu.AddItem(new MenuItem("zx", "- new: righteous glory"));
            vmenu.AddItem(new MenuItem("yy", "- new: anti vayne stealth and more"));
            vmenu.AddItem(new MenuItem("zz", "- new: vision ward/oracle's lens"));
            vmenu.AddItem(new MenuItem("zm", "- new: evade integration (see topic)"));
            vmenu.AddItem(new MenuItem("s", "- known issue: cleanse not working"));
            zmenu.AddSubMenu(vmenu);

            zmenu.AddItem(new MenuItem("acdebug", "Debug")).SetValue(false);
            zmenu.AddItem(new MenuItem("evade", "Evade Integration")).SetValue(true);
            zmenu.AddItem(new MenuItem("healthp", "Ally Priority:"))
                .SetValue(new StringList(new[] { "Low HP", "Most AD/AP", "Most HP" }, 1));
            zmenu.AddItem(new MenuItem("usecombo", "Combo (active)"))
                .SetValue(new KeyBind(32, KeyBindType.Press, true));

            Origin.AddSubMenu(zmenu);
            Origin.AddToMainMenu();

            // draw hanlder
            drawings.init();

            // auras and debffs
            spelldebuffhandler.init();

            // damage prediction
            projectionhandler.init();

            // object manager
            gametroyhandler.init();

            Obj_AI_Base.OnLevelUp += Obj_AI_Base_OnLevelUp;
            Obj_AI_Base.OnPlaceItemInSlot += Obj_AI_Base_OnPlaceItemInSlot;

            foreach (var autospell in spelldata.mypells)
                if (Player.GetSpellSlot(autospell.Name) != SpellSlot.Unknown)
                    Game.OnUpdate += autospell.OnTick;

            foreach (var item in spelldata.items)
                if (LeagueSharp.Common.Items.HasItem(item.Id))
                    Game.OnUpdate += item.OnTick;

            foreach (var summoner in spelldata.summoners)
                if (summoner.Slot != SpellSlot.Unknown ||
                    summoner.ExtraNames.Any(
                        x => Player.GetSpellSlot(x) != SpellSlot.Unknown))
                    Game.OnUpdate += summoner.OnTick;
        }

        public static IEnumerable<champion> ChampionPriority()
        {
            switch (Origin.Item("healthp").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    return Heroes.Where(h => h.Player.IsAlly).OrderBy(h => h.Player.Health / h.Player.MaxHealth * 100);
                case 1:
                    return Heroes.Where(h => h.Player.IsAlly)
                            .OrderByDescending(h => h.Player.FlatPhysicalDamageMod + h.Player.FlatMagicDamageMod);
                case 2:
                    return Heroes.Where(h => h.Player.IsAlly).OrderByDescending(h => h.Player.Health);
            }

            return null;
        }

        private static void Obj_AI_Base_OnPlaceItemInSlot(Obj_AI_Base sender, Obj_AI_BasePlaceItemInSlotEventArgs args)
        {
            if (!sender.IsMe)
                return;

            foreach (var item in spelldata.items)
                if (item.Id == (int) args.Id)
                    Game.OnUpdate += item.OnTick;
        }

        private static void NewItem(item item, Menu parent)
        {
            if (item.Maps.Contains((MapType) MapId) ||
                item.Maps.Contains(MapType.Common))
            {
                spelldata.items.Add(item.CreateMenu(parent));
            }
        }

        private static void NewSpell(spell spell, Menu parent)
        {
            if (Player.GetSpellSlot(spell.Name) != SpellSlot.Unknown)
                spelldata.mypells.Add(spell.CreateMenu(parent));
        }

        private static void NewSummoner(summoner summoner, Menu parent)
        {
            if (Player.GetSpellSlot(summoner.Name) != SpellSlot.Unknown)
                spelldata.summoners.Add(summoner.CreateMenu(parent));

            if (summoner.Name.Contains("smite") && SmiteInGame)
                spelldata.summoners.Add(summoner.CreateMenu(parent));
        }

        private static List<Type> GetItemGroup(string nspace)
        {
            return
                Assembly.GetExecutingAssembly()
                    .GetTypes()
                    .FindAll(t => t.IsClass && t.Namespace == "Activator." + nspace &&
                                  t.Name != "item" && t.Name != "spell" && t.Name != "summoner" &&
                                  !t.Name.Contains("c__")); // kek
        }

        private static void GetSlotDamage()
        {
            foreach (
                var spell in
                    Damage.Spells.Where(entry => entry.Key == Player.ChampionName).SelectMany(entry => entry.Value))
            {
                spelldata.damagelib.Add(spell.Damage, spell.Slot);
                Console.WriteLine("[A]: " + Player.ChampionName + ": " + spell.Slot + " " + spell.Stage + " - dmg added!");
            }
        }

        private static void GetHeroesInGame()
        {
            foreach (var i in ObjectManager.Get<Obj_AI_Hero>().Where(i => i.Team == Player.Team))
            {
                Heroes.Add(new champion(i, 0));
                Console.WriteLine("[A]: " + i.ChampionName + " ally added to table!");
            }

            foreach (var i in ObjectManager.Get<Obj_AI_Hero>().Where(i => i.Team != Player.Team))
            {
                Heroes.Add(new champion(i, 0));
                Console.WriteLine("[A]: " + i.ChampionName + " enemy added to table!");
            }
        }

        private static void GetSmiteSlot()
        {
            if (Player.GetSpell(SpellSlot.Summoner1).Name.ToLower().Contains("smite"))
            {
                SmiteInGame = true;
                Smite = SpellSlot.Summoner1;
            }

            if (Player.GetSpell(SpellSlot.Summoner2).Name.ToLower().Contains("smite"))
            {
                SmiteInGame = true;
                Smite = SpellSlot.Summoner2;
            }
        }

        private static void GetTroysInGame()
        {
            foreach (var i in ObjectManager.Get<Obj_AI_Hero>().Where(h => h.Team != Player.Team))
            {
                foreach (var item in gametroydata.troydata.Where(x => x.ChampionName == i.ChampionName))
                {
                    TroysInGame = true;
                    gametroy.Troys.Add(new gametroy(i, item.Slot, item.Name, 0, false));
                    Console.WriteLine("[A]: " + i.ChampionName + " troy detected/added to table!");
                }
            }
        }

        private static void SubMenuEx(Menu parent)
        {
            var menu = new Menu("Config", parent.Name + "sub");
            foreach (var hero in HeroManager.AllHeroes)
            {
                var side = hero.Team == Player.Team ? "[Ally]" : "[Enemy]";
                menu.AddItem(new MenuItem(parent.Name + "allon" + hero.NetworkId,
                    "Use for " + hero.ChampionName + " " + side)).SetValue(true);
            }

            parent.AddSubMenu(menu);
        }

        private static void SubMenu(Menu parent, bool enemy)
        {
            var menu = new Menu("Config", parent.Name + "sub");
            foreach (var hero in enemy ? HeroManager.Enemies : HeroManager.Allies)
            {
                var side = hero.Team == Player.Team ? "[Ally]" : "[Enemy]";
                menu.AddItem(new MenuItem(parent.Name + "useon" + hero.NetworkId,
                    "Use for " + hero.ChampionName + " " + side)).SetValue(true);
            }

            parent.AddSubMenu(menu);
        }

        private static void Obj_AI_Base_OnLevelUp(Obj_AI_Base sender, EventArgs args)
        {
            var hero = sender as Obj_AI_Hero;
            if (hero == null)
                return;

            if (!hero.IsMe || !Origin.Item("al6").GetValue<bool>())
                return;

            if (hero.ChampionName == "Jayce" || hero.ChampionName == "Udyr" ||
                hero.ChampionName == "Azir"  || hero.ChampionName == "Elise")
                return;

            switch (Player.Level)
            {
                case 6:
                    Player.Spellbook.LevelSpell(SpellSlot.R);
                    break;
            }
        }

        private static object NewInstance(Type type)
        {
            var target = type.GetConstructor(Type.EmptyTypes);
            var dynamic = new DynamicMethod(string.Empty, type, new Type[0], target.DeclaringType);
            var il = dynamic.GetILGenerator();

            il.DeclareLocal(target.DeclaringType);
            il.Emit(OpCodes.Newobj, target);
            il.Emit(OpCodes.Stloc_0);
            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Ret);

            var method = (Func<object>) dynamic.CreateDelegate(typeof (Func<object>));
            return method();
        }
    }
}