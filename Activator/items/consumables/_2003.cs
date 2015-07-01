using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

namespace Activator.Items.Consumables
{
    class _2003 : item
    {
        internal override int Id
        {
            get { return 2003; }
        }

        internal override int Priority
        {
            get { return 3; }
        }

        internal override string Name
        {
            get { return "Health Potion"; }
        }

        internal override string DisplayName
        {
            get { return "Health Potion"; }
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
            get { return new[] { MenuType.SelfLowHP, MenuType.SelfMuchHP }; }
        }

        internal override MapType[] Maps
        {
            get { return new[] { MapType.Common }; }
        }

        internal override int DefaultHP
        {
            get { return 50; }
        }

        internal override int DefaultMP
        {
            get { return 0; }
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

                    if (hero.Player.HasBuff("RegenerationPotion", true))
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
                }
            }
        }
    }
}
