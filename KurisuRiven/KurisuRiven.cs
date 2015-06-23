using System;
using System.Linq;
using System.Collections.Generic;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;
using SharpDX;

namespace KurisuRiven
{
    internal class KurisuRiven
    {
        #region Riven: Main

        private static int lastq;
        private static int lastw;
        private static int laste;
        private static int lastaa;
        private static int lasthd;

        private static bool canq;
        private static bool canw;
        private static bool cane;
        private static bool canmv;
        private static bool canaa;
        private static bool canws;
        private static bool canhd;
        private static bool hashd;

        private static bool didq;
        private static bool didw;
        private static bool dide;
        private static bool didws;
        private static bool didaa;
        private static bool didhd;
        private static bool didhs;
        private static bool ssfl;

        private static Menu menu;
        private static Spell q, w, e, r;
        private static Orbwalking.Orbwalker orbwalker;
        private static Obj_AI_Hero player = ObjectManager.Player;
        private static HpBarIndicator hpi = new HpBarIndicator();

        private static Obj_AI_Base qtarg; // semi q target
        private static Obj_AI_Hero rtarg; // riven target

        private static bool uo;
        private static bool cb;

        private static int cc;
        private static int pc;
        private static SpellSlot flash;

        private static float truerange;
        private static Vector3 movepos;

        #endregion

        # region Riven: Utils

        private static bool menubool(string item)
        {
            return menu.Item(item).GetValue<bool>();
        }

        private static int menuslide(string item)
        {
            return menu.Item(item).GetValue<Slider>().Value;
        }

        private static int menulist(string item)
        {
            return menu.Item(item).GetValue<StringList>().SelectedIndex;
        }

        private static float xtra(float dmg)
        {
           return r.IsReady() ? (float) (dmg + (dmg*0.2)) : dmg;
        }

        private static void useinventoryitems(Obj_AI_Base target)
        {
            if (Items.HasItem(3142) && Items.CanUseItem(3142))
                Items.UseItem(3142);

            if (target.Distance(player.ServerPosition, true) <= 450 * 450)
            {
                if (Items.HasItem(3144) && Items.CanUseItem(3144))
                    Items.UseItem(3144, target);
                if (Items.HasItem(3153) && Items.CanUseItem(3153))
                    Items.UseItem(3153, target);
            }
        }

        private static readonly string[] minionlist =
        {
            // summoners rift
            "SRU_Razorbeak", "SRU_Krug", "Sru_Crab", "SRU_Baron", "SRU_Dragon",
            "SRU_Blue", "SRU_Red", "SRU_Murkwolf", "SRU_Gromp", 
            
            // twisted treeline
            "TT_NGolem5", "TT_NGolem2", "TT_NWolf6", "TT_NWolf3",
            "TT_NWraith1", "TT_Spider"
        };

        #endregion

        public KurisuRiven()
        {
            Console.WriteLine("KurisuRiven enabled?");
            CustomEvents.Game.OnGameLoad += args =>
            {
                try
                {
                    if (player.ChampionName == "Riven")
                    {
                        w = new Spell(SpellSlot.W, 250f);
                        e = new Spell(SpellSlot.E, 270f);

                        q = new Spell(SpellSlot.Q, 260f);
                        q.SetSkillshot(0.25f, 100f, 2200f, false, SkillshotType.SkillshotCircle);

                        r = new Spell(SpellSlot.R, 1100f);
                        r.SetSkillshot(0.25f, 225f, 1600f, false, SkillshotType.SkillshotCone);

                        flash = player.GetSpellSlot("summonerflash");

                        OnNewPath();
                        OnPlayAnimation();
                        Interrupter();
                        OnGapcloser();
                        OnCast();
                        Drawings();
                        OnMenuLoad();

                        Game.OnUpdate += Game_OnUpdate;

                        if (menu.Item("Farm").GetValue<KeyBind>().Key == menu.Item("semiq").GetValue<KeyBind>().Key ||
                            menu.Item("Orbwalk").GetValue<KeyBind>().Key == menu.Item("semiq").GetValue<KeyBind>().Key ||
                            menu.Item("LaneClear").GetValue<KeyBind>().Key == menu.Item("semiq").GetValue<KeyBind>().Key ||
                            menu.Item("LastHit").GetValue<KeyBind>().Key == menu.Item("semiq").GetValue<KeyBind>().Key)
                        {
                            Game.PrintChat(
                                "<b><font color=\"#FF9900\">" +
                                "WARNING: Semi-Q Keybind Should not be the same key as any of " +
                                "the other orbwalking modes or it will not Work!</font></b>");
                        }
                    }
                }

                catch (Exception e)
                {
                    Console.WriteLine("Fatal Error: " + e.Message);
                }
            };
        }

        #region Riven: OnNewPath (Thanks Yomie/Detuks)
        private static void OnNewPath()
        {
            Obj_AI_Base.OnNewPath += (sender, args) =>
            {
                if (sender.IsMe && !args.IsDash)
                {
                    if (didq)
                    {
                        didq = false;
                        canaa = true;
                        canmv = true;
                    }
                }
            };
        }

        #endregion

