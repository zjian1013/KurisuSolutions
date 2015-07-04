﻿using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

namespace Activator.Spells.Health
{
    class undyingrage : spell
    {
        internal override string Name
        {
            get { return "undyingrage"; }
        }

        internal override string DisplayName
        {
            get { return "Undying Rage | R"; }
        }

        internal override float Range
        {
            get { return float.MaxValue; }
        }

        internal override MenuType[] Category
        {
            get { return new[] { MenuType.SelfLowHP }; }
        }

        internal override int DefaultHP
        {
            get { return 15; }
        }

        internal override int DefaultMP
        {
            get { return 0; }
        }

        public override void OnTick(EventArgs args)
        {
            if (!Menu.Item("use" + Name).GetValue<bool>())
                return;

            foreach (var hero in Activator.ChampionPriority())
            {
                if (hero.Player.NetworkId == Player.NetworkId)
                {
                    if (!Parent.Item(Parent.Name + "useon" + hero.Player.NetworkId).GetValue<bool>())
                        continue;

                    if (!Player.HasBuffOfType(BuffType.Invulnerability))
                    {
                        if (hero.Player.Health / hero.Player.MaxHealth*100 <=
                            Menu.Item("selflowhp" + Name + "pct").GetValue<Slider>().Value)
                        {
                            if (hero.IncomeDamage > 0)
                                UseSpell();
                        }
                    }
                }
            }
        }
    }
}
