using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace KurisuRiven
{
    internal class KurisuRiven
    {
        private static Menu _config;
        private static Obj_AI_Hero _tstarget;
        private static readonly Obj_AI_Hero _player = ObjectManager.Player;
        private static Orbwalking.Orbwalker _orbwalker;

        private static Spell Q = new Spell(SpellSlot.Q, 280f);
        private static Spell W = new Spell(SpellSlot.W, 260f);
        private static Spell E = new Spell(SpellSlot.E, 390f);
        private static Spell R = new Spell(SpellSlot.R, 900f);


        public KurisuRiven()
        {
            try
            {
                Console.WriteLine("Riven is loaded!");
                CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
            }
            catch (Exception ex)
            {
                //Game.PrintChat(ex.Message);
                Console.WriteLine(ex.ToString());
            }
        }

        private static bool combo;
        private static float range;
        private static int edelay;

        private void Game_OnGameLoad(EventArgs args)
        {
            try
            {
                Game.PrintChat("Riven loaded!");
                Game.OnGameUpdate += Game_OnGameUpdate;
                Drawing.OnDraw += Game_OnDraw;
                Game.OnGameProcessPacket += Game_OnGameProcessPacket;
                Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;

                _config = new Menu("KurisuRiven", "kriven", true);

                Menu menuOrb = new Menu("Orbwalker", "orbwalker");
                _orbwalker = new Orbwalking.Orbwalker(menuOrb);
                _config.AddSubMenu(menuOrb);

                Menu menuD = new Menu("Draw Settings: ", "dsettings");


                Menu menuC = new Menu("Combo Settings: ", "csettings");
                menuC.AddItem(new MenuItem("usevalor", "Use E logic")).SetValue(true);
                menuC.AddItem(new MenuItem("useburst", "Use Ki Burst (W)")).SetValue(true);
                menuC.AddItem(new MenuItem("autow", "Auto W min targets")).SetValue(new Slider(3, 1, 5));
                menuC.AddItem(new MenuItem("useblade", "Use R logic")).SetValue(true);
                menuC.AddItem(new MenuItem("windslasher", "Windslash if damage dealt %")).SetValue(new Slider(65));
                menuC.AddItem(new MenuItem("blockanim", "Block Q anim")).SetValue(false);
                menuC.AddItem(new MenuItem("cancelanim", "Cancel type: "))
                    .SetValue(new StringList(new[] {"Move", "Packet"}));
                _config.AddSubMenu(menuC);


                _config.AddItem(new MenuItem("useignote", "Use Ignite")).SetValue(true);
                //_config.AddItem(new MenuItem("useharass", "Harass")).SetValue(new KeyBind(67, KeyBindType.Press));
                _config.AddItem(new MenuItem("combokey", "Combo")).SetValue(new KeyBind(32, KeyBindType.Press));

                _config.AddToMainMenu();

                R.SetSkillshot(0.25f, 300f, 120f, false, SkillshotType.SkillshotCone);
            }
            catch (Exception ex)
            {
                //Game.PrintChat(ex.Message);
                Console.WriteLine(ex.ToString());
            }
        }

        private void Game_OnDraw(EventArgs args)
        {
            
        }

        

        // incomplete
        private void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe) return;
            switch (args.SData.Name)
            {
                case "RivenTriCleave":
                    //qdelay = Environment.TickCount;
                    break;
                case "RivenMartyr":
                    Orbwalking.LastAATick = 0;
                    break;
                case "ItemTiamatCleave":
                    Orbwalking.LastAATick = 0;
                    W.Cast();
                    break;
                case "RivenFeint":
                    //edelay = Environment.TickCount;
                    Orbwalking.LastAATick = 0;
                    if (_tstarget.Distance(_player.Position) <= 600f)
                    {
                        if (Items.HasItem(3077) && Items.CanUseItem(3077))
                            Items.UseItem(3077);
                        if (Items.HasItem(3074) && Items.CanUseItem(3074))
                            Items.UseItem(3074);
                    }
                    break;
            }
        }

        private static void UseItems(Obj_AI_Base target)
        {
            foreach (var i in _items.Where(i => Items.CanUseItem(i) && Items.HasItem(i)))
            {
                if (_player.Distance(target, true) <= Q.Range * Q.Range)
                    Items.UseItem(i);
            }
        }

        private void Game_OnGameProcessPacket(GamePacketEventArgs args)
        {
            combo = _config.Item("combokey").GetValue<KeyBind>().Active;
            range = _player.AttackRange + _player.Distance(_player.BBox.Minimum) + 1;

            GamePacket packet = new GamePacket(args.PacketData);
            if (packet.Header == 176) // block q anim
            {
                packet.Position = 1;
                if (packet.ReadInteger() == _player.NetworkId && _config.Item("blockanim").GetValue<bool>())
                    args.Process = false;
            }

            if (packet.Header == 101) // damage
            {
                packet.Position = 16;
                int sourceId = packet.ReadInteger();

                packet.Position = 1;
                int targetId = packet.ReadInteger();
                int dmgType = packet.ReadByte();

                Obj_AI_Hero trueTarget = ObjectManager.GetUnitByNetworkId<Obj_AI_Hero>(targetId);
                if (sourceId == _player.NetworkId && (dmgType == 4 || dmgType == 3))
                {
                    if (combo)
                    {
                        UseItems(trueTarget);
                        Q.Cast(trueTarget.Position, true);
                    }
                }
            }

            if (packet.Header == 56 && packet.Size() == 9)
            {
                packet.Position = 1;
                int sourceId = packet.ReadInteger();
                if (sourceId == _player.NetworkId)
                {
                    int targetId = _orbwalker.GetTarget().NetworkId;
                    Obj_AI_Hero truetarget = ObjectManager.GetUnitByNetworkId<Obj_AI_Hero>(targetId);

                    bool method2 = _config.Item("cancelanim").GetValue<StringList>().SelectedIndex == 1;
                    bool method1 = _config.Item("cancelanim").GetValue<StringList>().SelectedIndex == 0;

                    if (_player.Distance(_orbwalker.GetTarget().Position) <= range + 25 && Orbwalking.Move)
                    {
                        Vector3 triPos = truetarget.Position + _player.Position -
                                         Vector3.Normalize(_player.Position)*
                                         (_player.Distance(truetarget.Position) + 57);

                        if (combo && method2)
                        {
                            Packet.C2S.Move.Encoded(new Packet.C2S.Move.Struct(triPos.X, triPos.Y, 3,
                                _orbwalker.GetTarget().NetworkId)).Send();
                            Orbwalking.LastAATick = 0;
                        }
                        else if (combo && method1)
                        {
                            _player.IssueOrder(GameObjectOrder.MoveTo, new Vector3(triPos.X, triPos.Y, triPos.Z));
                            Orbwalking.LastAATick = 0;
                        }
                    }
                }
            }

            if (packet.Header == 97) // move
            {
                packet.Position = 12;
                if (packet.ReadInteger() == _player.NetworkId)
                {
                    Orbwalking.Move = true;
                    if (Orbwalking.Move && combo && _orbwalker.GetTarget().IsValid &&
                        _player.Distance(_orbwalker.GetTarget().Position) < range + 25)
                    {
                        // hmmmm
                    }
                }
            }

            if (packet.Header == 254 && packet.Size() == 24)
            {
                packet.Position = 1;
                if (packet.ReadInteger() == _player.NetworkId)
                {
                    Orbwalking.LastAATick = Environment.TickCount;
                    Orbwalking.LastMoveCommandT = Environment.TickCount;
                }
            }
        }

        private static int runicBladeCount;
        private void Game_OnGameUpdate(EventArgs args)
        {
            _tstarget = SimpleTs.GetTarget(R.Range, SimpleTs.DamageType.Physical);

            if (_config.Item("combokey").GetValue<KeyBind>().Active)
                CastCombo(_tstarget);

            AutoW();
            WindSlash();

            var buffs = _player.Buffs;
            foreach (var b in buffs.Where(b => b.Name == "rivenpassiveaaboost"))
            {
                runicBladeCount = b.Count;    
            }

            if (!_player.HasBuff("rivenpassiveaaboost"))
                runicBladeCount = 0;
        }

        private void CastCombo(Obj_AI_Base target)
        {
            if (target != null && target.IsValid)
            {
                if (_player.Distance(target.Position) > range + 25 ||
                    _player.Health * _player.MaxHealth / 100 <= 45)
                {
                    if (E.IsReady() && _config.Item("usevalor").GetValue<bool>())
                        E.Cast(target.Position);
                    if (R.IsReady() && !_player.HasBuff("RivenFengShuiEngine", true) &&
                        _config.Item("useblade").GetValue<bool>())
                    {
                        if (ComboDamage(target) >= target.Health)
                        {
                            R.Cast();
                            if (_config.Item("useignote").GetValue<bool>())
                            {
                                var ignote = _player.GetSpellSlot("summonerdot");
                                _player.SummonerSpellbook.CastSpell(ignote, target);
                            }
 
                        }
                            
                    }
                    if (W.IsReady() && (!Items.HasItem(3074) || !Items.CanUseItem(3074)) && ( !Items.HasItem(3077) || !Items.CanUseItem(3077)))
                        W.Cast();
                    if (Q.IsReady() && !E.IsReady() && _player.Distance(target.Position) > Q.Range)
                    {
                        if (edelay + 300 < Environment.TickCount)
                            Q.Cast(target.Position);
                    }

                }
            }
        }

        private static void WindSlash()
        {
            var value = _config.Item("windslasher").GetValue<Slider>().Value;
            foreach (
                var e in
                    ObjectManager.Get<Obj_AI_Hero>()
                        .Where(
                            e =>
                                e.Team != _player.Team && e.IsValid && !e.IsDead &&
                                e.Distance(_player.Position) < R.Range))
            {
                if (ComboDamage(e) > e.Health)
                {
                    PredictionOutput rPos = R.GetPrediction(e, true);
                    if (rPos.Hitchance >= HitChance.Medium && _player.HasBuff("rivenwindslashready", true))
                    {
                        if (ComboDamage(e) >= e.Health*e.MaxHealth/value)
                            R.Cast(rPos.CastPosition, true);
                        else if (ComboDamage(e) >= e.Health)
                            R.Cast(rPos.CastPosition);
                    }
                }

            }

        }
        private static readonly int[] _items = { 3077, 3074, 3144, 3153, 3142, 3112 };
        private static readonly int[] runicbladePassive =
        {
            20, 20, 25, 25, 25, 30, 30, 30, 35, 35, 35, 40, 40, 40, 45, 45, 45, 50
        };

        public static float DamageQ(Obj_AI_Base target)
        {
            double dmg = 0;
            if (Q.IsReady())
            {
                dmg += _player.CalcDamage(_player, Damage.DamageType.Physical,
                    -10 + (Q.Level*20) +
                    (0.35 + (Q.Level*0.05))*(_player.FlatPhysicalDamageMod + _player.BaseAttackDamage));

            }
            return (float) dmg*3;
        }

        private static float ComboDamage(Obj_AI_Base target)
        {
            double dmg = 0;
            if (target != null)
            {

                var count = runicBladeCount;
                if (count == 0) count = 1;

                double AA = _player.GetAutoAttackDamage(target);
                double RA = AA*runicbladePassive[_player.Level]/100*count;
                dmg += RA;

                SpellSlot ignite = _player.GetSpellSlot("summonerdot");

                // spells
                if (_player.SummonerSpellbook.CanUseSpell(ignite) == SpellState.Ready)
                    dmg += _player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
                if (Q.IsReady())
                    dmg += DamageQ(target);
                if (W.IsReady())
                    dmg += _player.GetSpellDamage(target, SpellSlot.W);
                if (R.IsReady())
                    dmg += _player.GetSpellDamage(target, SpellSlot.R);
         
                // items
                if (Items.HasItem(3077) && Items.CanUseItem(3077))
                    dmg += _player.GetItemDamage(target, Damage.DamageItems.Tiamat);
                if (Items.HasItem(3074) && Items.CanUseItem(3074))
                    dmg += _player.GetItemDamage(target, Damage.DamageItems.Hydra);
                if (Items.HasItem(3144) && Items.CanUseItem(3144))
                    dmg += _player.GetItemDamage(target, Damage.DamageItems.Bilgewater);
                if (Items.HasItem(3153) && Items.CanUseItem(3153))
                    dmg += _player.GetItemDamage(target, Damage.DamageItems.Botrk);

                if (R.IsReady())
                    dmg += dmg * 0.2;
            }

            return (float) dmg;
        }

        private void AutoW()
        {
            var getenemies =
                ObjectManager.Get<Obj_AI_Hero>()
                    .Where(
                        en =>
                            en.Team != _player.Team && en.IsValid && !en.IsDead &&
                            en.Distance(_player.Position) < W.Range);
            if (getenemies.Count() > 2)
                W.Cast();
        }

    }
}