        #region Riven: OnUpdate
        private static void Game_OnUpdate(EventArgs args)
        {
            didhs = menu.Item("harasskey").GetValue<KeyBind>().Active;

            // ulti check
            uo = player.GetSpell(SpellSlot.R).Name != "RivenFengShuiEngine";

            // hydra check
            hashd = Items.HasItem(3077) || Items.HasItem(3074);
            canhd = !didaa && (Items.CanUseItem(3077) || Items.CanUseItem(3074));

            // main target (riven targ)
            rtarg = TargetSelector.GetTarget(1200, TargetSelector.DamageType.Physical);
            truerange = player.AttackRange + player.Distance(player.BBox.Minimum) + 1;

            // if no valid target cancel to cursor pos
            if (!qtarg.IsValidTarget(truerange + 100))
                 qtarg = player;

            // can we burst?
            cb = rtarg != null && r.IsReady() && q.IsReady() &&
                ((ComboDamage(rtarg)/1.6) >= rtarg.Health || rtarg.CountEnemiesInRange(w.Range) >= menuslide("multic"));

            // set player skin
            player.SetSkin(player.BaseSkinName, menu.Item("skinset").GetValue<StringList>().SelectedIndex);

            // move behind me
            if (qtarg != player && qtarg.IsFacing(player) && qtarg.Distance(player.ServerPosition) < truerange + 120)
                movepos = player.ServerPosition + (player.ServerPosition - qtarg.ServerPosition).Normalized()*28;

            // move towards target (thanks yol0)
            if (qtarg != player && (!qtarg.IsFacing(player) || qtarg.Distance(player.ServerPosition) > truerange + 120))
                movepos = player.ServerPosition.Extend(qtarg.ServerPosition, 350);

            // move to game cursor pos
            if (qtarg == player)
                movepos = Game.CursorPos;

            // orbwalk movement
            orbwalker.SetAttack(canmv && canaa);
            orbwalker.SetMovement(canmv);

            // reqs ->
            SemiQ();
            AuraUpdate();
            CombatCore();
            Windslash();

            if (rtarg.IsValidTarget() && 
                menu.Item("combokey").GetValue<KeyBind>().Active)
            {
                TryFlashInitiate(rtarg);
                ComboTarget(rtarg);
            }

            if (didhs && rtarg.IsValidTarget())
            {
                HarassTarget(rtarg);
            }

            if (player.IsValid &&
                menu.Item("clearkey").GetValue<KeyBind>().Active)
            {
                Clear();
                Wave();
            }

            if (player.IsValid &&
                menu.Item("fleekey").GetValue<KeyBind>().Active)
            {
                Flee();
            }

        }

        #endregion

