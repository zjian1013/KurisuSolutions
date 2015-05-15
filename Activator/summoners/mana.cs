using System;
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

        internal override float Range
        {
            get { return 600f; }
        }

        internal override int Cooldown
        {
            get { return 180000; }
        }

        public override void OnTick(EventArgs args)
        {
            if (!Menu.Item("use" + Name).GetValue<bool>())
                return;

            foreach (var hero in champion.Heroes)
            { 
                if (hero.Player.IsRecalling() || hero.Player.InFountain())
                    return;

                if (hero.Player.Distance(Player.ServerPosition) <= Range)
                {
                    if (hero.Player.Mana/hero.Player.MaxMana*100 <= Menu.Item("SelfLowMP" + Name + "Pct").GetValue<Slider>().Value)
                    {
                        UseSpell();
                        RemoveSpell();
                    }
                }
            }
        }
    }
}
