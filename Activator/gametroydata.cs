#region Copyright © 2015 Kurisu Solutions
// All rights are reserved. Transmission or reproduction in part or whole,
// any form or by any means, mechanical, electronical or otherwise, is prohibited
// without the prior written consent of the copyright owner.
// 
// Document:	activator/gametroydata.cs
// Date:		06/06/2015
// Author:		Robin Kurisu
#endregion

using LeagueSharp;
using System.Collections.Generic;

namespace Activator
{
    public class gametroydata
    {
        public string Name { get; set; }
        public string ChampionName { get; set; }
        public SpellSlot Slot { get; set; }
        public float Radius { get; set; }
        public HitType[] HitType { get; set; }

        public static List<gametroydata> troydata = new List<gametroydata>(); 

        static gametroydata()
        {
            troydata.Add(new gametroydata
            {
                Name = "katarina_deathLotus_tar",
                ChampionName = "Katarina",
                Radius = 550f,
                Slot = SpellSlot.R,
                HitType = new[] { global::Activator.HitType.None }
            });

            troydata.Add(new gametroydata
            {
                Name = "Nautilus_R_sequence_impact",
                ChampionName = "Nautilus",
                Radius = 250f,
                Slot = SpellSlot.R,
                HitType = new[] { global::Activator.HitType.CrowdControl, global::Activator.HitType.Danger }
            });

            troydata.Add(new gametroydata
            {
                Name = "Acidtrail_buf",
                ChampionName = "Singed",
                Radius = 200f,
                Slot = SpellSlot.Q,
                HitType = new []{ global::Activator.HitType.None }
            });

            troydata.Add(new gametroydata
            {
                Name = "Tremors_cas",
                ChampionName = "Rammus",
                Radius = 450f,
                Slot = SpellSlot.R,
                HitType = new[] { global::Activator.HitType.None }
            });

            troydata.Add(new gametroydata
            {
                Name = "Crowstorm",
                ChampionName = "FiddleSticks",
                Radius = 450f,
                Slot = SpellSlot.R,
                HitType = new []{ global::Activator.HitType.Danger, global::Activator.HitType.Ultimate}
            });

            troydata.Add(new gametroydata
            {
                Name = "caitlyn_Base_yordleTrap_idle",
                ChampionName = "Caitlyn",
                Radius = 180f,
                Slot = SpellSlot.W,
                HitType = new[] { global::Activator.HitType.CrowdControl }
            });

            troydata.Add(new gametroydata
            {
                Name = "LuxLightstrike_tar",
                ChampionName = "Lux",
                Radius = 400f,
                Slot = SpellSlot.E,
                HitType = new[] { global::Activator.HitType.None }
            });

            troydata.Add(new gametroydata
            {
                Name = "ViktorChaosStorm",
                ChampionName = "Viktor",
                Radius = 425f,
                Slot = SpellSlot.R,
                HitType =
                    new[]
                    {
                        global::Activator.HitType.Danger, global::Activator.HitType.Ultimate,
                        global::Activator.HitType.CrowdControl
                    }
            });

            troydata.Add(new gametroydata
            {
                Name = "ViktorCatalyst",
                ChampionName = "Viktor",
                Radius = 400f,
                Slot = SpellSlot.W,
                HitType = new[] { global::Activator.HitType.CrowdControl }
            });

            troydata.Add(new gametroydata
            {
                Name = "cryo_storm",
                ChampionName = "Anivia",
                Radius = 450f,
                Slot = SpellSlot.R,
                HitType = new[] { global::Activator.HitType.CrowdControl }
            });

            troydata.Add(new gametroydata
            {
                Name = "ZiggsE",
                ChampionName = "Ziggs",
                Radius = 400f,
                Slot = SpellSlot.E,
                HitType = new []{ global::Activator.HitType.CrowdControl }
            });

            troydata.Add(new gametroydata
            {
                Name = "ZiggsWRing",
                ChampionName = "Ziggs",
                Radius = 350f,
                Slot = SpellSlot.W,
                HitType = new []{ global::Activator.HitType.CrowdControl }
            });

            troydata.Add(new gametroydata
            {
                Name = "CassMiasma_tar",
                ChampionName = "Cassiopeia",
                Radius = 350f,
                Slot = SpellSlot.W,
                HitType = new[] { global::Activator.HitType.CrowdControl, global::Activator.HitType.Spell }
            });

            troydata.Add(new gametroydata
            {
                Name = "Soraka_Base_E_rune",
                ChampionName = "Soraka",
                Radius = 375f,
                Slot = SpellSlot.E,
                HitType = new[] { global::Activator.HitType.CrowdControl }
            });

            troydata.Add(new gametroydata
            {
                Name = "W_Tar",
                ChampionName = "Morgana",
                Radius = 375f,
                Slot = SpellSlot.W,
                HitType = new []{ global::Activator.HitType.None }
            });
        }
    }
}
