using System;
using LeagueSharp.Common;

namespace Activator.Items.Consumables
{
    class _2010 : item
    {
        internal override int Id
        {
            get { return 2010; }
        }

        internal override string Name
        {
            get { return "Total Biscuit"; }
        }

        internal override float Range
        {
            get { return float.MaxValue; }
        }
        internal override MenuType[] Category
        {
            get { return new[] { MenuType.SelfLowHP, MenuType.SelfLowMP, MenuType.SelfMuchHP }; }
        }

        internal override int DefaultHP
        {
            get { return 50; }
        }

        internal override int DefaultMP
        {
            get { return 40; }
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
                        if (hero.IncomeDamage > 0)
                        {
                            UseItem();
                            RemoveItem();
                        }
                    }
                }
            }

            if (Player.MaxMana <= 100)
                return;

            if (Player.Mana / Player.MaxMana * 100 <= Menu.Item("SelfLowMP" + Name + "Pct").GetValue<Slider>().Value)
            {
                UseItem();
                RemoveItem();
            }
        }
    }
}
