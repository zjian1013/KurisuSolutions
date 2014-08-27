using LeagueSharp;
using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace K4Nami
{
    class Nami
    {
        public static Orbwalking.Orbwalker orbwalker;
        public static Obj_AI_Hero Player = ObjectManager.Player;
        public static Spellbook SB = Player.Spellbook;

        public static float QDelay = 0.5f;
        public static float QSpeed = 1750f;
        public static float QWidth = 200f;
        public static float QRange = 875f;

        public static Spell Q = new Spell(SpellSlot.Q, 875);
        public static Spell W = new Spell(SpellSlot.W, 725);
        public static Spell E = new Spell(SpellSlot.E, 800);

        public static void SetQ()
        {      
            Q.SetSkillshot(QDelay, QWidth, QSpeed, false, Prediction.SkillshotType.SkillshotCircle);
        }


        public static void CastCombo(Obj_AI_Base target) 
        {
            CastQ(target);
            CastW(target);
            CastE(target);
        }

        public static void CastQ(Obj_AI_Base target)
        {

        }

        public static void CastW(Obj_AI_Base target)
        {

        }

        public static void CastE(Obj_AI_Base target)
        {

        }



    }

   
}
