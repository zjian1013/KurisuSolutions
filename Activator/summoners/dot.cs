using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

namespace Activator.Summoners
{
    internal class dot : summoner
    {
        internal override string Name
        {
            get { return "summonerdot"; }
        }

        internal override string DisplayName
        {
            get { return "Ignite"; }
        }

        internal override float Range
        {
            get { return 600f; }
        }

        internal override int Cooldown
        {
            get { return 210000; }
        }

        public override void OnTick(EventArgs args)
        {
            if (!Menu.Item("use" + Name).GetValue<bool>() || !Slot.IsReady())
                return;

            foreach (
                var target in
                    ObjectManager.Get<Obj_AI_Hero>()
                        .Where(target => target.IsValidTarget(600) && !target.IsZombie)
                        .Where(target => !target.HasBuff("summonerdot", true)))
            {

                if (target.Health <= Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite))
                {
                    UseSpellOn(target, Menu.Item("mode" + Name).GetValue<StringList>().SelectedIndex == 1);
                    RemoveSpell();
                }
            }
        }
    }
}
