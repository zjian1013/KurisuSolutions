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
                var tar in
                    champion.Heroes
                        .Where(t =>  t.Player.IsValidTarget(600) && !t.Player.IsZombie)
                        .Where(t => !t.Player.HasBuff("summonerdot", true)))
            {
                if (!Parent.Item(Parent.Name + "allon" + tar.Player.ChampionName).GetValue<bool>())
                    continue;

                var ignotedmg = Player.GetSummonerSpellDamage(tar.Player, Damage.SummonerSpell.Ignite);

                // killsteal ignite
                if (Menu.Item("mode" + Name).GetValue<StringList>().SelectedIndex == 0)
                {
                    if (tar.Player.Health <= ignotedmg)
                    {
                        UseSpellOn(tar.Player);                        
                    }
                }

                // combo ignite
                if (Menu.Item("mode" + Name).GetValue<StringList>().SelectedIndex == 1)
                {
                    if (Player.ChampionName == "Cassiopeia" && 
                       !tar.Player.HasBuffOfType(BuffType.Poison))
                        return;

                    var totaldmg = 0d;
                    totaldmg += Player.GetAutoAttackDamage(tar.Player, true) * 3;

                    totaldmg += (from entry in spelldata.damagelib
                        let spellLevel = Player.GetSpell(entry.Value).Level
                        select
                            Player.GetSpell(entry.Value).State == SpellState.Ready
                                ? entry.Key(Player, tar.Player, spellLevel - 1)
                                : 0).Sum();

                    if ((float)(totaldmg + ignotedmg) >= tar.Player.Health)
                    {
                        if (tar.Player.Level <= 3)
                        {
                            if (tar.Player.InventoryItems.Any(
                                item => item.Id == (ItemId) 2003 || 
                                        item.Id == (ItemId) 2010))
                            {
                                return;
                            }
                        }

                        UseSpellOn(tar.Player, true);
                    }
                }
            }
        }
    }
}
