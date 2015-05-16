using System;
using LeagueSharp.Common;

namespace Activator.Items.Consumables
{
    class _2041 : item
    {
        internal override int Id
        {
            get { return 2041; }
        }

        internal override string Name
        {
            get { return "Crystalline Flask"; }
        }

        internal override float Range
        {
            get { return float.MaxValue; }
        }

        internal override MenuType[] Category
        {
            get { return new[] { MenuType.SelfLowMP, MenuType.SelfLowHP, MenuType.SelfMuchHP }; }
        }

        internal override int DefaultHP
        {
            get { return 55; }
        }

        internal override int DefaultMP
        {
            get { return 55; }
        }

        public override void OnTick(EventArgs args)
        {
            foreach (var hero in champion.Heroes)
            {
                if (hero.Player.NetworkId == Player.NetworkId)
                {
                    if (!Menu.Item("use" + Name).GetValue<bool>())
                        return;

                    if (hero.UsingMixedPot)
                        return;

                    if (hero.Player.IsRecalling() || Player.InFountain())
                        return;

                    if (hero.Player.Health / hero.Player.MaxHealth*100 <= Menu.Item("SelfLowHP" + Name + "Pct").GetValue<Slider>().Value)
                    {
                        UseItem();
                    }

                    if (hero.IncomeDamage/hero.Player.MaxHealth*100 >=
                        Menu.Item("SelfMuchHP" + Name + "Pct").GetValue<Slider>().Value)
                    {
                        UseItem();
                    }

                    if (hero.Player.MaxMana > 100)
                    {
                        if (hero.Player.Mana / hero.Player.MaxMana * 100 <= Menu.Item("SelfLowMP" + Name + "Pct").GetValue<Slider>().Value)
                        {
                            UseItem();
                        }
                    }
                }
            }
        }
    }
}
