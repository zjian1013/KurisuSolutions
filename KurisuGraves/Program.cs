using System;
using System.Linq;
using Color = System.Drawing.Color;
using LeagueSharp.Common;
using LeagueSharp;
using SharpDX;

namespace KurisuGraves
{                           
    // _____                     
    //|   __|___ ___ _ _ ___ ___ 
    //|  |  |  _| .'| | | -_|_ -|
    //|_____|_| |__,|\_/|___|___|
    // Copyright © Kurisu Solutions 2015

    internal class Program
    {
        // rushed a bit
        // rip code format
        private static Menu mainMenu;
        private static Obj_AI_Hero gtarg;
        private static Orbwalking.Orbwalker orbwalker;
        private static readonly Obj_AI_Hero Me = ObjectManager.Player;
        private static HpBarIndicator hpi = new HpBarIndicator();

        private static int LastE;
        private static int LastR;
        static void Main(string[] args)
        {
            Console.WriteLine("KurisuGraves injected..");
            CustomEvents.Game.OnGameLoad += Initialize;
        }

        private static readonly Spell Buckshot = new Spell(SpellSlot.Q, 950f);
        private static readonly Spell Smokescreen = new Spell(SpellSlot.W, 850f);
        private static readonly Spell Quickdraw = new Spell(SpellSlot.E, 425f);
        private static readonly Spell Chargeshot = new Spell(SpellSlot.R, 1000f);

        static void Initialize(EventArgs args)
        {
            try
            {
                // validate
                if (Me.ChampionName != "Graves")
                    return;

                // load menu
                Menu_OnLoad();

                // spell setup
                Chargeshot.SetSkillshot(
                    0.25f, 100f, 2100f, false, SkillshotType.SkillshotLine);
                Smokescreen.SetSkillshot(
                    0.25f, 250f, 1650f, false, SkillshotType.SkillshotCircle);
                Buckshot.SetSkillshot(
                    0.25f, 15f * (float) Math.PI / 180, 2000f, false, SkillshotType.SkillshotCone);

                // On Tick Event
                Game.OnUpdate += GravesOnUpdate;

                // On Draw Event
                Drawing.OnDraw += GravesOnDraw;
                Drawing.OnEndScene += GravesOnEndScene;

                // Anti-Gapclose Event
                AntiGapcloser.OnEnemyGapcloser += GravesReverseGapclose;

                // After Attack Event
                Orbwalking.AfterAttack += GravesAfterAttack;

                // On Spell Cast Event
                Obj_AI_Base.OnProcessSpellCast += GravesOnCast;
            }

            catch (Exception e)
            {
                Console.WriteLine("Fatal Error: " + e);
            }
        }

        private static void GravesOnEndScene(EventArgs args)
        {
            if (!mainMenu.Item("drawfill").GetValue<bool>())
                return;

            foreach (
                var enemy in
                    ObjectManager.Get<Obj_AI_Hero>()
                        .Where(ene => ene.IsValidTarget() && !ene.IsZombie))
            {
                    //? new ColorBGRA(0, 255, 0, 90)
                    //: new ColorBGRA(255, 255, 0, 90);

                hpi.unit = enemy;
                hpi.drawDmg(GetComboDamage(enemy), new ColorBGRA(255, 255, 0, 90));
            }
        }

        static void GravesOnUpdate(EventArgs args)
        {
            gtarg = TargetSelector.GetTarget(Chargeshot.Range, TargetSelector.DamageType.Physical);

            if (mainMenu.Item("combokey").GetValue<KeyBind>().Active)
            {
                GravesCombo();
            }

            if (mainMenu.Item("fleekey").GetValue<KeyBind>().Active)
            {
                if (Me.CanMove)
                    Me.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);

                if (Smokescreen.IsReady() && gtarg.IsValidTarget(Smokescreen.Range))
                    Smokescreen.CastIfHitchanceEquals(gtarg, HitChance.Medium);

                else if (Quickdraw.IsReady())
                {
                    if (Utils.GameTimeTickCount - LastE > 500)
                        Quickdraw.Cast(Game.CursorPos);
                }
            }

