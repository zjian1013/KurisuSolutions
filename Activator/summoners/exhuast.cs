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

        internal override float Range
        {
            get { return 650f; }
        }

        internal override int Cooldown
        {
            get { return 210000; }
        }

        public override void OnTick()
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
                if (hero.Attacker == null) 
                    continue;

                if (hero.Attacker.Distance(hero.Player.ServerPosition) > Range) 
                    continue;

                if (Menu.Item("use" + Name + "Ulti").GetValue<bool>())
                    if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Ultimate))
                        UseSpellOn(hero.Attacker);                

                if (hero.Player.Health/hero.Player.MaxHealth*100 <=
                    Menu.Item("a" + Name + "Pct").GetValue<Slider>().Value)
                {
                    if (!hero.Player.IsFacing(hero.Attacker) &&
                        hero.Attacker.NetworkId == highestadinrange.NetworkId)
                        UseSpellOn(hero.Attacker);                      
                }

                if (hero.Attacker.Health / hero.Attacker.MaxHealth * 100 <=
                         Menu.Item("e" + Name + "Pct").GetValue<Slider>().Value)
                {
                    if (!hero.Attacker.IsFacing(hero.Player))
                        UseSpellOn(hero.Attacker);                    
                }
            }
        }
    }
}
