﻿using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

namespace Activator.Spells.Shields
{
    class eyeofthestorm : spell
    {
        internal override string Name 
        {
            get { return "eyeofthestorm"; }
        }

        internal override string DisplayName
        {
            get { return "Eye of the Storm | E"; }
        }

        internal override float Range
        {
            get { return 800f; }
        }

        internal override MenuType[] Category
        {
            get { return new[] { MenuType.SelfLowHP, MenuType.SelfMuchHP, MenuType.SelfMinMP }; }
        }

        internal override int DefaultHP
        {
            get { return 95; }
        }

        internal override int DefaultMP
        {
            get { return 55; }
        }

        public override void OnTick(EventArgs args)
        {
            if (!Menu.Item("use" + Name).GetValue<bool>())
                return;

            if (Player.Mana/Player.MaxMana*100 <
                Menu.Item("SelfMinMP" + Name + "Pct").GetValue<Slider>().Value)
                return;

            foreach (var hero in Activator.ChampionPriority())
            {
                if (!Parent.Item(Parent.Name + "useon" + hero.Player.NetworkId).GetValue<bool>())
                    continue;

                if (hero.Player.Distance(Player.ServerPosition) <= Range)
                {
                    if (hero.IncomeDamage / hero.Player.MaxHealth * 100 >=
                        Menu.Item("SelfMuchHP" + Name + "Pct").GetValue<Slider>().Value)
                        UseSpellOn(hero.Player);

                    if (hero.Player.Health/hero.Player.MaxHealth*100 <=
                        Menu.Item("SelfLowHP" + Name + "Pct").GetValue<Slider>().Value)
                    {
                        if (hero.IncomeDamage > 0 || hero.MinionDamage > hero.Player.Health)
                            UseSpellOn(hero.Player);
                    }
                }
            }
        }
    }
}
