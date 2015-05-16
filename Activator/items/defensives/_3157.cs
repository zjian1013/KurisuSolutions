using System;
using LeagueSharp;
using LeagueSharp.Common;

namespace Activator.Items.Defensives
{
    class _3157 : item
    {
        internal override int Id
        {
            get { return 3157; }
        }

        internal override string Name
        {
            get { return "Zhonyas"; }
        }

        internal override string DisplayName
        {
            get { return "Zhonay's Hourglass"; }
        }

        internal override int Cooldown
        {
            get { return 90000; }
        }

        internal override float Range
        {
            get { return 750f; }
        }

        internal override MenuType[] Category
        {
            get { return new[] { MenuType.SelfLowHP, MenuType.SelfMuchHP, MenuType.Zhonyas }; }
        }

        internal override int DefaultHP
        {
            get { return 20; }
        }

        internal override int DefaultMP
        {
            get { return 0; }
        }

        public override void OnTick(EventArgs args)
        {
            if (!Menu.Item("use" + Name).GetValue<bool>())
            {
                return;
            }

            foreach (var hero in champion.Heroes)
            {
                if (hero.Player.NetworkId == Player.NetworkId)
                {
                    if (Menu.Item("use" + Name + "Norm").GetValue<bool>())
                    {
                        if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Danger))
                        {
                            UseItem();
                            RemoveItem(true);
                        }
                    }

                    if (Menu.Item("use" + Name + "Ulti").GetValue<bool>())
                    {
                        if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Ultimate))
                        {
                            UseItem();
                            RemoveItem(true);
                        }
                    }

                    if (Player.Health/Player.MaxHealth*100 <=
                        Menu.Item("SelfLowHP" + Name + "Pct").GetValue<Slider>().Value)
                    {
                        if (hero.IncomeDamage > 0)
                        {
                            UseItem();
                            RemoveItem(true);
                        }
                    }
                }
            }
        }
    }
}
