using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

namespace Activator.Summoners
{
    internal class mana : summoner
    {
        internal override string Name
        {
            get { return "summonermana"; }
        }

        internal override string DisplayName
        {
            get { return "Clarity"; }
        }
        internal override string[] ExtraNames
        {
            get { return new[] { "" }; }
        }

        internal override float Range
        {
            get { return 600f; }
        }

        internal override int Duration
        {
            get { return 1000; }
        }

        public override void OnTick(EventArgs args)
        {
            if (!Menu.Item("use" + Name).GetValue<bool>())
                return;

            foreach (var hero in Activator.ChampionPriority())
            {
                if (!Parent.Item(Parent.Name + "allon" + hero.Player.NetworkId).GetValue<bool>())
                    continue;

                if (hero.Player.MaxMana <= 200 || hero.Player.Distance(Player.ServerPosition) > Range)
                    continue;

                if (hero.Player.Mana/hero.Player.MaxMana*100 <= Menu.Item("SelfLowMP" + Name + "Pct").GetValue<Slider>().Value)
                {
                    if (!hero.Player.IsRecalling() && !hero.Player.InFountain())
                        UseSpell();
                }
            }
        }
    }
}
