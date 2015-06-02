using System;
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

        internal override float Range
        {
            get { return 850f; }
        }

        internal override int Cooldown
        {
            get { return 240000; }
        }

        public override void OnTick()
        {
            if (!Menu.Item("use" + Name).GetValue<bool>())
                return;

            if (Player.IsRecalling() || Player.InFountain())
                return;

            foreach (var hero in champion.Heroes)
            {
                if (hero.Player.Distance(Player.ServerPosition) > Range)
                    return;

                if (hero.Player.Health/hero.Player.MaxHealth*100 <=
                    Menu.Item("SelfLowHP" + Name + "Pct").GetValue<Slider>().Value)
                {
                    if (hero.IncomeDamage > 0 &&
                        hero.HitTypes.Except(ExcludedList).Any())
                        UseSpell();
                }

                if (hero.IncomeDamage/hero.Player.MaxHealth*100 >=
                    Menu.Item("SelfMuchHP" + Name + "Pct").GetValue<Slider>().Value)
                {
                    if (hero.IncomeDamage > 0 &&
                        hero.HitTypes.Except(ExcludedList).Any())
                        UseSpell();                       
                }
            }
        }
    }
}
