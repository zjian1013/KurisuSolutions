using System;
using LeagueSharp.Common;

namespace Activator.Items.Consumables
{
    class _2010 : item
    {
        internal override int Id
        {
            get { return 2010; }
        }

        internal override string Name
        {
            get { return "Total Biscuit"; }
        }

        internal override float Range
        {
            get { return float.MaxValue; }
        }
        internal override MenuType[] Category
        {
            get { return new[] { MenuType.SelfLowHP, MenuType.SelfLowMP, MenuType.SelfMuchHP }; }
        }

        internal override int DefaultHP
        {
            get { return 50; }
        }

        internal override int DefaultMP
        {
            get { return 40; }
        }

        public override void OnTick()
        {
            foreach (var hero in champion.Heroes)
            {
                if (hero.Player.NetworkId == Player.NetworkId)
                {
                    if (!Menu.Item("use" + Name).GetValue<bool>())
                        return;

                    if (hero.Player.Health/hero.Player.MaxHealth*100 <= 15 && 
                        hero.IncomeDamage > 0)
                        UseItem();

                    if (hero.Player.HasBuff("ItemMiniRegenPotion", true))
                        return;

                    if (hero.Player.IsRecalling() || hero.Player.InFountain())
                        return;

                    if (hero.Player.Health/hero.Player.MaxHealth*100 <=
                        Menu.Item("SelfLowHP" + Name + "Pct").GetValue<Slider>().Value && hero.IncomeDamage > 0)
                        UseItem();

                    if (hero.Player.MaxMana <= 200)
                        continue;

                    if (hero.Player.Mana / hero.Player.MaxMana * 100 <= 
                        Menu.Item("SelfLowMP" + Name + "Pct").GetValue<Slider>().Value)
                        UseItem();                       
                }
            }
        }
    }
}
