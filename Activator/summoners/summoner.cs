﻿using System;
using LeagueSharp;
using LeagueSharp.Common;

namespace Activator.Summoners
{
    public class summoner
    {
        internal virtual string Name { get; set; }
        internal virtual string DisplayName { get; set; }
        internal virtual string[] ExtraNames { get; set; }
        internal virtual float Range { get; set; }
        internal virtual int Duration { get; set; }
        internal virtual int DefaultMP { get; set; }
        internal virtual int DefaultHP { get; set; }

        public Menu Menu { get; private set; }
        public Menu Parent { get { return Menu.Parent; } }
        public SpellSlot Slot { get { return Player.GetSpellSlot(Name); } }
        public Obj_AI_Hero Player { get { return ObjectManager.Player; } }

        public void UseSpell(bool combo = false)
        {
            if (!combo || Activator.Origin.Item("usecombo").GetValue<KeyBind>().Active)
            {
                if (Utils.GameTimeTickCount - Activator.LastUsedTimeStamp > Activator.LastUsedDuration)
                {
                    if (Player.GetSpell(Slot).State == SpellState.Ready)
                    {
                        Player.Spellbook.CastSpell(Slot);
                        Activator.LastUsedTimeStamp = Utils.GameTimeTickCount;
                        Activator.LastUsedDuration = Duration;
                    }
                }
            }
        }

        public void UseSpellOn(Obj_AI_Base target, bool combo = false)
        {
            if (!combo || Activator.Origin.Item("usecombo").GetValue<KeyBind>().Active)
            {
                if (Utils.GameTimeTickCount - Activator.LastUsedTimeStamp > Activator.LastUsedDuration)
                {
                    if (Player.GetSpell(Slot).State == SpellState.Ready)
                    {
                        Player.Spellbook.CastSpell(Slot, target);
                        Activator.LastUsedTimeStamp = Utils.GameTimeTickCount;
                        Activator.LastUsedDuration = Duration;
                    }
                }
            }
        }

        public summoner CreateMenu(Menu root)
        {
            Menu = new Menu(DisplayName, "m" + Name);

            if (!Name.Contains("smite"))
                Menu.AddItem(new MenuItem("use" + Name, "Use " + DisplayName)).SetValue(true);

            if (Name == "summonerheal")
            {
                Menu.AddItem(new MenuItem("SelfLowHP" + Name + "Pct", "Use on Hero HP % <="))
                    .SetValue(new Slider(20));
                Menu.AddItem(new MenuItem("SelfMuchHP" + Name + "Pct", "Use on Hero Dmg Dealt % >="))
                    .SetValue(new Slider(45));
            }

            if (Name == "summonerboost")
            {
                var ccmenu = new Menu(DisplayName + " Debuffs", DisplayName.ToLower() + "cdeb");
                ccmenu.AddItem(new MenuItem(Name + "cignote", "Ignite")).SetValue(true);
                ccmenu.AddItem(new MenuItem(Name + "cexhaust", "Exhaust")).SetValue(true);
                ccmenu.AddItem(new MenuItem(Name + "cstun", "Stuns")).SetValue(true);
                ccmenu.AddItem(new MenuItem(Name + "ccharm", "Charms")).SetValue(true);
                ccmenu.AddItem(new MenuItem(Name + "ctaunt", "Taunts")).SetValue(true);
                ccmenu.AddItem(new MenuItem(Name + "cfear", "Fears")).SetValue(true);
                ccmenu.AddItem(new MenuItem(Name + "cflee", "Flee")).SetValue(true);
                ccmenu.AddItem(new MenuItem(Name + "csnare", "Snares")).SetValue(true);
                ccmenu.AddItem(new MenuItem(Name + "csilence", "Silences")).SetValue(true);
                ccmenu.AddItem(new MenuItem(Name + "csupp", "Supression")).SetValue(true);
                ccmenu.AddItem(new MenuItem(Name + "cpolymorph", "Polymorphs")).SetValue(true);
                ccmenu.AddItem(new MenuItem(Name + "cblind", "Blinds")).SetValue(true);
                ccmenu.AddItem(new MenuItem(Name + "cslow", "Slows")).SetValue(true);
                ccmenu.AddItem(new MenuItem(Name + "cpoison", "Poisons")).SetValue(true);
                Menu.AddSubMenu(ccmenu);

                Menu.AddItem(new MenuItem("use" + Name + "Number", "Minimum Spells to Use")).SetValue(new Slider(1, 1, 5));
                Menu.AddItem(new MenuItem("use" + Name + "Time", "Minumum Durration to Use")).SetValue(new Slider(2, 1, 5));
                Menu.AddItem(new MenuItem("use" + Name + "Od", "Use only on Dangerous")).SetValue(false);
                Menu.AddItem(new MenuItem("mode" + Name, "Mode: ")).SetValue(new StringList(new[] { "Always", "Combo" }));
            }

            if (Name == "summonerdot")
                Menu.AddItem(new MenuItem("mode" + Name, "Mode: "))
                    .SetValue(new StringList(new[] { "Killsteal", "Combo" }, 1));

            if (Name == "summonermana")
                Menu.AddItem(new MenuItem("SelfLowMP" + Name + "Pct", "Minimum Mana % <=")).SetValue(new Slider(40));

            if (Name == "summonerbarrier")
            {
                Menu.AddItem(new MenuItem("SelfLowHP" + Name + "Pct", "Use on Hero HP % <=")).SetValue(new Slider(20));
                Menu.AddItem(new MenuItem("SelfMuchHP" + Name + "Pct", "Use on Hero Dmg Dealt % >=")).SetValue(new Slider(45));
                Menu.AddItem(new MenuItem("use" + Name + "Ulti", "Use on Dangerous (Ultimates Only)")).SetValue(true);
            }

            if (Name == "summonerexhaust")
            {
                Menu.AddItem(new MenuItem("a" + Name + "Pct", "Exhaust on ally HP %")).SetValue(new Slider(35));
                Menu.AddItem(new MenuItem("e" + Name + "Pct", "Exhaust on enemy HP %")).SetValue(new Slider(35));
                Menu.AddItem(new MenuItem("use" + Name + "Ulti", "Use on Dangerous")).SetValue(true);
                Menu.AddItem(new MenuItem("mode" + Name, "Mode: ")).SetValue(new StringList(new[] { "Always", "Combo" }));
            }

            if (Activator.SmiteInGame && Name == "summonersmite")
            {
                Menu.AddItem(new MenuItem("usesmite", "Use Smite")).SetValue(new KeyBind('M', KeyBindType.Toggle, true));
                Menu.AddItem(new MenuItem("smitesmall", "Smite Small Camps")).SetValue(true);
                Menu.AddItem(new MenuItem("smitelarge", "Smite Large Camps")).SetValue(true);
                Menu.AddItem(new MenuItem("smitesuper", "Smite Epic Camps")).SetValue(true);
                Menu.AddItem(new MenuItem("smitemode", "Smite Enemies: "))
                    .SetValue(new StringList(new[] { "Killsteal", "Combo", "Nope" }, 1));
                Menu.AddItem(new MenuItem("savesmite", "Save a Smite Charge").SetValue(true));          
            }

            root.AddSubMenu(Menu);
            return this;
        }

        public virtual void OnDraw(EventArgs args)
        {

        }

        public virtual void OnTick(EventArgs args)
        {

        }
    }
}
