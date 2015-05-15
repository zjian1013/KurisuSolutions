using System.Collections.Generic;
using LeagueSharp;

namespace Activator
{
    public class gametroy
    {
        public float Damage;
        public bool Included;
        public string Name;
        public GameObject Obj;
        public Obj_AI_Hero Owner;
        public SpellSlot Slot;
        public int Start;

        public gametroy(
            Obj_AI_Hero owner, 
            SpellSlot slot, 
            string name, 
            int start, 
            bool inculded, 
            float incdmg = 0,
            GameObject obj = null)
        {
            Owner = owner;
            Slot = slot;
            Start = start;
            Name = name;
            Obj = obj;
            Included = inculded;
            Damage = incdmg;
        }

        public static List<gametroy> Troys = new List<gametroy>(); 

        static gametroy()
        {
            
        }
    }
}
