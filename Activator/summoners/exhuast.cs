using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

namespace Activator.Summoners
{
    internal class exhuast : summoner
    {
        internal override string Name
        {
            get { return "summonerexhaust"; }
        }

        internal override string DisplayName
        {
            get { return "Exhaust"; }
        }
        internal override string[] ExtraNames
        {
            get { return new[] { "" }; }
        }

        internal override float Range
        {
            get { return 650f; }
        }

        internal override int Duration
        {
            get { return 100; }
        }

        public override void OnTick(EventArgs args)
        {
            if (!Menu.Item("use" + Name).GetValue<bool>())
                return;

            if (Player.IsRecalling() || Player.InFountain())
                return;

            var highestadinrange =
                ObjectManager.Get<Obj_AI_Hero>()
                    .OrderByDescending(h => h.FlatPhysicalDamageMod)
                    .FirstOrDefault(x => x.IsEnemy && x.Distance(Player.ServerPosition) <= Range + 250);

            foreach (var hero in champion.Heroes)
            {
                var enemy = hero.Attacker as Obj_AI_Hero;
                if (enemy == null || highestadinrange == null) 
                    continue;

                if (enemy.Distance(hero.Player.ServerPosition) > Range) 
                    continue;

                if (Menu.Item("use" + Name + "Ulti").GetValue<bool>())
                {
                    if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Ultimate))
                    {
                        if (Parent.Item(Parent.Name + "allon" + enemy.ChampionName).GetValue<bool>())
                            UseSpellOn(enemy); 
                    }
                }            

                if (hero.Player.Health/hero.Player.MaxHealth*100 <=
                    Menu.Item("a" + Name + "Pct").GetValue<Slider>().Value)
                {
                    if (!hero.Player.IsFacing(enemy) && enemy.NetworkId == highestadinrange.NetworkId)
                    {
                        if (Parent.Item(Parent.Name + "allon" + enemy.ChampionName).GetValue<bool>())
                            UseSpellOn(enemy); 
                    }                     
                }

                if (enemy.Health / enemy.MaxHealth * 100 <=
                         Menu.Item("e" + Name + "Pct").GetValue<Slider>().Value)
                {
                    if (!enemy.IsFacing(hero.Player))
                    {
                        if (Parent.Item(Parent.Name + "allon" + enemy.ChampionName).GetValue<bool>())
                            UseSpellOn(enemy);  
                    }          
                }
            }
        }
    }
}
