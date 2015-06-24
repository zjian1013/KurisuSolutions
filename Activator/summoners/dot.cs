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
        internal override string[] ExtraNames
        {
            get { return new[] { "" }; }
        }

        internal override float Range
        {
            get { return 600f; }
        }

        internal override int Duration
        {
            get { return 100; }
        }

        private static Spell ignote;
        public dot()
        {
            ignote = new Spell(Player.GetSpellSlot(Name), Range);
        }

        public override void OnTick(EventArgs args)
        {
            if (!Menu.Item("use" + Name).GetValue<bool>())
                return;

            if (!ignote.IsReady())
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
                    }
                }

                // combo ignite
                if (Menu.Item("mode" + Name).GetValue<StringList>().SelectedIndex == 1)
                {
                    if (Player.ChampionName == "Cassiopeia" && 
                       !target.HasBuffOfType(BuffType.Poison))
                        return;

                    var totaldmg = 0d;
                    totaldmg += Player.GetAutoAttackDamage(target, true) * 3;

                    totaldmg += (from entry in spelldata.combod
                        let spellLevel = Player.GetSpell(entry.Value).Level
                        select
                            Player.GetSpell(entry.Value).State == SpellState.Ready
                                ? entry.Key(Player, target, spellLevel - 1)
                                : 0).Sum();

                    if ((float)(totaldmg + ignotedmg) >= target.Health)
                    {
                        if (target.Level <= 3)
                        {
                            if (target.InventoryItems.Any(
                                item => item.Id == (ItemId) 2003 || 
                                        item.Id == (ItemId) 2010))
                            {
                                return;
                            }
                        }

                        UseSpellOn(target, true);
                    }
                }
            }
        }
    }
}
