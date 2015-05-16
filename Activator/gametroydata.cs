using LeagueSharp;

namespace Activator
{
    public class gametroydata
    {
        public string Name { get; set; }
        public string ChampionName { get; set; }
        public SpellSlot Slot { get; set; }
        public HitType[] HitType { get; set; }

        static gametroydata()
        {
            spelldata.troydata.Add( new gametroydata
            {
                Name = "Nautilus_R_sequence_impact",
                ChampionName = "Nautilus",
                Slot = SpellSlot.R,
                HitType = new[] { global::Activator.HitType.CrowdControl, global::Activator.HitType.Danger }
            });

            spelldata.troydata.Add(new gametroydata
            {
                Name = "Acidtrail_buf",
                ChampionName = "Singed",
                Slot = SpellSlot.Q,
                HitType = new []{ global::Activator.HitType.Spell }
            });

            spelldata.troydata.Add(new gametroydata
            {
                Name = "Tremors_cas",
                ChampionName = "Rammus",
                Slot = SpellSlot.R,
                HitType = new[] { global::Activator.HitType.Spell }
            });

            spelldata.troydata.Add(new gametroydata
            {
                Name = "Crowstorm",
                ChampionName = "FiddleSticks",
                Slot = SpellSlot.R,
                HitType = new []{ global::Activator.HitType.Danger, global::Activator.HitType.Ultimate}
            });

            spelldata.troydata.Add(new gametroydata
            {
                Name = "caitlyn_Base_yordleTrap_idle",
                ChampionName = "Caitlyn",
                Slot = SpellSlot.W,
                HitType = new[] { global::Activator.HitType.CrowdControl }
            });

            spelldata.troydata.Add(new gametroydata
            {
                Name = "LuxLightstrike_tar",
                ChampionName = "Lux",
                Slot = SpellSlot.E,
                HitType = new []{global::Activator.HitType.CrowdControl }
            });

            spelldata.troydata.Add(new gametroydata
            {
                Name = "ViktorChaosStorm",
                ChampionName = "Viktor",
                Slot = SpellSlot.R,
                HitType =
                    new[]
                    {
                        global::Activator.HitType.Danger, global::Activator.HitType.Ultimate,
                        global::Activator.HitType.CrowdControl
                    }
            });

            spelldata.troydata.Add(new gametroydata
            {
                Name = "ViktorCatalyst",
                ChampionName = "Viktor",
                Slot = SpellSlot.W,
                HitType = new[] { global::Activator.HitType.CrowdControl }
            });

            spelldata.troydata.Add(new gametroydata
            {
                Name = "cryo_storm",
                ChampionName = "Anivia",
                Slot = SpellSlot.R,
                HitType = new[] { global::Activator.HitType.CrowdControl }
            });

            spelldata.troydata.Add(new gametroydata
            {
                Name = "ZiggsE",
                ChampionName = "Ziggs",
                Slot = SpellSlot.E,
                HitType = new []{ global::Activator.HitType.CrowdControl }
            });

            spelldata.troydata.Add(new gametroydata
            {
                Name = "ZiggsWRing",
                ChampionName = "Ziggs",
                Slot = SpellSlot.W,
                HitType = new []{ global::Activator.HitType.CrowdControl }
            });

            spelldata.troydata.Add(new gametroydata
            {
                Name = "CassMiasma_tar",
                ChampionName = "Cassiopeia",
                Slot = SpellSlot.W,
                HitType = new[] { global::Activator.HitType.CrowdControl, global::Activator.HitType.Spell }
            });

            spelldata.troydata.Add(new gametroydata
            {
                Name = "Soraka_Base_E_rune",
                ChampionName = "Soraka",
                Slot = SpellSlot.E,
                HitType = new[] { global::Activator.HitType.CrowdControl }
            });

            spelldata.troydata.Add(new gametroydata
            {
                Name = "Morgana_Base_W_Tar",
                ChampionName = "Morgana",
                Slot = SpellSlot.W,
                HitType = new []{ global::Activator.HitType.Spell  }
            });

        }
    }
}
