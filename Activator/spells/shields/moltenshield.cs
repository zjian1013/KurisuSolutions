using System;
using LeagueSharp;
using LeagueSharp.Common;

namespace Activator.Spells.Shields
{
    class moltenshield : spell
    {
        internal override string Name
        {
            get { return "moltenshield"; }
        }

        internal override string DisplayName
        {
            get { return "Molten Shield | E"; }
        }

        internal override float Range
        {
            get { return float.MaxValue; }
        }

        internal override MenuType[] Category
        {
            get { return new[] { MenuType.SelfLowHP, MenuType.SelfMuchHP, MenuType.SelfMinMP }; }
        }

        internal override int DefaultHP
        {
            get { return 95; }
        }

        internal override int DefaultMP
        {
            get { return 45; }
        }

        public override void OnTick(EventArgs args)
        {
            if (!Menu.Item("use" + Name).GetValue<bool>())
                return;

            if (Player.Mana/Player.MaxMana*100 <
                Menu.Item("SelfMinMP" + Name + "Pct").GetValue<Slider>().Value)
                return;

            foreach (var hero in champion.Heroes)
            {
                if (hero.Player.NetworkId == Player.NetworkId)
                {
                    if (!Parent.Item(Parent.Name + "useon" + hero.Player.ChampionName).GetValue<bool>())
                        continue;

                    if (hero.IncomeDamage / hero.Player.MaxHealth * 100 >=
                        Menu.Item("SelfMuchHP" + Name + "Pct").GetValue<Slider>().Value)
                        UseSpell();
   
                    if (hero.Player.Health / hero.Player.MaxHealth * 100 <=
                        Menu.Item("SelfLowHP" + Name + "Pct").GetValue<Slider>().Value)
                    {
                        if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.AutoAttack))
                        {
                            UseSpell();
                        }
                    }
                }
            }
        }
    }
}
