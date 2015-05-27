using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;

namespace KurisuBlitz
{
    //  _____ _ _ _                       _   
    // | __  | |_| |_ ___ ___ ___ ___ ___| |_ 
    // | __ -| | |  _|- _|  _|  _| .'|   | '_|
    // |_____|_|_|_| |___|___|_| |__,|_|_|_,_|
    //  Copyright © Kurisu Solutions 2015
   
    internal class Program
    {
        private static Menu _menu;
        private static Spell _q, _e, _r;
        private static Orbwalking.Orbwalker _orbwalker;
        private static readonly Obj_AI_Hero Me = ObjectManager.Player;

        static void Main(string[] args)
        {
            Console.WriteLine("Blitzcrank injected...");
            CustomEvents.Game.OnGameLoad += BlitzOnLoad;
        }

        private static void BlitzOnLoad(EventArgs args)
        {
            if (Me.ChampionName != "Blitzcrank")
                return;
           
            // Set spells      
            _q = new Spell(SpellSlot.Q, 1050f);
            _q.SetSkillshot(250f, 70f, 1800f, true, SkillshotType.SkillshotLine);

            _e = new Spell(SpellSlot.E, 150f);
            _r = new Spell(SpellSlot.R, 550f);

            // Load Menu
            _menu = new Menu("KurisuBlitz", "blitz", true);

            var blitzTs = new Menu("Selector", "tselect");
            TargetSelector.AddToMenu(blitzTs);
            _menu.AddSubMenu(blitzTs);

            var blitzOrb = new Menu("Orbwalker", "orbwalker");
            _orbwalker = new Orbwalking.Orbwalker(blitzOrb);
            _menu.AddSubMenu(blitzOrb);

            var menuD = new Menu("Drawings", "drawings");
            menuD.AddItem(new MenuItem("drawQ", "Draw Q")).SetValue(new Circle(true, Color.FromArgb(150, Color.White)));
            menuD.AddItem(new MenuItem("drawR", "Draw R")).SetValue(new Circle(true, Color.FromArgb(150, Color.White)));
            menuD.AddItem(new MenuItem("drawT", "Draw Target")).SetValue(true);
            _menu.AddSubMenu(menuD);

            var kkmenu = new Menu("Keybinds", "keybinds");
            kkmenu.AddItem(new MenuItem("combokey", "Combo (active)")).SetValue(new KeyBind(32, KeyBindType.Press));
            _menu.AddSubMenu(kkmenu);

            var spellmenu = new Menu("SpellMenu", "smenu");

            var menuQ = new Menu("Q Menu", "qmenu");
            menuQ.AddItem(new MenuItem("usecomboq", "Use in Combo")).SetValue(true);
            menuQ.AddItem(new MenuItem("qdash", "Q on Dashing Enemies")).SetValue(false);
            menuQ.AddItem(new MenuItem("qimm", "Q on Immobile Enemies")).SetValue(true);
            menuQ.AddItem(new MenuItem("qhitc", "Q Hitchance 1-Low, 4-Very High")).SetValue(new Slider(3, 1, 4));
            menuQ.AddItem(new MenuItem("minimumq", "Mininum Distance to Q")).SetValue(new Slider(200, 0, (int)_q.Range));
            menuQ.AddItem(new MenuItem("minimumhp", "Dont grab if below health %")).SetValue(new Slider(0));
            menuQ.AddItem(new MenuItem("intq", "Use for Interrupt")).SetValue(true);
            menuQ.AddItem(new MenuItem("secq", "Use for Killsteal")).SetValue(false);
            spellmenu.AddSubMenu(menuQ);

            var menuE = new Menu("E Menu", "emenu");
            menuE.AddItem(new MenuItem("usecomboe", "Use in Combo")).SetValue(true);
            menuE.AddItem(new MenuItem("inte", "Use for Interrupt")).SetValue(true);
            menuE.AddItem(new MenuItem("sece", "Use for Killsteal")).SetValue(false);
            spellmenu.AddSubMenu(menuE);

            var menuR = new Menu("R Menu", "rmenu");
            menuR.AddItem(new MenuItem("usecombor", "Use in Combo")).SetValue(true);
            menuR.AddItem(new MenuItem("intr", "Use for Interrupt")).SetValue(true);
            menuR.AddItem(new MenuItem("secr", "Use for Killsteal")).SetValue(false);
            spellmenu.AddSubMenu(menuR);


            _menu.AddSubMenu(spellmenu);

            var menuM = new Menu("Auto-Q", "bmisc");
            foreach (var obj in ObjectManager.Get<Obj_AI_Hero>().Where(obj => obj.Team != Me.Team))
            {
                menuM.AddItem(new MenuItem("dograb" + obj.ChampionName, obj.ChampionName))
                    .SetValue(new StringList(new[] { "Dont Grab ", "Normal Grab ", "Auto Grab" }, 1));
            }

            _menu.AddSubMenu(menuM);

            _menu.AddToMainMenu();

            // events
            Drawing.OnDraw += BlitzOnDraw;
            Game.OnUpdate += BlitzOnUpdate;
            Interrupter.OnPossibleToInterrupt += BlitzOnInterruptableSpell;

            Game.PrintChat("<font color=\"#FF9900\"><b>KurisuBlitz:</b></font> Loaded");

        }

