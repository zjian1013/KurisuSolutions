using System;
using LeagueSharp.Common;

namespace Activator.Items.Consumables
{
    internal class _2004 : item
    {
        internal override int Id
        {
            get { return 2004; }
        }

        internal override string Name
        {
            get { return "Mana Potion"; }
        }

        internal override float Range
        {
            get { return float.MaxValue; }
        }

        internal override MenuType[] Category
        {
            get { return new[] { MenuType.SelfLowMP }; }
        }

        internal override MapType[] Maps
        {
            get { return new[] { MapType.Common }; }
        }

        internal override int DefaultHP
        {
            get { return 0; }
        }

        internal override int DefaultMP
        {
            get { return 40; }
        }

        public override void OnTick(EventArgs args)
        {
            if (!Menu.Item("use" + Name).GetValue<bool>())
                return;

            foreach (var hero in champion.Heroes)
            {
                if (hero.Player.NetworkId == Player.NetworkId)
                {
                    if (hero.Player.MaxMana <= 200)
                        continue;

                    if (hero.Player.HasBuff("FlaskOfCrystalWater", true))
                        return;

                    if (hero.Player.Mana/hero.Player.MaxMana*100 <= 
                        Menu.Item("SelfLowMP" + Name + "Pct").GetValue<Slider>().Value)
                    {
                        if (hero.IncomeDamage > 0 || hero.MinionDamage > 0)
                        {
                            if (!hero.Player.IsRecalling() && !hero.Player.InFountain())
                                UseItem();
                        }
                    }
                }
            }
        }
    }
}
