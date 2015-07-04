using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

namespace Activator.Items.Cleansers
{
    class _3222 : item
    {
        internal override int Id
        {
            get { return 3222; }
        }

        internal override string Name
        {
            get { return "Mikaels"; }
        }

        internal override string DisplayName
        {
            get { return "Mikael's Crucible"; }
        }

        internal override int Priority
        {
            get { return 7; }
        }

        internal override int Duration
        {
            get { return 1000; }
        }

        internal override float Range
        {
            get { return 750f; }
        }

        internal override MenuType[] Category
        {
            get { return new[] { MenuType.SelfLowHP, MenuType.Cleanse, MenuType.ActiveCheck  }; }
        }

        internal override MapType[] Maps
        {
            get { return new[] { MapType.Common }; }
        }

        internal override int DefaultHP
        {
            get { return 10; }
        }

        internal override int DefaultMP
        {
            get { return 0; }
        }

        public override void OnTick(EventArgs args)
        {
            if (!Menu.Item("use" + Name).GetValue<bool>())
                return;

            foreach (var hero in Activator.ChampionPriority())
            {
                if (!Parent.Item(Parent.Name + "useon" + hero.Player.NetworkId).GetValue<bool>())
                    continue;

                if (hero.Player.Distance(Player.ServerPosition) > Range)
                    continue;

                if (hero.ForceQSS)
                {
                    UseItem();
                    hero.IncomeDamage = 0;
                    hero.ForceQSS = false;
                }

                if (hero.MikaelsBuffCount >= Menu.Item("use" + Name + "Number").GetValue<Slider>().Value &&
                    hero.MikaelsHighestBuffTime >= Menu.Item("use" + Name + "Time").GetValue<Slider>().Value)
                {
                    if (!Menu.Item("use" + Name + "Od").GetValue<bool>())
                    {
                        Utility.DelayAction.Add(Game.Ping + 80, delegate
                        {
                            UseItem(hero.Player, Menu.Item("mode" + Name).GetValue<StringList>().SelectedIndex == 1);
                        });
                    }
                }

                if (hero.Player.Health / hero.Player.MaxHealth * 100 <=
                    Menu.Item("selflowhp" + Name + "pct").GetValue<Slider>().Value)
                {
                    if (hero.IncomeDamage > 0)
                    {
                        UseItem(hero.Player, Menu.Item("mode" + Name).GetValue<StringList>().SelectedIndex == 1);
                    }
                }
            }        
        }
    }
}
