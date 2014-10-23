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
     * Revision 0985-2: 22/10/2014
     * + Fixed wasting valor when target was already in range.
     *
     * Revision 0985: 21/10/2014
     * + Ultimate overkill fix
     * + Fixed AA damage caclulations
     * + Fixed R check.
     * 
     * Revision 0984: 19/10/2014
     * + Updated target selctor <3.
     * + Removed older target seletor since it was redundant.
     * + Added E/W in jungle clear (E's to mouse cursor)
     * + Q gapclosing has been reworked and now works at level 1 again.
     *   limiter has ben removed will try to use less q's as possible,
     *   this should result in more Q weaving.
     * + Will ignore Q weaving if target can die (LowHP).
     * + Removed delay setting from Q and set with my personal delay prefference
     *   this just confused to many people.
     * 
     * Revision 0981: 18/10/2014
     * + Fixed major late game error when arround post level 17/18
     *   where the combo/assembly would just spaz out do nothing
     * + No longer uses items in jungle
     *   
     * Revision 098: 18/10/2014
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

        public KurisuRiven()
        {
            Console.WriteLine("KurisuRiven is loaded!");
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static Menu _config;
        private static Obj_AI_Hero _target;
        private static TargetSelector _ts;
        private static readonly Obj_AI_Hero _player = ObjectManager.Player;
        private static Orbwalking.Orbwalker _orbwalker;

        private static int _edelay;
        private static int _qdelay;
        private static int _runiccount;
        private static int _cleavecount;

        private static double _ritems;
        private static double _ua, _uq, _uw;
        private static double _ra, _rq, _rw, _rr, _ri;
        private static float _truerange;

        private static readonly Spell _q = new Spell(SpellSlot.Q, 280f);
        private static readonly Spell _w = new Spell(SpellSlot.W, 260f);
        private static readonly Spell _e = new Spell(SpellSlot.E, 390f);
        private static readonly Spell _r = new Spell(SpellSlot.R, 900f);

        private static double _now;
        private static double _autokill;
        private static readonly double qincrement = TimeSpan.FromMilliseconds(1700).TotalSeconds;
        private static readonly double eincrement = TimeSpan.FromMilliseconds(300).TotalSeconds;

        private static readonly int[] _items = { 3144, 3153, 3142, 3112 };
        private static readonly int[] _runicpassive =
        {
            20, 20, 25, 25, 25, 30, 30, 30, 35, 35, 35, 40, 40, 40, 45, 45, 45, 50, 50
        };

        private static readonly string[] JungleMinions =
        {
            "AncientGolem", "GreatWraith", "Wraith", "LizardElder", "Golem", "Worm", "Dragon", "GiantWolf" 
        
        };

        /// <summary>
        /// Riven Menu (On Game Load)
        /// </summary>
        /// <param name="args"></param>
        private void Game_OnGameLoad(EventArgs args)
        {
            _ts = new TargetSelector(_r.Range, TargetSelector.TargetingMode.NearMouse);
            Game.PrintChat("Riven: Loaded! Revision: 0985-2");
            Game.PrintChat("Riven: If you have any questions/concerns contact me on IRC/Forums. Would happily accept donations -- xrobinsong@gmail.com");
            Game.OnGameUpdate += Game_OnGameUpdate;
            Game.OnGameProcessPacket += Game_OnGameProcessPacket;
            Drawing.OnDraw += Game_OnDraw;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;

            _config = new Menu("KurisuRiven", "kriven", true);

            Menu menuOrb = new Menu("Orbwalker", "orbwalker");
            _orbwalker = new Orbwalking.Orbwalker(menuOrb);
            _config.AddSubMenu(menuOrb);

            Menu menuK = new Menu("Keybind Settings: ", "ksettings");
            menuK.AddItem(new MenuItem("combokey", "Combo")).SetValue(new KeyBind(32, KeyBindType.Press));
            menuK.AddItem(new MenuItem("clearkey", "Jungleclear")).SetValue(new KeyBind(86, KeyBindType.Press));
            _config.AddSubMenu(menuK);

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
            menuC.AddItem(new MenuItem("cancelanim", "Q Cancel type: ")).SetValue(new StringList(new[] { "Move", "Packet", "Delay" }));
            _config.AddSubMenu(menuC);

            Menu menuO = new Menu("Extra Settings: ", "osettings");
            menuO.AddItem(new MenuItem("useignote", "Use Ignite (Works)")).SetValue(true);
            menuO.AddItem(new MenuItem("useautow", "Enable auto W")).SetValue(true);
            menuO.AddItem(new MenuItem("autow", "Auto W min targets")).SetValue(new Slider(3, 1, 5));
            menuO.AddItem(new MenuItem("useautows", "Enable auto Windslash")).SetValue(true);
            menuO.AddItem(new MenuItem("autows", "Windslash if damage dealt %")).SetValue(new Slider(65, 1));
            menuO.AddItem(new MenuItem("autows2", "Windslash if targets hit >=")).SetValue(new Slider(3, 2, 5));
            menuO.AddItem(new MenuItem("blockanim", "Block Q animimation (fun)")).SetValue(false);
            _config.AddSubMenu(menuO);

            Menu menuJ = new Menu("Farm/Clear Settings: ", "jsettings");
            menuJ.AddItem(new MenuItem("jungleE", "Use E (Jungle)")).SetValue(true);
            menuJ.AddItem(new MenuItem("jungleW", "Use W (Jungle)")).SetValue(true);
            _config.AddSubMenu(menuJ);

            _config.AddItem(new MenuItem("tsmode", "Riven Mode:"))
                .SetValue(
                    new StringList(new[] { "LowHP", "MostAD", "MostAP", "Closest", "NearMouse", "LessAttack", "LessCast" }));
            _config.AddToMainMenu();

            _r.SetSkillshot(0.25f, 300f, 120f, false, SkillshotType.SkillshotCone);
        }


        /// <summary>
        /// Riven Game Update
        /// </summary>
        /// <param name="args"></param>
        private void Game_OnGameUpdate(EventArgs args)
        {
            try
            {
                _now = TimeSpan.FromMilliseconds(Environment.TickCount).TotalSeconds;
                bool _combo = _config.Item("combokey").GetValue<KeyBind>().Active;
                bool _jungle = _config.Item("clearkey").GetValue<KeyBind>().Active;

                _target = _ts.Target;
                
                SetTarget();

                if (_combo|| _autokill + qincrement > _now)
                {
                    if (_player.IsStunned || _player.IsRooted || _player.IsImmovable)
                        return;
                    CastCombo(_target);
                }

                if (_jungle)
                {
                    var target = _orbwalker.GetTarget();
                    if (target != null && JungleMinions.Any(name => target.Name.StartsWith(name) && target.IsValid && target.IsVisible))
                    {
                        if (!_e.IsReady() || !_config.Item("jungleE").GetValue<bool>()) return;
                        _e.Cast(Game.CursorPos);

                        if (!_w.IsReady() || !_config.Item("jungleW").GetValue<bool>()) return;

                        if (target.Distance(_player.Position) < _w.Range)
                            _w.Cast();
                    }
                }

                RefreshBuffs();

                if (_player.IsStunned)
                    return;

                AutoW();
                WindSlash();   
                
            }
            catch (Exception ex)
            {
                Console.WriteLine("Msg: " + ex.Message);
                Console.WriteLine("Raw: " + ex);
            }
        }


        /// <summary>
        /// Riven Drawings
        /// </summary>
        /// <param name="args"></param>
        private void Game_OnDraw(EventArgs args)
        {

            if (_config.Item("drawaa").GetValue<bool>() && !_player.IsDead)
                Utility.DrawCircle(_player.Position, _player.AttackRange + 25, Color.White, 1, 1);
            if (_config.Item("drawp").GetValue<bool>() && !_player.IsDead)
            {
                var wts = Drawing.WorldToScreen(_player.Position);
                Drawing.DrawText(wts[0] - 35, wts[1] + 30, Color.White, "Passive: " + _runiccount);
                Drawing.DrawText(wts[0] - 35, wts[1] + 10, Color.White, "Q: " + _cleavecount);
            }
            if (_config.Item("debugtrue").GetValue<bool>())
            {
                if (!_player.IsDead)
                {
                    Utility.DrawCircle(_player.Position, _truerange + 25, Color.Yellow, 1, 1);
                }
            }

            if (_config.Item("drawt").GetValue<bool>())
            {
                if (_target != null && !_target.IsDead && !_player.IsDead)
                {
                    Utility.DrawCircle(_target.Position, _target.BoundingRadius, Color.Red, 1, 1);
                    Utility.DrawCircle(_target.Position, _player.AttackRange + _e.Range, Color.Red, 1, 1);
                }
            }
            if (_config.Item("drawkill").GetValue<bool>())
            {
                if (_target != null && !_target.IsDead && !_player.IsDead)
                {
                    var ts = _target;
                    var wts = Drawing.WorldToScreen(_target.Position);
                    if ((float) (_ra + _rq*2 + _rw + _ri + _ritems) > ts.Health) 
                        Drawing.DrawText(wts[0] - 20, wts[1] + 40, Color.OrangeRed, "Kill!");
                    else if ((float) (_ra*2 + _rq*2 + _rw + _ritems) > ts.Health)
                        Drawing.DrawText(wts[0] - 40, wts[1] + 40, Color.OrangeRed, "Easy Kill!");
                    else if ((float) (_ua*3 + _uq*2 + _uw + _ri + _rr + _ritems) > ts.Health) 
                        Drawing.DrawText(wts[0] - 40, wts[1] + 40, Color.OrangeRed, "Full Combo Kill!");
                    else if ((float) (_ua*3 + _uq*3 + _uw + _rr + _ri + _ritems) > ts.Health) 
                        Drawing.DrawText(wts[0] - 40, wts[1] + 40, Color.OrangeRed, "Full Combo Hard Kill!");
                    else if ((float) (_ua*3 + _uq*3 + _uw + _rr + _ri + _ritems) < ts.Health) 
                        Drawing.DrawText(wts[0] - 40, wts[1] + 40, Color.OrangeRed, "Cant Kill!");

                }
            }
            if (_config.Item("debugdmg").GetValue<bool>())
            {
                if (_target != null && !_target.IsDead && !_player.IsDead)
                {
                    var wts = Drawing.WorldToScreen(_target.Position);
                    if (!_r.IsReady())
                        Drawing.DrawText(wts[0] - 75, wts[1] + 60, Color.Orange,
                            "Combo Damage: " + (float) (_ra*3 + _rq*3 + _rw + _rr + _ri + _ritems));
                    else
                        Drawing.DrawText(wts[0] - 75, wts[1] + 60, Color.Orange,
                            "Combo Damage: " + (float) (_ua*3 + _uq*3 + _uw + _rr + _ri + _ritems));
                }
            }
        }



        /// <summary>
        /// Riven On Process Spell
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            Obj_AI_Hero target = _target;
            bool _ultion = _player.HasBuff("RivenFengShuiEngine", true);
            bool _combo = _config.Item("combokey").GetValue<KeyBind>().Active;
            int _wslash = _config.Item("wslash").GetValue<StringList>().SelectedIndex;

            if (!sender.IsMe || _player.IsStunned) return;
            switch (args.SData.Name)
            {
                case "RivenTriCleave":
                    _qdelay = Environment.TickCount;
                    break;
                case "RivenMartyr":
                    Orbwalking.LastAATick = 0;
                    break;
                case "ItemTiamatCleave":
                    Orbwalking.LastAATick = 0;
                    Utility.DelayAction.Add(Game.Ping + 75, () => UseItems(target));
                    if (_w.IsReady() && _player.Distance(target.Position) < _w.Range && (_combo || _autokill + qincrement > _now))
                        Utility.DelayAction.Add(Game.Ping + 75, () => _w.Cast());
                    break;
                case "RivenFeint":                   
                    _edelay = Environment.TickCount;
                    if (_combo || _autokill + qincrement > _now) Utility.DelayAction.Add(Game.Ping + 50, () => UseItems(target));
                    Utility.DelayAction.Add(Game.Ping + 125, delegate
                    {
                        if ((_combo || _autokill + qincrement > _now)  && target.Distance(_player.Position) < _w.Range)
                        {
                            if (Items.HasItem(3077) && Items.CanUseItem(3077))
                                Items.UseItem(3077);
                            if (Items.HasItem(3074) && Items.CanUseItem(3074))
                                Items.UseItem(3074);
                        }
                    });
                    Orbwalking.LastAATick = 0;
                    if (_r.IsReady() && _ultion && _wslash == 1 && _config.Item("useblade").GetValue<bool>())
                    {
                        if (_cleavecount == 2)
                            _r.Cast(target.Position, true);
                    }
                    break;
                case "RivenFengShuiEngine":
                    Orbwalking.LastAATick = 0;
                    break;
                case "rivenizunablade":
                    if (_q.IsReady() && (_combo || _autokill + qincrement > _now))
                        _q.Cast(target.Position, true);
                    break;

            }

        }

        /// <summary>
        /// Riven on Process Packet
        /// </summary>
        /// <param name="args"></param>
        private void Game_OnGameProcessPacket(GamePacketEventArgs args)
        {
            bool combo = _config.Item("combokey").GetValue<KeyBind>().Active;
            bool clear = _config.Item("clearkey").GetValue<KeyBind>().Active;

            _truerange = _player.AttackRange + _player.Distance(_player.BBox.Minimum) + 1;

            GamePacket packet = new GamePacket(args.PacketData);

            if (_player.IsStunned) return;
            if (packet.Header == 176) 
            {
                packet.Position = 1;
                if (packet.ReadInteger() == _player.NetworkId && _config.Item("blockanim").GetValue<bool>())
                    args.Process = false;
            }

            if (packet.Header == 101 && (combo ||  _autokill + qincrement > _now))
            {
                packet.Position = 16;
                int sourceId = packet.ReadInteger();

                packet.Position = 1;
                int targetId = packet.ReadInteger();
                int dmgType = packet.ReadByte();

                Obj_AI_Hero trueTarget = ObjectManager.GetUnitByNetworkId<Obj_AI_Hero>(targetId);
                if (sourceId == _player.NetworkId && (dmgType == 4 || dmgType == 3) && _q.IsReady())
                {
                    UseItems(trueTarget);
                    _q.Cast(trueTarget.Position, true);
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
                    JungleMinions.Any(name => trueTarget.Name.StartsWith(name)) && _q.IsReady())
                {
                    _q.Cast(trueTarget.Position, true);
                }

            }

            if (packet.Header == 56 && packet.Size() == 9 && (combo ||  _autokill + qincrement > _now))
            {
                packet.Position = 1;
                int sourceId = packet.ReadInteger();
                if (sourceId == _player.NetworkId)
                {
                    int targetId = _orbwalker.GetTarget().NetworkId;
                    int method = _config.Item("cancelanim").GetValue<StringList>().SelectedIndex;

                    Obj_AI_Hero truetarget = ObjectManager.GetUnitByNetworkId<Obj_AI_Hero>(targetId);
                    if (_player.Distance(_orbwalker.GetTarget().Position) <= _truerange + 25 && Orbwalking.Move)
                    {
                        Vector3 movePos = truetarget.Position + _player.Position -
                                         Vector3.Normalize(_player.Position)*(_player.Distance(truetarget.Position) + 57);

                        switch (method)
                        {
                            case 1:
                                Packet.C2S.Move.Encoded(new Packet.C2S.Move.Struct(movePos.X, movePos.Y, 3, _orbwalker.GetTarget().NetworkId)).Send();
                                Orbwalking.LastAATick = 0;
                                break;
                            case 0:
                                _player.IssueOrder(GameObjectOrder.MoveTo, new Vector3(movePos.X, movePos.Y, movePos.Z));
                                Orbwalking.LastAATick = 0;
                                break;
                            case 2:                               
                                Utility.DelayAction.Add(75, delegate
                                {
                                    _player.IssueOrder(GameObjectOrder.MoveTo, new Vector3(movePos.X, movePos.Y, movePos.Z));
                                    Orbwalking.LastAATick = 0;
                                });
                                break;
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
    
                    if (_player.Distance(_orbwalker.GetTarget().Position) <= _truerange + 25 && Orbwalking.Move)
                    {
                        Vector3 movePos = truetarget.Position + _player.Position -
                                          Vector3.Normalize(_player.Position)*(_player.Distance(truetarget.Position) + 63);

                        if (JungleMinions.Any(name => truetarget.Name.StartsWith(name)))
                        {
                            Packet.C2S.Move.Encoded(new Packet.C2S.Move.Struct(movePos.X, movePos.Y, 3, _orbwalker.GetTarget().NetworkId)).Send();
                            Utility.DelayAction.Add(Game.Ping + 70, () => Orbwalking.LastAATick = 0);
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

        /// <summary>
        /// Buff Manager
        /// </summary>
        private void RefreshBuffs()
        {         
            var buffs = _player.Buffs;

            foreach (var b in buffs)
            {
                if (b.Name == "rivenpassiveaaboost")
                    _runiccount = b.Count;
                if (b.Name == "RivenTriCleave")
                    _cleavecount = b.Count;
            }

            if (!_player.HasBuff("rivenpassiveaaboost", true))
                _runiccount = 0;
            if (!_q.IsReady())
                _cleavecount = 0;
        }

        /// <summary>
        /// Riven Combo Logic
        /// </summary>
        /// <param name="target"></param>
        private void CastCombo(Obj_AI_Base target)
        {
            bool _ultion = _player.HasBuff("RivenFengShuiEngine", true);
            if (target != null && target.IsValid && target.IsVisible)
            {
                if (_player.Distance(target.Position) > _truerange + 25 ||
                    ((_player.Health/_player.MaxHealth)*100) <= 45) 
                {
                    if (_e.IsReady() && _config.Item("usevalor").GetValue<bool>())
                        _e.Cast(target.Position);
                    if (_config.Item("waitvalor").GetValue<bool>())
                        CheckR(target);
                }

                if (_w.IsReady() && _q.IsReady() && _e.IsReady() 
                    && target.Distance(_player.Position) < _w.Range)
                {
                    CheckR(target);
                    _w.Cast();
                }

                if (_r.IsReady() && _e.IsReady() && _ultion) 
                {
                    if (_cleavecount == 2)
                        _e.Cast(target.Position);
                }

                if (_w.IsReady() && !_e.IsReady() && (!Items.HasItem(3074) || !Items.CanUseItem(3074)) &&
                    (!Items.HasItem(3077) || !Items.CanUseItem(3077)))
                {
                    if (target.Distance(_player.Position) < _w.Range)
                        _w.Cast();
                }

                if (_q.IsReady() && !_e.IsReady() && _player.Distance(target.Position) > _q.Range && target.IsValid)
                {
                    if (TimeSpan.FromMilliseconds(_qdelay).TotalSeconds + qincrement < _now &&
                        TimeSpan.FromMilliseconds(_edelay).TotalSeconds + eincrement < _now)
                    {
                        _q.Cast(target.Position, true);
                    }
                }
            }
        }


        /// <summary>
        /// Riven Windslash & More
        /// </summary>
        private static void WindSlash()
        {
            bool _ultion = _player.HasBuff("RivenFengShuiEngine", true);
            int wsneed = _config.Item("autows").GetValue<Slider>().Value;
            int wslash = _config.Item("wslash").GetValue<StringList>().SelectedIndex;

            if (!_config.Item("useblade").GetValue<bool>()) return;
            foreach (
                var e in
                    ObjectManager.Get<Obj_AI_Hero>()
                        .Where(
                            e =>
                                e.Team != _player.Team && e.IsValid && !e.IsDead && !e.IsInvulnerable &&
                                e.Distance(_player.Position) < _r.Range))
            {
                CheckDamage(e);
                PredictionOutput rPos = _r.GetPrediction(e, true);
                if (_r.IsReady() && rPos.Hitchance >= HitChance.Medium && _ultion)
                {
                    if (_config.Item("useautows").GetValue<bool>() && wslash == 1)
                    {
                        if (rPos.AoeTargetsHitCount >= _config.Item("autows2").GetValue<Slider>().Value)
                            _r.Cast(rPos.CastPosition, true);
                        else if (_rr/e.MaxHealth*100 > e.Health/e.MaxHealth*wsneed) 
                        {
                            _r.Cast(rPos.CastPosition);
                        }
                        else if (e.Health < _rr + _ra*2 + _rq*1) 
                            _r.Cast(rPos.CastPosition);
                    }
                    
                    else if (e.Health < _rr)
                        _r.Cast(rPos.CastPosition, true);
                }

                if (_q.IsReady() && e.Health < _rq && _player.Distance(_target.Position) < _q.Range)
                    _q.Cast(_target.Position, true);
                else if (_q.IsReady() && e.Health < _rq + _ra * 2 + _ri && _player.Distance(_target.Position) < _q.Range)
                {
                    _target = e;
                    _autokill = TimeSpan.FromMilliseconds(Environment.TickCount).TotalSeconds;

                }
                else if (_q.IsReady() && e.Health < _rq*2 + _ri && _player.Distance(_target.Position) < _q.Range) 
                {
                    _target = e;
                    _autokill = TimeSpan.FromMilliseconds(Environment.TickCount).TotalSeconds;
                }

            }
        }
      
        /// <summary>
        /// Item Handler
        /// </summary>
        /// <param name="target"></param>
        private static void UseItems(Obj_AI_Base target)
        {
            foreach (var i in _items.Where(i => Items.CanUseItem(i) && Items.HasItem(i)))
            {
                if (_player.Distance(target, true) <= _q.Range*_q.Range) 
                    Items.UseItem(i, target);
            }
        }

        /// <summary>
        /// Damage Handler
        /// </summary>
        /// <param name="target"></param>
        private static void CheckDamage(Obj_AI_Base target)
        {
            if (target != null)
            {

                //int count = _runiccount;
                SpellSlot ignite = _player.GetSpellSlot("summonerdot");

                //if (count == 0) count = 1;
                double AA = _player.GetAutoAttackDamage(target);

                double TMT = Items.HasItem(3077) && Items.CanUseItem(3077) ? _player.GetItemDamage(target, Damage.DamageItems.Tiamat) : 0;
                double HYD = Items.HasItem(3074) && Items.CanUseItem(3074) ? _player.GetItemDamage(target, Damage.DamageItems.Hydra) : 0;
                double BWC = Items.HasItem(3144) && Items.CanUseItem(3144) ? _player.GetItemDamage(target, Damage.DamageItems.Bilgewater) : 0;
                double BRK = Items.HasItem(3153) && Items.CanUseItem(3153) ? _player.GetItemDamage(target, Damage.DamageItems.Botrk) : 0;

                _rr = _player.GetSpellDamage(target, SpellSlot.R);
                _ra = AA + (AA * (_runicpassive[_player.Level] / 100));
                _rq = _q.IsReady() ? DamageQ(target) : 0;
                _rw = _w.IsReady() ? _player.GetSpellDamage(target, SpellSlot.W) : 0;
                _ri = _player.SummonerSpellbook.CanUseSpell(ignite) == SpellState.Ready ? _player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite) : 0;

                _ritems = TMT + HYD + BWC + BRK;

                if (_r.IsReady())
                {
                    _ua = _ra + _player.CalcDamage(target, Damage.DamageType.Physical, _player.BaseAttackDamage+_player.FlatPhysicalDamageMod*0.2);
                    _uq = _rq + _player.CalcDamage(target, Damage.DamageType.Physical, _player.BaseAttackDamage+_player.FlatPhysicalDamageMod*0.2*0.7);
                    _uw = _rw + _player.CalcDamage(target, Damage.DamageType.Physical, _player.BaseAttackDamage+_player.FlatPhysicalDamageMod*0.2*1);
                    _rr = _rr + _player.CalcDamage(target, Damage.DamageType.Physical, _player.BaseAttackDamage+_player.FlatPhysicalDamageMod*0.2);
                }
                else
                {
                    _ua = _ra;
                    _uq = _rq;
                    _uw = _rw;
                }
            }
        }

        /// <summary>
        /// Gets the Q damage because DamageLib  is broken
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static float DamageQ(Obj_AI_Base target)
        {
            double dmg = 0;
            if (_q.IsReady())
            {
                dmg += _player.CalcDamage(_player, Damage.DamageType.Physical,
                    -10 + (_q.Level*20) +
                    (0.35 + (_q.Level*0.05))*(_player.FlatPhysicalDamageMod + _player.BaseAttackDamage));
            }

            return (float)dmg;
        }


        /// <summary>
        /// Ultimate Handler
        /// </summary>
        /// <param name="target"></param>
        private void CheckR(Obj_AI_Base target)
        {
            bool _utlion = _player.HasBuff("RivenFengShuiEngine", true);
            StringList index = _config.Item("bladewhen").GetValue<StringList>();

            // so we dont auto cast R when killsteal(autokill) takes over
            bool combo = _config.Item("combokey").GetValue<KeyBind>().Active;

            if (target != null && target.IsValid && target.Type == _player.Type &&
                _config.Item("useblade").GetValue<bool>() && combo) 
            {
                CheckDamage(target);
                switch (index.SelectedIndex)
                {
                    case 2:
                        if ((float) (_ua*3 + _uq*3 + _uw + _rr + _ri + _ritems) > target.Health && !_utlion) 
                        {
                            _r.Cast();
                            if (_config.Item("useignote").GetValue<bool>())
                                CastIgnite(target);
                        }
                        break;
                    case 1:
                        if ((float) (_ra*3 + _rq*3 + _rw + _rr + _ri + _ritems) > target.Health && !_utlion) 
                        {
                            _r.Cast();
                            if (_config.Item("useignote").GetValue<bool>() && _q.IsReady() )
                                CastIgnite(target);
                        }
                        break;
                    case 0:
                        if ((float) (_ra*2 + _rq*2 + _rw + _rr + _ri + _ritems) > target.Health && !_utlion) 
                        {
                            _r.Cast();
                            if (_config.Item("useignote").GetValue<bool>() && _q.IsReady())
                                CastIgnite(target);
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Ignite Handler
        /// </summary>
        /// <param name="target"></param>
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

        /// <summary>
        /// Ki Burst (W) when multiple in Rnage
        /// </summary>
        private void AutoW()
        {
            var getenemies =
                ObjectManager.Get<Obj_AI_Hero>()
                    .Where(
                        en =>
                            en.Team != _player.Team && en.IsValid && !en.IsDead &&
                            en.Distance(_player.Position) < _w.Range);
            if (getenemies.Count() >= _config.Item("autow").GetValue<Slider>().Value)
            {
                if (_w.IsReady() && _config.Item("useautow").GetValue<bool>())
                {
                    _w.Cast();
                }
            }

        }

        /// <summary>
        /// Set our target select mode
        /// </summary>
        private void SetTarget()
        {
            switch (_config.Item("tsmode").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    _ts.SetTargetingMode(TargetSelector.TargetingMode.LowHP);
                    break;
                case 1:
                    _ts.SetTargetingMode(TargetSelector.TargetingMode.MostAD);
                    break;
                case 2:
                    _ts.SetTargetingMode(TargetSelector.TargetingMode.MostAP);
                    break;
                case 3:
                    _ts.SetTargetingMode(TargetSelector.TargetingMode.Closest);
                    break;
                case 4:
                    _ts.SetTargetingMode(TargetSelector.TargetingMode.NearMouse);
                    break;
                case 5:
                    _ts.SetTargetingMode(TargetSelector.TargetingMode.LessAttack);
                    break;
                case 6:
                    _ts.SetTargetingMode(TargetSelector.TargetingMode.LessCast);
                    break;
            }
        }
    }
}
