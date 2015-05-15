using System;

namespace Activator.Items.Offensives
{
    class _3042 : item
    {
        internal override int Id
        {
            get { return 3042; }
        }

        internal override string Name
        {
            get { return "Soon™"; } // Muramana
        }

        internal override float Range
        {
            get { return float.MaxValue; }
        }

        internal override MenuType[] Category
        {
            get { return new[] { MenuType.EnemyLowHP, MenuType.ActiveCheck }; }
        }

        internal override int DefaultHP
        {
            get { return 95; }
        }

        internal override int DefaultMP
        {
            get { return 35; }
        }

        public override void OnTick(EventArgs args)
        {

        }
    }
}
