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
        internal override int Priority
        {
            get { return 7; }
        }

        internal override string Name
        {
            get { return "Muramana"; }
        }

        internal override float Range
        {
            get { return float.MaxValue; }
        }

        internal override int Duration
        {
            get { return 100; }
        }

        internal override MenuType[] Category
        {
            get { return new[] { MenuType.EnemyLowHP, MenuType.SelfLowMP,  MenuType.ActiveCheck }; }
        }

        internal override MapType[] Maps
        {
            get { return new[] { MapType.SummonersRift, MapType.HowlingAbyss }; }
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

        public override void OnTick(EventArgs args)
        {
            if (muramana)
            {
                if (Menu.Item("mode" + Name).GetValue<StringList>().SelectedIndex != 1 ||
                    Activator.Origin.Item("usecombo").GetValue<KeyBind>().Active)
                {
                    var manamune = Player.GetSpellSlot("Muramana");
                    if (manamune != SpellSlot.Unknown && !Player.HasBuff("Muramana"))
                    {
                        if (Player.Mana / Player.MaxMana * 100 > Menu.Item("SelfLowMP" + Name + "Pct").GetValue<Slider>().Value)
                            Player.Spellbook.CastSpell(manamune);

                        Utility.DelayAction.Add(400, () => muramana = false);
                    }
                }
            }

            if (!muramana && !Activator.Origin.Item("usecombo").GetValue<KeyBind>().Active)
            {
                var manamune = Player.GetSpellSlot("Muramana");
                if (manamune != SpellSlot.Unknown && Player.HasBuff("Muramana"))
                {
                    Player.Spellbook.CastSpell(manamune);
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
                    Utility.DelayAction.Add(500 + (int)(args.SData.CastFrame / 30), () => muramana = false);
                }
            }
        }
    }
}
