using LeagueSharp;
using System.Collections.Generic;

namespace Activator
{
    public class champion
    {
        public float IncomeDamage;
        public Obj_AI_Hero Player;
        public Obj_AI_Base Attacker;
        public int QSSBuffCount;
        public int QSSHighestBuffTime;
        public bool ForceQSS;

        public List<HitType> HitTypes = new List<HitType>();

        public static List<champion> Heroes = new List<champion>(); 

        public champion(Obj_AI_Hero player, float incdmg)
        {
            Player = player;
            IncomeDamage = incdmg;
        }
    }

}
