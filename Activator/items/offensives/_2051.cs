﻿using System;
using LeagueSharp;
using LeagueSharp.Common;

namespace Activator.Items.Offensives
{
    class _2051 : item
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
            get { return "Guardians"; }
        }

        internal override string DisplayName
        {
            get { return "Guardian's Horn"; }
        }

        internal override MapType[] Maps
        {
            get { return new[] { MapType.HowlingAbyss }; }
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
            get { return new[] { MenuType.SelfLowHP, MenuType.EnemyLowHP, MenuType.SelfCount }; }
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

                if ((Tar.Player.Health / Tar.Player.MaxHealth * 100) <= Menu.Item("EnemyLowHP" + Name + "Pct").GetValue<Slider>().Value)
                {
                    UseItem(Tar.Player, true);  
                }

                if ((Player.Health / Player.MaxHealth * 100) <= Menu.Item("SelfLowHP" + Name + "Pct").GetValue<Slider>().Value)
                {
                    UseItem(Tar.Player);
                }
            }

            if (Player.CountEnemiesInRange(Range) > Menu.Item("SelfCount" + Name).GetValue<Slider>().Value)
            {
                UseItem();
            }
        }
    }
}
