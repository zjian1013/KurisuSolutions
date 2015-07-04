﻿using System;
using System.Linq;
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
            get { return new[] { MenuType.SpellShield, MenuType.Zhonyas, MenuType.SelfMinMP }; }
        }

        internal override int DefaultHP
        {
            get { return 0; }
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
                Menu.Item("selfminmp" + Name + "pct").GetValue<Slider>().Value)
                return;

            foreach (var hero in Activator.ChampionPriority())
            {
                if (hero.Attacker == null || hero.Player.NetworkId != Player.NetworkId)
                    continue;

                if (!Parent.Item(Parent.Name + "useon" + hero.Player.NetworkId).GetValue<bool>() ||
                    hero.Attacker.Distance(hero.Player.ServerPosition) > Range)
                    continue;

                if (Menu.Item("ss" + Name + "All").GetValue<bool>())
                    if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Spell))
                        CastOnBestTarget((Obj_AI_Hero)hero.Attacker);

                if (Menu.Item("ss" + Name + "CC").GetValue<bool>())
                    if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.CrowdControl))
                        CastOnBestTarget((Obj_AI_Hero)hero.Attacker);

                if (Menu.Item("use" + Name + "Norm").GetValue<bool>())
                    if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Danger))
                        CastOnBestTarget((Obj_AI_Hero) hero.Attacker);

                if (Menu.Item("use" + Name + "Ulti").GetValue<bool>())
                    if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Ultimate))
                        CastOnBestTarget((Obj_AI_Hero)hero.Attacker);     
     
            }
        }
    }
}
