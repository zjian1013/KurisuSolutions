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
     * Revision 0981: 18/10/2014
     * + Fixed major late game error when arround post level 17/18
     *   where the combo/assembly would just spaz out do nothing
     * + No longer uses items in jungle
     *   
     * Reision 098: 18/10/2014
     * + Does not Q gapclose pre level 3 for technical reasons
     * + Windslash if Enemies hit >= setting (3)
     * + Jungle Q->AA Improved significantly
     * + New Q cance mode: Delay (MUST DELETE MenuConfig Folders to Update Menu!) 
     *   adjust the delay in extra settings
     * + Fixed casting W at the wrong times
     * + Fixed ATR when casting W while engaging (at least for me)
     * + Fixed items.
     * 
     * Revision 0975: 17/10/2014
     * + New dynamic combos based on distance and target health (Windslash must be set to Max Damage)
     * + Hopefully fixed combo damage text (debug/checktarget) not correctly calculating ultimate
     *   it was calculating off based ad ratios instead of the AD from all items
     *   this made the assembly visually say you couldn't kill a target when you actually could.
     *   
     * + W will now only cast after Hydra/Tiamat if holding combo
     *   same for other stuff that you may have noticed.
     * + Also think i fixed a possible ATR issue with tiamat and hydra
     * 
     * Revision 097: 16/10/2014
     * + Should be more stable
     * + Started working on 2nd combo not finished
     * 
     * Revision 0964: 16/10/2014
     * + Fixed menu glitch
     * + Trying to fix atr issues (why so many commits)
     * 
     * Revision 09633: 16/10/2014
     * + E -> R Fix
     * 
     * Revision 0963: 15/10/20144
     * + Added Q limit for gapclose
     * + Windslash rework
     * + Delay management (hopefully this fixes some connection issues)
     * + other things.. zz
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
     * Revision 090: 14/10/2014
     * + Test Release            
     */
    internal class KurisuRiven
    {
        private static Menu _config;
        private static Obj_AI_Hero _target;
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
                Game.PrintChat("Riven: Loaded! Revision: 0981");
                Game.PrintChat("Riven: This is an early test some stuff may not be perfect yet, if you have any questions/concerns contact me on IRC/Forums. ");
                Game.OnGameUpdate += Game_OnGameUpdate;
                Game.OnGameProcessPacket += Game_OnGameProcessPacket;
                Drawing.OnDraw += Game_OnDraw;
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
                menuC.AddItem(new MenuItem("useblade", "Use R logic")).SetValue(true);
                menuC.AddItem(new MenuItem("waitvalor", "Wait for E (Ult)")).SetValue(true);
                menuC.AddItem(new MenuItem("bladewhen", "Use R when: ")).SetValue(new StringList(new[] { "Easykill", "Normalkill", "Hardkill" }, 2));
                menuC.AddItem(new MenuItem("wslash", "Windslash: ")).SetValue(new StringList(new[] { "Only Kill", "Max Damage" }, 1));
                menuC.AddItem(new MenuItem("qsett", "Q gapclose limit")).SetValue(new Slider(1, 1, 3));
                menuC.AddItem(new MenuItem("cancelanim", "Q Cancel type: ")).SetValue(new StringList(new[] { "Move", "Packet", "Delay" }));
                _config.AddSubMenu(menuC);

                Menu menuO = new Menu("Extra Settings: ", "osettings");
                menuO.AddItem(new MenuItem("useignote", "Use Ignite (Works)")).SetValue(true);
                menuO.AddItem(new MenuItem("useautow", "Enable auto W")).SetValue(true);
                menuO.AddItem(new MenuItem("autow", "Auto W min targets")).SetValue(new Slider(3, 1, 5));
                menuO.AddItem(new MenuItem("useautows", "Enable auto Windslash")).SetValue(true);
                menuO.AddItem(new MenuItem("autows", "Windslash if damage dealt %")).SetValue(new Slider(65, 1));
                menuO.AddItem(new MenuItem("autows2", "Windslash if targets hit >=")).SetValue(new Slider(3, 2, 5));
                menuO.AddItem(new MenuItem("qdelay", "Q Delay Mode ammount")).SetValue(new Slider(50, 0, 250));
                menuO.AddItem(new MenuItem("blockanim", "Block Q animimation (fun)")).SetValue(false);
                _config.AddSubMenu(menuO);

                _config.AddItem(new MenuItem("combokey", "Combo")).SetValue(new KeyBind(32, KeyBindType.Press));
                _config.AddItem(new MenuItem("clearkey", "Jungleclear")).SetValue(new KeyBind(86, KeyBindType.Press));
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
        private static bool clear;
        private static float truerange;
        private static int runicbladecount;
        private static int tricleavecount;
        private void Game_OnGameUpdate(EventArgs args)
        {
            try
            {
                _target = SimpleTs.GetTarget(R.Range, SimpleTs.DamageType.Physical);

                if (_config.Item("combokey").GetValue<KeyBind>().Active)
                {
                    if (!_player.IsStunned)
                        CastCombo(_target);
                }

                if (!_player.IsStunned)
                {
                    AutoW();
                    WindSlash();                   
                }

                RefreshBuffs();
            }
            catch (Exception ex)
            {
                //Game.PrintChat(ex.Message);
                Console.WriteLine(ex);
            }
        }

        #endregion

        #region Riven : OnDraw
        private void Game_OnDraw(EventArgs args)
        {
            if (_config.Item("drawaa").GetValue<bool>() && !_player.IsDead)
                Utility.DrawCircle(_player.Position, _player.AttackRange + 25, Color.Khaki, 3, 20);
            if (_config.Item("drawp").GetValue<bool>() && !_player.IsDead)
            {
                var wts = Drawing.WorldToScreen(_player.Position);
                Drawing.DrawText(wts[0] - 35, wts[1] + 30, Color.Khaki, "Passive: " + runicbladecount);
                Drawing.DrawText(wts[0] - 35, wts[1] + 10, Color.Khaki, "Q: " + tricleavecount);
            }
            if (_config.Item("debugtrue").GetValue<bool>())
            {
                if (_target != null && !_target.IsDead && !_player.IsDead)
                {
                    Utility.DrawCircle(_target.Position, truerange + 25, Color.Orange, 1, 1);
                }
            }

            if (_config.Item("drawt").GetValue<bool>())
            {
                if (_target != null && !_target.IsDead && !_player.IsDead)
                {
                    Utility.DrawCircle(_target.Position, _target.BoundingRadius, Color.Red, 1, 1);
                    Utility.DrawCircle(_target.Position, _player.AttackRange + E.Range, Color.Red, 1, 1);
                }
            }
            if (_config.Item("drawkill").GetValue<bool>())
            {
                if (_target != null && !_target.IsDead && !_player.IsDead)
                {
                    var ts = _target;
                    CheckDamage(ts);
                    var wts = Drawing.WorldToScreen(_target.Position);
                    if ((float)(RA + RQ * 2 + RW + RI + RItems) > ts.Health)
                        Drawing.DrawText(wts[0] - 20, wts[1] + 40, Color.OrangeRed, "Kill!");
                    else if ((float)(RA * 2 + RQ * 2 + RW + RItems) > ts.Health)
                        Drawing.DrawText(wts[0] - 40, wts[1] + 40, Color.OrangeRed, "Easy Kill!");
                    else if ((float)(UA * 2 + UQ * 2 + UW + RI + RR + RItems) > ts.Health)
                        Drawing.DrawText(wts[0] - 40, wts[1] + 40, Color.OrangeRed, "Full Combo Kill!");
                    else if ((float)(UA * 6 + UQ * 3 + UW + RR + RI + RItems) > ts.Health)
                        Drawing.DrawText(wts[0] - 40, wts[1] + 40, Color.OrangeRed, "Full Combo Hard Kill!");
                    else 
                    {
                        Drawing.DrawText(wts[0] - 40, wts[1] + 40, Color.OrangeRed, "Cant Kill!");
                    }
                }
            }
            if (_config.Item("debugdmg").GetValue<bool>())
            {
                if (_target != null && !_target.IsDead && !_player.IsDead)
                {
                    var wts = Drawing.WorldToScreen(_target.Position);
                    if (!R.IsReady())
                        Drawing.DrawText(wts[0] - 75, wts[1] + 60, Color.Orange,
                            "Combo Damage: " + (float)(RA * 3 + RQ * 3 + RW + RR + RI + RItems));
                    else
                        Drawing.DrawText(wts[0] - 75, wts[1] + 60, Color.Orange,
                            "Combo Damage: " + (float)(UA * 6 + UQ * 3 + UW + RR + RI + RItems));
                }
            }
        }
        #endregion

        #region Riven : OnProcessSpell
        private static int valdelay;
        private static int tridelay;

        private void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            var target = _target;
            var ultiOn = _player.HasBuff("RivenFengShuiEngine", true);
            var wslash = _config.Item("wslash").GetValue<StringList>().SelectedIndex;

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
                    Utility.DelayAction.Add(Game.Ping + 75, () => UseItems(target));
                    if (W.IsReady() && combo)
                        Utility.DelayAction.Add(Game.Ping + 75, () => W.Cast());
                    break;
                case "RivenFeint":  
                    Orbwalking.LastAATick = 0;
                    valdelay = Environment.TickCount;
                    if (combo) Utility.DelayAction.Add(Game.Ping + 50, () => UseItems(target));
                    Utility.DelayAction.Add(Game.Ping + 125, delegate
                    {
                        if (target.Distance(_player.Position) <= _player.AttackRange + 25 && combo)
                        {
                            if (Items.HasItem(3077) && Items.CanUseItem(3077))
                                Items.UseItem(3077);
                            if (Items.HasItem(3074) && Items.CanUseItem(3074))
                                Items.UseItem(3074);
                        }
                    });
                    if (R.IsReady() && ultiOn && wslash == 1 && _config.Item("useblade").GetValue<bool>())
                    {
                        if (tricleavecount == 2 && target.Health < (float)(UA * 2 + UQ * 2 + UW + RR + RI + RItems))
                            R.Cast(target.Position, true);
                        if (tricleavecount <= 1 && target.Health < (float)(RA * 2 + RQ * 2 + RW + RI + RItems))
                            R.Cast(target.Position, true);
                    }

                    break;
                case "RivenFengShuiEngine":
                    //Utility.DelayAction.Add(Game.Ping + 75, () => UseItems(target));
                    //if (W.IsReady() && combo && target.Distance(_player.Position) < W.Range)
                    //    Utility.DelayAction.Add(Game.Ping + 75, () => W.Cast());
                    break;
                case "rivenizunablade":
                    if (Q.IsReady())
                        Q.Cast(target.Position, true);
                    break;

            }

        }

        #endregion

        #region Riven : OnProcessPacket
        private void Game_OnGameProcessPacket(GamePacketEventArgs args)
        {
            combo = _config.Item("combokey").GetValue<KeyBind>().Active;
            clear = _config.Item("clearkey").GetValue<KeyBind>().Active;
            var delay = _config.Item("qdelay").GetValue<Slider>().Value;

            truerange = _player.AttackRange + _player.Distance(_player.BBox.Minimum) + 1;

            GamePacket packet = new GamePacket(args.PacketData);

            if (packet.Header == 176) // block q anim
            {
                packet.Position = 1;
                if (packet.ReadInteger() == _player.NetworkId && _config.Item("blockanim").GetValue<bool>())
                    args.Process = false;
            }

            if (packet.Header == 101 && combo)
            {
                packet.Position = 16;
                int sourceId = packet.ReadInteger();

                packet.Position = 1;
                int targetId = packet.ReadInteger();
                int dmgType = packet.ReadByte();

                Obj_AI_Hero trueTarget = ObjectManager.GetUnitByNetworkId<Obj_AI_Hero>(targetId);
                if (sourceId == _player.NetworkId && (dmgType == 4 || dmgType == 3))
                {
                    UseItems(trueTarget);
                    Q.Cast(trueTarget.Position, true);
                }
            }

            if (packet.Header == 101 && clear) 
            {
                packet.Position = 16;
                int sourceId = packet.ReadInteger();

                packet.Position = 1;
                int targetId = packet.ReadInteger();
                int dmgType = packet.ReadByte();

                Obj_AI_Minion trueTarget = ObjectManager.GetUnitByNetworkId<Obj_AI_Minion>(targetId);
                if (sourceId == _player.NetworkId && (dmgType == 4 || dmgType == 3) &&
                    JungleMinions.Any(name => trueTarget.Name.StartsWith(name)))
                {
                    Q.Cast(trueTarget.Position, true);
                }

            }

            if (packet.Header == 56 && packet.Size() == 9 && combo)
            {
                packet.Position = 1;
                int sourceId = packet.ReadInteger();
                if (sourceId == _player.NetworkId)
                {
                    int targetId = _orbwalker.GetTarget().NetworkId;
                    Obj_AI_Hero truetarget = ObjectManager.GetUnitByNetworkId<Obj_AI_Hero>(targetId);

                    int method = _config.Item("cancelanim").GetValue<StringList>().SelectedIndex;
                    if (_player.Distance(_orbwalker.GetTarget().Position) <= truerange + 25 && Orbwalking.Move)
                    {
                        Vector3 movePos = truetarget.Position + _player.Position -
                                         Vector3.Normalize(_player.Position) *
                                         (_player.Distance(truetarget.Position) + 57);

                        if (method == 1)
                        {
                            Packet.C2S.Move.Encoded(new Packet.C2S.Move.Struct(movePos.X, movePos.Y, 3, _orbwalker.GetTarget().NetworkId)).Send();
                            Orbwalking.LastAATick = 0;
                        }
                        else if (method == 0)
                        {
                            _player.IssueOrder(GameObjectOrder.MoveTo, new Vector3(movePos.X, movePos.Y, movePos.Z));
                            Orbwalking.LastAATick = 0;
                        }
                        else if (method == 2)
                        {
                            _player.IssueOrder(GameObjectOrder.MoveTo, new Vector3(movePos.X, movePos.Y, movePos.Z));
                            //Packet.C2S.Move.Encoded(new Packet.C2S.Move.Struct(movePos.X, movePos.Y, 3, _orbwalker.GetTarget().NetworkId)).Send();
                            Utility.DelayAction.Add(Game.Ping + delay, () => Orbwalking.LastAATick = 0);
                        }
                    }
                }
            }

            if (packet.Header == 56 && packet.Size() == 9 && clear)
            {
                packet.Position = 1;
                int sourceId = packet.ReadInteger();
                if (sourceId == _player.NetworkId)
                {
                    int targetId = _orbwalker.GetTarget().NetworkId;
                    Obj_AI_Minion truetarget = ObjectManager.GetUnitByNetworkId<Obj_AI_Minion>(targetId);

                    int method = _config.Item("cancelanim").GetValue<StringList>().SelectedIndex;                 
                    if (_player.Distance(_orbwalker.GetTarget().Position) <= truerange + 25 && Orbwalking.Move)
                    {
                        Vector3 movePos = truetarget.Position + _player.Position -
                                         Vector3.Normalize(_player.Position) *
                                         (_player.Distance(truetarget.Position) + 63);

                        if (JungleMinions.Any(name => truetarget.Name.StartsWith(name)))
                        {
                            Packet.C2S.Move.Encoded(new Packet.C2S.Move.Struct(movePos.X, movePos.Y, 3, _orbwalker.GetTarget().NetworkId)).Send();
                            Utility.DelayAction.Add(Game.Ping + delay, () => Orbwalking.LastAATick = 0);
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
            if (target != null && target.IsValid && target.IsVisible)
            {
                if (_player.Distance(target.Position) > truerange + 25 ||
                    _player.Health * _player.MaxHealth / 100 <= 45 /*&& !R.IsReady()*/)
                {
                    if (E.IsReady() && _config.Item("usevalor").GetValue<bool>())
                        E.Cast(target.Position);
                    if (E.IsReady() && _config.Item("waitvalor").GetValue<bool>())
                        CheckR(target);
                }

                else if (W.IsReady() && Q.IsReady() && target.Distance(_player.Position) < W.Range)
                {
                    CheckR(target);
                    W.Cast();
                }

                if (R.IsReady() && E.IsReady() && _player.HasBuff("RivenFengShuiEngine", true)) // utli on
                {
                    if (tricleavecount == 2)
                        E.Cast(target.Position);
                }

                if (W.IsReady() && !E.IsReady() && (!Items.HasItem(3074) || !Items.CanUseItem(3074)) &&
                    (!Items.HasItem(3077) || !Items.CanUseItem(3077)))
                {
                    if (target.Distance(_player.Position) < W.Range)
                        W.Cast();
                }

                if (Q.IsReady() && !E.IsReady() && _player.Distance(target.Position) > Q.Range)
                {
                    if (valdelay + Game.Ping + 150 < Environment.TickCount &&
                        tridelay + Game.Ping + 100 < Environment.TickCount && _player.Level >= 3)
                    {
                        if (tricleavecount < _config.Item("qsett").GetValue<Slider>().Value)
                            Q.Cast(target.Position, true);
                    }
                }

                if (W.IsReady() && !E.IsReady() && !Q.IsReady())
                {
                    if (_player.Distance(target.Position) < W.Range)
                        W.Cast();
                }
            }
        }

        #endregion

        #region Riven : WindSlash
        private static void WindSlash()
        {
            if (!_config.Item("useblade").GetValue<bool>()) return;
            foreach (
                var e in
                    ObjectManager.Get<Obj_AI_Hero>()
                        .Where(
                            e =>
                                e.Team != _player.Team && e.IsValid && !e.IsDead && !e.IsInvulnerable &&
                                e.Distance(_player.Position) < R.Range))
            {
                CheckDamage(e);
                PredictionOutput rPos = R.GetPrediction(e, true);
                if (R.IsReady() && rPos.Hitchance >= HitChance.Medium &&
                    _player.HasBuff("RivenFengShuiEngine", true))
                {
                    if (rPos.AoeTargetsHitCount >= _config.Item("autows2").GetValue<Slider>().Value)
                        R.Cast(rPos.CastPosition, true); 
                    int wsneed = _config.Item("autows").GetValue<Slider>().Value;
                    int wslash = _config.Item("wslash").GetValue<StringList>().SelectedIndex;
                    if (e.Health < RR)
                        R.Cast(rPos.CastPosition, true);
                    else if (e.Health < RR + RA * 2 + RQ * 1 && wslash == 1)
                        R.Cast(rPos.CastPosition);
                    else if (RR / e.MaxHealth * 100 > e.Health / e.MaxHealth * wsneed && wslash == 1)
                    {
                        if (_config.Item("useautows").GetValue<bool>())
                            R.Cast(rPos.CastPosition);
                    }
                }
            }
        }

        #endregion


        private static readonly int[] _items = { 3144, 3153, 3142, 3112 };
        private static readonly int[] runicbladePassive =
        {
            20, 20, 25, 25, 25, 30, 30, 30, 35, 35, 35, 40, 40, 40, 45, 45, 45, 50, 50
        };

        private static readonly string[] JungleMinions =
        {
            "AncientGolem", "GreatWraith", "Wraith", "LizardElder", "Golem", "Worm", "Dragon", "GiantWolf" 
        
        };

        private static void RefreshBuffs()
        {
            var buffs = _player.Buffs;
            foreach (var b in buffs)
            {
                if (b.Name == "rivenpassiveaaboost")
                    runicbladecount = b.Count;
                if (b.Name == "RivenTriCleave")
                    tricleavecount = b.Count;
            }
            if (!_player.HasBuff("rivenpassiveaaboost", true))
                runicbladecount = 0;
            if (!Q.IsReady())
                tricleavecount = 0;
        }

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
                    -10 + (Q.Level * 20) +
                    (0.35 + (Q.Level * 0.05)) * (_player.FlatPhysicalDamageMod + _player.BaseAttackDamage));

            }
            return (float)dmg;
        }

        #region Riven : Check Damage
        private static double RItems;
        private static double UA, UQ, UW;
        private static double RA, RQ, RW, RR, RI;
        private static void CheckDamage(Obj_AI_Base target)
        {
            if (target != null)
            {
                var count = runicbladecount;
                var ignite = _player.GetSpellSlot("summonerdot");

                if (count == 0) count = 1;
                double AA = _player.GetAutoAttackDamage(target);

                RR = _player.GetSpellDamage(target, SpellSlot.R);
                RA = AA * runicbladePassive[_player.Level] / 100 * 3;
                RQ = Q.IsReady() ? DamageQ(target) : 0;
                RW = W.IsReady() ? _player.GetSpellDamage(target, SpellSlot.W) : 0;
                RI = _player.SummonerSpellbook.CanUseSpell(ignite) == SpellState.Ready ? _player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite) : 0;


                double TMT = Items.HasItem(3077) && Items.CanUseItem(3077) ? _player.GetItemDamage(target, Damage.DamageItems.Tiamat) : 0;
                double HYD = Items.HasItem(3074) && Items.CanUseItem(3074) ? _player.GetItemDamage(target, Damage.DamageItems.Hydra) : 0;
                double BWC = Items.HasItem(3144) && Items.CanUseItem(3144) ? _player.GetItemDamage(target, Damage.DamageItems.Bilgewater) : 0;
                double BRK = Items.HasItem(3153) && Items.CanUseItem(3153) ? _player.GetItemDamage(target, Damage.DamageItems.Botrk) : 0;

                RItems = TMT + HYD + BWC + BRK;

                if (R.IsReady())
                {
                    UA = RA + _player.CalcDamage(target, Damage.DamageType.Physical, AA * 0.2);
                    UQ = RQ + _player.CalcDamage(target, Damage.DamageType.Physical, AA * 0.2 * 0.7);
                    UW = RW + _player.CalcDamage(target, Damage.DamageType.Physical, AA * 0.2 * 1);
                    RR = RR + _player.CalcDamage(target, Damage.DamageType.Physical, AA * 0.2);
                }
                else
                {
                    UA = RA;
                    UQ = RQ;
                    UW = RW;
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
                            if ((float) (UA*6 + UQ*3 + UW + RR + RI + RItems) > target.Health)
                            {
                                R.Cast();
                                if (_config.Item("useignote").GetValue<bool>())
                                    CastIgnite(target);
                            }
                            break;
                        case 1:
                            if ((float) (RA*3 + RQ*3 + RW + RR + RI + RItems) > target.Health)
                            {
                                R.Cast();
                                if (_config.Item("useignote").GetValue<bool>())
                                    CastIgnite(target);
                            }
                            break;
                        case 0:
                            if ((float) (RA*2 + RQ*2 + RW + RR + RI + RItems) > target.Health)
                            {
                                R.Cast();
                                if (_config.Item("useignote").GetValue<bool>())
                                    CastIgnite(target);
                            }
                            break;
                    }
                }
            }
        }

        #endregion

        private static void CastIgnite(Obj_AI_Base target)
        {
            if (target != null && target.IsValid)
            {
                var ignote = _player.GetSpellSlot("summonerdot");
                if (_player.SummonerSpellbook.CanUseSpell(ignote) == SpellState.Ready)
                {
                    _player.SummonerSpellbook.CastSpell(ignote, target);
                }
            }
        }


        private void AutoW()
        {
            var getenemies =
                ObjectManager.Get<Obj_AI_Hero>()
                    .Where(
                        en =>
                            en.Team != _player.Team && en.IsValid && !en.IsDead &&
                            en.Distance(_player.Position) < W.Range);
            if (getenemies.Count() >= _config.Item("autow").GetValue<Slider>().Value)
            {
                if (W.IsReady() && _config.Item("useautow").GetValue<bool>())
                {
                    W.Cast();
                }
            }

        }
    }
}
