using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;
using LeagueSharp;
using LeagueSharp.Common;
using Activator.Items;
using Activator.Spells;
using Activator.Summoners;

namespace Activator
{
    internal class Activator
    {
        internal static Menu Origin;
        internal static Obj_AI_Hero Player = ObjectManager.Player;

        internal static SpellSlot Smite;
        internal static bool SmiteInGame;
        internal static bool TroysInGame;

        private static int gametick;
        private static void Main(string[] args)
        {
            Console.WriteLine("Activator injected!");
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            GetSmiteSlot();
            GetTroysInGame();
            GetHeroesInGame();
            GetSlotDamage();

            new drawings();

            // new menu
            Origin = new Menu("Activator", "activator", true);
            var cmenu = new Menu("Cleansers", "cleansers");
            GetItemGroup("Items.Cleansers").ForEach(t => NewItem((item) NewInstance(t), cmenu));

            var ccmenu = new Menu("Cleanse Debuffs", "cdeb");
            ccmenu.AddItem(new MenuItem("cexhaust", "Exhaust")).SetValue(true);
            ccmenu.AddItem(new MenuItem("cstun", "Stuns")).SetValue(true);
            ccmenu.AddItem(new MenuItem("ccharm", "Charms")).SetValue(true);
            ccmenu.AddItem(new MenuItem("ctaunt", "Taunts")).SetValue(true);
            ccmenu.AddItem(new MenuItem("cfear", "Fears")).SetValue(true);
            ccmenu.AddItem(new MenuItem("cflee", "Flee")).SetValue(true);
            ccmenu.AddItem(new MenuItem("csnare", "Snares")).SetValue(true);
            ccmenu.AddItem(new MenuItem("csilence", "Silences")).SetValue(true);
            ccmenu.AddItem(new MenuItem("csupp", "Supression")).SetValue(true);
            ccmenu.AddItem(new MenuItem("cpolymorph", "Polymorphs")).SetValue(true);
            ccmenu.AddItem(new MenuItem("cblind", "Blinds")).SetValue(true);
            ccmenu.AddItem(new MenuItem("cslow", "Slows")).SetValue(true);
            ccmenu.AddItem(new MenuItem("cpoison", "Poisons")).SetValue(true);
            cmenu.AddSubMenu(ccmenu);
            cmenu.AddItem(new MenuItem("qssdebug", "Debug")).SetValue(false);   

            Origin.AddSubMenu(cmenu);

            var dmenu = new Menu("Defensives", "dmenu");
            GetItemGroup("Items.Defensives").ForEach(t => NewItem((item) NewInstance(t), dmenu));
            Origin.AddSubMenu(dmenu);

            var smenu = new Menu("Summoners", "smenu");
            GetItemGroup("Summoners").ForEach(t => NewSummoner((summoner) NewInstance(t), smenu));
            Origin.AddSubMenu(smenu);

            var omenu = new Menu("Offensives", "omenu");
            GetItemGroup("Items.Offensives").ForEach(t => NewItem((item) NewInstance(t), omenu));
            Origin.AddSubMenu(omenu);

            var imenu = new Menu("Consumables", "imenu");
            GetItemGroup("Items.Consumables").ForEach(t => NewItem((item) NewInstance(t), imenu));
            Origin.AddSubMenu(imenu);

            var amenu = new Menu("Auto Spells", "amenu");
            GetItemGroup("Spells.Evaders").ForEach(t => NewSpell((spell)NewInstance(t), amenu));
            GetItemGroup("Spells.Shields").ForEach(t => NewSpell((spell)NewInstance(t), amenu));
            GetItemGroup("Spells.Health").ForEach(t => NewSpell((spell)NewInstance(t), amenu));
            GetItemGroup("Spells.Slows").ForEach(t => NewSpell((spell)NewInstance(t), amenu));
            GetItemGroup("Spells.Heals").ForEach(t => NewSpell((spell)NewInstance(t), amenu));
            Origin.AddSubMenu(amenu);

            var zmenu = new Menu("Misc/Settings", "settings");

            if (SmiteInGame)
            {
                var ddmenu = new Menu("Drawings", "drawings");
                ddmenu.AddItem(new MenuItem("drawfill", "Draw Smite Fill")).SetValue(true);
                ddmenu.AddItem(new MenuItem("drawsmite", "Draw Smite Range")).SetValue(true);
                zmenu.AddSubMenu(ddmenu);
            }

            zmenu.AddItem(new MenuItem("evadeon", "Evade Integration")).SetValue(false);
            zmenu.AddItem(new MenuItem("evadefow", "Evade Integration (FoW)")).SetValue(false);
            zmenu.AddItem(new MenuItem("usecombo", "Combo Key")).SetValue(new KeyBind(32, KeyBindType.Press, true));


            Origin.AddSubMenu(zmenu);       

            Origin.AddToMainMenu();

            // auras and buffs
            spelldebuffhandler.Load();

            // damage prediction 
            projectionhandler.Load();

            // ground object spells
            gametroyhandler.Load();

            // on update/tick
            Game.OnUpdate += Game_OnUpdate;

            // auto level r
            Obj_AI_Base.OnLevelUp += Obj_AI_Base_OnLevelUp;

        }

