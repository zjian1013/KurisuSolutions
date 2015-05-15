using System;
using LeagueSharp.Common;

namespace Activator.Spells.Heals
{
    class removescurvy : spell
    {
        internal override string Name
        {
            get { return "removescurvey"; }
        }

        internal override string DisplayName
        {
            get { return "Remove Scurvy | W"; }
        }

        internal override float Range
        {
            get { return float.MaxValue; }
        }

        internal override MenuType[] Category
        {
            get { return new[] { MenuType.SelfLowHP, MenuType.Cleanse, MenuType.SelfMinMP, MenuType.SelfMuchHP,  }; }
        }

        internal override int DefaultHP
        {
            get { return 50; }
        }

        internal override int DefaultMP
        {
            get { return 55; }
        }

        public override void OnTick(EventArgs args)
        {
            if (!Menu.Item("use" + Name).GetValue<bool>())
                return;

            foreach (var hero in champion.Heroes)
            {
                if (hero.Player.Distance(Player.ServerPosition) > Range)
                    return;

                if (Player.Mana / Player.MaxMana * 100 > Menu.Item("SelfMinMP" + Name + "Pct").GetValue<Slider>().Value)
                {
                    if (hero.Player.Health / hero.Player.MaxHealth * 100 <=
                        Menu.Item("SelfLowHP" + Name + "Pct").GetValue<Slider>().Value)
                    {
                        UseSpell();
                    }

                    if (hero.IncomeDamage / hero.Player.MaxHealth * 100 >=
                        Menu.Item("SelfMuchHP" + Name + "Pct").GetValue<Slider>().Value)
                    {
                        UseSpell();
                    }
                }
            }
        }
    }
}
