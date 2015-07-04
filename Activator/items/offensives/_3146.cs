﻿using System;
using LeagueSharp.Common;

namespace Activator.Items.Offensives
{
    class _3146 : item
    {
        internal override int Id
        {
            get { return 3146; }
        }

        internal override int Priority
        {
            get { return 7; }
        }

        internal override string Name
        {
            get { return "Gunblade"; }
        }

        internal override string DisplayName
        {
            get { return "Hextech Gunblade"; }
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
            get { return new[] { MenuType.SelfLowHP, MenuType.EnemyLowHP }; }
        }

        internal override MapType[] Maps
        {
            get { return new[] { MapType.Common }; }
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
            if (Menu.Item("use" + Name).GetValue<bool>() && Tar != null)
            {
                if (!Parent.Item(Parent.Name + "useon" + Tar.Player.NetworkId).GetValue<bool>())
                    return;

                if (Tar.Player.Health / Tar.Player.MaxHealth * 100 <= Menu.Item("enemylowhp" + Name + "pct").GetValue<Slider>().Value)
                {
                    UseItem(Tar.Player, true);
                }

                if (Player.Health / Player.MaxHealth * 100 <= Menu.Item("selflowhp" + Name + "pct").GetValue<Slider>().Value)
                {
                    UseItem(Tar.Player, true);
                }
            }
        }
    }
}
