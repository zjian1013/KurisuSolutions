using System;
using LeagueSharp;
using LeagueSharp.Common;

namespace Activator.Spells.Evaders
{
    class sivire : spell
    {
        internal override string Name
        {
            get { return "sivire"; }
        }

        internal override string DisplayName
        {
            get { return "Spell Shield | E"; }
        }

        internal override float Range
        {
            get { return float.MaxValue; }
        }

        internal override MenuType[] Category
        {
            get { return new[] { MenuType.SpellShield, MenuType.Zhonyas }; }
        }

        internal override int DefaultHP
        {
            get { return 30; }
        }

        internal override int DefaultMP
        {
            get { return 40; }
        }

        public override void OnTick()
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
                        
                    }
                }

                if (Menu.Item("ss" + Name + "CC").GetValue<bool>())
                {
                    if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.CrowdControl))
                    {
                        UseSpell();
                        
                    }
                }

                if (Menu.Item("use" + Name + "Norm").GetValue<bool>())
                {
                    if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Danger))
                    {
                        UseSpell();
                        
                    }
                }

                if (Menu.Item("use" + Name + "Ulti").GetValue<bool>())
                {
                    if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Ultimate))
                    {
                        UseSpell();
                        
                    }
                }
            }
        }
    }
}
