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
            if (!Menu.Item("use" + Name).GetValue<bool>())
                return;

            foreach (
                var target in
                    ObjectManager.Get<Obj_AI_Hero>()
                        .Where(target => target.IsValidTarget(600) && !target.IsZombie)
                        .Where(target => !target.HasBuff("summonerdot", true)))
            {

                var ignotedmg = Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);

                // killsteal ignite
                if (Menu.Item("mode" + Name).GetValue<StringList>().SelectedIndex == 0)
                {
                    if (target.Health <= ignotedmg)
                    {
                        UseSpellOn(target);
                        RemoveSpell();
                    }
                }

                // combo ignite
                if (Menu.Item("mode" + Name).GetValue<StringList>().SelectedIndex == 1)
                {
                    var fulldmg = 0d;

                    foreach (var entry in spelldata.combodelagate)
                    {
                        var spellLevel = Player.GetSpell(entry.Value).Level;
                        fulldmg += Player.GetSpell(entry.Value).State == SpellState.Ready
                            ? entry.Key(Player, target, spellLevel - 1)
                            : 0;
                    }

                    if ((float)(fulldmg + ignotedmg) >= target.Health)
                    {
                        UseSpellOn(target, true);
                        RemoveSpell();
                    }
                }
            }
        }
    }
}
