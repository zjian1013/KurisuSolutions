using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = System.Drawing.Color;

namespace KurisuMorgana
{
    internal class KurisuMorgana
    {
        private static Menu Config;
        private static Orbwalking.Orbwalker Orbwalker;
        private static Obj_AI_Hero Player = ObjectManager.Player;

        public KurisuMorgana()
        {
            Console.WriteLine("Kurisu assembly is loading...");
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static Spell darkbinding = new Spell(SpellSlot.Q, 1175f);
        private static Spell tormentsoil = new Spell(SpellSlot.W, 900f);
        private static Spell blackshield = new Spell(SpellSlot.E, 750f);
        private static Spell soulshackle = new Spell(SpellSlot.R, 600f);

        private static List<Spell> SpellList = new List<Spell>();

        private void SetSkills()
        {
            SpellList.AddRange(new[] { darkbinding, tormentsoil, blackshield, soulshackle });
            darkbinding.SetSkillshot(0.25f, 80f, 1400f, true, SkillshotType.SkillshotLine);
            tormentsoil.SetSkillshot(0.25f, 175f, 1200f, false, SkillshotType.SkillshotCircle);
        }

        private void Game_OnGameLoad(EventArgs args)
        {
            Game.PrintChat("<font color=\"#BF80FF\">[</font><font color=\"#BF80FF\">KurisuMorgana</font><font color=\"#BF80FF\">]</font><font color=\"#B366FF\"> - <u>the Fallen Angel v1.3</u>  </font>- Kurisu ©");
            try
            {
                Config = new Menu("Kurisu: Morgana", "morgana", true);
                Menu morgOrb = new Menu("Orbwalker", "orbwalker");
                Orbwalker = new Orbwalking.Orbwalker(morgOrb);
                Config.AddSubMenu(morgOrb);
           
                Menu morgTS = new Menu("Target Selector", "target selecter");
                SimpleTs.AddToMenu(morgTS);
                Config.AddSubMenu(morgTS);

                Menu morgDraws = new Menu("Morgana: Drawings", "drawings");
                morgDraws.AddItem(new MenuItem("drawQ", "Draw Q")).SetValue(new Circle(true, Color.FromArgb(150, Color.MediumSlateBlue)));
                morgDraws.AddItem(new MenuItem("drawW", "Draw W")).SetValue(new Circle(false, Color.FromArgb(150, Color.DarkSlateBlue)));
                morgDraws.AddItem(new MenuItem("drawE", "Draw E")).SetValue(new Circle(true, Color.FromArgb(150, Color.DarkSlateBlue)));
                morgDraws.AddItem(new MenuItem("drawR", "Draw R")).SetValue(new Circle(true, Color.FromArgb(150, Color.MediumSlateBlue)));
                Config.AddSubMenu(morgDraws);

                Menu morgBind = new Menu("Morgana: Dark Binding", "bind");
                morgBind.AddItem(new MenuItem("useq", "Use Q")).SetValue(true);
                morgBind.AddItem(new MenuItem("minuseq", "Min distance Q")).SetValue(new Slider(50, 50, 250));
                morgBind.AddItem(new MenuItem("", ""));
                morgBind.AddItem(new MenuItem("qdash", "Auto Q Dashing")).SetValue(true);
                morgBind.AddItem(new MenuItem("qimmobile", "Auto Q Immoble")).SetValue(true);
                morgBind.AddItem(new MenuItem("qgap", "Auto Q Gapcloser")).SetValue(true);
                Config.AddSubMenu(morgBind);

                Menu morgSoil = new Menu("Morgana: Torment Soil", "soil");
                morgSoil.AddItem(new MenuItem("usew", "Use W")).SetValue(true);
                morgSoil.AddItem(new MenuItem("", ""));
                morgSoil.AddItem(new MenuItem("wimmobile", "Use W only on immobile")).SetValue(true);
                Config.AddSubMenu(morgSoil);

                Menu morgShield = new Menu("Morgana: Black Shield", "shield");
                morgShield.AddItem(new MenuItem("usee", "Use E")).SetValue(true);
                morgShield.AddItem(new MenuItem("minshieldpct", "Minumum mana %")).SetValue(new Slider(40, 0, 100));
                morgShield.AddItem(new MenuItem("edangerous", "Shield dangerous spells")).SetValue(true);
                var allies = from hero in ObjectManager.Get<Obj_AI_Hero>()
                            where hero.IsAlly == true
                           select hero.SkinName;
                foreach (string a in allies)
                    morgShield.AddItem(new MenuItem("shield" + a, a)).SetValue(true);

                Config.AddSubMenu(morgShield);

                #region L# Reqs
                Game.OnGameUpdate += Game_OnGameUpdate;
                Obj_AI_Base.OnProcessSpellCast += Game_OnProcessSpellCast;
                AntiGapcloser.OnEnemyGapcloser += Game_OnGapCloser;
                Drawing.OnDraw += Game_DrawingOnDraw;
                #endregion

                SetSkills();
                Config.AddToMainMenu();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Game.PrintChat("Error occured with morgana assembly(OnLoadMenu)");
            }
        }

        private void Game_DrawingOnDraw(EventArgs args)
        {
            foreach (var spell in SpellList)
            {
                var circle = Config.SubMenu("drawings").Item("draw" + spell.Slot.ToString()).GetValue<Circle>();
                if (circle.Active)
                    Utility.DrawCircle(Player.Position, spell.Range, circle.Color, 5, 55);
                    
            }

        }

        private void Game_OnGapCloser(ActiveGapcloser sender)
        {
            if (Config.Item("useq").GetValue<bool>() && Config.Item("qgap").GetValue<bool>())
                    darkbinding.Cast(sender);
        }

        private void Game_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (Config.Item("usee").GetValue<bool>() && Config.Item("edangerous").GetValue<bool>())
            {
                
                Obj_AI_Hero attacker = ObjectManager.Get<Obj_AI_Hero>().First(n => n.NetworkId == sender.NetworkId);
                if (sender.Type == GameObjectType.obj_AI_Hero && sender.IsEnemy)
                {
                    Console.WriteLine(args.SData.Name);
                    if (args.Target.Type == GameObjectType.obj_AI_Hero && args.Target.IsAlly && Player.Distance(args.Target.Position) < blackshield.Range)
                    {   
                            Obj_AI_Hero target = ObjectManager.Get<Obj_AI_Hero>().First(n => n.NetworkId == args.Target.NetworkId);
                            if (target != null && KurisuLib.CCList.Any(cc => cc.Contains(args.SData.Name)) && Config.Item("shield" + target.SkinName).GetValue<bool>())
                                blackshield.CastOnUnit(target, true);
                                //Console.WriteLine(target.SkinName);
                    }
                   
                }
            }
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            Obj_AI_Hero _target = SimpleTs.GetTarget(1150, SimpleTs.DamageType.Magical);
            DarkBinding(_target);
            TormentedSoil(_target);

            Utility.DrawCircle(_target.Position, _target.BoundingRadius, Color.MediumSlateBlue);
        }

