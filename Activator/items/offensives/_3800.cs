using System;
using LeagueSharp;
using LeagueSharp.Common;

namespace Activator.Items.Offensives
{
    class _3800 : item
    {
        internal override int Id
        {
            get { return 3800; }
        }

        internal override string Name
        {
            get { return "Righteous"; }
        }

        internal override string DisplayName
        {
            get { return "Righteous Glory (Broken)"; }
        }

        internal override int Cooldown
        {
            get { return 60000; }
        }

        internal override float Range
        {
            get { return 600f; }
        }

        internal override MenuType[] Category
        {
            get { return new[] { MenuType.EnemyLowHP, MenuType.ActiveCheck }; }
        }

        internal override int DefaultHP
        {
            get { return 55; }
        }

        internal override int DefaultMP
        {
            get { return 0; }
        }

        public override void OnTick()
        {

        }
    }
}
