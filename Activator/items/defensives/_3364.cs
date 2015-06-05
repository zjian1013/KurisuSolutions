using System;

namespace Activator.Items.Defensives
{
    class _3364 : item
    {
        internal override int Id
        {
            get { return 3364; }
        }

        internal override string Name
        {
            get { return "Oracles"; }
        }

        internal override string DisplayName
        {
            get { return "Oracle's Lens (Broken)"; }
        }

        internal override int Cooldown
        {
            get { return 75000; }
        }

        internal override float Range
        {
            get { return 600f; }
        }

        internal override int DefaultHP
        {
            get { return 99; }
        }

        internal override MenuType[] Category
        {
            get { return new[] { MenuType.Stealth, MenuType.ActiveCheck }; }
        }

        public override void OnTick(EventArgs args)
        {

        }
    }
}