        private IEnumerable<Obj_AI_Hero> autoSoilTarget()
        {
            var targets = from hero in ObjectManager.Get<Obj_AI_Hero>()
                         where hero.Team != Player.Team && Vector2.DistanceSquared(Player.Position.To2D(), 
                               hero.ServerPosition.To2D()) < tormentsoil.Range * tormentsoil.Range
                        select hero;

            return targets;
        }

        private IEnumerable<Obj_AI_Hero> GetShackleTargets(GameObject target)
        {
            var enemies = from hero in ObjectManager.Get<Obj_AI_Hero>()
                         where hero.IsEnemy && hero.IsValid &&
                               hero.Distance(target.Position) < soulshackle.Range
                        select hero;

            return enemies;
        }

        private void DarkBinding(Obj_AI_Base target)
        {
            PredictionOutput predicition = darkbinding.GetPrediction(target);
            if (predicition.Hitchance == HitChance.High && Orbwalker.ActiveMode.ToString() == "Combo")
                if (Config.Item("useq").GetValue<bool>() && darkbinding.IsReady())
                    darkbinding.Cast(predicition.CastPosition);

            if (predicition.Hitchance == HitChance.Immobile)
                if (Config.Item("useq").GetValue<bool>() && Config.Item("qimmobile").GetValue<bool>() && darkbinding.IsReady())
                    darkbinding.Cast(predicition.CastPosition);

            if (predicition.Hitchance == HitChance.Dashing)
                if (Config.Item("useq").GetValue<bool>() && Config.Item("qdash").GetValue<bool>() && darkbinding.IsReady())
                    darkbinding.Cast(predicition.CastPosition);
        }

        private void TormentedSoil(Obj_AI_Base target)
        {
            PredictionOutput prediction = tormentsoil.GetPrediction(target);
            if (prediction.Hitchance == HitChance.Medium)
                if (Config.Item("usew").GetValue<bool>() && tormentsoil.IsReady() && !Config.Item("wimmobile").GetValue<bool>() && Orbwalker.ActiveMode.ToString() == "Combo")
                    tormentsoil.Cast(prediction.CastPosition);
            if (Config.Item("wimmobile").GetValue<bool>())
            {
                foreach (var enemy in autoSoilTarget())
                {
                    if (prediction.Hitchance == HitChance.Immobile)
                        tormentsoil.Cast(prediction.CastPosition);
                }
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
