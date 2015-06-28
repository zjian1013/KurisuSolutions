﻿using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

namespace Activator.Spells.Evaders
{
    class fizzjump : spell
    {
        internal override string Name
        {
            get { return "fizzjump"; }
        }

        internal override string DisplayName
        {
            get { return "Playful / Trickster | E"; }
        }

        internal override float Range
        {
            get { return float.MaxValue; }
        }

        internal override MenuType[] Category
        {
            get { return new[] { MenuType.SelfMuchHP, MenuType.Zhonyas, MenuType.SelfMinMP }; }
        }

        internal override int DefaultHP
        {
            get { return 30; }
        }

        internal override int DefaultMP
        {
            get { return 45; }
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
                if (hero.Player.NetworkId == Player.NetworkId)
                {
                    if (!Parent.Item(Parent.Name + "useon" + hero.Player.ChampionName).GetValue<bool>())
                        continue;

                    if (Menu.Item("use" + Name + "Norm").GetValue<bool>())
                        if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Danger))
                            UseSpellTowards(Game.CursorPos);

                    if (Menu.Item("use" + Name + "Ulti").GetValue<bool>())
                        if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Ultimate))
                            UseSpellTowards(Game.CursorPos);

                    if (hero.IncomeDamage / Player.MaxHealth * 100 >=
                        Menu.Item("SelfMuchHP" + Name + "Pct").GetValue<Slider>().Value)
                            UseSpellTowards(Game.CursorPos);                
                }
            }
        }
    }
}