        #region Riven: Menu
        private static void OnMenuLoad()
        {
            menu = new Menu("Kurisu's Riven", "kurisuriven", true);

            var tsmenu = new Menu("Selector", "selector");
            TargetSelector.AddToMenu(tsmenu);
            menu.AddSubMenu(tsmenu);

            var orbwalkah = new Menu("Orbwalker", "rorb");
            orbwalker = new Orbwalking.Orbwalker(orbwalkah);
            menu.AddSubMenu(orbwalkah);

            var keybinds = new Menu("Keybinds", "keybinds");
            keybinds.AddItem(new MenuItem("combokey", "Combo")).SetValue(new KeyBind(32, KeyBindType.Press));
            keybinds.AddItem(new MenuItem("harasskey", "Harass")).SetValue(new KeyBind(67, KeyBindType.Press));
            keybinds.AddItem(new MenuItem("clearkey", "Jungle/Laneclear")).SetValue(new KeyBind(86, KeyBindType.Press));
            keybinds.AddItem(new MenuItem("jumpkey", "Walljump")).SetValue(new KeyBind(65, KeyBindType.Press));
            keybinds.AddItem(new MenuItem("fleekey", "Flee")).SetValue(new KeyBind(65, KeyBindType.Press));

            var mitem = new MenuItem("semiqlane", "Use Semi-Q Laneclear");
            mitem.ValueChanged += (sender, args) =>
            {
                if (menu.Item("Farm").GetValue<KeyBind>().Key == args.GetNewValue<KeyBind>().Key ||
                    menu.Item("Orbwalk").GetValue<KeyBind>().Key == args.GetNewValue<KeyBind>().Key ||
                    menu.Item("LaneClear").GetValue<KeyBind>().Key == args.GetNewValue<KeyBind>().Key ||
                    menu.Item("LastHit").GetValue<KeyBind>().Key == args.GetNewValue<KeyBind>().Key)
                {
                    Game.PrintChat(
                        "<b><font color=\"#FF9900\">" +
                        "WARNING: Semi-Q Keybind Should not be the same key as any of " +
                        "the other orbwalking modes or it will not Work!</font></b>");
                }
            };

            keybinds.AddItem(mitem).SetValue(new KeyBind(71, KeyBindType.Press));
            keybinds.AddItem(new MenuItem("semiq", "Use Semi-Q Harass/Jungle")).SetValue(true);
            menu.AddSubMenu(keybinds);

            var drMenu = new Menu("Drawings", "drawings");
            drMenu.AddItem(new MenuItem("drawengage", "Draw Engage Range")).SetValue(true);
            drMenu.AddItem(new MenuItem("drawdmg", "Draw Damage Bar")).SetValue(true);
            drMenu.AddItem(new MenuItem("drawburst", "Draw Burst Range")).SetValue(true);
            drMenu.AddItem(new MenuItem("drawstatus", "Draw Ult Mode Text")).SetValue(true);
            drMenu.AddItem(new MenuItem("drawtarg", "Draw Target")).SetValue(false);
            menu.AddSubMenu(drMenu);

            var combo = new Menu("Combo", "combo");
            var qmenu = new Menu("Q  Settings", "rivenq");
            qmenu.AddItem(new MenuItem("autoaq", "CanQ Delay (ms)")).SetValue(new Slider(15, 0, 300));
            qmenu.AddItem(new MenuItem("qint", "Interrupt with 3rd Q")).SetValue(true);
            qmenu.AddItem(new MenuItem("keepq", "Keep Q Buff Up")).SetValue(true);
            qmenu.AddItem(new MenuItem("usegap", "Gapclose with Q")).SetValue(true);
            qmenu.AddItem(new MenuItem("gaptime", "Gapclose Q Delay (ms)")).SetValue(new Slider(110, 50, 200));
            combo.AddSubMenu(qmenu);

            var wmenu = new Menu("W Settings", "rivenw");
            wmenu.AddItem(new MenuItem("usecombow", "Use W in Combo")).SetValue(true);
            wmenu.AddItem(new MenuItem("wgap", "Use W on Gapcloser")).SetValue(true);
            wmenu.AddItem(new MenuItem("wint", "Use W to Interrupt")).SetValue(true);
            combo.AddSubMenu(wmenu);

            var emenu = new Menu("E  Settings", "rivene");
            emenu.AddItem(new MenuItem("usecomboe", "Use E in Combo")).SetValue(true);
            emenu.AddItem(new MenuItem("emode", "Use E Mode"))
                .SetValue(new StringList(new[] { "E -> W/R -> Tiamat -> Q", "E -> Tiamat -> W/R -> Q" } ));
            emenu.AddItem(new MenuItem("erange", "E Only if Target > AARange or Engage")).SetValue(true);
            emenu.AddItem(new MenuItem("vhealth", "Or Use E if HP% <=")).SetValue(new Slider(40));
            emenu.AddItem(new MenuItem("ashield", "Shield Spells While LastHit")).SetValue(true);
            combo.AddSubMenu(emenu);

            var rmenu = new Menu("R  Settings", "rivenr");
            rmenu.AddItem(new MenuItem("user", "Use R in Combo")).SetValue(true);
            rmenu.AddItem(new MenuItem("useignote", "Use R + Smart Ignite")).SetValue(true);
            rmenu.AddItem(new MenuItem("multib", "Flash R/W if Can Burst Target")).SetValue(true);
            rmenu.AddItem(new MenuItem("multic", "Flash R/W if Hit >= (6 = OFF)")).SetValue(new Slider(4, 2, 6));
            rmenu.AddItem(new MenuItem("overk", "Dont R if Target HP % <=")).SetValue(new Slider(25, 1, 99));
            rmenu.AddItem(new MenuItem("userq", "Use R Only if Q Count <=")).SetValue(new Slider(1, 1, 3));
            rmenu.AddItem(new MenuItem("ultwhen", "Use R When"))
                .SetValue(new StringList(new[] {"Normal Kill", "Hard Kill", "Always"}, 1));
            rmenu.AddItem(new MenuItem("usews", "Use Windslash (R2) in Combo")).SetValue(true);
            rmenu.AddItem(new MenuItem("wsmode", "Windslash (R2) for"))
                .SetValue(new StringList(new[] {"Kill Only", "Kill Or MaxDamage"}, 1));
            rmenu.AddItem(new MenuItem("rmulti", "Windslash if enemies hit >=")).SetValue(new Slider(3, 2, 5));
            combo.AddSubMenu(rmenu);

            menu.AddSubMenu(combo);

            var harass = new Menu("Harass", "harass");
            harass.AddItem(new MenuItem("qtoo", "Use 3rd Q:"))
                .SetValue(new StringList(new[] {"Away from Target", "To Ally Turret", "To Cursor"}, 1));
            harass.AddItem(new MenuItem("useharassw", "Use W in Harass")).SetValue(true);
            harass.AddItem(new MenuItem("usegaph", "Use E in Harass (Gapclose)")).SetValue(true);
            harass.AddItem(new MenuItem("useitemh", "Use Tiamat/Hydra")).SetValue(true);
            menu.AddSubMenu(harass);

            var farming = new Menu("Farming", "farming");

            var jg = new Menu("Jungle", "jungle");
            jg.AddItem(new MenuItem("uselaneq", "Use Q in Laneclear")).SetValue(true);
            jg.AddItem(new MenuItem("uselanew", "Use W in Laneclear")).SetValue(true);
            jg.AddItem(new MenuItem("wminion", "Use W Minions >=")).SetValue(new Slider(3, 1, 6));
            jg.AddItem(new MenuItem("uselanee", "Use E in Laneclear")).SetValue(true);
            farming.AddSubMenu(jg);

            var wc = new Menu("WaveClear", "waveclear");
            wc.AddItem(new MenuItem("usejungleq", "Use Q in Jungle")).SetValue(true);
            wc.AddItem(new MenuItem("usejunglew", "Use W in Jungle")).SetValue(true);
            wc.AddItem(new MenuItem("usejunglee", "Use E in Jungle")).SetValue(true);
            farming.AddSubMenu(wc);

            menu.AddSubMenu(farming);

            var skinchange = new Menu("Skin Changer", "skinchange");
            skinchange.AddItem(new MenuItem("skinset", ""))
                .SetValue(
                    new StringList(
                        new[] { "Classic", "Redeemed", "Crimson", "Battle Bunny", "Championship", "Dragonblade" }, 4));
            menu.AddSubMenu(skinchange);
            menu.AddToMainMenu();

        }

        #endregion

        #region Riven : Flash Initiate

        private static void TryFlashInitiate(Obj_AI_Hero target)
        {
            // use r at appropriate distance
            // on spell cast takes over

            if (!menubool("multib"))
                return;

            if (!menu.Item("combokey").GetValue<KeyBind>().Active ||
                !target.IsValid<Obj_AI_Hero>() || uo || !menubool("user"))
                return;

            if (rtarg == null || !cb || uo)
                return;

            if (!flash.IsReady())
                return;

            if (e.IsReady() && target.Distance(player.ServerPosition) <= e.Range + w.Range + 300)
            {
                if (target.Distance(player.ServerPosition) > e.Range + truerange)
                {
                    e.Cast(target.ServerPosition);
                }
            }

            if (!e.IsReady() && target.Distance(player.ServerPosition) <= w.Range + 300)
            {
                if (target.Distance(player.ServerPosition) > truerange + 35)
                {
                    r.Cast();
                }
            }
        }

        #endregion

        #region Riven: Combo

