using System;
using LeagueSharp.Common;

namespace Activator.Spells.Evaders
{
    class nocturneshroudofdarkness : spell
    {
        internal override string Name
        {
            get { return "nocturneshroudofdarkness"; }
        }

        internal override string DisplayName
        {
            get { return "Shrowd of Darkness | W"; }
        }

        internal override float Range
        {
            get { return float.MaxValue; }
        }

        internal override MenuType[] Category
        {
            get { return new[] { MenuType.SelfMuchHP, MenuType.SpellShield,  MenuType.Zhonyas }; }
        }

        internal override int DefaultHP
        {
            get { return 30; }
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
                if (hero.Player.NetworkId != Player.NetworkId)
                    return;

                if (Menu.Item("ss" + Name + "All").GetValue<bool>())
                {
                    if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Spell))
                    {
                        UseSpell();
                        RemoveSpell();
                    }
                }

                if (Menu.Item("ss" + Name + "CC").GetValue<bool>())
                {
                    if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.CrowdControl))
                    {
                        UseSpell();
                        RemoveSpell();
                    }
                }

                if (Menu.Item("use" + Name + "Norm").GetValue<bool>())
                {
                    if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Danger))
                    {
                        UseSpell();
                        RemoveSpell();
                    }
                }

                if (Menu.Item("use" + Name + "Ulti").GetValue<bool>())
                {
                    if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Ultimate))
                    {
                        UseSpell();
                        RemoveSpell();
                    }
                }

                if (hero.IncomeDamage/hero.Player.MaxHealth*100 >=
                    Menu.Item("SelfMuchHP" + Name + "Pct").GetValue<Slider>().Value)
                {
                    UseSpell();
                    RemoveSpell();         
                }
            }
        }
    }
}
