﻿using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

namespace Activator.Summoners
{
    internal class barrier : summoner
    {
        internal override string Name
        {
            get { return "summonerbarrier"; }
        }

        internal override string DisplayName
        {
            get { return "Barrier"; }
        }

        internal override string[] ExtraNames
        {
            get { return new[] { "" }; }
        }

        internal override float Range
        {
            get { return float.MaxValue; }
        }

        internal override int Duration
        {
            get { return 2000; }
        }

        public override void OnTick(EventArgs args)
        {
            if (!Menu.Item("use" + Name).GetValue<bool>())
                return;

            foreach (var hero in champion.Heroes)
            {
                if (hero.Player.NetworkId != Player.NetworkId)
                    return;

                if (hero.Player.Health / hero.Player.MaxHealth * 100 <=
                    Menu.Item("SelfLowHP" + Name + "Pct").GetValue<Slider>().Value)
                {
                    if (hero.IncomeDamage > 0 && !Player.IsRecalling() && !Player.InFountain())
                    {
                        UseSpell();
                    }
                }

                if (hero.IncomeDamage / hero.Player.MaxHealth * 100 >=
                    Menu.Item("SelfMuchHP" + Name + "Pct").GetValue<Slider>().Value)
                {
                    if (hero.IncomeDamage > 0 && !Player.IsRecalling() && !Player.InFountain())
                    {
                        UseSpell();
                    }
                }

                if (Menu.Item("use" + Name + "Ulti").GetValue<bool>())
                {
                    if (hero.IncomeDamage > 0 && 
                        hero.HitTypes.Contains(HitType.Ultimate))
                    { 
                        UseSpell();
                    }
                }
            }
        }
    }
}
