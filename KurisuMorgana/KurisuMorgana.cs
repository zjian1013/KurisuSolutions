using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using Color = System.Drawing.Color;

namespace KurisuMorgana
{
    internal class KurisuMorgana
    {
        private static Menu _config;
        private static Orbwalking.Orbwalker _orbwalker;
        private static readonly Obj_AI_Hero Player = ObjectManager.Player;

        public KurisuMorgana()
        {
            Console.WriteLine("Kurisu assembly is loading...");
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static readonly Spell Darkbinding = new Spell(SpellSlot.Q, 1175f);
        private static readonly Spell Tormentsoil = new Spell(SpellSlot.W, 900f);
        private static readonly Spell Blackshield = new Spell(SpellSlot.E, 750f);
        private static readonly Spell Soulshackle = new Spell(SpellSlot.R, 600f);

        private static readonly List<Spell> SpellList = new List<Spell>();

        private static void SetSkills()
        {
            SpellList.AddRange(new[] { Darkbinding, Tormentsoil, Blackshield, Soulshackle });
            Darkbinding.SetSkillshot(0.25f, 80f, 1400f, true, SkillshotType.SkillshotLine);
            Tormentsoil.SetSkillshot(0.25f, 175f, 1200f, false, SkillshotType.SkillshotCircle);
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            Game.PrintChat("<font color=\"#F2F2F2\">[Morgana]</font><font color=\"#D9D9D9\"> - <u>the Fallen Angel v1.0.2</u>  </font>- Kurisu ©");
    
            try
            {


                _config = new Menu("Kurisu: Morgana", "morgana", true);
                var morgOrb = new Menu("Orbwalker", "orbwalker");
                _orbwalker = new Orbwalking.Orbwalker(morgOrb);
                _config.AddSubMenu(morgOrb);

                var morgTS = new Menu("Target Selector", "target selecter");
                SimpleTs.AddToMenu(morgTS);
                _config.AddSubMenu(morgTS);


                var morgDraws = new Menu("Morgana: Drawings", "drawings");

                morgDraws.AddItem(new MenuItem("drawQ", "Draw Q")).SetValue(new Circle(true, Color.FromArgb(150, Color.MediumSlateBlue)));
                morgDraws.AddItem(new MenuItem("drawW", "Draw W")).SetValue(new Circle(false, Color.FromArgb(150, Color.DarkSlateBlue)));
                morgDraws.AddItem(new MenuItem("drawE", "Draw E")).SetValue(new Circle(true, Color.FromArgb(150, Color.DarkSlateBlue)));
                morgDraws.AddItem(new MenuItem("drawR", "Draw R")).SetValue(new Circle(true, Color.FromArgb(150, Color.MediumSlateBlue)));
                _config.AddSubMenu(morgDraws);

                var morgBind = new Menu("Morgana: Dark Binding", "bind");
                morgBind.AddItem(new MenuItem("useq", "Use Q")).SetValue(true);
                morgBind.AddItem(new MenuItem("minuseq", "Min distance Q")).SetValue(new Slider(50, 50, 250));
                morgBind.AddItem(new MenuItem("", ""));
                morgBind.AddItem(new MenuItem("qdash", "Auto Q Dashing")).SetValue(true);
                morgBind.AddItem(new MenuItem("qimmobile", "Auto Q Immoble")).SetValue(true);
                morgBind.AddItem(new MenuItem("qgap", "Auto Q Gapcloser")).SetValue(true);
                _config.AddSubMenu(morgBind);

                var morgSoil = new Menu("Morgana: Torment Soil", "soil");
                morgSoil.AddItem(new MenuItem("usew", "Use W")).SetValue(true);
                morgSoil.AddItem(new MenuItem("", ""));
                morgSoil.AddItem(new MenuItem("wimmobile", "Use W only on immobile")).SetValue(true);
                _config.AddSubMenu(morgSoil);

                var morgShield = new Menu("Morgana: Black Shield", "shield");
                morgShield.AddItem(new MenuItem("usee", "Use E")).SetValue(true);
                morgShield.AddItem(new MenuItem("minshieldpct", "Minumum mana %")).SetValue(new Slider(40));
                morgShield.AddItem(new MenuItem("edangerous", "Shield dangerous spells")).SetValue(true);
                morgShield.AddItem(new MenuItem(" ", " "));
                var supSpe = new Menu("Supported Spells", "suppspells");
;

                var allies = ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.IsAlly).Select(hero => hero.SkinName);
                var enemies = ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.IsEnemy);
                foreach (var e in enemies)
                {

                    foreach (var s in KurisuLib.CcList)
                    {
                        if (s.HeroName == e.SkinName)
                        {
                            supSpe.AddItem(new MenuItem(s.SpellMenuName, e.SkinName + " " + s.SpellMenuName)).SetValue(true) ;
                            //subMenu.AddItem(new MenuItem("ss" + s.SDataName + s.Slot.ToString(), s.SpellMenuName)).SetValue(true);
                            //subMenu.AddItem(new MenuItem("DangerLevel" + s.SDataName, "Danger Level")).SetValue(new Slider(s.DangerLevel, 0, 5));
                            //supSpe.AddSubMenu(subMenu);

                        }

                    }
                }
                morgShield.AddSubMenu(supSpe);

                foreach (var a in allies)
                {
                    morgShield.AddItem(new MenuItem("shield" + a, a)).SetValue(true);
                }
                _config.AddSubMenu(morgShield);

                #region L# Reqs
                Game.OnGameUpdate += Game_OnGameUpdate;
                Obj_AI_Base.OnProcessSpellCast += Game_OnProcessSpellCast;
                AntiGapcloser.OnEnemyGapcloser += Game_OnGapCloser;
                Drawing.OnDraw += Game_DrawingOnDraw;
                //Game.OnGameProcessPacket += Game_OnGameProcessPacket;
                #endregion

                SetSkills();
                _config.AddToMainMenu();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Game.PrintChat("Error occured with morgana assembly(OnLoadMenu)");
            }
        }

