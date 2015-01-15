using LeagueSharp;

namespace Oracle
{
    public struct GameObj
    {
        public float Damage;
        public bool Included;
        public string Name;
        public GameObject Obj;

        public GameObj(string name, GameObject obj, bool included, float incdmg)
        {
            Name = name;
            Obj = obj;
            Included = included;
            Damage = incdmg;
        }
    }
}