        private static void BlitzOnInterruptableSpell(Obj_AI_Base unit, InterruptableSpell spell)
        {
            if (_menu.Item("intq").GetValue<bool>())
            {
                var prediction = _q.GetPrediction(unit);
                if (prediction.Hitchance >= HitChance.Low)
                {
                    _q.Cast(prediction.CastPosition);
                }
            }

            if (_menu.Item("intr").GetValue<bool>())
            {
                if (unit.Distance(Me.ServerPosition, true) <= _r.RangeSqr)
                {
                    _r.Cast();
                }
            }

            if (_menu.Item("inte").GetValue<bool>())
            {
                if (unit.Distance(Me.ServerPosition, true) <= _e.RangeSqr)
                {
                    _e.CastOnUnit(Me);
                    Me.IssueOrder(GameObjectOrder.AttackUnit, unit);
                }
            }
        }

        private static void BlitzOnDraw(EventArgs args)
        {
            var target = TargetSelector.GetTarget(_q.Range * 2, TargetSelector.DamageType.Physical);

            if (!Me.IsDead)
            {
                var rcircle = _menu.Item("drawR").GetValue<Circle>();
                var qcircle = _menu.Item("drawQ").GetValue<Circle>();

                if (qcircle.Active)
                    Render.Circle.DrawCircle(Me.Position, _q.Range, qcircle.Color);

                if (rcircle.Active)
                    Render.Circle.DrawCircle(Me.Position, _r.Range, qcircle.Color);

                if (target.IsValidTarget(_q.Range * 2) && _menu.Item("drawT").GetValue<bool>())
                    Render.Circle.DrawCircle(target.Position, target.BoundingRadius - 30, Color.Yellow, 3);
            }
        }

        private static void BlitzOnUpdate(EventArgs args)
        {
            // kill secure
            Secure(_menu.Item("secq").GetValue<bool>(),
                   _menu.Item("sece").GetValue<bool>(),
                   _menu.Item("secr").GetValue<bool>());

            if ((int) (Me.Health/Me.MaxHealth*100) >= _menu.Item("minimumhp").GetValue<Slider>().Value)
            {
                // auto grab
                AutoCast(_menu.Item("qdash").GetValue<bool>(),
                         _menu.Item("qimm").GetValue<bool>());

                if (_menu.Item("combokey").GetValue<KeyBind>().Active)
                {
                    Combo(_menu.Item("usecomboq").GetValue<bool>(),
                          _menu.Item("usecomboe").GetValue<bool>());
                }
            }
        }

