using System;
using LeagueSharp;
using LeagueSharp.Common;

namespace Activator.Items.Cleansers
{
    class _3139 : item
    {
        internal override int Id
        {
            get { return 3139; }
        }

        internal override string Name
        {
            get { return "Mercurial"; }
        }

        internal override string DisplayName
        {
            get { return "Mercurial Scimitar"; }
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
            get { return new[] { MapType.HowlingAbyss, MapType.SummonersRift }; }
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
                    if (hero.Player.Distance(Player.ServerPosition) > Range)
                        return;

                    if (hero.ForceQSS)
                    {
                        UseItem();
                        hero.IncomeDamage = 0;
                        hero.ForceQSS = false;
                    }

                    if (hero.MercurialBuffCount >= Menu.Item("use" + Name + "Number").GetValue<Slider>().Value &&
                        hero.MercurialHighestBuffTime >= Menu.Item("use" + Name + "Time").GetValue<Slider>().Value)
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