        private static void ComboTarget(Obj_AI_Base target)
        {
            // orbwalk ->
            OrbTo(target);

            // ignite ->
            var ignote = player.GetSpellSlot("summonerdot");
            if (player.Spellbook.CanUseSpell(ignote) == SpellState.Ready)
            {
                if (rtarg.Distance(player.ServerPosition) <= 600)
                {
                    if (cc <= menuslide("userq") && q.IsReady() && menubool("useignote"))
                    {
                        if (ComboDamage(target) >= target.Health &&
                            target.Health/target.MaxHealth*100 > menuslide("overk"))
                        {
                            if (r.IsReady() && uo)
                            {
                                player.Spellbook.CastSpell(ignote, target);
                            }
                        }
                    }
                }
            }

            if (e.IsReady() && cane && menubool("usecomboe") &&
               (player.Health/player.MaxHealth*100 <= menuslide("vhealth") ||
                 target.Distance(player.ServerPosition) > truerange + 25))
            {
                if (menubool("usecomboe"))
                    e.Cast(target.ServerPosition);

                if (target.Distance(player.ServerPosition) <= r.Range)
                {          
                    if (menulist("emode") == 1)
                    {
                        if (canhd && hashd && !cb)
                        {
                            Items.UseItem(3077);
                            Items.UseItem(3074);
                        }

                        else
                        {
                            CheckR();
                        }
                    }

                    if (menulist("emode") == 0)
                    {
                        CheckR();
                    }
                }
            }

            else if (w.IsReady() && canw && menubool("usecombow") && 
                target.Distance(player.ServerPosition) <= w.Range + 25)
            {
                if (menulist("emode") == 0)
                {
                    if (menubool("usecombow"))
                        w.Cast();

                    if (canhd && hashd)
                    {
                        Items.UseItem(3077);
                        Items.UseItem(3074);
                    }
                }

                if (menulist("emode") == 1)
                {
                    if (canhd && hashd && !cb)
                    {
                        Items.UseItem(3077);
                        Items.UseItem(3074);
                        if (menubool("usecombow"))
                            Utility.DelayAction.Add(250, () => w.Cast());
                    }

                    else
                    {
                        CheckR();
                        if (menubool("usecombow"))
                            w.Cast();
                    }
                }

                useinventoryitems(target);
                CheckR();
            }

            else if (q.IsReady() && target.Distance(player.ServerPosition) <= q.Range + 30)
            {
                useinventoryitems(target);
                CheckR();

                if (menulist("emode") == 0 || (ComboDamage(target)/1.7) >= target.Health)
                {
                    if (Items.CanUseItem(3077) || Items.CanUseItem(3074))
                        return;
                }

                if (canq)
                    q.Cast(target.ServerPosition);
            }

            else if (target.Distance(player.ServerPosition) > truerange + 100)
            {
                if (menubool("usegap"))
                {
                    if (!e.IsReady() && Utils.GameTimeTickCount - lastq >= menuslide("gaptime")*10 && !didaa)
                    {
                        if (q.IsReady() && Utils.GameTimeTickCount - laste >= 700)
                        {
                            q.Cast(target.ServerPosition);
                        }
                    }
                }
            }
        }

        #endregion

        #region Riven: Harass

        private static void HarassTarget(Obj_AI_Base target)
        {
            Vector3 qpos;
            switch (menulist("qtoo"))
            {
                case 0:
                    qpos = player.ServerPosition + 
                        (player.ServerPosition - target.ServerPosition).Normalized()*500;
                    break;
                case 1:
                    qpos = ObjectManager.Get<Obj_AI_Turret>()
                        .Where(t => (t.IsAlly)).OrderBy(t => t.Distance(player.Position)).First().Position;
                    break;
                default:
                    qpos = Game.CursorPos;
                    break;
            }

            if (q.IsReady())
                OrbTo(target);

            if (cc >= 2 && canq)
            {
                canaa = false;
            }

            if (cc == 2 && canq && q.IsReady())
            {
                player.IssueOrder(GameObjectOrder.MoveTo, qpos);
                Utility.DelayAction.Add(200, () =>
                {
                    q.Cast(qpos);
                });
            }

            if (!player.ServerPosition.Extend(target.ServerPosition, q.Range*3).UnderTurret(true))
            {
                if (q.IsReady() && canq && cc < 2)
                {
                    if (target.Distance(player.ServerPosition) <= truerange + q.Range)
                    {
                        q.Cast(target.ServerPosition);
                    }
                }
            }

            if (e.IsReady() && cane && q.IsReady() && cc < 1 &&
                target.Distance(player.ServerPosition) > truerange + 100 &&
                target.Distance(player.ServerPosition) <= e.Range + truerange + 50)
            {
                if (!player.ServerPosition.Extend(target.ServerPosition, e.Range).UnderTurret(true))
                {
                    if (menubool("usegaph"))
                        e.Cast(target.ServerPosition);
                }
            }

            else if (w.IsReady() && canw && target.Distance(player.ServerPosition) <= w.Range + 10)
            {
                if (!target.ServerPosition.UnderTurret(true))
                {
                    if (menubool("useharassw"))
                        w.Cast();
                }
            }

        }
        #endregion
         
        #region Riven: Windslash

