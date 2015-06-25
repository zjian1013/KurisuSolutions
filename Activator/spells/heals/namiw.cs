using System;
using LeagueSharp;
using LeagueSharp.Common;

namespace Activator.Spells.Heals
{
    class namiw : spell
    {
        internal override string Name
        {
            get { return "namiw"; }
        }

        internal override string DisplayName
        {
            get { return "Ebb and Flow | W"; }
        }

        internal override float Range
        {
            get { return 725f; }
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
                Player.GetSpell(Player.GetSpellSlot(Name)).State != SpellState.Ready)
                return;

            if (Player.Mana/Player.MaxMana*100 <
                Menu.Item("SelfMinMP" + Name + "Pct").GetValue<Slider>().Value)
                return;

            foreach (var hero in champion.Heroes)
            {
                if (hero.Player.Distance(Player.ServerPosition) <= Range)
                {
                    if (!Parent.Item(Parent.Name + "useon" + hero.Player.ChampionName).GetValue<bool>())
                        continue;

                    if (hero.Player.Health/hero.Player.MaxHealth*100 <=
                        Menu.Item("SelfLowHP" + Name + "Pct").GetValue<Slider>().Value)
                        UseSpellOn(hero.Player);

                    if (hero.IncomeDamage/hero.Player.MaxHealth*100 >=
                        Menu.Item("SelfMuchHP" + Name + "Pct").GetValue<Slider>().Value)
                        UseSpellOn(hero.Player);
                }
            }
        }
    }
}
