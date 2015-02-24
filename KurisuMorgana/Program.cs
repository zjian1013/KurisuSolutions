using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

namespace KurisuMorgana
{
    internal class Program
    {
        private static Menu _menu;
        private static Spell _q, _w, _e, _r;
        private static Orbwalking.Orbwalker _orbwalker;
        private static readonly Obj_AI_Hero Me = ObjectManager.Player;

        static void Main(string[] args)
        {
            Console.WriteLine("Morgana injected...");
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            if (Me.ChampionName != "Morgana")
                return;

            // set spells
            _q = new Spell(SpellSlot.Q, 1175f);
            _q.SetSkillshot(0.25f, 72f, 1400f, true, SkillshotType.SkillshotLine);

            _w = new Spell(SpellSlot.W, 900f);
            _w.SetSkillshot(0.25f, 175f, 1200f, false, SkillshotType.SkillshotCircle);

            _e = new Spell(SpellSlot.E, 750f);
            _r = new Spell(SpellSlot.R, 600f);

            _menu = new Menu("KurisuMorgana", "morgana", true);

            var orbmenu = new Menu("Orbwalker", "orbwalker");
            _orbwalker = new Orbwalking.Orbwalker(orbmenu);
            _menu.AddSubMenu(orbmenu);

            var tsmenu = new Menu("Selector", "selector");
            TargetSelector.AddToMenu(tsmenu);
            _menu.AddSubMenu(tsmenu);

            var drmenu = new Menu("Drawings", "drawings");
            drmenu.AddItem(new MenuItem("drawq", "Draw Q")).SetValue(true);
            drmenu.AddItem(new MenuItem("draww", "Draw W")).SetValue(true);
            drmenu.AddItem(new MenuItem("drawe", "Draw E")).SetValue(true);
            drmenu.AddItem(new MenuItem("drawr", "Draw R")).SetValue(true);
            drmenu.AddItem(new MenuItem("drawkill", "Draw killable")).SetValue(true);
            drmenu.AddItem(new MenuItem("drawtarg", "Draw target circle")).SetValue(true);
            drmenu.AddItem(new MenuItem("debugdmg", "Debug combo damage")).SetValue(false);
            _menu.AddSubMenu(drmenu);

            var spellmenu = new Menu("Spells", "spells");

            var menuQ = new Menu("Q Menu", "qmenu");
            menuQ.AddItem(new MenuItem("hitchanceq", "Binding Hitchance ")).SetValue(new Slider(3, 1, 4));
            menuQ.AddItem(new MenuItem("useqcombo", "Use in combo")).SetValue(true);
            menuQ.AddItem(new MenuItem("useharassq", "Use in harass")).SetValue(true);
            menuQ.AddItem(new MenuItem("useqanti", "Use on gapcloser")).SetValue(true);
            menuQ.AddItem(new MenuItem("useqauto", "Use on immobile")).SetValue(true);
            menuQ.AddItem(new MenuItem("useqdash", "Use on dashing")).SetValue(true);
            spellmenu.AddSubMenu(menuQ);

            var menuW = new Menu("W Menu", "wmenu");
            menuW.AddItem(new MenuItem("hitchancew", "Tormentsoil Hitchance ")).SetValue(new Slider(2, 1, 4));
            menuW.AddItem(new MenuItem("usewcombo", "Use in combo")).SetValue(true);
            menuW.AddItem(new MenuItem("useharassw", "Use in harass")).SetValue(true);       
            menuW.AddItem(new MenuItem("usewauto", "Use on immobile")).SetValue(true);
            menuW.AddItem(new MenuItem("waitfor", "Wait for bind or immobile")).SetValue(true);
            menuW.AddItem(new MenuItem("calcw", "Calculated ticks")).SetValue(new Slider(3, 1, 5));
            spellmenu.AddSubMenu(menuW);

            var menuR = new Menu("R Menu", "rmenu");
            menuR.AddItem(new MenuItem("usercombo", "Enable")).SetValue(true);
            menuR.AddItem(new MenuItem("rkill", "Use in combo if killable")).SetValue(true);
            menuR.AddItem(new MenuItem("rcount", "Use in combo if enemies >= ")).SetValue(new Slider(3, 1, 5));
            menuR.AddItem(new MenuItem("useautor", "Use automatic if enemies >= ")).SetValue(new Slider(4, 2, 5));
            spellmenu.AddSubMenu(menuR);

            spellmenu.AddItem(new MenuItem("harassmana", "Harass Mana %")).SetValue(new Slider(55, 0, 99));
            _menu.AddSubMenu(spellmenu);

            var menuM = new Menu("Misc", "morgmisc");
            foreach (var obj in ObjectManager.Get<Obj_AI_Hero>().Where(obj => obj.Team != Me.Team))
            {
                menuM.AddItem(new MenuItem("dobind" + obj.ChampionName, obj.ChampionName))
                    .SetValue(new StringList(new[] { "Dont Bind ", "Normal Bind ", "Auto Bind" }, 1));
            }

            _menu.AddSubMenu(menuM);

            _menu.AddToMainMenu();

            Game.PrintChat("<font color=\"#FF9900\"><b>KurisuMorgana:</b></font> Loaded");
            Game.PrintChat("<b>Oracle#</b> is <b>recommended</b> for advance spell shield support!");

            // events
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnGameUpdate += Game_OnGameUpdate;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (!Me.IsValidTarget(300, false))
            {
                return;
            }

            CheckDamage(TargetSelector.GetTarget(_q.Range + 10, TargetSelector.DamageType.Magical));


            AutoCast(_menu.Item("useqdash").GetValue<bool>(),
                     _menu.Item("useqauto").GetValue<bool>(),
                     _menu.Item("usewauto").GetValue<bool>());

            if (_orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                Combo(_menu.Item("useqcombo").GetValue<bool>(),
                      _menu.Item("usewcombo").GetValue<bool>(), 
                      _menu.Item("usercombo").GetValue<bool>());
            }

            if (_orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                Harass(_menu.Item("useqcombo").GetValue<bool>(),
                       _menu.Item("usewcombo").GetValue<bool>());
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Me.IsValidTarget(300, false))
            {
                var ticks = _menu.Item("calcw").GetValue<Slider>().Value;
                Render.Circle.DrawCircle(Me.Position, Me.BoundingRadius - 50, System.Drawing.Color.White, 3);

                if (_menu.Item("drawq").GetValue<bool>())
                    Render.Circle.DrawCircle(Me.Position, _q.Range + 10, System.Drawing.Color.White, 3);
                if (_menu.Item("draww").GetValue<bool>())
                    Render.Circle.DrawCircle(Me.Position, _w.Range, System.Drawing.Color.White, 3);
                if (_menu.Item("drawe").GetValue<bool>())
                    Render.Circle.DrawCircle(Me.Position, 750f, System.Drawing.Color.White, 3);
                if (_menu.Item("drawr").GetValue<bool>())
                    Render.Circle.DrawCircle(Me.Position, _r.Range + 10, System.Drawing.Color.White, 3);

                var target = TargetSelector.GetTarget(_q.Range + 10, TargetSelector.DamageType.Magical);
                if (target.IsValidTarget(_q.Range + 10))
                {
                    if (_menu.Item("drawtarg").GetValue<bool>())
                    {
                        Render.Circle.DrawCircle(target.Position, target.BoundingRadius - 50, System.Drawing.Color.Yellow, 6);                       
                    }

                    if (_menu.Item("drawkill").GetValue<bool>())
                    {
                        var wts = Drawing.WorldToScreen(target.Position);
                        if (_ma*3 + _mi + _mq + _guise >= target.Health)
                            Drawing.DrawText(wts[0] - 20, wts[1] + 20, System.Drawing.Color.LimeGreen, "Q Kill!");
                        else if (_ma*3 + _mi + _mw * ticks + _guise >= target.Health)
                            Drawing.DrawText(wts[0] - 20, wts[1] + 20, System.Drawing.Color.LimeGreen, "W Kill!");
                        else if (_mq + _mw * ticks + _ma * 3 + _mi + _guise >= target.Health)
                            Drawing.DrawText(wts[0] - 20, wts[1] + 20, System.Drawing.Color.LimeGreen, "Q + W Kill!");
                        else if (_mq + _mw * ticks + _ma * 3 + _mi + _mr + _guise >= target.Health)
                            Drawing.DrawText(wts[0] - 20, wts[1] + 20, System.Drawing.Color.LimeGreen, "Q + R + W Kill!");
                        else if (_mq + _mw * ticks + _ma * 3 + _mr + _mi + _guise < target.Health)
                            Drawing.DrawText(wts[0] - 20, wts[1] + 20, System.Drawing.Color.LimeGreen, "Cant Kill");
                    }

                    if (_menu.Item("debugdmg").GetValue<bool>())
                    {
                        var wts = Drawing.WorldToScreen(target.Position);
                        Drawing.DrawText(wts[0] - 75, wts[1] + 40, System.Drawing.Color.Yellow,
                                "Combo Damage: " + (float)(_ma * 3 + _mq + _mw * ticks + _mi + _mr + _guise));
                    }
                }
            }
        }

