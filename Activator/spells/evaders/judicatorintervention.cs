using System;
using LeagueSharp.Common;

namespace Activator.Spells.Evaders
{
    class judicatorintervention : spell
    {
        internal override string Name
        {
            get { return "judicatorintervention"; }
        }

        internal override string DisplayName
        {
            get { return "Intervention | R"; }
        }

        internal override float Range
        {
            get { return 900f; }
        }

        internal override MenuType[] Category
        {
            get { return new[] { MenuType.SelfLowHP,  MenuType.Zhonyas }; }
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

            foreach (var hero in champion.Heroes)
            {
                if (hero.Player.Distance(Player.ServerPosition) > Range)
                    return;

                if (hero.Player.Health/hero.Player.MaxHealth <=
                    Menu.Item("SelfLowHP" + Name + "Pct").GetValue<Slider>().Value)
                {
                    if(hero.IncomeDamage > 0)
                    {
                        UseSpellOn(hero.Player);
                        RemoveSpell();
                    }
                }

                if (Menu.Item("use" + Name + "Norm").GetValue<bool>())
                {
                    if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Danger))
                    {
                        UseSpellOn(hero.Player);
                        RemoveSpell();
                    }
                }

                if (Menu.Item("use" + Name + "Ulti").GetValue<bool>())
                {
                    if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Ultimate))
                    {
                        UseSpellOn(hero.Player);
                        RemoveSpell();
                    }
                }
            }
        }
    }
}