        private static void AutoCast(bool dashing, bool immobile)
        {
            if (_q.IsReady())
            {
                foreach (var itarget in ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.IsValidTarget(_q.Range)))
                {
                    if (dashing && _menu.Item("dograb" + itarget.ChampionName).GetValue<StringList>().SelectedIndex == 2)
                        if (itarget.Distance(Me.ServerPosition) > _menu.Item("minimumq").GetValue<Slider>().Value)
                            _q.CastIfHitchanceEquals(itarget, HitChance.Dashing);

                    if (immobile && _menu.Item("dograb" + itarget.ChampionName).GetValue<StringList>().SelectedIndex == 2)
                        if (itarget.Distance(Me.ServerPosition) > _menu.Item("minimumq").GetValue<Slider>().Value)
                            _q.CastIfHitchanceEquals(itarget, HitChance.Immobile);
                }
            }

            if (_r.IsReady())
            {
                foreach (var rtarget in ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.IsValidTarget(_r.Range)))
                {
                    if (_menu.Item("usecombor").GetValue<bool>())
                    {
                        if (!_e.IsReady() && rtarget.HasBuffOfType(BuffType.Knockup))
                            _r.Cast();
                    }
                }
            }
        }

        private static void Combo(bool useq, bool usee)
        {
            if (useq && _q.IsReady())
            {
                var qtarget = TargetSelector.GetTargetNoCollision(_q);
                if (qtarget.IsValidTarget())
                {
                    var poutput = _q.GetPrediction(qtarget);
                    if (poutput.Hitchance >= (HitChance) _menu.Item("qhitc").GetValue<Slider>().Value + 2)
                    {
                        if (qtarget.Distance(Me.ServerPosition) > _menu.Item("minimumq").GetValue<Slider>().Value)
                        {
                            if (_menu.Item("dograb" + qtarget.ChampionName).GetValue<StringList>().SelectedIndex != 0) 
                                _q.Cast(poutput.CastPosition);
                        }
                    }
                }
            }

            if (usee && _e.IsReady())
            {
                var etarget = TargetSelector.GetTarget(250, TargetSelector.DamageType.Physical);
                if (etarget.IsValidTarget())
                {
                    if (_menu.Item("usecomboe").GetValue<bool>() && !_q.IsReady())
                        _e.CastOnUnit(Me);                   
                }
            }
        }

        private static void Secure(bool useq, bool usee, bool user)
        {
            if (user && _r.IsReady())
            {
                var rtarget = ObjectManager.Get<Obj_AI_Hero>().FirstOrDefault(h => h.IsEnemy);
                if (rtarget.IsValidTarget(_r.Range))
                {
                    if (Me.GetSpellDamage(rtarget, SpellSlot.R) >= rtarget.Health)
                        _r.Cast();
                }
            }

            if (usee && _e.IsReady())
            {
                var etarget = ObjectManager.Get<Obj_AI_Hero>().FirstOrDefault(h => h.IsEnemy);
                if (etarget.IsValidTarget(_e.Range))
                {
                    if (Me.GetSpellDamage(etarget, SpellSlot.E) >= etarget.Health)
                        _e.CastOnUnit(Me);
                }
            }

            if (useq && _q.IsReady())
            {
                var qtarget = ObjectManager.Get<Obj_AI_Hero>().FirstOrDefault(h => h.IsEnemy);
                if (qtarget.IsValidTarget(_q.Range))
                {
                    if (Me.GetSpellDamage(qtarget, SpellSlot.Q) >= qtarget.Health)
                    {
                        var poutput = _q.GetPrediction(qtarget);
                        if (poutput.Hitchance >= HitChance.Medium)
                        {
                            if (qtarget.Distance(Me.ServerPosition) >
                                _menu.Item("minimumq").GetValue<Slider>().Value)
                            {
                                _q.Cast(poutput.CastPosition);
                            }
                        }
                    }
                }
            }
        }
    }
}
