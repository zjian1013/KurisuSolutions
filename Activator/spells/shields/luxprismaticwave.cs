using System;
using LeagueSharp.Common;

namespace Activator.Spells.Shields
{
    class luxprismaticwave : spell
    {
        internal override string Name
        {
            get { return "luxprismaticwave"; }
        }

        internal override string DisplayName
        {
            get { return "Prismatic Barrier | W"; }
        }

        internal override float Range
        {
            get { return 1075f; }
        }

        internal override MenuType[] Category
        {
            get { return new[] { MenuType.SelfLowHP, MenuType.SelfMuchHP, MenuType.SelfMinMP }; }
        }

        internal override int DefaultHP
        {
            get { return 95; }
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
                    if (hero.IncomeDamage / hero.Player.MaxHealth * 100 >=
                        Menu.Item("SelfMuchHP" + Name + "Pct").GetValue<Slider>().Value)
                    {
                        UseSpellTowards(hero.Player.ServerPosition);
                        RemoveSpell();
                    }

                    if (hero.Player.Health / hero.Player.MaxHealth * 100 <=
                        Menu.Item("SelfLowHP" + Name + "Pct").GetValue<Slider>().Value)
                    {
                        if (hero.IncomeDamage > 0)
                        {
                            UseSpellTowards(hero.Player.ServerPosition);
                            RemoveSpell();
                        }
                    }
                }
            }
        }
    }
}
