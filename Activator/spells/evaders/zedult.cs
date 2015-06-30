using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

namespace Activator.Spells.Evaders
{
    class zedult : spell
    {
        internal override string Name
        {
            get { return "zedult"; }
        }

        internal override string DisplayName
        {
            get { return "Death Mark | R"; }
        }

        internal override float Range
        {
            get { return 625f; }
        }

        internal override MenuType[] Category
        {
            get { return new[] { MenuType.Zhonyas }; }
        }

        public override void OnTick(EventArgs args)
        {
            if (!Menu.Item("use" + Name).GetValue<bool>())
                return;

            foreach (var hero in Activator.ChampionPriority())
            {
                if (hero.Player.NetworkId == Player.NetworkId)
                { 
                    if (!Parent.Item(Parent.Name + "useon" + hero.Player.NetworkId).GetValue<bool>())
                        continue;

                    if (hero.Attacker == null)
                        continue;

                    if (hero.Attacker.Distance(hero.Player.ServerPosition) > Range)
                        continue;

                    if (Menu.Item("use" + Name + "Norm").GetValue<bool>())
                        if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Danger))
                            CastOnBestTarget((Obj_AI_Hero)hero.Attacker);

                    if (Menu.Item("use" + Name + "Ulti").GetValue<bool>())
                        if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Ultimate))
                            CastOnBestTarget((Obj_AI_Hero)hero.Attacker);
                }
            }
        }
    }
}
