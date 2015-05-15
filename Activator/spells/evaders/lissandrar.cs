using System;

namespace Activator.Spells.Evaders
{
    class lissandrar : spell
    {
        internal override string Name
        {
            get { return "lissandrar"; }
        }

        internal override string DisplayName
        {
            get { return "Frozen Tomb | R"; }
        }

        internal override float Range
        {
            get { return float.MaxValue; }
        }

        internal override MenuType[] Category
        {
            get { return new[] { MenuType.Zhonyas }; }
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
                if (hero.Player.NetworkId == Player.NetworkId)
                {
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
}
