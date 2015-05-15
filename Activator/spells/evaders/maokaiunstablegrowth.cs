using System;
using LeagueSharp;
using LeagueSharp.Common;

namespace Activator.Spells.Evaders
{
    class maokaiunstablegrowth : spell
    {
        internal override string Name
        {
            get { return "maokaiunstablegrowth"; }
        }

        internal override string DisplayName
        {
            get { return "Twisted Advance | W"; }
        }

        internal override float Range
        {
            get { return 525f; }
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
            if (!Menu.Item("use" + Name).GetValue<bool>())
                return;

            foreach (var hero in champion.Heroes)
            {
                if (hero.Player.NetworkId == Player.NetworkId)
                {
                    if (hero.Attacker == null)
                        return;

                    if (hero.Attacker.Distance(hero.Player.ServerPosition) > Range)
                        return;

                    if (Menu.Item("ss" + Name + "All").GetValue<bool>())
                    {
                        if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Spell))
                        {
                            CastOnBestTarget((Obj_AI_Hero)hero.Attacker);
                            RemoveSpell();
                        }
                    }

                    if (Menu.Item("ss" + Name + "CC").GetValue<bool>())
                    {
                        if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.CrowdControl))
                        {
                            CastOnBestTarget((Obj_AI_Hero)hero.Attacker);
                            RemoveSpell();
                        }
                    }
                    if (Menu.Item("use" + Name + "Norm").GetValue<bool>())
                    {
                        if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Danger))
                        {
                            CastOnBestTarget((Obj_AI_Hero) hero.Attacker);
                            RemoveSpell();
                        }
                    }

                    if (Menu.Item("use" + Name + "Ulti").GetValue<bool>())
                    {
                        if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Ultimate))
                        {
                            CastOnBestTarget((Obj_AI_Hero)hero.Attacker);
                            RemoveSpell();
                        }
                    }
                }
            }
        }
    }
}