        private static void Game_DrawingOnDraw(EventArgs args)
        {
            for (var i = 0; i < SpellList.Count; i++)
            {
                var spell = SpellList[i];
                var circle = _config.SubMenu("drawings").Item("draw" + spell.Slot.ToString()).GetValue<Circle>();
                if (circle.Active)
                    Utility.DrawCircle(Player.Position, spell.Range, circle.Color, 5, 55);
            }
        }

        private static void Game_OnGapCloser(ActiveGapcloser sender)
        {
            if (!_config.Item("useq").GetValue<bool>() || !_config.Item("qgap").GetValue<bool>()) return;
            Darkbinding.Cast(sender.Sender);
        }

        private static void Game_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (_config.Item("usee").GetValue<bool>() && _config.Item("edangerous").GetValue<bool>())
            {
                if (sender.Type == GameObjectType.obj_AI_Hero && sender.IsEnemy)
                {
                    var targetList = ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.IsAlly).OrderBy(h => h.Distance(args.End));
                    foreach (var a in targetList)
                    {
                        foreach (var spell in KurisuLib.CcList)
                        {
                            if (spell.SDataName == args.SData.Name)
                            {
                                Console.WriteLine(args.SData.Name);
                                Console.WriteLine(spell.Type);
                                switch (spell.Type)
                                {
                                    case Skilltype.Circle:
                                        if (a.Distance(args.End) <= 250f && Blackshield.IsReady())
                                        {
                                            if (_config.Item(spell.SpellMenuName).GetValue<bool>() && _config.Item("shield" + a.SkinName).GetValue<bool>())
                                                Blackshield.CastOnUnit(a, true);
                                            //Console.WriteLine("Circle " + args.SData.Name);
                                        }
                                        break;
                                    case Skilltype.Line:
                                        if (a.Distance(args.End) <= 100f && Blackshield.IsReady())
                                        {
                                            if (_config.Item(spell.SpellMenuName).GetValue<bool>() && _config.Item("shield" + a.SkinName).GetValue<bool>())
                                                Blackshield.CastOnUnit(a, true);
                                            //Console.WriteLine("Line " + args.SData.Name);
                                        }
                                        break;
                                    case Skilltype.Unknown:
                                        if (Blackshield.IsReady())
                                            if (_config.Item(spell.SpellMenuName).GetValue<bool>() && _config.Item("shield" + a.SkinName).GetValue<bool>())
                                                Blackshield.CastOnUnit(a, true);
                                            //Console.WriteLine("Unkown " + spell.SDataName);
                                        break;
                                }

                            }
                        }
                    }
                }
            }
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            var target = SimpleTs.GetTarget(1150, SimpleTs.DamageType.Magical);
            DarkBinding(target);
            TormentedSoil(target);

            //Utility.DrawCircle(target.Position, target.BoundingRadius, Color.MediumSlateBlue);
        }

        private static IEnumerable<Obj_AI_Hero> AutoSoilTarget()
        {
            var targets = from hero in ObjectManager.Get<Obj_AI_Hero>()
                          where hero.Team != Player.Team && Vector2.DistanceSquared(Player.Position.To2D(),
                                hero.ServerPosition.To2D()) < Tormentsoil.Range * Tormentsoil.Range
                          select hero;

            return targets;
        }

        private static IEnumerable<Obj_AI_Hero> GetShackleTargets(GameObject target)
        {
            var enemies = from hero in ObjectManager.Get<Obj_AI_Hero>()
                          where hero.IsEnemy && hero.IsValid &&
                                hero.Distance(target.Position) < Soulshackle.Range
                          select hero;

            return enemies;
        }

        private static void DarkBinding(Obj_AI_Base target)
        {

            var predicition = Darkbinding.GetPrediction(target);

            if (predicition.Hitchance == HitChance.High && _orbwalker.ActiveMode.ToString() == "Combo")
                if (_config.Item("useq").GetValue<bool>() && Darkbinding.IsReady())
                    Darkbinding.Cast(predicition.CastPosition);

            switch (predicition.Hitchance)
            {
                case HitChance.Immobile:
                    if (_config.Item("useq").GetValue<bool>() && _config.Item("qimmobile").GetValue<bool>() && Darkbinding.IsReady())
                        Darkbinding.Cast(predicition.CastPosition);
                    break;

                case HitChance.Dashing:
                    if (_config.Item("useq").GetValue<bool>() && _config.Item("qdash").GetValue<bool>() && Darkbinding.IsReady())
                        Darkbinding.Cast(predicition.CastPosition);
                    break;
            }
        }

        private static void TormentedSoil(Obj_AI_Base target)
        {
            if (_orbwalker.ActiveMode.ToString() == "Combo" && _config.Item("usew").GetValue<bool>())
            {
                var prediction = Tormentsoil.GetPrediction(target);
                if (prediction.Hitchance == HitChance.Medium)
                    if (Tormentsoil.IsReady() &&
                        !_config.Item("wimmobile").GetValue<bool>())
                        Tormentsoil.Cast(prediction.CastPosition);
            }

            if (!_config.Item("wimmobile").GetValue<bool>()) return;
            foreach (var enemy in AutoSoilTarget())
            {
                var autopred = Tormentsoil.GetPrediction(enemy);
                if (autopred.Hitchance == HitChance.Immobile && Tormentsoil.IsReady() && enemy.Distance(Player.Position) < Tormentsoil.Range)
                    Tormentsoil.Cast(autopred.CastPosition);
            }
        }

        private void SoulShackles()
        {
            var targets = GetShackleTargets(Player);
            if (targets.Count() >= 1)
            {

            }
        }
    }
}