        private static void Combo(bool useq, bool usew, bool user)
        {
            if (useq && _q.IsReady())
            {
                var qtarget = TargetSelector.GetTarget(_q.Range + 10, TargetSelector.DamageType.Magical);
                if (qtarget.IsValidTarget(_q.Range + 10))
                {
                    var poutput = _q.GetPrediction(qtarget);
                    if (poutput.Hitchance >= (HitChance) _menu.Item("hitchanceq").GetValue<Slider>().Value + 2)
                    {
                        _q.Cast(poutput.CastPosition);
                    }
                }
            }

            if (usew && _w.IsReady())
            {             
                var wtarget = TargetSelector.GetTarget(_w.Range + 10, TargetSelector.DamageType.Magical);            
                if (wtarget.IsValidTarget(_w.Range))
                {
                    var poutput = _w.GetPrediction(wtarget);
                    if (poutput.Hitchance >= (HitChance)_menu.Item("hitchancew").GetValue<Slider>().Value + 2)
                    {
                        if (!_menu.Item("waitfor").GetValue<bool>() ||
                            _mw*_menu.Item("calcw").GetValue<Slider>().Value >= wtarget.Health)
                        {
                            _w.Cast(poutput.CastPosition);
                        }
                    }                  
                }
            }

            if (user && _r.IsReady())
            {
                var ticks = _menu.Item("calcw").GetValue<Slider>().Value;
                var rtarget = TargetSelector.GetTarget(_r.Range, TargetSelector.DamageType.Magical);
                if (rtarget.IsValidTarget(_r.Range))
                {
                    if (_mr + _mq + _mw * ticks + _ma * 3 + _mi + _guise >= rtarget.Health)
                    {
                        if (_menu.Item("rkill").GetValue<bool>())
                        {
                            if (_e.IsReady())
                                _e.CastOnUnit(Me);
                            _r.Cast();
                        }
                    }

                    if (Me.CountEnemiesInRange(_r.Range) >= _menu.Item("rcount").GetValue<Slider>().Value)
                    {
                        if (_e.IsReady())
                            _e.CastOnUnit(Me);

                        _r.Cast();
                    }
                }
            }
        }

