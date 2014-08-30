using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KurisuNami
{
    class NamiLogic
    {
        public static Orbwalking.Orbwalker orbwalker;
        public static Spellbook Spells = Player.Spellbook;
        public static Obj_AI_Hero Player = ObjectManager.Player;

        public static Spell Q = new Spell(SpellSlot.Q, 750);
        public static Spell W = new Spell(SpellSlot.W, 725);
        public static Spell E = new Spell(SpellSlot.E, 800);
        public static Spell R = new Spell(SpellSlot.R, 2750);
         
        public static void CastCombo(Obj_AI_Base target)
        {

            
        }


    }
}
