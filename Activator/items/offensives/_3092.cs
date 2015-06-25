﻿using System;
using LeagueSharp.Common;

namespace Activator.Items.Offensives
{
    class _3092 : item
    {
        internal override int Id 
        {
            get { return 3092; }
        }

        internal override int Priority
        {
            get { return 7; }
        }

        internal override string Name 
        {
            get { return "Queens"; }
        }

        internal override string DisplayName
        {
            get { return "Frost Queen's Claim"; }
        }

        internal override float Range
        {
            get { return 850f; }
        }

        internal override int Duration
        {
            get { return 2000; }
        }

        internal override int DefaultHP
        {
            get { return 90; }
        }

        internal override MenuType[] Category
        {
            get { return new[] { MenuType.ActiveCheck, MenuType.SelfLowHP, MenuType.EnemyLowHP }; }
        }

        internal override MapType[] Maps
        {
            get { return new[] { MapType.Common }; }
        }

        public override void OnTick(EventArgs args)
        {
            if (Menu.Item("use" + Name).GetValue<bool>() && Target != null)
            {
                if (!Parent.Item(Parent.Name + "useon" + Target.ChampionName).GetValue<bool>())
                    return;

                if (Target.Health / Target.MaxHealth * 100 <= Menu.Item("EnemyLowHP" + Name + "Pct").GetValue<Slider>().Value)
                {
                    UseItem(Target.ServerPosition, Menu.Item("mode" + Name).GetValue<StringList>().SelectedIndex == 1);
                }

                if (Player.Health / Player.MaxHealth * 100 <= Menu.Item("SelfLowHP" + Name + "Pct").GetValue<Slider>().Value)
                {
                    UseItem(Target.ServerPosition, Menu.Item("mode" + Name).GetValue<StringList>().SelectedIndex == 1);
                }
            }
        }
    }
}
