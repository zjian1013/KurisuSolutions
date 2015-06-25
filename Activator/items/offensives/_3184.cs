using System;
using LeagueSharp.Common;

namespace Activator.Items.Offensives
{
    class _3184 : item
    {
        internal override int Id
        {
            get { return 3184; }
        }

        internal override int Priority
        {
            get { return 7; }
        }

        internal override string Name
        {
            get { return "Entropy"; }
        }

        internal override string DisplayName
        {
            get { return "Entropy"; }
        }

        internal override int Duration
        {
            get { return 100; }
        }

        internal override float Range
        {
            get { return 750f; }
        }

        internal override MenuType[] Category
        {
            get { return new[] { MenuType.SelfLowHP, MenuType.EnemyLowHP }; }
        }

        internal override MapType[] Maps
        {
            get { return new[] { MapType.CrystalScar, MapType.HowlingAbyss }; }
        }

        internal override int DefaultHP
        {
            get { return 95; }
        }

        internal override int DefaultMP
        {
            get { return 0; }
        }

        public override void OnTick(EventArgs args)
        {
            if (Menu.Item("use" + Name).GetValue<bool>() && Target != null)
            {
                if (!Parent.Item(Parent.Name + "useon" + Target.ChampionName).GetValue<bool>())
                    return;

                if (Target.Health/Target.MaxHealth*100 <=
                    Menu.Item("EnemyLowHP" + Name + "Pct").GetValue<Slider>().Value)
                {
                    UseItem(Target, true);
                }

                if (Player.Health/Player.MaxHealth*100 <=
                    Menu.Item("SelfLowHP" + Name + "Pct").GetValue<Slider>().Value)
                {
                    UseItem(Target, true);
                }
            }
        }
    }
}