        private static void Windslash()
        {
            if (uo && menubool("usews") && r.IsReady())
            {
                foreach (var target in ObjectManager.Get<Obj_AI_Hero>().Where(h => h.IsValidTarget(r.Range)))
                {
                    if (target.IsZombie)
                        return;

                    // only kill or killsteal etc ->
                    if (r.GetDamage(target) >= target.Health && canws)
                    {
                        if (r.GetPrediction(target, true).Hitchance == HitChance.VeryHigh)
                            r.Cast(r.GetPrediction(target, true).CastPosition);
                    }
                }

                // kill or maxdamage ->
                if (menulist("wsmode") == 1 && rtarg.IsValidTarget(r.Range))
                {
                    r.CastIfWillHit(rtarg, menuslide("rmulti"));

                    if (rtarg.IsZombie)
                        return;

                    var po = r.GetPrediction(rtarg, true);
                    if ((r.GetDamage(rtarg) / rtarg.MaxHealth * 100) >= rtarg.Health / rtarg.MaxHealth * 50)
                    {
                        if (po.Hitchance >= HitChance.VeryHigh && canws)
                            r.Cast(po.CastPosition);
                    }

                    if (q.IsReady() && rtarg.Health <= xtra((float) 
                       (r.GetDamage(rtarg) + player.GetAutoAttackDamage(rtarg)*2 + Qdmg(rtarg)* 2)))
                    {
                        if (rtarg.Distance(player.ServerPosition) <= truerange + 100)
                        {
                            if (po.Hitchance >= HitChance.VeryHigh && canws)
                                r.Cast(po.CastPosition);
                        }
                    }
                }
            }        
        }

        #endregion

        #region Riven: Lane/Jungle

