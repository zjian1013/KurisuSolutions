using System;
using LeagueSharp;
using LeagueSharp.Common;

namespace Activator.Spells.Evaders
{
    class vladimirsanguinepool : spell
    {
        internal override string Name
        {
            get { return "vladimirsanguinepool"; }
        }

        internal override string DisplayName
        {
            get { return "Sanguine Pool | W"; }
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
            get { return 0; }
        }

        internal override int DefaultMP
        {
            get { return 0; }
        }

        public override void OnTick(EventArgs args)
        {
            if (!Menu.Item("use" + Name).GetValue<bool>() ||
                Player.GetSpell(Slot).State != SpellState.Ready)
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
            }
        }
    }
}
