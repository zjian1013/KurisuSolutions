using System;
using LeagueSharp.Common;

namespace Activator.Items.Defensives
{
    class _3069 : item
    {
        internal override int Id
        {
            get { return 3069; }
        }

        internal override string Name
        {
            get { return "Talisman"; }
        }

        internal override string DisplayName
        {
            get { return "Talisman of Ascension"; }
        }

        internal override int Cooldown
        {
            get { return 60000; }
        }

        internal override float Range
        {
            get { return 600f; }
        }

        internal override MenuType[] Category
        {
            get { return new[] { MenuType.EnemyLowHP, MenuType.SelfLowHP, MenuType.SelfMuchHP, MenuType.Zhonyas }; }
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
                    return;

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
            }

            if (Target != null)
            {
                if (Target.Health / Target.MaxHealth * 100 <= Menu.Item("EnemyLowHP" + Name + "Pct").GetValue<Slider>().Value)
                {
                    UseItem(true);
                    RemoveItem(true);
                }

                foreach (var hero in champion.Heroes)
                {
                        return;

                    if (hero.Player.Health/Player.MaxHealth*100 <=
                        Menu.Item("SelfLowHP" + Name + "Pct").GetValue<Slider>().Value)
                    {
                        UseItem();
                        RemoveItem(true);
                    }
                }
            }

        }
    }
}