        private static void Clear()
        {
            var minions = MinionManager.GetMinions(player.Position, 600f,
                MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

            foreach (var unit in minions)
            {
                OrbTo(unit);
                if (q.IsReady() && unit.Distance(player.ServerPosition) <= q.Range + 100)
                {
                    if (canq && menubool("usejungleq"))
                        q.Cast(unit.ServerPosition);
                }

                if (w.IsReady() && unit.Distance(player.ServerPosition) <= w.Range + 10)
                {
                    if (canw && menubool("usejunglew"))
                        w.Cast();
                }

                if (e.IsReady() && (unit.Distance(player.ServerPosition) > truerange + 30 ||
                    player.Health/player.MaxHealth*100 <= 70))
                {
                    if (cane && menubool("usejunglee"))
                        e.Cast(unit.ServerPosition);
                }
            }
        }

        private static void Wave()
        {
            var minions = MinionManager.GetMinions(player.Position, 600f);
            foreach (var unit in minions)
            {
                if (q.IsReady() && unit.Distance(player.ServerPosition) <= truerange + 100)
                {
                    if (canq && menubool("uselaneq") && minions.Count >= 2)
                        q.Cast(unit.ServerPosition);
                }

                if (w.IsReady())
                {
                    if (minions.Count(m => m.Distance(player.ServerPosition) <= w.Range + 10) >= menuslide("wminion"))
                    {
                        if (canw && menubool("uselanew"))
                        {
                            Items.UseItem(3077);
                            Items.UseItem(3074);
                            w.Cast();
                        }
                    }
                }

                if (e.IsReady())
                {
                    if (unit.Distance(player.ServerPosition) > truerange + 30)
                    {
                        if (cane && menubool("uselanee"))
                            e.Cast(unit.ServerPosition);
                    }

                    else if (player.Health / player.MaxHealth * 100 <= 70)
                    {
                        if (cane && menubool("uselanee"))
                            e.Cast(unit.ServerPosition);
                    }
                }
            }
        }

        #endregion

        #region Riven: Flee

        private static void Flee()
        {
            if (canmv)
            {
                player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            }

            if (ssfl)
            {
                if (Utils.GameTimeTickCount - lastq >= 600)
                {
                    q.Cast(Game.CursorPos);
                }

                if (cane && e.IsReady())
                {
                    if (cc >= 2 || !q.IsReady() && !player.HasBuff("RivenTriCleave", true))
                    {
                        if (!player.ServerPosition.Extend(Game.CursorPos, e.Range).IsWall())
                            e.Cast(Game.CursorPos);
                    }
                }
            }

            else
            {
                if (q.IsReady())
                {
                    q.Cast(Game.CursorPos);
                }

                if (e.IsReady() && Utils.GameTimeTickCount - lastq >= 250)
                {
                    e.Cast(Game.CursorPos);
                }
            }
        }

        #endregion

        #region Riven: SemiQ 

        private static void SemiQ()
        {
            if (canq && Utils.GameTimeTickCount - lastaa >= 150 && menubool("semiq"))
            {
                if (q.IsReady() && Utils.GameTimeTickCount - lastaa < 1200 && qtarg != null)
                {
                    if (qtarg.IsValidTarget(q.Range + 100) && 
                       !menu.Item("harasskey").GetValue<KeyBind>().Active &&
                       !menu.Item("combokey").GetValue<KeyBind>().Active)
                    {
                        if (qtarg.IsValid<Obj_AI_Hero>())
                            q.Cast(qtarg.ServerPosition);
                    }

                    if (!menu.Item("harasskey").GetValue<KeyBind>().Active &&
                        !menu.Item("clearkey").GetValue<KeyBind>().Active)
                    {
                        if (qtarg.IsValidTarget(q.Range + 100) && !qtarg.Name.Contains("Mini"))
                        {
                            if (!qtarg.Name.StartsWith("Minion") && minionlist.Any(name => qtarg.Name.StartsWith(name)))
                            {
                                q.Cast(qtarg.ServerPosition);
                            }
                        }

                        if (qtarg.IsValidTarget(q.Range + 100))
                        {
                            if (qtarg.IsValid<Obj_AI_Minion>() || qtarg.IsValid<Obj_AI_Turret>())
                            {
                                if (menu.Item("semiqlane").GetValue<KeyBind>().Active)
                                    q.Cast(qtarg.ServerPosition);
                            }

                            if (qtarg.IsValid<Obj_AI_Hero>() || qtarg.IsValid<Obj_AI_Turret>())
                            {
                                if (uo)
                                    q.Cast(qtarg.ServerPosition);
                            }
                        }
                    }
                }
            }        
        }

        #endregion

        #region Riven: Check R
        private static void CheckR()
        {
            if (!r.IsReady() || uo || !menubool("user"))
                return;

            if (menulist("ultwhen") == 2 && cc <= menuslide("userq"))
                r.Cast();

            var enemies = HeroManager.Enemies.Where(ene => ene.IsValidTarget(r.Range + 250));
            var targets = enemies as IList<Obj_AI_Hero> ?? enemies.ToList();
            foreach (var target in targets)
            {
                if (cc <= menuslide("userq") && (q.IsReady() || Utils.GameTimeTickCount - lastq < 1000))
                {
                    if (targets.Count(ene => ene.Distance(player.ServerPosition) <= 650) >= 2)
                    {
                        r.Cast();
                    }

                    if (targets.Count() < 2 && target.Health/target.MaxHealth*100 <= menuslide("overk"))
                    {
                        return;
                    }

                    if (menulist("ultwhen") == 0)
                    {
                        if ((ComboDamage(target)/1.7) >= target.Health)
                        {
                            r.Cast();
                        }
                    }

                    // hard kill ->
                    if (menulist("ultwhen") == 1)
                    {
                        if (ComboDamage(target) >= target.Health)
                        {
                            r.Cast();
                        }
                    }
                }
            }        
        }

        #endregion

        #region Riven: On Cast

        private static void OnCast()
        {
            Obj_AI_Base.OnProcessSpellCast += (sender, args) =>
            {
                if (sender.IsEnemy && sender.Type == player.Type && menubool("ashield"))
                {
                    var epos = player.ServerPosition +
                              (player.ServerPosition - sender.ServerPosition).Normalized()*300;

                    if (player.Distance(sender.ServerPosition) <= args.SData.CastRange)
                    {
                        switch (args.SData.TargettingType)
                        {
                            case SpellDataTargetType.Unit:

                                if (args.Target.NetworkId == player.NetworkId)
                                {
                                    if (orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LastHit)
                                    {
                                        e.Cast(epos);
                                    }
                                }

                                break;
                            case SpellDataTargetType.SelfAoe:

                                if (orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LastHit)
                                {
                                    e.Cast(epos);
                                }

                                break;
                        }
                    }
                }

                if (!sender.IsMe)
                    return;

                switch (args.SData.Name)
                {
                    case "RivenTriCleave":
                        didq = true;
                        canmv = false;
                        lastq = Utils.GameTimeTickCount;
                        canq = false;

                        if (!uo)
                            ssfl = false;

                        // cancel q animation
                        if (qtarg.IsValid && qtarg.Distance(player.ServerPosition) <= e.Range + q.Range)
                        {
                            Utility.DelayAction.Add(100 + 100 - Game.Ping/2,
                                () => player.IssueOrder(GameObjectOrder.MoveTo, movepos));
                        }

                        break;
                    case "RivenMartyr":
                        didw = true;
                        lastw = Utils.GameTimeTickCount;
                        canw = false;

                        break;
                    case "RivenFeint":
                        dide = true;
                        laste = Utils.GameTimeTickCount;
                        cane = false;

                        if (menu.Item("fleekey").GetValue<KeyBind>().Active)
                        {
                            if (uo && r.IsReady() && cc == 2 && q.IsReady())
                            {
                                if (rtarg == null || rtarg.Distance(player.ServerPosition) > r.Range)
                                    r.Cast(Game.CursorPos);
                            }
                        }

                        if (menu.Item("combokey").GetValue<KeyBind>().Active)
                        {
                            if (cc >= 2)
                            {
                                CheckR();
                                Utility.DelayAction.Add(Game.Ping + 100, () => q.Cast(Game.CursorPos));
                            }
                        }

                        break;
                    case "RivenFengShuiEngine":
                        ssfl = true;

                        if (rtarg != null && cb)
                        {
                            if (!flash.IsReady() || !menubool("multib"))
                                return;

                            var ww = w.IsReady() ? w.Range + 10 : truerange;

                            if (menu.Item("combokey").GetValue<KeyBind>().Active)
                            {
                                if (rtarg.Distance(player.ServerPosition) > e.Range + ww &&
                                    rtarg.Distance(player.ServerPosition) <= e.Range + ww + 300)
                                {
                                    player.Spellbook.CastSpell(flash, rtarg.ServerPosition);
                                }
                            }
                        }

                        break;
                    case "rivenizunablade":
                        ssfl = false;
                        didws = true;
                        canws = false;

                        if (q.IsReady() && rtarg.IsValidTarget(1200))
                            q.Cast(rtarg.ServerPosition);
          
                        break;
                    case "ItemTiamatCleave":
                        lasthd = Utils.GameTimeTickCount;
                        didhd = true;
                        canws = true;
                        canhd = false;

                        if (menulist("wsmode") == 1 && uo && canws)
                        {
                            if (menu.Item("combokey").GetValue<KeyBind>().Active)
                            {
                                if (cb && r.GetPrediction(rtarg).Hitchance >= HitChance.Medium &&
                                    rtarg.IsValidTarget() && !rtarg.IsZombie)
                                {
                                    Utility.DelayAction.Add(150, 
                                        () => r.Cast(r.GetPrediction(rtarg).CastPosition));
                                }
                            }
                        }

                        if (menulist("emode") == 1 && menu.Item("combokey").GetValue<KeyBind>().Active)
                        {
                            CheckR();
                            Utility.DelayAction.Add(Game.Ping + 175, () => q.Cast(Game.CursorPos));
                        }

                        break;
                }

                if (args.SData.Name.Contains("Attack"))
                {
                    if (menu.Item("combokey").GetValue<KeyBind>().Active)
                    {
                        if (cb || !menubool("usecombow") || !menubool("usecomboe"))
                        {
                            // delay till after aa
                            Utility.DelayAction.Add(
                                50 + (int) (player.AttackDelay*100) + Game.Ping/2 + menuslide("autoaq"), delegate
                                {
                                    if (Items.CanUseItem(3077))
                                        Items.UseItem(3077);
                                    if (Items.CanUseItem(3074))
                                        Items.UseItem(3074);
                                });
                        }
                    }

                    else if (menu.Item("clearkey").GetValue<KeyBind>().Active ||
                            (menu.Item("harasskey").GetValue<KeyBind>().Active && menubool("useitemh")))
                    {
                        if (qtarg.IsValid<Obj_AI_Base>() && !qtarg.Name.StartsWith("Minion"))
                        {
                            Utility.DelayAction.Add(
                                50 + (int) (player.AttackDelay*100) + Game.Ping/2 + menuslide("autoaq"), delegate
                                {
                                    if (Items.CanUseItem(3077))
                                        Items.UseItem(3077);
                                    if (Items.CanUseItem(3074))
                                        Items.UseItem(3074);
                                });
                        }
                    }
                }

                if (!didq && args.SData.Name.Contains("Attack"))
                {
                    didaa = true;
                    canaa = false;
                    canq = false;
                    canw = false;
                    cane = false;
                    canws = false;
                    lastaa = Utils.GameTimeTickCount;
                    qtarg = (Obj_AI_Base) args.Target;
                }
            };
        }

        #endregion

        #region Riven: Misc Events
        private static void Interrupter()
        {
            Interrupter2.OnInterruptableTarget += (sender, args) =>
            {
                if (menubool("wint") && w.IsReady())
                {
                    if (!sender.Position.UnderTurret(true))
                    {
                        if (sender.IsValidTarget(w.Range))
                            w.Cast();

                        if (sender.IsValidTarget(w.Range + e.Range) && e.IsReady())
                        {
                            e.Cast(sender.ServerPosition);
                        }
                    }
                }

                if (menubool("qint") && q.IsReady() && cc >= 2)
                {
                    if (!sender.Position.UnderTurret(true))
                    {
                        if (sender.IsValidTarget(q.Range))
                            q.Cast(sender.ServerPosition);

                        if (sender.IsValidTarget(q.Range + e.Range) && e.IsReady())
                        {
                            e.Cast(sender.ServerPosition);
                        }
                    }
                }
            };
        }

        private static void OnGapcloser()
        {
            AntiGapcloser.OnEnemyGapcloser += gapcloser =>
            {
                if (menubool("wgap") && w.IsReady())
                {
                    if (gapcloser.Sender.IsValidTarget(w.Range))
                    {
                        if (!gapcloser.Sender.ServerPosition.UnderTurret(true))
                        {
                            w.Cast();
                        }
                    }
                    
                }

                if (q.IsReady() && cc == 2)
                {
                    if (gapcloser.Sender.IsValidTarget(q.Range) && !player.IsFacing(gapcloser.Sender))
                    {
                        if (Items.CanUseItem((int) Items.GetWardSlot().Id))
                        {
                            q.Cast(Game.CursorPos);

                            if (didq)
                                Items.UseItem((int) Items.GetWardSlot().Id, gapcloser.Sender.Position);
                        }
                    }
                }
            };
        }

        private void OnPlayAnimation()
        {
            Obj_AI_Base.OnPlayAnimation += (sender, args) =>
            {
                if (!(didq || didw || didws || dide) && !player.IsDead)
                {
                    if (sender.IsMe)
                    {
                        if (args.Animation.Contains("Idle"))
                        {
                            canq = false;
                            canaa = true;
                        }

                        if (args.Animation.Contains("Run"))
                        {
                            canq = false;
                            canaa = true;
                        }
                    }
                }

            };
        }

        #endregion

        #region Riven: Aura

        private static void AuraUpdate()
        {
            if (!player.IsDead)
            {
                foreach (var buff in player.Buffs)
                {
                    if (buff.Name == "RivenTriCleave")
                        cc = buff.Count;

                    if (buff.Name == "rivenpassiveaaboost")
                        pc = buff.Count;
                }

                if (player.HasBuff("RivenTriCleave", true))
                {
                    if (Utils.GameTimeTickCount - lastq >= 3650)
                    {
                        if (!player.IsRecalling() && !player.Spellbook.IsChanneling)
                        {
                            if (menubool("keepq"))
                                q.Cast(Game.CursorPos);
                        }
                    }
                }

                if (!player.HasBuff("rivenpassiveaaboost", true))
                    Utility.DelayAction.Add(1000, () => pc = 1);

                if (!player.HasBuff("RivenTriCleave", true))
                    Utility.DelayAction.Add(1000, () => cc = 0);
            }
        }

        #endregion

        #region Riven : Combat/Orbwalk
        private static void OrbTo(Obj_AI_Base target)
        {
            if (canaa && canmv)
            {
                if (!(didq || didw || dide || didaa))
                {
                    if (target.Distance(player.ServerPosition) <= truerange + 50)
                    {
                        canq = false;
                        player.IssueOrder(GameObjectOrder.AttackUnit, target);
                    }
                }
            }
        }

        private static void CombatCore()
        {
            if (didhd && canhd && Utils.GameTimeTickCount - lasthd >= 250)
            {
                 didhd = false;
            }

            if (didq)
            {
                if (Utils.GameTimeTickCount - lastq >= (int)(player.AttackCastDelay * 1000) + 310)
                {
                    didq = false;
                    canmv = true;
                    canaa = true;
                }
            }

            if (didw)
            {
                if (Utils.GameTimeTickCount - lastw >= 266)
                {
                    didw = false;
                    canmv = true;
                    canaa = true;
                }
            }

            if (dide)
            {
                if (Utils.GameTimeTickCount - laste >= 300)
                {
                    dide = false;
                    canmv = true;
                }             
            }

            if (didaa)
            {
                if (Utils.GameTimeTickCount - lastaa >= (player.AttackDelay * 100) + 100 - Game.Ping + menuslide("autoaq"))
                {
                    didaa = false;
                    canmv = true;
                    canq = true;
                    cane = true;
                    canw = true;
                    canws = true;
                }
            }

            if (!canw && w.IsReady())
            {
                if (!(didaa || didq || dide))
                {
                    canw = true;
                }
            }

            if (!cane && e.IsReady())
            {
                if (!(didaa|| didq || didw))
                {
                    cane = true;
                }
            }

            if (!canws && r.IsReady())
            {
                if (!(didaa || didw) && uo)
                {
                    canws = true;
                }
            }

            if (!canaa)
            {
                if (!(didq || didw|| dide || didws || didhd || didhs))
                {
                    if (Utils.GameTimeTickCount - lastaa >= 1000)
                    {
                        canaa = true;
                    }
                }
            }

            if (!canmv)
            {
                if (!(didq || didw || dide || didws || didhd || didhs))
                {
                    if (Utils.GameTimeTickCount - lastaa >= 1100)
                    {
                        canmv = true;
                    }
                }
            }
        }

        #endregion

        #region Riven: Math/Damage

        private static float ComboDamage(Obj_AI_Base target)
        {
            if (target == null)
                return 0f;

            var ignote = player.GetSpellSlot("summonerdot");
            var ad = (float)player.GetAutoAttackDamage(target);
            var runicpassive = new[] { 0.2, 0.25, 0.3, 0.35, 0.4, 0.45, 0.5 };

            var ra = ad +
                        (float)
                            ((+player.FlatPhysicalDamageMod + player.BaseAttackDamage) *
                            runicpassive[player.Level / 3]);

            var rw = Wdmg(target);
            var rq = Qdmg(target);
            var rr = r.IsReady() ? r.GetDamage(target) : 0;

            var ii = (ignote != SpellSlot.Unknown && player.GetSpell(ignote).State == SpellState.Ready && r.IsReady()
                ? player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite)
                : 0);

            var tmt = Items.HasItem(3077) && Items.CanUseItem(3077)
                ? player.GetItemDamage(target, Damage.DamageItems.Tiamat)
                : 0;

            var hyd = Items.HasItem(3074) && Items.CanUseItem(3074)
                ? player.GetItemDamage(target, Damage.DamageItems.Hydra)
                : 0;

            var bwc = Items.HasItem(3144) && Items.CanUseItem(3144)
                ? player.GetItemDamage(target, Damage.DamageItems.Bilgewater)
                : 0;

            var brk = Items.HasItem(3153) && Items.CanUseItem(3153)
                ? player.GetItemDamage(target, Damage.DamageItems.Botrk)
                : 0;

            var items = tmt + hyd + bwc + brk;

            var damage = (rq * 3 + ra * 3 + rw + rr + ii + items);

            return xtra((float) damage);
        }


