using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

namespace Activator.Spells.Evaders
{
    class fioradance : spell
    {
        internal override string Name
        {
            get { return "fioradance"; }
        }

        internal override string DisplayName
        {
            get { return "Blade Waltz | R"; }
        }

        internal override float Range
        {
            get { return 400f; }
        }

        internal override MenuType[] Category
        {
            get { return new[] { MenuType.SelfLowHP, MenuType.Zhonyas }; }
        }

        internal override int DefaultHP
        {
            get { return 15; }
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
                if (hero.Player.NetworkId == Player.NetworkId)
                {
                    if (hero.Attacker == null)
                        continue;

                    if (!Parent.Item(Parent.Name + "useon" + hero.Player.NetworkId).GetValue<bool>() || 
                        hero.Attacker.Distance(hero.Player.ServerPosition) > Range)
                        continue;

                    if (Menu.Item("use" + Name + "Norm").GetValue<bool>())
                    {
                        if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Danger))
                            CastOnBestTarget((Obj_AI_Hero)hero.Attacker);
                    }

                    if (Menu.Item("use" + Name + "Ulti").GetValue<bool>())
                    {
                        if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Ultimate))
                            CastOnBestTarget((Obj_AI_Hero)hero.Attacker);
                    }

                    if (hero.Player.Health/hero.Player.MaxHealth*100 <=
                        Menu.Item("selflowhp" + Name + "pct").GetValue<Slider>().Value)
                    {
                        if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Ultimate))
                                CastOnBestTarget((Obj_AI_Hero)hero.Attacker);
                    }

                    if (hero.IncomeDamage > hero.Player.Health)
                    {
                        CastOnBestTarget((Obj_AI_Hero)hero.Attacker); 
                    }
                }
            }
        }
    }
}