            if (gtarg.IsValidTarget() && Chargeshot.IsReady())
            {
                if (Kappa(gtarg.ServerPosition, Chargeshot.Width, Chargeshot.Range) >= 
                    mainMenu.Item("rmulti2").GetValue<Slider>().Value)
                {
                    Chargeshot.CastIfHitchanceEquals(gtarg, HitChance.Medium);
                }
            }
        }

        static void GravesReverseGapclose(ActiveGapcloser gapcloser)
        {
            var attacker = gapcloser.Sender;
            if (attacker.IsValidTarget())
            {
                if (Buckshot.IsReady() && attacker.Distance(Me.ServerPosition) <= 
                    mainMenu.Item("minqongap").GetValue<Slider>().Value)
                    Buckshot.Cast(attacker);

                if (Smokescreen.IsReady() && mainMenu.Item("usewongap").GetValue<bool>())
                {
                    if (attacker.Distance(Me.ServerPosition) < 420 &&
                        attacker.Distance(Me.ServerPosition) > 300)
                    {
                        Smokescreen.Cast(attacker);
                    }
                }
            }      
        }

        static void GravesAfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (mainMenu.Item("combokey").GetValue<KeyBind>().Active)
            {
                var hero = target as Obj_AI_Hero;
                if (hero.IsValidTarget())
                {
                    CastE(hero);
                }
            }
        }

        static void GravesOnCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe)
                return;

            switch (args.SData.Name)
            {
                case "GravesMove":
                    LastE = Utils.GameTimeTickCount;
                    break;
                case "GravesClusterShot":
                    if (mainMenu.Item("combokey").GetValue<KeyBind>().Active)
                    {
                        if (gtarg.IsValidTarget(Quickdraw.Range + 450) && Quickdraw.IsReady())
                        {
                            Utility.DelayAction.Add(Game.Ping + 250, () =>
                            {
                                CastE(gtarg);
                            });
                        }
                    }
                    break;

                case "GravesSmokeGrenade":
                    if (mainMenu.Item("fleekey").GetValue<KeyBind>().Active)
                    {
                        if (gtarg.IsValidTarget(Quickdraw.Range + 450) && Quickdraw.IsReady())
                        {
                            Utility.DelayAction.Add(Game.Ping + 250, () =>
                            {
                                Quickdraw.Cast(Game.CursorPos);
                            });                  
                        }
                    }

                    if (mainMenu.Item("combokey").GetValue<KeyBind>().Active)
                    {
                        if (gtarg.IsValidTarget(Quickdraw.Range + 450) && Quickdraw.IsReady())
                        {
                            Utility.DelayAction.Add(Game.Ping + 250, () =>
                            {
                                CastE(gtarg);
                            });
                        }
                    }
                    break;

                case "GravesChargeShot":
                    LastR = Utils.GameTimeTickCount;
                    if (mainMenu.Item("combokey").GetValue<KeyBind>().Active)
                    {
                        if (gtarg.Distance(Me.ServerPosition) <= Chargeshot.Range)
                        {
                            Utility.DelayAction.Add(Game.Ping + 250, () =>
                            {
                                if (gtarg.Distance(Me.ServerPosition) > Quickdraw.Range)
                                    Quickdraw.Cast(gtarg.ServerPosition);
                                else
                                    CastE(gtarg);
                            });            
                        }
                    }
                    break;
            }
        }

        static void GravesOnDraw(EventArgs args)
        {
            var acircle = mainMenu.Item("drawaa").GetValue<Circle>();
            var mcircle = mainMenu.Item("drawminqq").GetValue<Circle>();
            var rcircle = mainMenu.Item("drawrrange").GetValue<Circle>();
            var qcircle = mainMenu.Item("drawqrange").GetValue<Circle>();

            if (acircle.Active)
                Render.Circle.DrawCircle(Me.Position, Me.AttackRange, acircle.Color, 3);

            if (rcircle.Active)
                Render.Circle.DrawCircle(Me.Position, Chargeshot.Range, rcircle.Color, 3);

            if (qcircle.Active)
                Render.Circle.DrawCircle(Me.Position, Buckshot.Range, qcircle.Color, 3);

            if (mcircle.Active)
                Render.Circle.DrawCircle(Me.Position, mainMenu.Item("minqrange").GetValue<Slider>().Value, mcircle.Color, 3);
        }

        static void GravesCombo()
        {
            var minqrange = mainMenu.Item("minqrange").GetValue<Slider>().Value;

            var rtarget = TargetSelector.GetTarget(Chargeshot.Range, TargetSelector.DamageType.Physical);
            if (rtarget.IsValidTarget(Chargeshot.Range))
            {
                var user = mainMenu.Item("usercombo").GetValue<bool>();
                if (rtarget.Distance(Me.ServerPosition) <= Quickdraw.Range + Me.AttackRange)
                {
                    if (Chargeshot.IsReady())
                    {
                        if (Quickdraw.IsReady() && Buckshot.IsReady() && user)
                        {
                            if (GetComboDamage(rtarget) >= rtarget.Health)
                                Chargeshot.CastIfHitchanceEquals(rtarget, HitChance.High);
                        }

                        if (GetRDamage(rtarget) >= rtarget.Health && user)
                            Chargeshot.CastIfHitchanceEquals(rtarget, HitChance.High);
                    }
                }
            }

            var qtarget = TargetSelector.GetTarget(Buckshot.Range, TargetSelector.DamageType.Physical);
            if (qtarget.Distance(Me.ServerPosition) <= minqrange)
            {
                if (Buckshot.IsReady())
                    Buckshot.CastIfHitchanceEquals(qtarget, HitChance.High);
            }

            if (qtarget.Distance(Me.ServerPosition) <= 450)
            {
                if (Smokescreen.IsReady() && Utils.GameTimeTickCount - LastR >= 1200)
                    Smokescreen.CastIfHitchanceEquals(qtarget, HitChance.Medium);
            }

            if (qtarget.Distance(Me.ServerPosition) > Me.AttackRange + 100)
            {
                if (Quickdraw.IsReady() && Utils.GameTimeTickCount - LastR >= 1200)
                {
                    if (qtarget.Distance(Me.ServerPosition) > Me.AttackRange + 100)
                    {
                        CastE(qtarget);
                    }
                }
            }
        }

        static void GravesHarass(Obj_AI_Hero target)
        {
            if (target.IsValidTarget(Chargeshot.Range))
            {
                
            }
        }


        static float GetRDamage(Obj_AI_Hero target)
        {
            if (target == null)
                return 0f;

            // impact physical damage
            var irdmg = Chargeshot.IsReady() && KappaHD(target.ServerPosition, Chargeshot.Width, Chargeshot.Range) < 1
                ? (float)Me.CalcDamage(target, Damage.DamageType.Physical,
                    new double[] { 250, 400, 550 }[Chargeshot.Level - 1] + 1.5 * Me.FlatPhysicalDamageMod)
                : 0;

            // explosion damage
            var erdmg = Chargeshot.IsReady() && KappaHD(target.ServerPosition, Chargeshot.Width, Chargeshot.Range) >= 1
                ? (float)Me.CalcDamage(target, Damage.DamageType.Physical,
                    new double[] { 200, 320, 440 }[Chargeshot.Level - 1] + 1.2 * Me.FlatPhysicalDamageMod)
                : 0;

            return irdmg + erdmg;
        }

        static float GetComboDamage(Obj_AI_Hero target)
        {
            if (target == null)
                return 0f;

            // atackspeed sterioid
            var edmg = Quickdraw.IsReady() ? (float) (Me.GetAutoAttackDamage(target) * 3) : 0;

            // buckshot damage
            var qdmg = Buckshot.IsReady() ? (float)(Me.GetSpellDamage(target, SpellSlot.Q)) : 0;

            // smokescreen damage
            var wdmg = Smokescreen.IsReady() ? (float)(Me.GetSpellDamage(target, SpellSlot.W)) : 0;

            // impact physical damage
            var irdmg = Chargeshot.IsReady() && KappaHD(target.ServerPosition, Chargeshot.Width, Chargeshot.Range) < 1
                ? (float) Me.CalcDamage(target, Damage.DamageType.Physical,
                    new double[] { 250, 400, 550 }[Chargeshot.Level - 1] + 1.5 * Me.FlatPhysicalDamageMod)
                : 0;
                
            // explosion damage
            var erdmg = Chargeshot.IsReady() && KappaHD(target.ServerPosition, Chargeshot.Width, Chargeshot.Range) >= 1
                ? (float) Me.CalcDamage(target, Damage.DamageType.Physical,
                    new double[] { 200, 320, 440 }[Chargeshot.Level - 1] + 1.2 * Me.FlatPhysicalDamageMod)
                : 0;

            return qdmg + edmg + wdmg + irdmg + erdmg;
        }

        // Counts the number of enemy objects in path of player and the spell.
        static int Kappa(Vector3 endpos, float width, float range, bool minion = false)
        {
            var end = endpos.To2D();
            var start = Me.ServerPosition.To2D();
            var direction = (end - start).Normalized();
            var endposition = start + direction * range;

            return (from unit in ObjectManager.Get<Obj_AI_Base>().Where(b => b.Team != Me.Team)
                where Me.ServerPosition.Distance(unit.ServerPosition) <= range
                where unit is Obj_AI_Hero || unit is Obj_AI_Minion && minion
                let proj = unit.ServerPosition.To2D().ProjectOn(start, endposition)
                let projdist = unit.Distance(proj.SegmentPoint)
                where unit.BoundingRadius + width > projdist
                select unit).Count();
        }

        // Counts the number of enemy objects in front of the player from the local player.
        static int KappaHD(Vector3 endpos, float width, float range, bool minion = false)
        {
            var end = endpos.To2D();
            var start = Me.ServerPosition.To2D();
            var direction = (end - start).Normalized();
            var endposition = start + direction * start.Distance(endpos);

            return (from unit in ObjectManager.Get<Obj_AI_Base>().Where(b => b.Team != Me.Team)
                    where Me.ServerPosition.Distance(unit.ServerPosition) <= range
                    where unit is Obj_AI_Hero || unit is Obj_AI_Minion && minion
                    let proj = unit.ServerPosition.To2D().ProjectOn(start, endposition)
                    let projdist = unit.Distance(proj.SegmentPoint)
                    where unit.BoundingRadius + width > projdist
                    select unit).Count();
        }

        static void CastE(Obj_AI_Base target)
        {
            var start = Me.ServerPosition.To2D();
            var endpos = Prediction.GetPrediction(target, 0.25f).UnitPosition.To2D();
            var orbrange = Orbwalking.GetRealAutoAttackRange(target);

            var position = Geometry.CircleCircleIntersection(start, endpos, Quickdraw.Range, orbrange);
            if (position.Count() > 0)
            {
                Quickdraw.Cast(position.MinOrDefault(x => x.Distance(Game.CursorPos)));
            }

            else
            {
                if (Buckshot.IsReady())
                    Buckshot.CastIfHitchanceEquals(target, HitChance.High);

                // dash back
                else
                {
                    Quickdraw.Cast(Me.ServerPosition.Extend(target.ServerPosition, -Quickdraw.Range));
                }
            }
        }

        // Instantiates the menu when called.
        static void Menu_OnLoad()
        {
            mainMenu = new Menu("Kurisu's Graves", "kurisugraves", true);

            var tsmenu = new Menu("Selector", "selector");
            TargetSelector.AddToMenu(tsmenu);
            mainMenu.AddSubMenu(tsmenu);

            var owmenu = new Menu("Orbwalker", "orbwalker");
            orbwalker = new Orbwalking.Orbwalker(owmenu);
            mainMenu.AddSubMenu(owmenu);

            var kbmenu = new Menu("Keybinds", "keys");
            kbmenu.AddItem(new MenuItem("fleekey", "Use Flee")).SetValue(new KeyBind(65, KeyBindType.Press));
            kbmenu.AddItem(new MenuItem("combokey", "Use Combo")).SetValue(new KeyBind(32, KeyBindType.Press));
            //kbmenu.AddItem(new MenuItem("harasskey", "Use Harass")).SetValue(new KeyBind(67, KeyBindType.Press));
            mainMenu.AddSubMenu(kbmenu);

            var drmenu = new Menu("Drawings", "drawings");
            drmenu.AddItem(new MenuItem("drawaa", "Draw AA Range"))
                .SetValue(new Circle(true, Color.FromArgb(150, Color.Firebrick)));
            drmenu.AddItem(new MenuItem("drawqrange", "Draw Q Range"))
                .SetValue(new Circle(false, Color.FromArgb(150, Color.Firebrick)));
            drmenu.AddItem(new MenuItem("drawminqq", "Draw Min Q Range"))
                .SetValue(new Circle(true, Color.FromArgb(150, Color.Red)));
            drmenu.AddItem(new MenuItem("drawrrange", "Draw R Range"))
                .SetValue(new Circle(true, Color.FromArgb(150, Color.Firebrick)));
            drmenu.AddItem(new MenuItem("drawfill", "Draw HpBar Fill")).SetValue(true);

            mainMenu.AddSubMenu(drmenu);

            var combo = new Menu("Combo", "combo");

            //combo.AddItem(new MenuItem("useqcombo", "Use Q in combo")).SetValue(true);
            combo.AddItem(new MenuItem("useqongap", "Use Q on gapclosers")).SetValue(true);
            combo.AddItem(new MenuItem("usewongap", "Use W on gapclosers")).SetValue(true);
            combo.AddItem(new MenuItem("minqrange", "Minimum Q range")).SetValue(new Slider(595, 0, 950));
            combo.AddItem(new MenuItem("minqongap", "Minimum Q gapclose range")).SetValue(new Slider(375, 0, 950));
            //combo.AddItem(new MenuItem("usewcombo", "Use W in combo")).SetValue(true);
            combo.AddItem(new MenuItem("useecombo", "Use E in combo")).SetValue(true);
            combo.AddItem(new MenuItem("ewherecom", "Use E to"))
                .SetValue(new StringList(new[] { "Safe Position" } ));
            combo.AddItem(new MenuItem("usercombo", "Use R in combo")).SetValue(true);
            combo.AddItem(new MenuItem("rmulti", "Use R in combo if hit >=")).SetValue(new Slider(1, 1, 5));
            combo.AddItem(new MenuItem("rmulti2", "Use R auto if hit >=")).SetValue(new Slider(4, 1, 5));
            //rmenu.AddItem(new MenuItem("usersteald", "Use R Steal Drake")).SetValue(true);
            //rmenu.AddItem(new MenuItem("userstealb", "Use R Steal Nashor")).SetValue(true);
            mainMenu.AddSubMenu(combo);

            //var harass = new Menu("Harass", "harass");
            //harass.AddItem(new MenuItem("useqharas", "Use Q in Harass")).SetValue(true);
            //harass.AddItem(new MenuItem("usewharas", "Use W in Harass")).SetValue(false);
            //harass.AddItem(new MenuItem("useeharas", "Use E in Harass")).SetValue(true);
            //harass.AddItem(new MenuItem("ewherehar", "Use E to"))
            //    .SetValue(new StringList(new[] { "Cursor Position", "Target Position", "Safe Position" }, 2));
            //mainMenu.AddSubMenu(harass);

            //var farming = new Menu("Farming", "gfarm");
            //mainMenu.AddSubMenu(farming);

            mainMenu.AddToMainMenu();
        }
    }
}
