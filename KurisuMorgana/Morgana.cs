using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KurisuMorgana
{
    internal class Morgana
    {
        public static Obj_AI_Hero me = ObjectManager.Player;
        public static Spellbook spellbook = me.Spellbook;
        public static Orbwalking.Orbwalker orbwalker;

        public static Spell q = new Spell(SpellSlot.Q, 1175);
        public static Spell w = new Spell(SpellSlot.W, 900);
        public static Spell e = new Spell(SpellSlot.E, 750);
        public static Spell r = new Spell(SpellSlot.R, 600);

        public static SpellDataInst qdata = spellbook.GetSpell(SpellSlot.Q);
        public static SpellDataInst wdata = spellbook.GetSpell(SpellSlot.W);
        public static SpellDataInst edata = spellbook.GetSpell(SpellSlot.E);
        public static SpellDataInst rdata = spellbook.GetSpell(SpellSlot.R);            

        public static void SetSkills()
        {
            Extensions.SpellList.AddRange(new[] { q, w, e, r });
            q.SetSkillshot(0.25f, 72f, 1200f, true, SkillshotType.SkillshotLine);
            w.SetSkillshot(0.25f, 175f, 1200f, false, SkillshotType.SkillshotCircle);
        }


        public static void CastCombo(Obj_AI_Base unit)
        {
            if (KurisuMorgana.Config.SubMenu("combo").Item("useQ").GetValue<bool>())
                CastSmartQ(unit);
            if (KurisuMorgana.Config.SubMenu("combo").Item("useW").GetValue<bool>())
                CastSmartW(unit);

        }

        public static void CastHarass(Obj_AI_Base unit)
        {

            if (KurisuMorgana.Config.SubMenu("harass").Item("useW2").GetValue<bool>())
            {
                if (me.Mana > KurisuMorgana.Config.SubMenu("harass").Item("harassPct").GetValue<Slider>().Value)
                {
                    CastSmartW(unit);
                }
            }
                
        }

        public static void CastSmartQ(Obj_AI_Base unit)
        {
            PredictionOutput po = q.GetPrediction(unit);

            if (!q.IsReady())
                return;

            if (po.Hitchance == HitChance.High)
                q.Cast(po.CastPosition, true);
        }

        public static void CastSmartW(Obj_AI_Base unit)
        {
            PredictionOutput po = w.GetPrediction(unit);

            if (!w.IsReady())
                return;
            if (q.Collision && KurisuMorgana.Config.SubMenu("combo").Item("useWif").GetValue<bool>())
                return;
            if (po.Hitchance == HitChance.High)
                w.Cast(po.CastPosition, true);

        }

        public static void CastAutoW()
        {
            foreach (var enemy in Extensions.autoSoilTarget)
            {
                var po = w.GetPrediction(enemy);
                if (po.Hitchance == HitChance.Immobile)
                    w.Cast(po.CastPosition);
                        break;
            }

        }

        public static void CastAutoQ()
        {
            foreach (var enemy in Extensions.autoBindTarget)
            {
                var po = q.GetPrediction(enemy);
                if (po.Hitchance == HitChance.Immobile)
                    q.Cast(po.CastPosition);
                break;
            }
        }

        public static void Laneclear()
        {
            var mPos = MinionManager.GetBestCircularFarmLocation(MinionManager.GetMinions(me.Position, w.Range).Select(m => m.ServerPosition.To2D()).ToList(), w.Width, w.Range);
            if (KurisuMorgana.Config.SubMenu("laneclear").Item("wclear").GetValue<bool>() && w.IsReady())
            {
                if (mPos.MinionsHit >= KurisuMorgana.Config.SubMenu("laneclear").Item("wclearNum").GetValue<Slider>().Value && me.Distance(mPos.Position) <= w.Range)
                {
                    if (me.Mana > KurisuMorgana.Config.SubMenu("laneclear").Item("wclearPct").GetValue<Slider>().Value)
                        w.Cast(mPos.Position);
                }
            }

        }

    }
}
