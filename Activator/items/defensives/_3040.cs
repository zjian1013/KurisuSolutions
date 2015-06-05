using System;
using System.Linq;
using LeagueSharp.Common;

namespace Activator.Items.Defensives
{
    class _3040 : item
    {
        internal override int Id
        {
            get { return 3040; }
        }

        internal override string Name
        {
            get { return "Seraphs"; }
        }

        internal override string DisplayName
        {
            get { return "Seraph's Embrace"; }
        }

        internal override int Cooldown
        {
            get { return 120000; }
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
            get { return 50; }
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
                    if (Menu.Item("use" + Name + "Norm").GetValue<bool>() && 
                        hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Danger))
                        UseItem();

                    if (Menu.Item("use" + Name + "Ulti").GetValue<bool>() && 
                        hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Ultimate))
                        UseItem();

                    if (hero.Player.Health/hero.Player.MaxHealth*100 <=
                        Menu.Item("SelfLowHP" + Name + "Pct").GetValue<Slider>().Value)
                    {
                        if (hero.IncomeDamage > 0 || hero.MinionDamage > hero.Player.Health)
                            UseItem();
                    }

                    if (hero.IncomeDamage/hero.Player.MaxHealth*100 >=
                        Menu.Item("SelfMuchHP" + Name + "Pct").GetValue<Slider>().Value)
                        UseItem();
                }
            }
        }
    }
}
