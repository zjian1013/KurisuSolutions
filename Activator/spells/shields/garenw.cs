using System;
using LeagueSharp.Common;

namespace Activator.Spells.Shields
{
    class garenw : spell
    {
        internal override string Name
        {
            get { return "garenw"; }
        }

        internal override string DisplayName
        {
            get { return "Courage | W"; }
        }

        internal override float Range
        {
            get { return float.MaxValue; }
        }

        internal override MenuType[] Category
        {
            get { return new[] { MenuType.SelfLowHP, MenuType.SelfMuchHP }; }
        }

        internal override int DefaultHP
        {
            get { return 95; }
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

                if (hero.IncomeDamage / hero.Player.MaxHealth * 100 >=
                    Menu.Item("SelfMuchHP" + Name + "Pct").GetValue<Slider>().Value)
                {
                    UseSpell();
                    RemoveSpell();
                }

                if (hero.Player.Health / hero.Player.MaxHealth * 100 <=
                    Menu.Item("SelfLowHP" + Name + "Pct").GetValue<Slider>().Value)
                {
                    if (hero.IncomeDamage > 0)
                    {
                        UseSpell();
                        RemoveSpell();
                    }
                }      
            }
        }
    }
}
