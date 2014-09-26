using LeagueSharp;
using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = System.Drawing.Color;

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
        private static Spell r = new Spell(SpellSlot.R, 400f);
        private static readonly List<Spell> spellList = new List<Spell>();

        private int pTarget;
        private bool pHit = false;

        public KurisuFiora()
        {
            Console.WriteLine("Kurisu assembly is starting...");
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        #region Fiora: OnGameLoad
        private void Game_OnGameLoad(EventArgs args)
        {
            try
            {

                Game.PrintChat("<font color=\"#CCFFF2\">[KurisuFiora]</font><font color=\"#99FFE6\"> - <u>the Grand Duelist v1.3.1</u></font> - Kurisu ©");
                spellList.Add(q);
                spellList.Add(r);
                var enemy = from hero in ObjectManager.Get<Obj_AI_Hero>()
                            where hero.IsEnemy == true
                            select hero;               
                _config = new Menu("KurisuFiora", "fiora", true);
                _config.AddSubMenu(new Menu("Orbwalker", "orbwalker"));
                _orbwalker = new Orbwalking.Orbwalker(_config.SubMenu("orbwalker"));

                Menu fioraSelector = new Menu("Target Selector", "targetselector");
                SimpleTs.AddToMenu(fioraSelector);
                _config.AddSubMenu(fioraSelector);

                Menu fioraDraws = new Menu("Drawings", "drawings");
                fioraDraws.AddItem(new MenuItem("drawQ", "Draw Q")).SetValue(new Circle(true, Color.FromArgb(150, Color.SpringGreen)));
                fioraDraws.AddItem(new MenuItem("drawR", "Draw R")).SetValue(new Circle(true, Color.FromArgb(150, Color.MediumTurquoise)));
                _config.AddSubMenu(fioraDraws);


                Menu fioraSpells = new Menu("Fiora: General", "combo");
                fioraSpells.AddItem(new MenuItem("useq", "Use Q")).SetValue(true);
                //fioraSpells.AddItem(new MenuItem("useqminion", "Use minion to gapclose")).SetValue(true);
                fioraSpells.AddItem(new MenuItem("qrange", "Minimum range to q")).SetValue(new Slider(250, 1, (int)q.Range));
                fioraSpells.AddItem(new MenuItem("usee", "Use E")).SetValue(true);
                fioraSpells.AddItem(new MenuItem("smana", "Combo min mana % ")).SetValue(new Slider(0, 0, 99));
                _config.AddSubMenu(fioraSpells);

                Menu fioraParry = new Menu("Fiora: Riposte", "fparry");
                fioraParry.AddItem(new MenuItem("usew", "Use W")).SetValue(true);
                fioraParry.AddItem(new MenuItem("wsett", "Minimum damage to W")).SetValue(new Slider(50, 1, 200));
                fioraParry.AddItem(new MenuItem("wdodge", "Use W on Hiteffect Spells")).SetValue(true);               
                fioraParry.AddItem(new MenuItem("", ""));
                foreach (var e in enemy)
                {
                    var qdata = e.Spellbook.GetSpell(SpellSlot.Q);
                    var wdata = e.Spellbook.GetSpell(SpellSlot.W);
                    var edata = e.Spellbook.GetSpell(SpellSlot.E);
                    if (KurisuLib.DangerousList.Any(spell => spell.Contains(qdata.SData.Name)))
                        fioraParry.AddItem(new MenuItem("ws" + e.SkinName, qdata.SData.Name)).SetValue(true);
                    if (KurisuLib.DangerousList.Any(spell => spell.Contains(wdata.SData.Name)))
                        fioraParry.AddItem(new MenuItem("ws" + e.SkinName, wdata.SData.Name)).SetValue(true);
                    if (KurisuLib.DangerousList.Any(spell => spell.Contains(edata.SData.Name)))
                        fioraParry.AddItem(new MenuItem("ws" + e.SkinName, edata.SData.Name)).SetValue(true);
                }
                _config.AddSubMenu(fioraParry);

                Menu fioraWaltz = new Menu("Fiora: Blade Waltz", "fbw");
                fioraWaltz.AddItem(new MenuItem("user", "Use R")).SetValue(true);
                fioraWaltz.AddItem(new MenuItem("rdodge", "Dodge dangerous spells")).SetValue(true);
                fioraWaltz.AddItem(new MenuItem("", ""));                                         
                foreach (var e in enemy)
                {
                    SpellDataInst rdata = e.Spellbook.GetSpell(SpellSlot.R);
                    Console.WriteLine(rdata.SData.Name);
                    if (KurisuLib.DangerousList.Any(spell => spell.Contains(rdata.SData.Name)))
                        fioraWaltz.AddItem(new MenuItem("ds" + e.SkinName, rdata.SData.Name)).SetValue(true);
                }
 
                _config.AddSubMenu(fioraWaltz);

                Menu fioraMisc = new Menu("Fiora: Misc", "fmisc");
                //fioraMisc.AddItem(new MenuItem("ksteal", "Killsteal")).SetValue(true);
                fioraMisc.AddItem(new MenuItem("usepackets", "Use Packets")).SetValue(true);
                fioraMisc.AddItem(new MenuItem("isteal", "Use Ignite")).SetValue(true);
                fioraMisc.AddItem(new MenuItem("usetiamat", "Use Tiamat/Hydra")).SetValue(true);
                _config.AddSubMenu(fioraMisc);

                Menu fioraDebug = new Menu("Fiora: Debug", "fdbg");
                fioraDebug.AddItem(new MenuItem("aadebug", "ConsoleWrite aa tick")).SetValue(false);
                _config.AddSubMenu(fioraDebug);

                //_config.AddItem(new MenuItem("stickytarg", "Stick to target")).SetValue(true);

                _config.AddToMainMenu();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Game.PrintChat("Error something went wrong with fiora assembly(Menu)");
            }

            #region L# Reqs
            Game.OnGameUpdate += Game_OnGameUpdate;
            Game.OnGameProcessPacket += Game_OnGameProcessPacket;
            Game.OnGameSendPacket += Game_OnGameSendPacket;
            Obj_AI_Base.OnProcessSpellCast += Game_OnGameProcessSpellCast;
            Drawing.OnDraw += Drawing_OnDraw;
            #endregion
        }

        #endregion

        #region Fiora: DrawingOnDraw
        private void Drawing_OnDraw(EventArgs args)
        {
            foreach (var spell in spellList)
            {
                var circle = _config.Item("draw" + spell.Slot.ToString()).GetValue<Circle>();
                if (circle.Active)
                    Utility.DrawCircle(_player.Position, spell.Range, circle.Color, 5, 55);
            }

            Utility.DrawCircle(_target.Position, _target.BoundingRadius, Color.MediumTurquoise, 5, 55);
        }
        #endregion

        #region Fiora: OnGameSendPacket
        private void Game_OnGameSendPacket(GamePacketEventArgs args)
        {
            try
            {
                if (args.PacketData[0] == 119)
                    args.Process = false;

                if (args.PacketData[0] == 154 && _orbwalker.ActiveMode.ToString() == "Combo")
                {
                    Packet.C2S.Cast.Struct PCast = Packet.C2S.Cast.Decoded(args.PacketData);
                    if (PCast.Slot == SpellSlot.E)
                    {
                        //Console.WriteLine("aacancel");
                        Packet.C2S.Move.Encoded(new Packet.C2S.Move.Struct(0, 0, 3, _orbwalker.GetTarget().NetworkId)).Send();
                        Orbwalking.ResetAutoAttackTimer();
                        if ((Items.HasItem(3077) && Items.CanUseItem(3077) || (Items.HasItem(3074) && Items.CanUseItem(3074)) && _config.Item("usetiamat").GetValue<bool>()))
                        {
                            Utility.DelayAction.Add(Game.Ping + 125, delegate
                            {
                                Items.UseItem(3077);
                                Items.UseItem(3074);
                                Packet.C2S.Move.Encoded(new Packet.C2S.Move.Struct(0, 0, 3, _orbwalker.GetTarget().NetworkId)).Send();
                                Orbwalking.ResetAutoAttackTimer();
                                //Console.WriteLine("aacancel: 2");
                            });
                        }

                    }
                }
            }
            catch
            {
                Game.PrintChat("Error something went wrong with fiora assembly(OnGameSendPacket)");
            }

        }
        #endregion

        #region Fiora: OnGameProccessPacket
        private void Game_OnGameProcessPacket(GamePacketEventArgs args)
        {
            GamePacket packet = new GamePacket(args.PacketData);

            if (args.PacketData[0] == Packet.S2C.Damage.Header)
            {
                Packet.S2C.Damage.Struct PDamage = Packet.S2C.Damage.Decoded(args.PacketData);
                var source = PDamage.SourceNetworkId;
                var target = PDamage.TargetNetworkId;

                pTarget = target;
                pHit = true;
            }
        }
        #endregion

        #region Fiora: OnGameUpdate
        private void Game_OnGameUpdate(EventArgs args)
        {        
            try
            {
                //if (_config.Item("ksteal").GetValue<bool>())
                //    kSteal();
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

        #region Fiora: OnGameProcessSpellCast
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
                        incDmg = _player.GetAutoAttackDamage(attacker);
                    if (_config.Item("wdodge").GetValue<bool>())
                        if (KurisuLib.OnHitEffectList.Any(spell => spell.Contains(args.SData.Name)) && w.IsReady())
                            w.Cast();
                    if (_config.Item("rdodge").GetValue<bool>() && _config.Item("ds" + sender.SkinName).GetValue<bool>())
                        if (KurisuLib.DangerousList.Any(spell => spell.Contains(args.SData.Name)) && sender.Distance(_player) < 400f && r.IsReady())
                            r.Cast(sender);
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
                dmg += _player.GetSpellDamage(enemy, SpellSlot.Q);
            if (r.IsReady())
                dmg += _player.GetSpellDamage(enemy, SpellSlot.R);
            //if (Items.HasItem(3077))
            //    dmg += _player.GetSpellDamage(enemy, "ItemTiamatCleave");
           // if (Items.HasItem(3074))
            //    dmg += _player.GetSpellDamage(enemy, "ItemTiamatCleave");
            if (ignote != SpellSlot.Unknown && _config.Item("isteal").GetValue<bool>())
                dmg += _player.GetSpellDamage(enemy, ignote);

            return (float)dmg;
        }

        private bool usePackets()
        {
            return _config.Item("usepackets").GetValue<bool>();
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
                        if (actualHeroManaPercent > minMana && pTarget == target.NetworkId && pHit)
                        {
                            _player.Spellbook.CastSpell(SpellSlot.E);
                            pHit = false;
                        }
                    }


                }
            }
        }

        private void useW(double damage = 0)
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
            if (target != null && _config.Item("useq").GetValue<bool>())
            {
                float minRange = _config.Item("qrange").GetValue<Slider>().Value;
                if (q.IsReady() && _player.Distance(target.ServerPosition) < 600f)
                {
                    if (_player.Distance(target.ServerPosition) >= minRange)
                        q.Cast(target, usePackets());
                }

                /*if (_config.Item("useq").GetValue<bool>() && _config.Item("useqminion").GetValue<bool>())
                {
                    var pos = from minion in ObjectManager.Get<Obj_AI_Minion>()
                             where minion.Distance(target.Position) < q.Range &&
                                   minion.Distance(_player.Position) < q.Range
                            select minion;

                    if (target.Distance(_player.Position) > q.Range)
                    {
                        foreach (var m in pos)
                        {
                            q.Cast(m, usePackets());
                            break;
                        }
                    }
                }*/


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
                        {
                            if (_config.Item("isteal").GetValue<bool>())
                            {
                                SpellSlot ignote = Utility.GetSpellSlot(_player, "summonerdot");
                                _player.SummonerSpellbook.CastSpell(ignote, target);
                            }
                            r.CastOnUnit(target, usePackets());
                        }

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
                    var qdmg = _player.GetSpellDamage(e, SpellSlot.Q);
                    var rdmg = _player.GetSpellDamage(e, SpellSlot.R);
                    if (q.IsReady() && e != null && e.IsValid && e.Health < qdmg)
                        q.CastOnUnit(e, usePackets());
                    if (r.IsReady() && e != null && e.IsValid && e.Health < rdmg)
                        r.CastOnUnit(e, usePackets());
                }
            }
        }

    }
}
