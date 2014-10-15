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
     * Revision 096: 15/10/2014
     * + Combo Rework
     * + Added Use R when Killable, Hardkill on default
     * + Same with text draw Hardkill/Killable etc
     * + Windslash Invulnerable check.
     * + ComboDamage rework, more accurate.
     * + Smart Q Gapclosing (let me know if you want this as a setting)
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
                Game.PrintChat("Riven: Loaded! Rev.096");
                Game.PrintChat("Riven: This is an early test some stuff may not be perfect yet, if you have any questions/concerns contact me on IRC/Forums. ");              
                Game.OnGameUpdate += Game_OnGameUpdate;
                Game.OnGameProcessPacket += Game_OnGameProcessPacket;
                Drawing.OnDraw += Game_OnDraw;
                //Obj_AI_Base.OnPlayAnimation += Obj_AI_Base_OnPlayAnimation;
                Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;

                _config = new Menu("KurisuRiven", "kriven", true);

                Menu menuTS = new Menu("Target Selector", "tselector");
                SimpleTs.AddToMenu(menuTS);
                _config.AddSubMenu(menuTS);

                Menu menuOrb = new Menu("Orbwalker", "orbwalker");
                _orbwalker = new Orbwalking.Orbwalker(menuOrb);
                _config.AddSubMenu(menuOrb);

                Menu menuD = new Menu("Draw Settings: ", "dsettings");
                menuD.AddItem(new MenuItem("drawaa", "Draw aa range")).SetValue(true);
                menuD.AddItem(new MenuItem("drawp", "Draw passive count")).SetValue(true);
                menuD.AddItem(new MenuItem("drawt", "Draw target")).SetValue(true);
                menuD.AddItem(new MenuItem("drawkill", "Draw killable")).SetValue(true);
                menuD.AddItem(new MenuItem("debugdmg", "Debug combo damage")).SetValue(false);
                menuD.AddItem(new MenuItem("debugtrue", "Debug true range")).SetValue(false);
                _config.AddSubMenu(menuD);

                Menu menuC = new Menu("Combo Settings: ", "csettings");
                menuC.AddItem(new MenuItem("usevalor", "Use E logic")).SetValue(true);
                menuC.AddItem(new MenuItem("useburst", "Use Ki Burst (W)")).SetValue(true);
                menuC.AddItem(new MenuItem("autow", "Auto W min targets")).SetValue(new Slider(3, 1, 5));
                menuC.AddItem(new MenuItem("useblade", "Use R logic")).SetValue(true);
                menuC.AddItem(new MenuItem("bladewhen", "Use R when: ")).SetValue(new StringList(new[] { "Easykill", "Normalkill", "Hardkill" }, 2));
                menuC.AddItem(new MenuItem("windlogic", "Usw Windslash logic")).SetValue(true);
                menuC.AddItem(new MenuItem("windslasher", "Windslash if damage dealt %")).SetValue(new Slider(70));
                menuC.AddItem(new MenuItem("blockanim", "Block Q animimation (fun)")).SetValue(false);
                //menuC.AddItem(new MenuItem("nonq", "Use non target Q")).SetValue(false);
                menuC.AddItem(new MenuItem("cancelanim", "Q Cancel type: ")) .SetValue(new StringList(new[] {"Move", "Packet"}));
                _config.AddSubMenu(menuC);


                _config.AddItem(new MenuItem("useignote", "Use Ignite")).SetValue(true);
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

            var buffs = _player.Buffs;
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
            if (_config.Item("debugtrue").GetValue<bool>() && !_tstarget.IsDead)
            {
                Utility.DrawCircle(_tstarget.Position, range + 25, Color.Orange, 1, 1); 
            }
                
            if (_config.Item("drawt").GetValue<bool>() && _tstarget != null && !_tstarget.IsDead)
            {
                Utility.DrawCircle(_tstarget.Position, _tstarget.BoundingRadius, Color.Red, 1, 1);
                Utility.DrawCircle(_tstarget.Position, _player.AttackRange + E.Range, Color.Red, 1, 1);
            }
            if (_config.Item("drawkill").GetValue<bool>() && _tstarget != null && !_tstarget.IsDead)
            {
                var ts = _tstarget;
                var wts = Drawing.WorldToScreen(_tstarget.Position);

                CheckDamage(ts);
                if (ts.Health < (float)(RA + RQ + RQ * 2 + RW + RI + RItems))
                    Drawing.DrawText(wts[0] - 20, wts[1] + 40, Color.OrangeRed, "Kill!");
                else if (ts.Health < (float)(RA * 2 + RQ * 2 + RW + RItems))
                    Drawing.DrawText(wts[0] - 40, wts[1] + 40, Color.OrangeRed, "Easy Kill!");
                else if (ts.Health < (float)(RA * 2 + RQ * 2 + RW + RI + RItems))
                    Drawing.DrawText(wts[0] - 40, wts[1] + 40, Color.OrangeRed, "FullCombo Kill!");
                else if (ts.Health < (float)(RA * 3 + RQ * 3 + RW + RI + RItems))
                    Drawing.DrawText(wts[0] - 40, wts[1] + 40, Color.OrangeRed, "FullCombo Hard Kill!");     
                else
                {
                    Drawing.DrawText(wts[0] - 40, wts[1] + 40, Color.OrangeRed, "Cant Kill!");
                }
            }
            if (_config.Item("debugdmg").GetValue<bool>() && _tstarget != null && !_tstarget.IsDead && !_player.IsDead)
            {
                var wts = Drawing.WorldToScreen(_tstarget.Position);
                Drawing.DrawText(wts[0] - 75, wts[1] +60, Color.Orange, "Combo Damage: " + (float)(RA * 3 + RQ * 3 + RW + RI + RItems));
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
                    if (_tstarget.Distance(_player.Position) < range + 25 &&
                        _player.HasBuff("RivenFengShuiEngine", true) && combo)
                    {
                        if (Q.IsReady() && triCleaveCount < 1)
                            Q.Cast(_tstarget.Position);
                    }
                    break;
                case "RivenFeint":
                    Orbwalking.LastAATick = 0;
                    valdelay = Environment.TickCount;
                    if (_tstarget.Distance(_player.Position) <= _player.AttackRange + 25 && combo)
                    {
                        if (Items.HasItem(3077) && Items.CanUseItem(3077))
                            Items.UseItem(3077);
                        if (Items.HasItem(3074) && Items.CanUseItem(3074))
                            Items.UseItem(3074);
                    }
                    break;
                case "rivenizunablade":
                    if (triCleaveCount == 2 && combo)
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
                            Packet.C2S.Move.Encoded(new Packet.C2S.Move.Struct(movePos.X, movePos.Y, 3, _orbwalker.GetTarget().NetworkId)).Send();
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
            //Game.PrintChat(Environment.TickCount.ToString(CultureInfo.InvariantCulture));
            //Game.PrintChat(tridelay.ToString(CultureInfo.InvariantCulture));
            if (target != null && target.IsValid)
            {
                if (_player.Distance(target.Position) > range + 25 ||
                    _player.Health*_player.MaxHealth/100 <= 45)
                {
                    if (E.IsReady() && _config.Item("usevalor").GetValue<bool>())
                        E.Cast(target.Position);
                }

                CheckR(target);

                if (W.IsReady() && (!Items.HasItem(3074) || !Items.CanUseItem(3074)) &&
                    (!Items.HasItem(3077) || !Items.CanUseItem(3077)))
                {
                    if (target.Distance(_player.Position) < W.Range)
                        W.Cast();
                }

                if (Q.IsReady() && !E.IsReady() && _player.Distance(target.Position) > Q.Range)
                {
                    if (valdelay + 300 < Environment.TickCount && tridelay + 350 < Environment.TickCount)
                    {
                        Q.Cast(target.Position, true);
                    }
                }

                    
                
            }
        }

        #endregion

        #region Riven : WindSlash
        private static void WindSlash()
        {
            var wsbool = _config.Item("windlogic").GetValue<bool>();
            var value = _config.Item("windslasher").GetValue<Slider>().Value;
            foreach (
                var e in
                    ObjectManager.Get<Obj_AI_Hero>()
                        .Where(
                            e =>
                                e.Team != _player.Team && e.IsValid && !e.IsDead && !e.IsInvulnerable &&
                                e.Distance(_player.Position) < R.Range))
            {
                PredictionOutput rPos = R.GetPrediction(e, true);
                if (rPos.Hitchance >= HitChance.Medium && _player.HasBuff("RivenFengShuiEngine", true))
                {
                    if (R.GetDamage(e) >= e.Health*e.MaxHealth/value && wsbool)
                        R.Cast(rPos.CastPosition, true);
                    else if (R.GetDamage(e) >= e.Health)
                        R.Cast(rPos.CastPosition);

                }
            }

        }
        #endregion

        private static readonly int[] _items = { 3077, 3074, 3144, 3153, 3142, 3112 };
        private static readonly int[] runicbladePassive =
        {
            20, 20, 25, 25, 25, 30, 30, 30, 35, 35, 35, 40, 40, 40, 45, 45, 45, 50
        };

        private static void UseItems(Obj_AI_Base target)
        {
            foreach (var i in _items.Where(i => Items.CanUseItem(i) && Items.HasItem(i)))
            {
                if (_player.Distance(target, true) <= Q.Range * Q.Range)
                    Items.UseItem(i);
            }
        }

        public static float DamageQ(Obj_AI_Base target)
        {
            double dmg = 0;
            if (Q.IsReady())
            {
                dmg += _player.CalcDamage(_player, Damage.DamageType.Physical,
                    -10 + (Q.Level*20) +
                    (0.35 + (Q.Level*0.05))*(_player.FlatPhysicalDamageMod + _player.BaseAttackDamage));

            }
            return (float) dmg;
        }

        #region Riven : Check Damage
        private static double RItems;
        private static double UA, UQ, UW;
        private static double RA, RQ, RW, RR, RI;
        private static void CheckDamage(Obj_AI_Base target)
        {
            if (target != null)
            {
                var count = runicBladeCount;
                var ignite = _player.GetSpellSlot("summonerdot");

                if (count == 0) count = 1;               
                double AA = _player.GetAutoAttackDamage(target) * 3;

                RA = AA * runicbladePassive[_player.Level] / 100 * count;
                RQ = Q.IsReady() ? DamageQ(target) : 0;
                RW = W.IsReady() ? _player.GetSpellDamage(target, SpellSlot.W) : 0;
                RI = _player.SummonerSpellbook.CanUseSpell(ignite) == SpellState.Ready ? _player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite) : 0;

                if (R.IsReady())
                    RR = _player.GetSpellDamage(target, SpellSlot.R);
         
                double TMT = Items.HasItem(3077) && Items.CanUseItem(3077)
                    ? _player.GetItemDamage(target, Damage.DamageItems.Tiamat)
                    : 0;
                double HYD = Items.HasItem(3074) && Items.CanUseItem(3074)
                    ? _player.GetItemDamage(target, Damage.DamageItems.Hydra)
                    : 0;
                double BWC = Items.HasItem(3144) && Items.CanUseItem(3144)
                    ? _player.GetItemDamage(target, Damage.DamageItems.Bilgewater)
                    : 0;
                double BRK = Items.HasItem(3153) && Items.CanUseItem(3153)
                    ? _player.GetItemDamage(target, Damage.DamageItems.Botrk)
                    : 0;

                RItems = TMT + HYD + BWC + BRK;

                if (R.IsReady() && !_player.HasBuff("RivenFengShuiEngine", true))
                {
                    UA = RA + _player.CalcDamage(target, Damage.DamageType.Physical, _player.BaseAttackDamage*0.2);
                    UQ = RQ + _player.CalcDamage(target, Damage.DamageType.Physical, _player.BaseAttackDamage * 0.2 * 0.7);
                    UW = RW + _player.CalcDamage(target, Damage.DamageType.Physical, _player.BaseAttackDamage * 0.2 * 1);
                    RR = RR + _player.CalcDamage(target, Damage.DamageType.Physical, _player.BaseAttackDamage * 0.2);
                }
            }
        }
        #endregion

        #region Riven: CheckR
        private void CheckR(Obj_AI_Base target)
        {
            var index = _config.Item("bladewhen").GetValue<StringList>();
            if (target != null && target.IsValid && target.Type == _player.Type && _config.Item("useblade").GetValue<bool>())
            {
                CheckDamage(target);
                if (!_player.HasBuff("RivenFengShuiEngine", true))
                {
                    switch (index.SelectedIndex)
                    {
                        case 2:
                            if (target.Health < (float)(UA * 3 + UQ * 3 + UW + RR + RI + RItems))
                                R.Cast();
                            break;
                        case 1:
                            if (target.Health < (float)(RA * 3 + RQ * 3 + RW + RR + RI + RItems))
                                R.Cast();
                            break;
                        case 0:
                            if (target.Health < (float)(RA * 2 + RQ * 2 + RW + RR + RI + RItems))
                                R.Cast();
                            break;
                    }
                }
            }
        }

        #endregion

        private void AutoW()
        {
            var getenemies =
                ObjectManager.Get<Obj_AI_Hero>()
                    .Where(
                        en =>
                            en.Team != _player.Team && en.IsValid && !en.IsDead &&
                            en.Distance(_player.Position) < W.Range);
            if (getenemies.Count() >= _config.Item("autow").GetValue<Slider>().Value)
                W.Cast();
        }
    }
}
