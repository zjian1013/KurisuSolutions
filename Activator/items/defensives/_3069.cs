using System;
using System.Linq;
using LeagueSharp.Common;

namespace Activator.Items.Defensives
{
    class _3069 : item
    {
        internal override int Id
        {
            get { return 3069; }
        }
        internal override int Priority
        {
            get { return 4; }
        }

        internal override string Name
        {
            get { return "Talisman"; }
        }

        internal override string DisplayName
        {
            get { return "Talisman of Ascension"; }
        }

        internal override int Duration
        {
            get { return 2000; }
        }

        internal override float Range
        {
            get { return 600f; }
        }

        internal override MenuType[] Category
        {
            get { return new[] { MenuType.EnemyLowHP, MenuType.SelfLowHP, MenuType.Zhonyas }; }
        }

        internal override MapType[] Maps
        {
            get { return new[] { MapType.Common }; }
        }

        internal override int DefaultHP
        {
            get { return 40; }
        }

        internal override int DefaultMP
        {
            get { return 0; }
        }

        public override void OnTick(EventArgs args)
        {
            if (!Menu.Item("use" + Name).GetValue<bool>())
                return;

            foreach (var hero in champion.Heroes)
            {
                if (!Parent.Item(Parent.Name + "useon" + hero.Player.ChampionName).GetValue<bool>())
                    continue;

                if (hero.Player.Distance(Player.ServerPosition) <= Range)
                {
                    if (Menu.Item("use" + Name + "Norm").GetValue<bool>())
                        if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Danger))
                            UseItem();

                    if (Menu.Item("use" + Name + "Ulti").GetValue<bool>())
                        if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Ultimate))
                            UseItem();

                    if (hero.Player.Health/hero.Player.MaxHealth*100 <=
                        Menu.Item("SelfLowHP" + Name + "Pct").GetValue<Slider>().Value)
                    {
                        if (hero.IncomeDamage > 0 && hero.Attacker != null &&
                            hero.Attacker.Distance(hero.Player.ServerPosition) <= 600)
                            UseItem();    
                    }
                }
            }

            if (Target != null)
            {
                if (Target.Health / Target.MaxHealth * 100 <= Menu.Item("EnemyLowHP" + Name + "Pct").GetValue<Slider>().Value)
                {
                    UseItem();
                }
            }
        }
    }
}
