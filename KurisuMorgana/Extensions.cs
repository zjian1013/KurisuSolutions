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
    internal class Extensions
    {
        internal static Obj_AI_Hero me = ObjectManager.Player;


        /// <summary>
        /// Object Lists
        /// </summary>
        public static List<Obj_AI_Hero> adclist = ObjectManager.Get<Obj_AI_Hero>().Where(
            h => h.Team == me.Team && h.IsValid).OrderByDescending(h => h.FlatPhysicalDamageMod).ToList();
        public static List<Obj_AI_Hero> apclist = ObjectManager.Get<Obj_AI_Hero>().Where(
            h => h.Team == me.Team && h.IsValid).OrderByDescending(h => h.FlatMagicDamageMod).ToList();
        public static List<Obj_AI_Hero> tanklist = ObjectManager.Get<Obj_AI_Hero>().Where(
            h => h.Team == me.Team && h.IsValid).OrderByDescending(h => h.FlatHPPoolMod).ToList();

        public static IEnumerable<Obj_AI_Hero> autoBindTarget = ObjectManager.Get<Obj_AI_Hero>().Where(
            h => h.Team != me.Team && Vector2.DistanceSquared(me.Position.To2D(), h.ServerPosition.To2D()) < Morgana.q.Range * Morgana.q.Range);

        public static IEnumerable<Obj_AI_Hero> autoSoilTarget = ObjectManager.Get<Obj_AI_Hero>().Where(
            h => h.Team != me.Team && Vector2.DistanceSquared(me.Position.To2D(), h.ServerPosition.To2D()) < Morgana.w.Range * Morgana.w.Range );

        /// <sumary>
        /// Spell Lists
        /// </sumary>

        public static readonly List<Spell> SpellList = new List<Spell>();

     }

}

