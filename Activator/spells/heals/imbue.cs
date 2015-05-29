﻿using System;
using LeagueSharp;
using LeagueSharp.Common;

namespace Activator.Spells.Heals
{
    class imbue : spell
    {
        internal override string Name
        {
            get { return "imbue"; }
        }

        internal override string DisplayName
        {
            get { return "Imbue | Q"; }
        }

        internal override float Range
        {
            get { return 750f; }
        }

        internal override MenuType[] Category
        {
            get { return new[] { MenuType.SelfLowHP, MenuType.SelfMuchHP, MenuType.SelfMinMP  }; }
        }

        internal override int DefaultHP
        {
            get { return 90; }
        }

        internal override int DefaultMP
        {
            get { return 55; }
        }

        public override void OnTick()
        {
            if (!Menu.Item("use" + Name).GetValue<bool>())
                return;

            if (Player.Mana/Player.MaxMana*100 <
                Menu.Item("SelfLMinMP" + Name + "Pct").GetValue<Slider>().Value)
                return;

            foreach (var hero in champion.Heroes)
            {
                if (hero.Player.Distance(Player.ServerPosition) <= Range)
                {
                    if (hero.Player.Health/hero.Player.MaxHealth*100 <=
                        Menu.Item("SelfLowHP" + Name + "Pct").GetValue<Slider>().Value)
                        UseSpellOn(hero.Player);

                    if (hero.IncomeDamage/hero.Player.MaxHealth*100 >=
                        Menu.Item("SelfMuchHP" + Name + "Pct").GetValue<Slider>().Value)
                        UseSpellOn(hero.Player);
                }
            }
        }
    }
}
