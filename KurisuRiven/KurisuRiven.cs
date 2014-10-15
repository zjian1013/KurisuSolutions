using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

namespace KurisuRiven
{

                    
    /*   _____ _             
     *  | __  |_|_ _ ___ ___ 
     *  |    -| | | | -_|   |
     *  |__|__|_|\_/|___|_|_|
     *  
     * Revision 095: 14/10/2014
     * + W should no longer miss
     * + Windslash tweaks
     * + Tiamat range required 300 (from 600)
     * + Lag free drawings and world to screen drawings
     * + Fixed Runic blade passive not correctly counting
     * + Q gapclose (not tested x.x)
     * 
     * 
     * Revision 090: 14/10/2014
     * + Beta Release            
     */
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
                Console.WriteLine("KurisuRiven is loaded!");
                CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }


        #region Riven: OnGameLoad
        private void Game_OnGameLoad(EventArgs args)
        {
            try
            {
                Game.PrintChat("Riven: Loaded!");
                Game.PrintChat("Riven: This is an early test R logic is not perfect yet, if you have any questions/concerns contact me on IRC. ");
                Game.PrintChat("If you enjoyed this assembly and would like to support me and my future wok I accept donations to my paypal: xrobinsong@gmail.com. ");
                Game.PrintChat("Happy pwning dem noobs =)");

                Game.OnGameUpdate += Game_OnGameUpdate;
                Game.OnGameProcessPacket += Game_OnGameProcessPacket;
                Drawing.OnDraw += Game_OnDraw;
                //Obj_AI_Base.OnPlayAnimation += Obj_AI_Base_OnPlayAnimation;
                Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;

                _config = new Menu("KurisuRiven", "kriven", true);

                Menu menuOrb = new Menu("Orbwalker", "orbwalker");
                _orbwalker = new Orbwalking.Orbwalker(menuOrb);
                _config.AddSubMenu(menuOrb);

                Menu menuD = new Menu("Draw Settings: ", "dsettings");
                menuD.AddItem(new MenuItem("drawaa", "Draw aa range")).SetValue(true);
                menuD.AddItem(new MenuItem("drawp", "Draw passive count")).SetValue(true);
                menuD.AddItem(new MenuItem("drawjs", "Draw jumpsots")).SetValue(true);
                menuD.AddItem(new MenuItem("drawt", "Draw target")).SetValue(true);
                menuD.AddItem(new MenuItem("drawkill", "Draw killable")).SetValue(true);
                _config.AddSubMenu(menuD);

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

        #endregion

        #region Riven : OnPlayAnimation
        //private void Obj_AI_Base_OnPlayAnimation(GameObject sender, GameObjectPlayAnimationEventArgs args)
        //{
            //combo = _config.Item("combokey").GetValue<KeyBind>().Active;

            //if (!sender.IsMe) return;

            //if (args.Animation.Contains("Spell1a"))
            //{
            //    if (combo)
            //    {             
            //        var movePos = 
            //    }
            //}
        //}

        #endregion
       
        #region Riven : OnGameUpdate

        private static bool combo;
        private static float range;
        private static int runicBladeCount;
        private static int triCleaveCount;
        private void Game_OnGameUpdate(EventArgs args)
        {
            _tstarget = SimpleTs.GetTarget(R.Range, SimpleTs.DamageType.Physical);

            AutoW();
            WindSlash();

            if (_config.Item("combokey").GetValue<KeyBind>().Active)
                CastCombo(_tstarget);

            BuffInstance[] buffs = _player.Buffs;
            foreach (var b in buffs)
            {
                if (b.Name == "rivenpassiveaaboost")
                    runicBladeCount = b.Count;
                if (b.Name == "RivenTriCleave")
                    triCleaveCount = b.Count;
            }

            if (!_player.HasBuff("rivenpassiveaaboost", true))
                runicBladeCount = 0;
            if (!Q.IsReady())
                triCleaveCount = 0;
         }

        #endregion

        #region Riven : OnDraw
        private void Game_OnDraw(EventArgs args)
        {        
            if (_config.Item("drawaa").GetValue<bool>() && !_player.IsDead)
                Utility.DrawCircle(_player.Position, _player.AttackRange, Color.Khaki, 1, 1);
            if (_config.Item("drawp").GetValue<bool>() && !_player.IsDead)
            {
                var wts = Drawing.WorldToScreen(_player.Position);
                Drawing.DrawText(wts[0]-35, wts[1]+30, Color.Khaki, "Passive: " +runicBladeCount);
                Drawing.DrawText(wts[0] - 35, wts[1] + 10, Color.Khaki, "Q: " + triCleaveCount);
            }
            if (_config.Item("drawt").GetValue<bool>() && _tstarget != null && !_tstarget.IsDead)
            {
                Utility.DrawCircle(_tstarget.Position, _tstarget.BoundingRadius, Color.Red, 1, 1);
            }
            if (_config.Item("drawkill").GetValue<bool>() && _tstarget != null && !_tstarget.IsDead)
            {
                var wts = Drawing.WorldToScreen(_tstarget.Position);
                if (ComboDamage(_tstarget) > _tstarget.Health)                   
                    Drawing.DrawText(wts[0], wts[1], Color.Red, "Killable");
                else
                {
                    Drawing.DrawText(wts[0]-15, wts[1]-10, Color.Orange, "Harass target!");
                }
            }
        }
        #endregion

        #region Riven : OnProcessSpell
        private static int valdelay;
        private static int tridelay;

        private void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe) return;
            switch (args.SData.Name)
            {
                case "RivenTriCleave":
                    tridelay = Environment.TickCount;
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
                    valdelay = Environment.TickCount;
                    if (_tstarget.Distance(_player.Position) <= 300f)
                    {
                        if (Items.HasItem(3077) && Items.CanUseItem(3077))
                            Items.UseItem(3077);
                        if (Items.HasItem(3074) && Items.CanUseItem(3074))
                            Items.UseItem(3074);
                    }
                    if (triCleaveCount == 2 && _player.HasBuff("rivenwindslashready", true))
                    {
                         PredictionOutput rPos = R.GetPrediction(_tstarget, true);
                         if (rPos.Hitchance >= HitChance.Medium)
                             R.Cast(rPos.CastPosition, true);
                    }
                    break;
                case "rivenizunablade":
                    Q.Cast(_tstarget.Position, true);
                    break;
            }
        }

        #endregion
     
        #region Riven : OnProcessPacket
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
                        Vector3 movePos = truetarget.Position + _player.Position -
                                         Vector3.Normalize(_player.Position)*
                                         (_player.Distance(truetarget.Position) + 57);

                        if (combo && method2)
                        {
                            Packet.C2S.Move.Encoded(new Packet.C2S.Move.Struct(movePos.X, movePos.Y, 3,
                                _orbwalker.GetTarget().NetworkId)).Send();
                            Orbwalking.LastAATick = 0;
                        }
                        else if (combo && method1)
                        {
                            _player.IssueOrder(GameObjectOrder.MoveTo, new Vector3(movePos.X, movePos.Y, movePos.Z));
                            Orbwalking.LastAATick = 0;
                        }
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

        #endregion

        #region Riven : Combo
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
                    if (W.IsReady() && (!Items.HasItem(3074) || !Items.CanUseItem(3074)) &&
                        (!Items.HasItem(3077) || !Items.CanUseItem(3077)))
                    {
                        if (target.Distance(_player.Position) < W.Range)
                            W.Cast();
                    }
                       
                    if (Q.IsReady() && !E.IsReady() && _player.Distance(target.Position) > Q.Range)
                    {
                        if (valdelay + 300 < Environment.TickCount && tridelay + 1.7 < Environment.TickCount)
                            Q.Cast(target.Position, true);
                    }

                }
            }
        }

        #endregion

        private static void UseItems(Obj_AI_Base target)
        {
            foreach (var i in _items.Where(i => Items.CanUseItem(i) && Items.HasItem(i)))
            {
                if (_player.Distance(target, true) <= Q.Range * Q.Range)
                    Items.UseItem(i);
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
