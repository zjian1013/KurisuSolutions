using System;
using LeagueSharp;
using LeagueSharp.Common;

namespace Activator.Summoners
{
    internal class barrier : summoner
    {
        internal override string Name
        {
            get { return "summonerbarrier"; }
        }

        internal override string DisplayName
        {
            get { return "Barrier"; }
        }

        internal override float Range
        {
            get { return float.MaxValue; }
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

            foreach (var hero in champion.Heroes)
            {
                if (hero.Player.NetworkId != Player.NetworkId)
                    return;

                if (hero.Player.Health / hero.Player.MaxHealth * 100 <=
                    Menu.Item("SelfLowHP" + Name + "Pct").GetValue<Slider>().Value)
                {
                    if (hero.IncomeDamage > 0 && !hero.HitTypes.Contains(HitType.MinionAttack))
                    {
                        UseSpell();
                        hero.IncomeDamage = 0;
                    }
                }

                if (hero.IncomeDamage / hero.Player.MaxHealth * 100 >=
                    Menu.Item("SelfMuchHP" + Name + "Pct").GetValue<Slider>().Value)
                {
                    if (hero.IncomeDamage > 0 && !hero.HitTypes.Contains(HitType.MinionAttack))
                    {
                        UseSpell();
                        hero.IncomeDamage = 0;
                    }
                }

                if (Menu.Item("use" + Name + "Ulti").GetValue<bool>())
                {
                    if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Ultimate))
                    {
                        UseSpell();                    
                        hero.IncomeDamage = 0;
                    }
                }
            }
        }
    }
}
