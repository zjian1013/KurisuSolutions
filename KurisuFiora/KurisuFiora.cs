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
        private int aacount = 0;
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

                Game.PrintChat("<font color=\"#99FFE6\">[</font><font color=\"#33FFCC\">KurisuFiora</font><font color=\"#99FFE6\">]</font><font color=\"#99FFE6\"> - the Grand Duelist v1.3</font> - Loaded");
                spellList.Add(q);
                spellList.Add(r);

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

                
                Menu fioraSpells = new Menu("Fiora: Spells", "combo");
                fioraSpells.AddItem(new MenuItem("useq", "Use Q")).SetValue(true);
                fioraSpells.AddItem(new MenuItem("qrange", "Minimum range to q")).SetValue(new Slider(250, 1, (int)q.Range));
                fioraSpells.AddItem(new MenuItem("usew", "Use W")).SetValue(true);
                fioraSpells.AddItem(new MenuItem("wsett", "Minimum damage to W")).SetValue(new Slider(50, 1, 200));
                fioraSpells.AddItem(new MenuItem("usee", "Use E")).SetValue(true);
                fioraSpells.AddItem(new MenuItem("smana", "Combo min mana % ")).SetValue(new Slider(0, 0, 99));               
                _config.AddSubMenu(fioraSpells);

                Menu fioraWaltz = new Menu("Fiora: Blade Waltz", "fbw");
                fioraWaltz.AddItem(new MenuItem("user", "Use R")).SetValue(true);
                fioraWaltz.AddItem(new MenuItem("rdodge", "Dodge dangerous spells")).SetValue(true);
                _config.AddSubMenu(fioraWaltz);

                Menu fioraClear = new Menu("Fiora: Lane/Jungle Clear", "fclr");
                //fioraClear.AddItem(new MenuItem("clearq", "Use Q")).SetValue(true);
                //fioraClear.AddItem(new MenuItem("cleare", "Use E")).SetValue(true);
                //fioraClear.AddItem(new MenuItem("cmana", "Clear min mana % ")).SetValue(new Slider(55, 0, 99)); 
                _config.AddSubMenu(fioraClear);

                Menu fioraMisc = new Menu("Fiora: Misc", "fmisc");
                fioraMisc.AddItem(new MenuItem("ksteal", "Kill Steal")).SetValue(true);
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
                        Console.WriteLine("aacancel");
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
                                Console.WriteLine("aacancel: 2");
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

            if (args.PacketData[0]  == Packet.S2C.Damage.Header)
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
                if (_config.Item("aadebug").GetValue<bool>())
                    Console.WriteLine(Orbwalking.LastAATick);             
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
                        incDmg = DamageLib.getDmg(attacker, DamageLib.SpellType.AD);
                    if (spellSlot == SpellSlot.Q && w.IsReady() && (
                        sender.BaseSkinName == "Rengar" || sender.BaseSkinName == "Garen" || sender.BaseSkinName == "Nasus" ||
                        sender.BaseSkinName == "Shyvanna" || sender.BaseSkinName == "Leona" || sender.BaseSkinName == "Gankplank" ||
                        sender.BaseSkinName == "MissFortune" || sender.BaseSkinName == "Talon"))
                        w.Cast();
                    if (spellSlot == SpellSlot.W && w.IsReady() && (
                        sender.BaseSkinName == "Sivir" || sender.BaseSkinName == "Renekton" || sender.BaseSkinName == "Darius" ||
                        sender.BaseSkinName == "Jax"))
                        w.Cast();
                    if (_config.Item("rdodge").GetValue<bool>())
                    {
                        if (spellSlot == SpellSlot.R && sender.Distance(_player) < 400f && r.IsReady() && (
                            sender.BaseSkinName == "Malphite" || sender.BaseSkinName == "Cassiopeia" || sender.BaseSkinName == "Garen" ||
                            sender.BaseSkinName == "Graves" || sender.BaseSkinName == "Hecarim" || sender.BaseSkinName == "Jarven" ||
                            sender.BaseSkinName == "Amumu" || sender.BaseSkinName == "Tristana" || sender.BaseSkinName == "Syndra" ||
                            sender.BaseSkinName == "Darius" || sender.BaseSkinName == "Annie" || sender.BaseSkinName == "Sona" ||
                            sender.BaseSkinName == "Galio" || sender.BaseSkinName == "Veigar"))
                            r.Cast(sender);
                        if (spellSlot == SpellSlot.R && r.IsReady() && (
                            sender.BaseSkinName == "Jinx" || sender.BaseSkinName == "Ashe") && sender.Distance(_player) < 400f)
                            r.Cast(sender);
                    }
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
            if (ignote != SpellSlot.Unknown && _config.Item("isteal").GetValue<bool>())
                dmg += DamageLib.getDmg(enemy, DamageLib.SpellType.IGNITE);

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
                                _player.SummonerSpellbook.CastSpell(ignote , target);
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
                    var qdmg = DamageLib.getDmg(e, DamageLib.SpellType.Q);
                    var rdmg = DamageLib.getDmg(e, DamageLib.SpellType.R);
                    if (q.IsReady() && e != null && e.IsValid && e.Health < qdmg)
                        q.CastOnUnit(e, usePackets());
                    if (r.IsReady() && e != null && e.IsValid && e.Health < rdmg)
                        r.CastOnUnit(e, usePackets());
                }
            }
        }

    }
}
