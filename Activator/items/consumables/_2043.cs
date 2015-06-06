using System;

namespace Activator.Items.Consumables
{
    class _2043 : item
    {
        internal override int Id
        {
            get { return 2043; }
        }

        internal override string Name
        {
            get { return "Soon™"; }
        }

        internal override float Range
        {
            get { return 600f; }
        }

        internal override MenuType[] Category
        {
            get { return new[] { MenuType.Stealth, MenuType.ActiveCheck }; }
        }

        internal override MapType[] Maps
        {
            get { return new[] { MapType.SummonersRift }; }
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

        }
    }
}