        private static void Harass(bool useq, bool usew)
        {
            if (useq && _q.IsReady())
            {
                var qtarget = TargetSelector.GetTarget(_q.Range - 300, TargetSelector.DamageType.Magical);
                if (qtarget.IsValidTarget(_q.Range - 300))
                {
                    var poutput = _q.GetPrediction(qtarget);
                    if (poutput.Hitchance >= (HitChance)_menu.Item("hitchanceq").GetValue<Slider>().Value + 2)
                    {
                        if ((int)(Me.Mana / Me.MaxMana * 100) >= _menu.Item("harassmana").GetValue<Slider>().Value)
                            _q.Cast(poutput.CastPosition);
                    }
                }
            }

            if (usew && _w.IsReady())
            {
                var wtarget = TargetSelector.GetTarget(_w.Range + 10, TargetSelector.DamageType.Magical);
                if (wtarget.IsValidTarget(_w.Range))
                {
                    var poutput = _w.GetPrediction(wtarget);
                    if (poutput.Hitchance >= (HitChance)_menu.Item("hitchancew").GetValue<Slider>().Value + 2)
                    {
                        if ((int)(Me.Mana / Me.MaxMana * 100) >= _menu.Item("harassmana").GetValue<Slider>().Value)
                            _w.Cast(poutput.CastPosition);
                    }
                }           
            }
        }

