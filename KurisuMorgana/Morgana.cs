using LeagueSharp;
using LeagueSharp.Common;
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
            q.SetSkillshot(0.25f, 72f, 1200f, true, SkillshotType.SkillshotLine);
            w.SetSkillshot(0.25f, 175f, 1200f, false, SkillshotType.SkillshotCircle);
        }


        public static void CastCombo(Obj_AI_Base unit)
        {
            if (KurisuMorgana.Config.Item("useQ").GetValue<bool>())
                CastSmartQ(unit);
            if (KurisuMorgana.Config.Item("useW").GetValue<bool>())
                CastSmartW(unit);

        }

        public static void CastHarass(Obj_AI_Base unit)
        {
            if (KurisuMorgana.Config.Item("useW2").GetValue<bool>())
                CastSmartW(unit);
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

            if (po.Hitchance == HitChance.Immobile)
                w.Cast(unit, true);

            if (po.Hitchance == HitChance.Low)
                w.Cast(po.CastPosition);

        }

        public static void CastSmartE(Obj_AI_Base unit)
        {

        }

        public static bool HasMana()
        {
            if (qdata.ManaCost < me.Mana)
                return false;
            if (wdata.ManaCost < me.Mana)
                return false;
            if (edata.ManaCost < me.Mana)
                return false;
            if (rdata.ManaCost < me.Mana)
                return false;
            else
                return true;
        }


    }
}
