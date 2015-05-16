using LeagueSharp;
using System.Collections.Generic;

namespace Activator
{
    public class champion
    {
        public float IncomeDamage;
        public Obj_AI_Hero Player;
        public Obj_AI_Base Attacker;

        public bool ForceQSS;
        public bool UsingManaPot;
        public bool UsingHealthPot;
        public bool UsingMixedPot;

        public int QSSBuffCount;
        public int QSSHighestBuffTime;

        public List<HitType> HitTypes = new List<HitType>();

        public static List<champion> Heroes = new List<champion>(); 

        public champion(Obj_AI_Hero player, float incdmg)
        {
            Player = player;
            IncomeDamage = incdmg;
        }
    }
}