        private static void AutoCast(bool dashing, bool immobile, bool soil)
        {
            if (_q.IsReady())
            {
                var itarget =
                    ObjectManager.Get<Obj_AI_Hero>()
                        .FirstOrDefault(h => h.IsEnemy && h.Distance(Me.ServerPosition, true) <= _q.RangeSqr);

                if (itarget.IsValidTarget(_q.Range))
                {
                    if (dashing && _menu.Item("dobind" + itarget.ChampionName).GetValue<StringList>().SelectedIndex == 2)
                        _q.CastIfHitchanceEquals(itarget, HitChance.Dashing);

                    if (immobile && _menu.Item("dobind" + itarget.ChampionName).GetValue<StringList>().SelectedIndex == 2)
                        _q.CastIfHitchanceEquals(itarget, HitChance.Immobile);
                }
            }

            if (_w.IsReady() && soil)
            {
                var itarget =
                    ObjectManager.Get<Obj_AI_Hero>()
                        .FirstOrDefault(h => h.IsEnemy && h.Distance(Me.ServerPosition, true) <= _w.RangeSqr);

                if (itarget.IsValidTarget(_w.Range))
                    _w.CastIfHitchanceEquals(itarget, HitChance.Immobile);          
            }

            if (_r.IsReady())
            {
                if (Me.CountEnemiesInRange(_r.Range) >= _menu.Item("useautor").GetValue<Slider>().Value)
                {
                    if (_e.IsReady())
                        _e.CastOnUnit(Me);
                    _r.Cast();
                }           
            }
        }  

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (gapcloser.Sender.IsValidTarget(_q.Range + 10))
            {
                if (_menu.Item("useqanti").GetValue<bool>())
                {
                    var poutput = _q.GetPrediction(gapcloser.Sender);
                    if (poutput.Hitchance >= HitChance.Low)
                    {
                        _q.Cast(poutput.CastPosition);
                    }
                }
            }
        }

        private static float _mq, _mw, _mr;
        private static float _ma, _mi, _guise;
        private static void CheckDamage(Obj_AI_Base target)
        {
            if (target == null)
            {
                return;
            }

            var qready = Me.Spellbook.CanUseSpell(SpellSlot.Q) == SpellState.Ready;
            var wready = Me.Spellbook.CanUseSpell(SpellSlot.W) == SpellState.Ready;
            var rready = Me.Spellbook.CanUseSpell(SpellSlot.R) == SpellState.Ready;
            var iready = Me.Spellbook.CanUseSpell(Me.GetSpellSlot("summonerdot")) == SpellState.Ready;

            _ma = (float) Me.GetAutoAttackDamage(target);
            _mq = (float) (qready ? Me.GetSpellDamage(target, SpellSlot.Q) : 0);
            _mw = (float) (wready ? Me.GetSpellDamage(target, SpellSlot.W) : 0);
            _mr = (float) (rready ? Me.GetSpellDamage(target, SpellSlot.R) : 0);
            _mi = (float) (iready ? Me.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite) : 0);

            _guise = (float) (Items.HasItem(3151)
                ? Me.GetItemDamage(target, Damage.DamageItems.LiandrysTorment)
                : 0);
        }
    }
}