        private static void Obj_AI_Base_OnLevelUp(Obj_AI_Base sender, EventArgs args)
        {
            var hero = sender as Obj_AI_Hero;
            if (hero == null)
                return;

            if (!hero.IsMe)
                return;

            if (hero.ChampionName == "Jayce" || hero.ChampionName == "Udyr")
                return;

            switch (Player.Level)
            {
                case 6:
                    Player.Spellbook.LevelSpell(SpellSlot.R);
                    break;
                case 11:
                    Player.Spellbook.LevelSpell(SpellSlot.R);
                    break;
                case 16:
                    Player.Spellbook.LevelSpell(SpellSlot.R);
                    break;
            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (Environment.TickCount - gametick < 
                Origin.Item("ticklimit").GetValue<Slider>().Value)
                return;

            foreach (var item in spelldata.items)
                if (LeagueSharp.Common.Items.HasItem(item.Id) &&
                    LeagueSharp.Common.Items.CanUseItem(item.Id))
                        item.OnTick();

            foreach (var summoner in spelldata.summoners)
                if (Player.Spellbook.CanUseSpell(summoner.Slot) == SpellState.Ready)
                    summoner.OnTick();

            foreach (var autospell in spelldata.mypells)
                if (Player.Spellbook.CanUseSpell(autospell.Slot) == SpellState.Ready)
                    autospell.OnTick();

            gametick = Environment.TickCount;
        }

        private static void NewItem(item item, Menu parent)
        {
            spelldata.items.Add(item.CreateMenu(parent));
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
                                 !t.Name.Contains("c__")); // wtf
        }

        public static void GetSlotDamage()
        {
            foreach (
                var spell in
                    Damage.Spells.Where(entry => entry.Key == Player.ChampionName).SelectMany(entry => entry.Value))
            {
                spelldata.combod.Add(spell.Damage, spell.Slot);
                Console.WriteLine(Player.ChampionName + ": " + spell.Slot + " " + spell.Stage + " - dmg added!");
            }
        }

        public static void GetHeroesInGame()
        {
            foreach (var i in ObjectManager.Get<Obj_AI_Hero>().Where(i => i.Team == Player.Team))
            {
                champion.Heroes.Add(new champion(i, 0));
                Console.WriteLine(i.ChampionName + " ally added to table!");
            }
        }

        public static void GetSmiteSlot()
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

        public static void GetTroysInGame()
        {
            foreach (var i in ObjectManager.Get<Obj_AI_Hero>().Where(h => h.Team != Player.Team))
            {
                foreach (var item in spelldata.troydata.Where(x => x.ChampionName == i.ChampionName))
                {
                    TroysInGame = true;
                    gametroy.Troys.Add(new gametroy(i, item.Slot, item.Name, 0, false));
                    Console.WriteLine(i.ChampionName + " troy detected/added to table!");
                }
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

            var method = (Func<object>)dynamic.CreateDelegate(typeof(Func<object>));
            return method();
        }
    }
}
