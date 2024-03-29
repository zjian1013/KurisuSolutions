﻿using System;
using LeagueSharp;
using LeagueSharp.Common;

namespace Activator.Spells.Slows
{
    class evelynnw : spell
    {
        internal override string Name
        {
            get { return "evelynnw"; }
        }

        internal override string DisplayName
        {
            get { return "Dark Frenzy | W"; }
        }

        internal override MenuType[] Category
        {
            get { return new[] { MenuType.SlowRemoval, MenuType.ActiveCheck }; }
        }

        public override void OnTick(EventArgs args)
        {
            if (!Menu.Item("use" + Name).GetValue<bool>())
                return;

            if (Player.HasBuffOfType(BuffType.Slow) && Menu.Item("use" + Name + "sr").GetValue<bool>())
            {
                if (!Parent.Item(Parent.Name + "useon" + Player.NetworkId).GetValue<bool>())
                    return;

                UseSpell(Menu.Item("mode" + Name).GetValue<StringList>().SelectedIndex == 1);
            }
        }
    }
}
