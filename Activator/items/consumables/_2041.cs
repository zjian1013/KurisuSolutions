using System;
using System.Linq;
using LeagueSharp.Common;

namespace Activator.Items.Consumables
{
    class _2041 : item
    {
        internal override int Id
        {
            get { return 2041; }
        }
        internal override int Priority
        {
            get { return 3; }
        }

        internal override string Name
        {
            get { return "Crystalline Flask"; }
        }

        internal override string DisplayName
        {
            get { return "Crystalline Flask"; }
        }

        internal override int Duration
        {
            get { return 100; }
        }

        internal override float Range
        {
            get { return float.MaxValue; }
        }

        internal override MenuType[] Category
        {
            get { return new[] { MenuType.SelfLowMP, MenuType.SelfLowHP, MenuType.SelfMuchHP }; }
        }

        internal override MapType[] Maps
        {
            get { return new[] { MapType.SummonersRift, MapType.TwistedTreeline, MapType.CrystalScar }; }
        }

        internal override int DefaultHP
        {
            get { return 55; }
        }

        internal override int DefaultMP
        {
            get { return 25; }
        }

        public override void OnTick(EventArgs args)
        {
            foreach (var hero in Activator.ChampionPriority())
            {
                if (hero.Player.NetworkId == Player.NetworkId)
                {
                    if (!Menu.Item("use" + Name).GetValue<bool>() ||
                        !LeagueSharp.Common.Items.CanUseItem(Id))
                        return;

                    if (hero.Player.HasBuff("ItemCrystalFlask", true))
                        return;

                    if (hero.Player.Health/hero.Player.MaxHealth*100 <=
                        Menu.Item("SelfLowHP" + Name + "Pct").GetValue<Slider>().Value)
                    {
                        if (hero.IncomeDamage > 0 || hero.MinionDamage > 0)
                        {
                            if (!hero.Player.IsRecalling() && !hero.Player.InFountain())
                                UseItem();
                        }
                    }

                    if (hero.IncomeDamage/hero.Player.MaxHealth*100 >=
                        Menu.Item("SelfMuchHP" + Name + "Pct").GetValue<Slider>().Value)
                    {
                        if (hero.IncomeDamage > 0 || hero.MinionDamage > 0)
                        {
                            if (!hero.Player.IsRecalling() && !hero.Player.InFountain())
                                UseItem();
                        }
                    }

                    if (hero.Player.MaxMana <= 200) 
                        continue;

                    if (hero.Player.Mana / hero.Player.MaxMana * 100 <= 
                        Menu.Item("SelfLowMP" + Name + "Pct").GetValue<Slider>().Value)
                    {
                        if (!hero.Player.IsRecalling() && !hero.Player.InFountain())
                            UseItem();
                    }
                }
            }
        }
    }
}
