using System;
using LeagueSharp;
using LeagueSharp.Common;

namespace Activator.Items.Cleansers
{
    class _3140 : item
    {
        internal override int Id
        {
            get { return 3140; }
        }

        internal override string Name
        {
            get { return "Quicksilver"; }
        }

        internal override string DisplayName
        {
            get { return "Quicksilver Sash"; }
        }

        internal override int Priority
        {
            get { return 6; }
        }

        internal override int Duration
        {
            get { return 1000; }
        }

        internal override float Range
        {
            get { return float.MaxValue; }
        }

        internal override MenuType[] Category
        {
            get { return new[] { MenuType.Cleanse, MenuType.ActiveCheck }; }
        }

        internal override MapType[] Maps
        {
            get { return new[] { MapType.Common }; }
        }

        internal override int DefaultHP
        {
            get { return 5; }
        }

        internal override int DefaultMP
        {
            get { return 0; }
        }

        public override void OnTick(EventArgs args)
        {
            foreach (var hero in champion.Heroes)
            {
                if (!Menu.Item("use" + Name).GetValue<bool>())
                    return;

                if (hero.Player.NetworkId == Player.NetworkId)
                {
                    if (hero.ForceQSS)
                    {
                        UseItem();
                        hero.IncomeDamage = 0;
                        hero.ForceQSS = false;
                    }

                    if (hero.QSSBuffCount >= Menu.Item("use" + Name + "Number").GetValue<Slider>().Value &&
                        hero.QSSHighestBuffTime >= Menu.Item("use" + Name + "Time").GetValue<Slider>().Value)
                    {
                        if (!Menu.Item("use" + Name + "Od").GetValue<bool>())
                        {
                            Utility.DelayAction.Add(Game.Ping + 80, delegate
                            {
                                UseItem(Menu.Item("mode" + Name).GetValue<StringList>().SelectedIndex == 1);
                            });
                        }
                    }
                }
            }
        }
    }
}
