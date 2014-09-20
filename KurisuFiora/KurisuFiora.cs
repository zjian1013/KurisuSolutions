using LeagueSharp;
using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KurisuFiora
{
    internal class KurisuFiora
    {
        private static Orbwalking.Orbwalker _orbwalker;
        private static Menu _config;
        private static Obj_AI_Hero _player = ObjectManager.Player;
        private static Obj_AI_Hero _target;

        private static Spell q = new Spell(SpellSlot.Q, 600f);
        private static Spell w = new Spell(SpellSlot.W, float.MaxValue);
        private static Spell e = new Spell(SpellSlot.E, float.MaxValue);
        private static Spell r = new Spell(SpellSlot.R, 600f);

        private static bool usepackets = false;
        private static List<Spell> dangerousSpells = new List<Spell>();


        public KurisuFiora()
        {
            if (_player.BaseSkinName != "Fiora")
                return;
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        #region OnGameLoad
        void Game_OnGameLoad(EventArgs args)
        {


            #region L# Menu
            try
            {
                _config = new Menu("KurisuFiora", "fiora", true);
                _config.AddSubMenu(new Menu("Orbwalker", "orbwalker"));
                _orbwalker = new Orbwalking.Orbwalker(_config.SubMenu("orbwalker"));

                Menu fioraSelector = new Menu("Target Selector", "targetselector");
                SimpleTs.AddToMenu(fioraSelector);
                _config.AddSubMenu(fioraSelector);
                


                #region Tidy: Spells
                Menu fioraSpells = new Menu("Fiora: Spells", "combo");
                fioraSpells.AddItem(new MenuItem("useq", "Use Q")).SetValue(true);
                fioraSpells.AddItem(new MenuItem("qrange", "Minimum range to q")).SetValue(new Slider(250, 1, (int)q.Range));
                fioraSpells.AddItem(new MenuItem("usew", "Use W")).SetValue(true);
                fioraSpells.AddItem(new MenuItem("wsett", "Minimum damage to W")).SetValue(new Slider(50, 1, 200));
                fioraSpells.AddItem(new MenuItem("usee", "Use E")).SetValue(true);
                fioraSpells.AddItem(new MenuItem("smana", "Combo min mana % ")).SetValue(new Slider(0, 0, 99)); 
                _config.AddSubMenu(fioraSpells);
                #endregion

                #region Tidy: Balde Waltz
                Menu fioraWaltz = new Menu("Fiora: Blade Waltz", "fbw");
                fioraWaltz.AddItem(new MenuItem("user", "Use R")).SetValue(true);
                fioraWaltz.AddItem(new MenuItem("rdodge", "Dodge spells (soon)")).SetValue(false);
                _config.AddSubMenu(fioraWaltz);
                #endregion

                Menu fioraClear = new Menu("Fiora: Lane/Jungle Clear", "fclr");
                //fioraClear.AddItem(new MenuItem("clearq", "Use Q")).SetValue(true);
                //fioraClear.AddItem(new MenuItem("cleare", "Use E")).SetValue(true);
                //fioraClear.AddItem(new MenuItem("cmana", "Clear min mana % ")).SetValue(new Slider(55, 0, 99)); 
                _config.AddSubMenu(fioraClear);


                _config.AddItem(new MenuItem("usepackets", "Use Packets")).SetValue(usepackets);
                _config.AddItem(new MenuItem("ksteal", "Kill Steal")).SetValue(true);

                _config.AddToMainMenu();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Game.PrintChat("Error something went wrong with fiora assembly(Menu)");
            }
            #endregion

            #region L# Reqs
            Game.OnGameUpdate += Game_OnGameUpdate;
            //Game.OnGameProcessPacket += Game_OnGameProcessPacket;
            Obj_AI_Base.OnProcessSpellCast += Game_OnGameProcessSpellCast;
            #endregion
        }
        #endregion

        #region OnGameUpdate
        private void Game_OnGameUpdate(EventArgs args)
        {
            try
            {
                Console.WriteLine(usepackets);
                if (_orbwalker.ActiveMode.ToString() == "Combo")
                {
                    _target = SimpleTs.GetTarget(750, SimpleTs.DamageType.Physical);
                    useCombo(_target);
                }

                if (_orbwalker.ActiveMode.ToString() == "LaneClear")
                {

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Game.PrintChat("KurisuFiora: Unexpected Error Occured(OnGameUpdate)");
            }
        }
        #endregion

        #region OnGameProcessSpellCast
        private void Game_OnGameProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            double incDmg = 0;
            if (sender.Type == GameObjectType.obj_AI_Hero && sender.IsEnemy && args.Target.IsMe)
            {
                Obj_AI_Hero attacker = ObjectManager.Get<Obj_AI_Hero>().First(n => n.NetworkId == sender.NetworkId);
                if (attacker != null)
                {
                    SpellSlot spellSlot = Utility.GetSpellSlot(attacker, args.SData.Name);
                    if (spellSlot == SpellSlot.Unknown)
                        incDmg = DamageLib.getDmg(attacker, DamageLib.SpellType.AD);
                    if (spellSlot == SpellSlot.R && sender.BaseSkinName == "Malphite")
                        useR(sender);
                }

            }
            useW(incDmg);
        }
        #endregion
        
        private void useCombo(Obj_AI_Hero target)
        {
                useE(target);
                useQ(target);
                useR(target);
        }

        private static float comboDamage(Obj_AI_Base enemy)
        {
            var dmg = 0d;
            var ignote = Utility.GetSpellSlot(_player, "summonerdot");

            if (q.IsReady())
                dmg += DamageLib.getDmg(enemy, DamageLib.SpellType.Q);
            //if (e.IsReady())
            //    dmg += DamageLib.getDmg(enemy, DamageLib.SpellType.E);
            if (r.IsReady())
                dmg += DamageLib.getDmg(enemy, DamageLib.SpellType.R);
            if (Items.HasItem(3077))
                dmg += DamageLib.getDmg(enemy, DamageLib.SpellType.TIAMAT);
            if (Items.HasItem(3074))
                dmg += DamageLib.getDmg(enemy, DamageLib.SpellType.HYDRA);
            if (ignote != SpellSlot.Unknown)
                dmg += DamageLib.getDmg(enemy, DamageLib.SpellType.IGNITE);

            return (float)dmg;
        }


        private void useE(Obj_AI_Hero target)
        {
            if (target != null)
            {
                if (_config.Item("usee").GetValue<bool>())
                {
                    int minMana = _config.Item("smana").GetValue<Slider>().Value;
                    int actualHeroManaPercent = (int)((_player.Mana / _player.MaxMana) * 100);
                    if (e.IsReady() && _player.Distance(target.Position) < _player.AttackRange)
                    {
                        if (actualHeroManaPercent > minMana)
                            _player.Spellbook.CastSpell(SpellSlot.E);
                    }


                }
            }
        }


        private void useW(double damage)
        {
            int incDamageSlider = _config.Item("wsett").GetValue<Slider>().Value;
            int incDamagePercent = (int)(_player.Health / damage * 100);
            if (_config.Item("usew").GetValue<bool>())
            {
                if (_player.Spellbook.CanUseSpell(SpellSlot.W) != SpellState.Unknown)
                {
                    if (w.IsReady() && damage > incDamageSlider)
                    _player.Spellbook.CastSpell(SpellSlot.W);
                }
                    
            }
        }

        private void useQ(Obj_AI_Hero target)
        {
            if (target != null)
            {
                float minRange = _config.Item("qrange").GetValue<Slider>().Value;
                if (q.IsReady() && _player.Distance(target.ServerPosition) < 600f)
                {
                    if (_player.Distance(target.ServerPosition) >= minRange)
                        q.Cast(target, usepackets);
                }
            }
        }

        private void useR(Obj_AI_Base target)
        {
            if (target != null)
            {
                if (_config.Item("user").GetValue<bool>())
                {
                    if (r.IsReady() && _player.Distance(target.Position) < r.Range)
                    {
                        if (target.Health <= comboDamage(target))
                            r.CastOnUnit(target);
                    }
                }
            }
        }



        private void kSteal()
        {
            if (_config.Item("ksteal").GetValue<bool>())
            {
                List<Obj_AI_Hero> enemy = ObjectManager.Get<Obj_AI_Hero>().Where(h => h.Team != _player.Team).ToList();

                foreach (Obj_AI_Hero e in enemy)
                {
                    var qdmg = DamageLib.getDmg(e, DamageLib.SpellType.Q);
                    var rdmg = DamageLib.getDmg(e, DamageLib.SpellType.R);
                    if (q.IsReady() && e != null && e.IsValid && e.Health < qdmg)
                        q.CastOnUnit(e);
                    if (r.IsReady() && e != null && e.IsValid && e.Health < rdmg)
                        r.CastOnUnit(e);
                }
            }
        }

    }
}