        private static double Wdmg(Obj_AI_Base target)
        {
            double dmg = 0;
            if (w.IsReady() && target != null)
            {
                dmg += player.CalcDamage(target, Damage.DamageType.Physical,
                    new[] {50, 80, 110, 150, 170}[w.Level - 1] + 1*player.FlatPhysicalDamageMod + player.BaseAttackDamage);
            }

            return dmg;
        }

        private static double Qdmg(Obj_AI_Base target)
        {
            double dmg = 0;
            if (q.IsReady() && target != null)
            {
                dmg += player.CalcDamage(target, Damage.DamageType.Physical,
                    -10 + (q.Level * 20) + (0.35 + (q.Level * 0.05)) * (player.FlatPhysicalDamageMod + player.BaseAttackDamage));
            }

            return dmg;
        }

        #endregion

        #region Riven: Drawings

        private static void Drawings()
        {
            Drawing.OnDraw += args =>
            {
                if (!player.IsDead)
                {
                    if (menubool("drawstatus"))
                    {
                        var mypos = Drawing.WorldToScreen(player.Position);
                        Drawing.DrawText(mypos[0] - 60, mypos[1] + 10, Color.White,
                            "Ult When: " + menu.Item("ultwhen").GetValue<StringList>().SelectedValue);
                    }

                    if (menubool("drawengage"))
                    {
                        Render.Circle.DrawCircle(player.Position,
                            player.AttackRange + e.Range + 10, Color.White, 2);
                    }

                    if (menubool("drawburst") && cb && flash.IsReady())
                    {
                        var ee = e.IsReady() ? e.Range : 0f;
                        var ww = w.IsReady() ? w.Range + 10 : truerange;
                        Render.Circle.DrawCircle(player.Position, ee + ww + 300, Color.GreenYellow, 2);
                    }

                    if (menubool("drawtarg") && rtarg != null)
                    {
                        Render.Circle.DrawCircle(rtarg.Position, rtarg.BoundingRadius + 10, Color.Red);
                    }
                }
            };

            Drawing.OnEndScene += args =>
            {
                if (!menubool("drawdmg"))
                    return;

                foreach (
                    var enemy in
                        ObjectManager.Get<Obj_AI_Hero>()
                            .Where(ene => ene.IsValidTarget() && !ene.IsZombie))
                {
                    var color = r.IsReady() &&
                                (enemy.Health <= ComboDamage(enemy) / 1.6 ||
                                 enemy.CountEnemiesInRange(w.Range) >= menuslide("multic"))
                        ? new ColorBGRA(0, 255, 0, 90)
                        : new ColorBGRA(255, 255, 0, 90);

                    hpi.unit = enemy;
                    hpi.drawDmg(ComboDamage(enemy), color);
                }

            };
        }

        #endregion

    }
}
