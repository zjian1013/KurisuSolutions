using System;
using LeagueSharp;
using LeagueSharp.Common;

namespace Activator.Items.Offensives
{
    class _3144 : item
    {
        internal override int Id
        {
            get { return 3144; }
        }

        internal override string Name
        {
            get { return "Cutlass"; }
        }

        internal override string DisplayName
        {
            get { return "Bilgewater's Cutlass"; }
        }

        internal override int Cooldown
        {
            get { return 90000; }
        }

        internal override float Range
        {
            get { return 450f; }
        }

        internal override MenuType[] Category
        {
            get { return new[] { MenuType.SelfLowHP, MenuType.EnemyLowHP }; }
        }

        internal override int DefaultHP
        {
            get { return 95; }
        }

        internal override int DefaultMP
        {
            get { return 0; }
        }

        public override void OnTick()
        {
            if (Menu.Item("use" + Name).GetValue<bool>() && Target != null)
            {
                if ((Target.Health / Target.MaxHealth * 100) <= Menu.Item("EnemyLowHP" + Name + "Pct").GetValue<Slider>().Value)
                {
                    UseItem(Target, true);   
                }

                if ((Player.Health / Player.MaxHealth * 100) <= Menu.Item("SelfLowHP" + Name + "Pct").GetValue<Slider>().Value)
                {
                    UseItem(Target, true); 
                }
            }
        }
    }
}
