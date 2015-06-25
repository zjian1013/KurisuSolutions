using System;
using LeagueSharp;
using LeagueSharp.Common;

namespace Activator.Summoners
{
    internal class boost : summoner
    {
        internal override string Name
        {
            get { return "summonerboost"; }
        }

        internal override string DisplayName
        {
            get { return "Cleanse"; }
        }

        internal override string[] ExtraNames
        {
            get { return new[] { "" }; }
        }

        internal override float Range
        {
            get { return float.MaxValue; }
        }

        internal override int Duration
        {
            get { return 3000; }
        }

        public override void OnTick(EventArgs args)
        {
            foreach (var hero in champion.Heroes)
            {
                if (!Menu.Item("use" + Name).GetValue<bool>())
                    return;

                if (hero.Player.NetworkId != Player.NetworkId) 
                    return;

                if (!Parent.Item(Parent.Name + "allon" + hero.Player.ChampionName).GetValue<bool>())
                    return;

                if (hero.Player.Distance(Player.ServerPosition) > Range)
                    return;

                if (hero.CleanseBuffCount >= Menu.Item("use" + Name + "Number").GetValue<Slider>().Value &&
                    hero.CleanseHighestBuffTime >= Menu.Item("use" + Name + "Time").GetValue<Slider>().Value)
                {
                    if (!Menu.Item("use" + Name + "Od").GetValue<bool>())
                    {
                        Utility.DelayAction.Add(Game.Ping + 80, delegate
                        {
                            UseSpell(Menu.Item("mode" + Name).GetValue<StringList>().SelectedIndex == 1);                          
                        });
                    }
                }
            }
        }
    }
}
