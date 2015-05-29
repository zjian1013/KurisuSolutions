using System;
using LeagueSharp;
using LeagueSharp.Common;

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
            get { return "Muramana"; }
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

        public _3042()
        {
            Obj_AI_Base.OnProcessSpellCast += OnCast;
        }

        private bool muramana;

        public override void OnTick()
        {
            if (!muramana)
            {
                var muramoon = Activator.Player.GetSpellSlot("Muramana");
                if (muramoon != SpellSlot.Unknown && Activator.Player.HasBuff("Muramana"))
                {
                    if (Menu.Item("mode" + Name).GetValue<StringList>().SelectedIndex != 1 ||
                        Activator.Origin.Item("usecombo").GetValue<KeyBind>().Active)
                    {
                        Activator.Player.Spellbook.CastSpell(muramoon);
                    }
                }
            }

            if (muramana)
            {
                var muramoon = Activator.Player.GetSpellSlot("Muramana");
                if (muramoon != SpellSlot.Unknown && !Activator.Player.HasBuff("Muramana"))
                {
                    Activator.Player.Spellbook.CastSpell(muramoon);
                    Utility.DelayAction.Add(500, () => muramana = false);
                }
            }
        }

        private void OnCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe || !LeagueSharp.Common.Items.HasItem(Id))
            {
                return;
            }

            if (args.SData.HaveHitEffect)
                muramana = true;

            if (args.SData.IsAutoAttack())
            {
                if (Activator.Origin.Item("usecombo").GetValue<KeyBind>().Active ||
                    args.Target.Type == GameObjectType.obj_AI_Hero)
                {
                    muramana = true;
                }
                else
                {
                    Utility.DelayAction.Add(500, () => muramana = false);
                }
            }

            else
            {
                Utility.DelayAction.Add(500, () => muramana = false);
            }
        }
    }
}
