using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace KurisuRiven
{
    internal class Program
    {
        private static Menu Config;
        private static Obj_AI_Hero Me = ObjectManager.Player;
        private static Obj_AI_Hero Maintarget;
        private static Orbwalking.Orbwalker Orbwalker;
        static void Main(string[] args)
        {
            Console.WriteLine("Riven injected...");
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            if (Me.ChampionName != "Riven")
            {
                return;
            }

            // load spells
            w = new Spell(SpellSlot.W, 250f);
            e = new Spell(SpellSlot.E, 250f);

            q = new Spell(SpellSlot.Q, 260f);
            q.SetSkillshot(0.25f, 100f, 1400f, false, SkillshotType.SkillshotCircle);

            r = new Spell(SpellSlot.R, 900);
            r.SetSkillshot(0.25f, 300f, 120f, false, SkillshotType.SkillshotCone);

            // load menu
            Menu();

            // call events
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnGameUpdate += Game_OnGameUpdate;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Obj_AI_Base.OnPlayAnimation += Obj_AI_Base_OnPlayAnimation;

            // print chat -- load success
            Game.PrintChat("<font color=\"#7CFC00\"><b>KurisuRivenTest:</b></font> Loaded");
        }

        #region Riven Menu
        private static void Menu()
        {
            Config = new Menu("KurisuRiven", "kurisuriven", true);

            var TsMenu = new Menu("Selector", "selector");
            TargetSelector.AddToMenu(TsMenu);
            Config.AddSubMenu(TsMenu);

            var OMenu = new Menu("Orbwalker", "orbwalker");
            Orbwalker = new Orbwalking.Orbwalker(OMenu);
            Config.AddSubMenu(OMenu);

            var SMenu = new Menu("Spells", "Spells");

            var menuQ = new Menu("Q Menu", "cleave");
            menuQ.AddItem(new MenuItem("prediction", "Predict movement")).SetValue(true);
            SMenu.AddSubMenu(menuQ);

            var menuE = new Menu("E Menu", "valor");
            menuE.AddItem(new MenuItem("vhealth", "Valor health %")).SetValue(new Slider(40, 1));
            SMenu.AddSubMenu(menuE);

            var menuR = new Menu("R Menu", "blade");
            menuR.AddItem(new MenuItem("user", "Use R")).SetValue(true);
            menuR.AddItem(new MenuItem("usews", "Use Windslash")).SetValue(true);
            SMenu.AddSubMenu(menuR);

            Config.AddSubMenu(SMenu);

            //var XMenu = new Menu("Extra", "extra");
            //XMenu.AddItem(new MenuItem("modekey", "Switch combo")).SetValue(new KeyBind(88, KeyBindType.Press));
            //XMenu.AddItem(new MenuItem("rivenmode", "Combo: "))
            //    .SetValue(new StringList(new[] { "Normal", "Burst", "Shy" }));
            //mainmenu.AddSubMenu(XMenu);

            var DMenu = new Menu("Debug", "debug");
            DMenu.AddItem(new MenuItem("debugdmg", "Debug combo damage")).SetValue(false);
            DMenu.AddItem(new MenuItem("fleemode", "Flee")).SetValue(new KeyBind(65, KeyBindType.Press));
            Config.AddSubMenu(DMenu);


            Config.AddToMainMenu();
        }

        #endregion

        private static void CheckDamage(Obj_AI_Base target)
        {
            if (target == null)
            {
                return;
            }

            var ad = Me.GetAutoAttackDamage(target);
            var ignite = Me.GetSpellSlot("summonerdot");

            var tmt = Items.HasItem(3077) && Items.CanUseItem(3077)
                ? Me.GetItemDamage(target, Damage.DamageItems.Tiamat)
                : 0;

            var hyd = Items.HasItem(3074) && Items.CanUseItem(3074)
                ? Me.GetItemDamage(target, Damage.DamageItems.Hydra)
                : 0;

            var bwc = Items.HasItem(3144) && Items.CanUseItem(3144) ?
                Me.GetItemDamage(target, Damage.DamageItems.Bilgewater)
                : 0;

            var brk = Items.HasItem(3153) && Items.CanUseItem(3153)
                ? Me.GetItemDamage(target, Damage.DamageItems.Botrk)
                : 0;

            rr = Me.GetSpellDamage(target, SpellSlot.R) - 20;
            ra = ad + (ad * (runicpassive[Me.Level] / 100) * runiccount);
            rq = q.IsReady() ? DamageQ(target) : 0;

            rw = w.IsReady()
                ? Me.GetSpellDamage(target, SpellSlot.W)
                : 0;

            ri = Me.Spellbook.CanUseSpell(ignite) == SpellState.Ready
                ? Me.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite)
                : 0;

            ritems = tmt + hyd + bwc + brk;

            ua = r.IsReady()
                ? ra +
                  Me.CalcDamage(target, Damage.DamageType.Physical,
                      Me.BaseAttackDamage + Me.FlatPhysicalDamageMod * 0.2)
                : ua;

            uq = r.IsReady()
                ? rq +
                  Me.CalcDamage(target, Damage.DamageType.Physical,
                      Me.BaseAttackDamage + Me.FlatPhysicalDamageMod * 0.2 * 0.7)
                : uq;

            uw = r.IsReady()
                ? rw +
                  Me.CalcDamage(target, Damage.DamageType.Physical,
                      Me.BaseAttackDamage + Me.FlatPhysicalDamageMod * 0.2 * 1)
                : uw;

            rr = r.IsReady()
                ? Me.GetSpellDamage(target, SpellSlot.R)
                : 0;
        }

        private static float DamageQ(Obj_AI_Base target)
        {
            double dmg = 0;
            if (q.IsReady())
            {
                dmg += Me.CalcDamage(target, Damage.DamageType.Physical,
                    -10 + (q.Level * 20) +
                    (0.35 + (q.Level * 0.05)) * (Me.FlatPhysicalDamageMod + Me.BaseAttackDamage));
            }

            return (float)dmg;
        }


        private static void Drawing_OnDraw(EventArgs args)
        {
            if (!Me.IsDead)
            {
                //var wts = Drawing.WorldToScreen(Me.Position);

                Render.Circle.DrawCircle(Me.Position, wrange, System.Drawing.Color.White);
                Render.Circle.DrawCircle(Me.Position, Me.AttackRange + e.Range + 10, System.Drawing.Color.White);

                if (Maintarget.IsValidTarget(900))
                {
                    Render.Circle.DrawCircle(
                        Maintarget.Position, Maintarget.BoundingRadius + 30, System.Drawing.Color.Yellow, 8);
                }

                //Drawing.DrawText(
                //    wts[0] - 50, wts[1] + 30, System.Drawing.Color.White, mode.SelectedValue + " Combo");
                //if (mode.SelectedIndex == 1 && !hashydra &&
                //    orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo)
                //{
                //    Drawing.DrawText(
                //        wts[0] - 70, wts[1] + 50, System.Drawing.Color.Yellow, "Warning: No Tiamat!");
                //}
            }

            if (Config.Item("debugdmg").GetValue<bool>() && Maintarget.IsValidTarget(1000))
            {
                var wts = Drawing.WorldToScreen(Maintarget.Position);
                if (!r.IsReady())
                    Drawing.DrawText(wts[0] - 75, wts[1] + 40, System.Drawing.Color.Yellow,
                        "Combo Damage: " + (float)(ra * 3 + rq * 3 + rw + rr + ri + ritems));
                else
                    Drawing.DrawText(wts[0] - 75, wts[1] + 40, System.Drawing.Color.Yellow,
                        "Combo Damage: " + (float)(ua * 3 + uq * 3 + uw + rr + ri + ritems));
            }

            if (Maintarget.IsValidTarget(1000))
            {
                var wts = Drawing.WorldToScreen(Maintarget.Position);

                if ((float)(ra + rq * 2 + rw + ri + ritems) > Maintarget.Health)
                    Drawing.DrawText(wts[0] - 20, wts[1] + 20, System.Drawing.Color.LawnGreen, "Kill!");
                else if ((float)(ra * 2 + rq * 2 + rw + ritems) > Maintarget.Health)
                    Drawing.DrawText(wts[0] - 40, wts[1] + 20, System.Drawing.Color.LawnGreen, "Easy Kill!");
                else if ((float)(ua * 3 + uq * 2 + uw + ri + rr + ritems) > Maintarget.Health)
                    Drawing.DrawText(wts[0] - 40, wts[1] + 20, System.Drawing.Color.LawnGreen, "Full Combo Kill!");
                else if ((float)(ua * 3 + uq * 3 + uw + rr + ri + ritems) > Maintarget.Health)
                    Drawing.DrawText(wts[0] - 40, wts[1] + 20, System.Drawing.Color.LawnGreen, "Full Combo Hard Kill!");
                else if ((float)(ua * 3 + uq * 3 + uw + rr + ri + ritems) < Maintarget.Health)
                    Drawing.DrawText(wts[0] - 40, wts[1] + 20, System.Drawing.Color.LawnGreen, "Cant Kill!");
            }
        }

        private static void Obj_AI_Base_OnPlayAnimation(GameObject sender, GameObjectPlayAnimationEventArgs args)
        {
            if (!sender.IsMe)
            {
                return;
            }


            if (args.Animation.Contains("Idle"))
            {
                canattack = true;
            }

            if (args.Animation.Contains("Attack"))
            {
                lastattack = Environment.TickCount;
                canmove = false;
                isattacking = true;
                cancleave = false;
                cankiburst = false;
                candash = false;
                canwindslash = false;
            }

            if (args.Animation.Contains("Spell1a") || args.Animation.Contains("Spell1b"))
            {
                lastcleave = Environment.TickCount;
                isattacking = false;
                iscleaving = true;
                cancleave = false;
                canmove = false;

                if (Config.Item("fleemode").GetValue<KeyBind>().Active)
                {
                    return;
                }

                if (Maintarget.IsValidTarget(truerange + 100))
                {
                    Utility.DelayAction.Add(
                        140, () => Me.IssueOrder(GameObjectOrder.MoveTo, new Vector3(movePos.X, movePos.Y, movePos.Z)));
                }
            }

            if (args.Animation.Contains("Spell1c"))
            {
                if (Config.Item("fleemode").GetValue<KeyBind>().Active)
                {
                    return;
                }

                if (Maintarget.IsValidTarget(truerange + 100))
                {
                    if (args.Animation != "Idle")
                        Me.IssueOrder(GameObjectOrder.MoveTo, new Vector3(movePos.X, movePos.Y, movePos.Z));
                }
            }
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            // tickcounts ftw
            switch (args.SData.Name)
            {
                case "RivenMartyr":
                    lastkiburst = Environment.TickCount;
                    iskibursting = true;
                    cankiburst = false;
                    canmove = false;
                    //if (mode.SelectedIndex == 0 ||
                    //    mode.SelectedIndex == 2)
                    //{
                    if (q.IsReady() && cancleave && Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                        Utility.DelayAction.Add(Game.Ping + 70, () => q.Cast(Maintarget.ServerPosition));
                    //}
                    break;
                case "ItemTiamatCleave":
                    lasthydra = Environment.TickCount;
                    //if (mode.SelectedIndex == 1)
                    //{
                    //if (ulton && r.IsReady() && canwindslash)
                    //    r.Cast(maintarget.ServerPosition);
                    //}
                    break;
                case "RivenFeint":
                    lastdash = Environment.TickCount;
                    isdashing = true;
                    canmove = false;
                    candash = false;
                    //if (mode.SelectedIndex == 2)
                    //{
                    //if (ulton && r.IsReady() && canwindslash)
                    //    r.CastIfHitchanceEquals(maintarget, HitChance.Medium);
                    //}
                    break;
                case "RivenFengShuiEngine":
                    if (Maintarget.Distance(Me.ServerPosition) <= wrange)
                    {
                        if (w.IsReady())
                            w.Cast();
                    }
                    break;
                case "rivenizunablade":
                    canwindslash = false;
                    if (q.IsReady())
                        q.Cast(Maintarget.ServerPosition);

                    break;
            }

        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            Windslash();
            Maintarget = TargetSelector.GetTarget(1100, TargetSelector.DamageType.Physical);
            CheckDamage(Maintarget);

            Orbwalker.SetMovement(canmove);
            Orbwalker.SetMovement(canattack);

            wrange = ulton || r.IsReady() ? w.Range + 135 : w.Range;
            animationtime = 1 / (0.318 * Me.AttackSpeedMod);
            truerange = Me.AttackRange + Me.Distance(Me.BBox.Minimum) + 1;

            ulton = Me.GetSpell(SpellSlot.R).Name != "RivenFengShuiEngine";
            hashydra = Items.HasItem(3077) || Items.HasItem(3074);
            canhydra = Items.CanUseItem(3077) || Items.CanUseItem(3074);

            if (Maintarget.IsValidTarget(1000))
            {
                movePos = Maintarget.ServerPosition +
                          Vector3.Normalize(Me.Position - Maintarget.ServerPosition) *
                          (Me.Distance(Maintarget.ServerPosition) + 51);
            }
            else
            {
                movePos = Game.CursorPos;
            }

            if (Config.Item("fleemode").GetValue<KeyBind>().Active)
            {
                if (candash && e.IsReady())
                {
                    e.Cast(Game.CursorPos);
                }

                if (!e.IsReady() && Environment.TickCount - lastcleave >= 300 && Environment.TickCount - lastdash >= 500)
                {
                    q.Cast(Game.CursorPos);
                }

                if (canmove)
                {
                    Me.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                }
            }

            //if (mode.SelectedIndex == 1 && (r.Level < 1 || !r.IsReady()))
            //{
            //    Utility.DelayAction.Add(
            //        1300, delegate
            //        {
            //            if (mode.SelectedIndex == 1 && r.Level < 1)
            //            {
            //                mainmenu.Item("rivenmode").SetValue(new StringList(new[] { "Normal", "Burst", "Shy" }, 0));
            //                Game.PrintChat(
            //                    "<font color=\"#FFFF00\"><b>Ultimate is required for burst combo.</b></font>");
            //            }
            //        });
            //}

            foreach (var b in Me.Buffs)
            {
                if (b.Name == "RivenTriCleave")
                    cleavecount = b.Count;

                if (b.Name == "rivenpassiveaaboost")
                    runiccount = b.Count;
            }

            if (!Me.HasBuff("RivenTriCleave", true))
                Utility.DelayAction.Add(1000, () => cleavecount = 0);

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                Combo(Maintarget);
            }

            //mode = mainmenu.Item("rivenmode").GetValue<StringList>();

            // other modes broke somewhat and aren't ready yet
            //if (mainmenu.Item("modekey").GetValue<KeyBind>().Active)
            //{
            //    switch (mode.SelectedIndex)
            //    {
            //        case 2:
            //            Utility.DelayAction.Add(
            //                200,
            //                () =>
            //                    mainmenu.Item("rivenmode")
            //                        .SetValue(new StringList(new[] { "Normal", "Burst", "Shy" }, 0)));
            //            break;
            //        case 1:
            //            Utility.DelayAction.Add(
            //                200,
            //                () =>
            //                    mainmenu.Item("rivenmode")
            //                        .SetValue(new StringList(new[] { "Normal", "Burst", "Shy" }, 2)));
            //            break;
            //        case 0:
            //            Utility.DelayAction.Add(
            //                200,
            //                () =>
            //                    mainmenu.Item("rivenmode")
            //                        .SetValue(new StringList(new[] { "Normal", "Burst", "Shy" }, 1)));
            //            break;
            //    }
            //}

            if (!canwindslash && !isattacking && ulton && r.IsReady())
            {
                canwindslash = true;
            }

            if (!candash && !(iscleaving || isattacking || iskibursting) && e.IsReady())
            {
                candash = true;
            }

            if (!cankiburst && !(iscleaving || isattacking || isdashing) && w.IsReady())
            {
                cankiburst = true;
            }

            if (!canmove && !(isattacking || iscleaving || iskibursting || isdashing) &&
                Environment.TickCount - lastattack >= 188)
            {
                // Convert.ToInt32(animationtime * 100) - 20
                canmove = true;
            }

            if (!canattack && !(iscleaving || isdashing || iskibursting) &&
                Environment.TickCount - lastattack >= 164)
            {
                // Convert.ToInt32(animationtime * 100) - 20
                canattack = true;
            }

            if (isattacking && Environment.TickCount - lastattack >=
                (Convert.ToInt32(animationtime * 100) - 20) - (Me.Level * 10 / 2))
            {
                //97
                isattacking = false;
                cancleave = true;
                canmove = true;
                candash = true;
                cankiburst = true;
                canwindslash = true;
            }

            if (iscleaving && Environment.TickCount - lastcleave >= 273)
            {
                iscleaving = false;
                canmove = true;
                canattack = true;
            }

            if (iskibursting && Environment.TickCount - lastkiburst >= 148)
            {
                iskibursting = false;
                canattack = true;
                canmove = true;
            }

            if (isdashing && Environment.TickCount - lastdash >= 500)
            {
                isdashing = false;
                canmove = true;
            }
        }

        private static void Windslash()
        {
            if (!ulton || !Config.Item("usews").GetValue<bool>())
            {
                return;
            }

            var target = ObjectManager.Get<Obj_AI_Hero>().FirstOrDefault(x => x.IsValidTarget(r.Range));
            if (target != null)
            {
                if (target.Health <= rr && canwindslash)
                {
                    r.CastIfHitchanceEquals(target, HitChance.Medium);
                }
            }
        }


        private static void Combo(Obj_AI_Base target)
        {
            if (!target.IsValidTarget(r.Range * 2))
            {
                return;
            }

            Orb(target);
            if (candash && e.IsReady() &&
                (target.Distance(Me.ServerPosition) > truerange + 50 &&
                 target.Distance(Me.ServerPosition) <= truerange + e.Range + 100 ||
                 Me.Health / Me.MaxHealth * 100 <= Config.Item("vhealth").GetValue<Slider>().Value))
            {
                ItemData.Youmuus_Ghostblade.GetItem().Cast();
                ItemData.Blade_of_the_Ruined_King.GetItem().Cast(target);
                ItemData.Bilgewater_Cutlass.GetItem().Cast(target);

                e.Cast(target.ServerPosition);
                if (hashydra && canhydra)
                {
                    if (w.IsReady())
                    {
                        Items.UseItem(3077);
                        Items.UseItem(3074);
                    }

                    Utility.DelayAction.Add(250, () => CheckR(target));
                }

                if (!hashydra || !canhydra)
                {
                    CheckR(target);
                }
            }

            if (w.IsReady() && cankiburst && target.Distance(Me.ServerPosition) <= wrange)
            {
                if (hashydra && canhydra)
                {
                    Items.UseItem(3077);
                    Items.UseItem(3074);
                    Utility.DelayAction.Add(250, () => CheckR(target));
                    Utility.DelayAction.Add(300, () => w.Cast());
                }

                if (!hashydra || !canhydra)
                {
                    CheckR(target);
                    w.Cast();
                }
            }

            // cleaves
            if (cancleave && q.IsReady() && target.Distance(Me.ServerPosition) <= truerange + 100)
            {
                if (Environment.TickCount - lasthydra >= 800 &&
                    Environment.TickCount - lastdash >= 200)
                {
                    if (Config.Item("prediction").GetValue<bool>())
                        q.CastIfHitchanceEquals(target, HitChance.Medium);
                    else
                        q.Cast(target.ServerPosition);
                }
            }

            // gapclose
            if (target.Distance(Me.ServerPosition) > truerange + 101)
            {
                if (Environment.TickCount - lastcleave >= 1000 && !isattacking)
                {
                    if (e.IsReady())
                    {
                        return;
                    }

                    q.Cast(target.ServerPosition);
                }
            }
        }

        private static void Orb(Obj_AI_Base target)
        {
            if (canmove && canattack)
            {
                if (target.IsValidTarget(truerange + 100))
                {
                    canmove = false;
                    Me.IssueOrder(GameObjectOrder.AttackUnit, target);
                }
            }
        }

        private static void CheckR(Obj_AI_Base target)
        {
            if (target.IsValidTarget(r.Range + 100))
            {
                if (ulton || !Config.Item("user").GetValue<bool>())
                {
                    return;
                }

                if ((float)ua * 3 + uq * 3 + uw + rr + ri + ritems >= target.Health)
                {
                    if (cleavecount <= 1)
                        r.Cast();
                }
            }
        }

        #region Riven: Fat List Incoming

        private static Spell q;
        private static Spell w;
        private static Spell e;
        private static Spell r;
        private static Vector3 movePos;

        private static double rr, ri, ritems;
        private static double rq, rw, ra;
        private static double uq, uw, ua;

        private static bool isattacking;
        private static bool iscleaving;
        private static bool iskibursting;
        private static bool isdashing;

        private static bool hashydra;
        private static bool canhydra;
        private static float wrange;
        private static bool ulton;
        private static bool canwindslash = true;
        private static bool canmove = true;
        private static bool canattack = true;
        private static bool cancleave = true;
        private static bool cankiburst = true;
        private static bool candash = true;

        private static int lasthydra;
        private static int lastattack;
        private static int lastcleave;
        private static int lastkiburst;
        private static int lastdash;
        private static int cleavecount;
        private static int runiccount;

        private static float truerange;
        private static double animationtime;

        private static readonly int[] runicpassive =
        {
            20, 20, 25, 25, 25, 30, 30, 30, 35, 35, 35, 40, 40, 40, 45, 45, 45, 50, 50
        };

        private static readonly string[] minionList =
        {
            "SRU_Razorbeak", "SRU_Krug", "Sru_Crab", "SRU_Baron", "SRU_Dragon",
            "SRU_Blue", "SRU_Red", "SRU_Murkwolf", "SRU_Gromp"
        };

        #endregion
    }
}
