using System;
using LeagueSharp;
using LeagueSharp.Common;

namespace Activator.Spells.Heals
{
    class judicatorblessing : spell
    {
        internal override string Name
        {
            get { return "judicatorblessing"; }
        }

        internal override string DisplayName
        {
            get { return "Divine Blessing | W"; }
        }

        internal override float Range
        {
            get { return 900f; }
        }

        internal override MenuType[] Category
        {
            get { return new[] { MenuType.SelfLowHP, MenuType.SelfMuchHP, MenuType.SelfMinMP }; }
        }

        internal override int DefaultHP
        {
            get { return 90; }
        }

        internal override int DefaultMP
        {
            get { return 55; }
        }

        public override void OnTick(EventArgs args)
        {
            if (!Menu.Item("use" + Name).GetValue<bool>() ||
                Player.GetSpell(Slot).State != SpellState.Ready)
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
                        UseSpellOn(hero.Player);
                    }

                    if (hero.IncomeDamage / hero.Player.MaxHealth * 100 >=
                        Menu.Item("SelfMuchHP" + Name + "Pct").GetValue<Slider>().Value)
                    {
                        UseSpellOn(hero.Player);
                    }
                }
            }
        }
    }
}
