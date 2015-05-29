using System;
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

        internal override int Cooldown
        {
            get { return 180000; }
        }

        internal override float Range
        {
            get { return 750f; }
        }

        internal override MenuType[] Category
        {
            get { return new[] { MenuType.SelfLowHP, MenuType.Cleanse, MenuType.ActiveCheck  }; }
        }

        internal override int DefaultHP
        {
            get { return 15; }
        }

        internal override int DefaultMP
        {
            get { return 0; }
        }

        public override void OnTick()
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
                    }

                    if (hero.QSSBuffCount >= Menu.Item("use" + Name + "Number").GetValue<Slider>().Value)
                    {
                        if (!Menu.Item("use" + Name + "Od").GetValue<bool>())
                        {
                            Utility.DelayAction.Add(Game.Ping + 80, delegate
                            {
                                UseItem(Menu.Item("mode" + Name).GetValue<StringList>().SelectedIndex == 1);
                                if (!LeagueSharp.Common.Items.CanUseItem(Id))
                                {
                                    hero.QSSBuffCount = 0;
                                    hero.QSSHighestBuffTime = 0;
                                    hero.IncomeDamage = 0;
                                }
                            });
                        }
                    }
                }
            }
        }
    }
}
