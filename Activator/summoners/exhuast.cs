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

        public override void OnTick(EventArgs args)
        {
            if (!Menu.Item("use" + Name).GetValue<bool>())
                return;

            if (Player.IsRecalling() || Player.InFountain())
                return;

            var highestadinrange =
                ObjectManager.Get<Obj_AI_Hero>()
                    .OrderByDescending(h => h.FlatPhysicalDamageMod)
                    .FirstOrDefault(x => x.Distance(Player.ServerPosition) <= Range + 250);

            foreach (var hero in champion.Heroes)
            {
                if (hero.Attacker != null)
                {
                    if (hero.Attacker.Distance(hero.Player.ServerPosition) <= Range)
                    {
                        if (Menu.Item("use" + Name + "Ulti").GetValue<bool>())
                        {
                            if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Ultimate))
                            {
                                UseSpellOn(hero.Attacker);
                                RemoveSpell();
                            }
                        }
                    }

                    if (hero.Attacker.Distance(Player.ServerPosition) <= Range)
                    {
                        if (hero.Player.Health/hero.Player.MaxHealth*100 <=
                            Menu.Item("a" + Name + "Pct").GetValue<Slider>().Value)
                        {
                            if (hero.Attacker.IsFacing(hero.Player) && hero.Attacker.NetworkId == highestadinrange.NetworkId)
                            {
                                UseSpellOn(hero.Attacker);
                                RemoveSpell();
                            }
                        }
                    }

                    else if (hero.Attacker.Health/hero.Attacker.MaxHealth*100 <=
                             Menu.Item("e" + Name + "Pct").GetValue<Slider>().Value)
                    {
                        if (!hero.Attacker.IsFacing(hero.Player))
                        {
                            UseSpellOn(hero.Attacker);
                            RemoveSpell();
                        }
                    }
                }
            }
        }
    }
}
