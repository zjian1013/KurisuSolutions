﻿using System;
using System.Linq;
using LeagueSharp.Common;

namespace Activator.Summoners
{
    internal class heal : summoner
    {
        internal override string Name
        {
            get { return "summonerheal"; }
        }

        internal override string DisplayName
        {
            get { return "Heal"; }
        }
        internal override string[] ExtraNames
        {
            get { return new[] { "" }; }
        }

        internal override float Range
        {
            get { return 850f; }
        }

        internal override int Duration
        {
            get { return 0; }
        }

        public override void OnTick(EventArgs args)
        {
            if (!Menu.Item("use" + Name).GetValue<bool>())
                return;

            foreach (var hero in Activator.ChampionPriority())
            {
                if (!Parent.Item(Parent.Name + "allon" + hero.Player.NetworkId).GetValue<bool>())
                    continue;

                if (hero.Player.Distance(Player.ServerPosition) <= Range)
                {
                    if (hero.Player.Health / hero.Player.MaxHealth * 100 <=
                        Menu.Item("SelfLowHP" + Name + "Pct").GetValue<Slider>().Value)
                    {
                        if (hero.IncomeDamage > 0 && !hero.Player.IsRecalling() && !hero.Player.InFountain() ||
                            hero.IncomeDamage > hero.Player.Health)
                            UseSpell();
                    }

                    if (hero.IncomeDamage / hero.Player.MaxHealth * 100 >=
                        Menu.Item("SelfMuchHP" + Name + "Pct").GetValue<Slider>().Value)
                    {
                        if (hero.Player.MaxHealth - hero.Player.Health > (75 + (15 * Activator.Player.Level)))
                        {
                            if (hero.IncomeDamage > 0 && !hero.Player.IsRecalling() &&
                                !hero.Player.InFountain())
                                UseSpell();
                        }
                    }
                }
            }
        }
    }
}
