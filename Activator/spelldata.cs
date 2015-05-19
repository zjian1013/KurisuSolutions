using System.Collections.Generic;
using Activator.Items;
using Activator.Spells;
using Activator.Summoners;
using LeagueSharp;
using LeagueSharp.Common;

namespace Activator
{
    public class spelldata
    {
        public string SDataName { get; set; }
        public string MenuName { get; set; }
        public bool Wait { get; set; }
        public int WaitDelay { get; set; }
        public int DangerLevel { get; set; }
        public HitType[] HitType { get; set; }

        public static List<item> items = new List<item>(); 
        public static List<spell> mypells = new List<spell>();
        public static List<summoner> summoners = new List<summoner>();
        public static List<spelldata> spells = new List<spelldata>();
        public static List<gametroydata> troydata = new List<gametroydata>(); 
        public static Dictionary<SpellDamageDelegate, SpellSlot> combodelagate = new Dictionary<SpellDamageDelegate, SpellSlot>();

        static spelldata()
        {
            spells.Add(new spelldata
            {
                SDataName = "aatroxq",
                MenuName = "Dark Flight | Q",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 3,
                HitType = new []{ global::Activator.HitType.CrowdControl, global::Activator.HitType.Danger }
            });

            spells.Add(new spelldata
            {
                SDataName = "aatroxe",
                MenuName = "Blades of Torment | E",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 2,
                HitType = new[] { global::Activator.HitType.CrowdControl, global::Activator.HitType.Danger }
            });

            spells.Add(new spelldata
            {
                SDataName = "ahriseduce",
                MenuName = "Charm | E",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 4,
                HitType = new[] { global::Activator.HitType.Danger, global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "akalimota",
                MenuName = "Mark of the Assassin | Q",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 2,
                HitType = new []{ global::Activator.HitType.Danger, }
            });

            spells.Add(new spelldata
            {
                SDataName = "akalismokebomb",
                MenuName = "Twilight Shroud | W",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 0,
                HitType = new[] { global::Activator.HitType.Stealth }
            });

            spells.Add(new spelldata
            {
                SDataName = "pulverize",
                MenuName = "Pulverize | Q",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 3,
                HitType = new [] { global::Activator.HitType.Danger, global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "headbutt",
                MenuName = "Headbutt | W",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 2,
                HitType = new[] { global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "bandagetoss",
                MenuName = "Bandage Toss | Q",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 3,
                HitType = new []{ global::Activator.HitType.Danger, global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "curseofthesadmummy",
                MenuName = "Curse Sad Mummy | R",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 5,
                HitType =
                    new[]
                    {
                        global::Activator.HitType.Danger, global::Activator.HitType.Ultimate,
                        global::Activator.HitType.CrowdControl
                    }
            });

            spells.Add(new spelldata
            {
                SDataName = "flashfrost",
                MenuName = "Flash Frost | Q",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 2,
                HitType = new []{global::Activator.HitType.CrowdControl}
            });

            spells.Add(new spelldata
            {
                SDataName = "frostbite",
                MenuName = "Frostbite | E",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 4,
                HitType = new []{ global::Activator.HitType.Danger}
            });

            spells.Add(new spelldata
            {
                SDataName = "glacialstorm",
                MenuName = "Glacial Storm | R",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 2,
                HitType = new []{ global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "infernalguardian",
                MenuName = "Tibbers | R",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 5,
                HitType =
                    new[]
                    {
                        global::Activator.HitType.Danger, global::Activator.HitType.Ultimate,
                        global::Activator.HitType.CrowdControl
                    }
            });

            spells.Add(new spelldata
            {
                SDataName = "volley",
                MenuName = "Volley | W",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 3,
                HitType = new []{ global::Activator.HitType.Danger, global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "enchantedcrystalarrow",
                MenuName = "Enchanted Arrow | R",
                Wait = true,
                WaitDelay = 0,
                DangerLevel = 5,
                HitType =
                    new[]
                    {
                        global::Activator.HitType.Ultimate, global::Activator.HitType.Danger,
                        global::Activator.HitType.CrowdControl
                    }
            });

            spells.Add(new spelldata
            {
                SDataName = "bardq",
                MenuName = "Cosmic Binding | Q",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 3,
                HitType = new []{ global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "rocketgrabmissile",
                MenuName = "Rocket Grab | Q",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 5,
                HitType = new []{ global::Activator.HitType.Danger, global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "powerfist",
                MenuName = "Power Fist | E",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 3,
                HitType = new[] { global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName  = "staticfield",
                MenuName = "Static Field | R",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 2,
                HitType = new []{ global::Activator.HitType.Danger, global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "brandblaze",
                MenuName = "Sear | Q",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 3,
                HitType = new[] { global::Activator.HitType.Danger, global::Activator.HitType.CrowdControl }

            });

            spells.Add(new spelldata
            {
                SDataName = "brandwildfire",
                MenuName = "Pyroclasm | R", 
                Wait = true,
                WaitDelay = 100,
                DangerLevel = 5,
                HitType = new []{global::Activator.HitType.Ultimate, global::Activator.HitType.Danger }
            });

            spells.Add(new spelldata
            {
                SDataName = "braumq",
                MenuName = "Winter's Bite | Q",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 3,
                HitType = new []{ global::Activator.HitType.CrowdControl, global::Activator.HitType.Danger }
            });

            spells.Add(new spelldata
            {
                SDataName = "braumqmissile",
                MenuName = "Winter's Bite Missile",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 3,
                HitType = new []{ global::Activator.HitType.Danger, global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "braumrwrapper",
                MenuName = "Glacial Fissure | R",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 5,
                HitType =
                    new[]
                    {
                        global::Activator.HitType.Danger, global::Activator.HitType.Ultimate,
                        global::Activator.HitType.CrowdControl
                    }
            });

            spells.Add(new spelldata
            {
                SDataName = "caitlynentrapment",
                MenuName = "90 Caliber Net | E",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 2,
                HitType = new[] { global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "caitlynaceinthehole",
                MenuName = "Ace in the Hole | R",
                Wait = true,
                WaitDelay = 1000,
                DangerLevel = 5,
                HitType = new[] { global::Activator.HitType.Danger, global::Activator.HitType.Ultimate }
            });

            spells.Add(new spelldata
            {
                SDataName = "cassiopeianoxiousblast",
                MenuName = "Noxious Blast | Q",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 3,
                HitType = new[] { global::Activator.HitType.Danger }
            });

            spells.Add(new spelldata
            {
                SDataName = "cassiopeiapetrifyinggaze",
                MenuName = "Petrifying Gaze | R",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 5,
                HitType =
                    new[]
                    {
                        global::Activator.HitType.Danger, global::Activator.HitType.Ultimate,
                        global::Activator.HitType.CrowdControl
                    }
            });

            spells.Add(new spelldata
            {
                SDataName = "rupture",
                MenuName = "Rupture | Q",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 3,
                HitType = new []{ global::Activator.HitType.Danger, global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "feralscream",
                MenuName = "Feral Scream | W",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 2,
                HitType = new[] { global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "feast",
                MenuName = "Feast | R",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 4,
                HitType = new[] {  global::Activator.HitType.Danger, global::Activator.HitType.Ultimate }
            });

            spells.Add(new spelldata
            {
                SDataName = "dariuscleave",
                MenuName = "Decimate | Q",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 3,
                HitType = new []{ global::Activator.HitType.Danger}
            });

            spells.Add(new spelldata
            {
                SDataName = "dariusgrabcone",
                MenuName = "Apprehend | E",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 1,
                HitType = new []{ global::Activator.HitType.CrowdControl, global::Activator.HitType.Danger }
            });

            spells.Add(new spelldata
            {
                SDataName = "darusexecute",
                MenuName = "Noxian Guillotine | R",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 4,
                HitType = new []{ global::Activator.HitType.Danger, global::Activator.HitType.Ultimate }
            });

            spells.Add(new spelldata
            {
                SDataName = "dianaarc",
                MenuName = "Crescent Strike | Q",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 2,
                HitType = new []{ global::Activator.HitType.Danger }
            });

            spells.Add(new spelldata
            {
                SDataName = "dianavortex",
                MenuName = "Moonfall | E",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 3,
                HitType = new[] { global::Activator.HitType.Danger, global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "dianateleport",
                MenuName = "Lunar Rush | R",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 4,
                HitType = new []{ global::Activator.HitType.Danger, global::Activator.HitType.Ultimate }
            });

            spells.Add(new spelldata
            {
                SDataName = "dravendoubleshot",
                MenuName = "Stand Aside | E",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 3,
                HitType = new []{ global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "dravenrcast",
                MenuName = "Whirling Death | R",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 3,
                HitType = new[] {  global::Activator.HitType.Danger, global::Activator.HitType.Ultimate }
            });

            spells.Add(new spelldata
            {
                SDataName = "infectedcleavermissilecast",
                MenuName = "Infected Cleaver | Q",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 2,
                HitType = new[] { global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "elisehumane",
                MenuName = "Cocoon | E",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 4,
                HitType = new[] { global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "evelynnr",
                MenuName = "Agony's Embrace | R",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 5,
                HitType =
                    new[]
                    {
                        global::Activator.HitType.Danger, global::Activator.HitType.Ultimate,
                        global::Activator.HitType.CrowdControl
                    }
            });

            spells.Add(new spelldata
            {
                SDataName = "ezrealtrueshotbarrage",
                MenuName = "Trueshot Barrage | R",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 4,
                HitType = new[] { global::Activator.HitType.Danger, global::Activator.HitType.Ultimate }
            });

            spells.Add(new spelldata
            {
                SDataName =  "terrify",
                MenuName =  "Terrify | Q",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 4,
                HitType = new[] { global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "fiddlesticksdarkwind",
                MenuName = "Dark Wind | E",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 1,
                HitType = new[] { global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "fioradance",
                MenuName = "Blade Waltz | R",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 5,
                HitType = new []{ global::Activator.HitType.Danger, global::Activator.HitType.Ultimate }
            });

            spells.Add(new spelldata
            {
                SDataName = "fizzjumptwo",
                MenuName =  "Playful / Trickster | E",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 3,
                HitType = new[] { global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "galioresolutesmite",
                MenuName = "Resolute Smite | Q",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 3,
                HitType = new[] { global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "galioidolofdurand",
                MenuName = "Idol of Durand | R",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 5,
                HitType =
                    new[]
                    {
                        global::Activator.HitType.CrowdControl, global::Activator.HitType.Ultimate,
                        global::Activator.HitType.Danger
                    }
            });

            spells.Add(new spelldata
            {
                SDataName = "parley",
                MenuName = "Parley | Q",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 3,
                HitType = new[] { global::Activator.HitType.AutoAttack, global::Activator.HitType.Danger }
            });

            spells.Add(new spelldata
            {
                SDataName = "garenq",
                MenuName = "Decisive Strike | Q",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 2,
                HitType = new[] { global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "garenr",
                MenuName = "Demacian Justice | R",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 5,
                HitType = new[] { global::Activator.HitType.Danger, global::Activator.HitType.Ultimate }
            });

            spells.Add(new spelldata
            {
                SDataName = "gragasq",
                MenuName = "Barrel Roll | Q",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 2,
                HitType = new[] { global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "gragase",
                MenuName = "Body Slam | E",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 4,
                HitType = new[] { global::Activator.HitType.CrowdControl, global::Activator.HitType.Danger }
            });

            spells.Add(new spelldata
            {
                SDataName = "gragasr",
                MenuName = "Explosive Cask | R",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 5,
                HitType =
                    new[]
                    {
                        global::Activator.HitType.Danger, global::Activator.HitType.Ultimate,
                        global::Activator.HitType.CrowdControl
                    }
            });

            spells.Add(new spelldata
            {
                SDataName = "gravesclustershot",
                MenuName = "Buckshot | Q",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 3,
                HitType = new[] { global::Activator.HitType.Danger }
            });

            spells.Add(new spelldata
            {
                SDataName = "graveschargeshot",
                MenuName = "Collateral Damage | R",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 5,
                HitType = new[] { global::Activator.HitType.Danger, global::Activator.HitType.Ultimate }
            });

            spells.Add(new spelldata
            {
                SDataName = "hecarimult",
                MenuName = "Onslaught of Shadows",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 5,
                HitType =
                    new[]
                    {
                        global::Activator.HitType.Danger, global::Activator.HitType.Ultimate,
                        global::Activator.HitType.CrowdControl
                    }
            });

            spells.Add(new spelldata
            {
                SDataName = "heimerdingere",
                MenuName = "Electric Grenade | E",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 3,
                HitType = new[] { global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "ireliaequilibriumstrike",
                MenuName = "Equilibrium Strike | E",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 4,
                HitType = new[] { global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "howlinggale",
                MenuName = "Howing Gale | Q",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 1,
                HitType = new[] { global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "sowthewind",
                MenuName = "Zephyr | W",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 1,
                HitType = new[] { global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "reapthewhirlwind",
                MenuName = "Monsoon | R",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 1,
                HitType = new[] { global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "jarvanivdragonstrike",
                MenuName = "Dragon Strike | Q",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 3,
                HitType = new[] { global::Activator.HitType.CrowdControl, global::Activator.HitType.Danger }
            });

            spells.Add(new spelldata
            {
                SDataName = "jarvanivcataclysm",
                MenuName = "Cataclysm | R",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 2,
                HitType = new[] { global::Activator.HitType.Ultimate, global::Activator.HitType.Danger }
            });

            spells.Add(new spelldata
            {
                SDataName = "jaxcounterstrike",
                MenuName = "Counter Strike | E",
                Wait = true,
                WaitDelay = 2200,
                DangerLevel = 3,
                HitType = new[] { global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "jaycethunderingblow",
                MenuName = "Thundering Blow | E",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 2,
                HitType = new[] { global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "jinxrwrapper",
                MenuName = "Super Mega Death Rocket | R",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 5,
                HitType = new[] { global::Activator.HitType.Danger, global::Activator.HitType.Ultimate }
            });

            spells.Add(new spelldata
            {
                SDataName = "karmaq",
                MenuName = "Inner Flame | Q",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 3,
                HitType = new[] { global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "kalistamysticshot",
                MenuName = "Pierce | Q",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 1,
                HitType = new[] { global::Activator.HitType.Danger }
            });

            spells.Add(new spelldata
            {
                SDataName = "kalistaexpungewrapper",
                MenuName = "Rend | E",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 3,
                HitType = new[] { global::Activator.HitType.CrowdControl, global::Activator.HitType.Danger }
            });

            spells.Add(new spelldata
            {
                SDataName = "nullance",
                MenuName = "Null Sphere | Q",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 2,
                HitType = new[] { global::Activator.HitType.Danger }
            });

            spells.Add(new spelldata
            {
                SDataName = "forcepulse",
                MenuName = "Force Pulse | E",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 4,
                HitType = new[] { global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "katarinaq",
                MenuName = "Bouncing Blades | Q",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 2,
                HitType = new[] { global::Activator.HitType.Danger }
            });

            spells.Add(new spelldata
            {
                SDataName = "judicatorreckoning",
                MenuName = "Reckoning | Q",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 2,
                HitType = new[] { global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "khazixq",
                MenuName = "Taste Their Fear | Q",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 2,
                HitType = new[] { global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "khazixqlong",
                MenuName = "Taste Their Fear | Q",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 3,
                HitType = new[] { global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "khazixw",
                MenuName = "Void Spike | W",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 1,
                HitType = new[] { global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "khazixwlong",
                MenuName = "Void Spike | W",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 3,
                HitType = new[] { global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "leblancsoulshackle",
                MenuName = "Soul Shackle | E",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 3,
                HitType = new[] { global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "leblancsoulshacklem",
                MenuName = "Soul Shackle | R",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 4,
                HitType = new[] { global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "leblancchaosorb",
                MenuName = "Chaos Orb | Q",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 1,
                HitType = new[] { global::Activator.HitType.Danger }
            });

            spells.Add(new spelldata
            {
                SDataName = "leblancchaosorbm",
                MenuName = "Chaos Orb | R",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 5,
                HitType = new[] { global::Activator.HitType.Danger, global::Activator.HitType.Ultimate }
            });

            spells.Add(new spelldata
            {
                SDataName = "leblancslidem",
                MenuName = "Distortion | R",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 5,
                HitType = new[] { global::Activator.HitType.Danger, global::Activator.HitType.Ultimate }
            });

            spells.Add(new spelldata
            {
                SDataName = "blindmonkqone",
                MenuName = "Sonic Wave | Q",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 3,
                HitType = new[] { global::Activator.HitType.Danger }
            });

            spells.Add(new spelldata
            {
                SDataName = "blindmonkqtwo",
                MenuName = "Lee Singer | Q",
                Wait = false,
                WaitDelay = 0,
                HitType = new[] { global::Activator.HitType.Danger }
            });

            spells.Add(new spelldata
            {
                SDataName = "blindmonkrkick",
                MenuName = "Dragon's Rage | R",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 5,
                HitType = new[] { global::Activator.HitType.Danger, global::Activator.HitType.Ultimate }
            });

            spells.Add(new spelldata
            {
                SDataName = "leonashieldofdaybreak",
                MenuName = "Shield of Daybreak | Q",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 3,
                HitType = new[] { global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "leonasolarflare",
                MenuName = "Solar Flare | R",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 5,
                HitType =
                    new[]
                    {
                        global::Activator.HitType.CrowdControl, global::Activator.HitType.Danger,
                        global::Activator.HitType.Ultimate
                    }
            });

            spells.Add(new spelldata
            {
                SDataName = "lissandraw",
                MenuName = "Ring of Frost | W",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 3,
                HitType = new[] { global::Activator.HitType.CrowdControl, global::Activator.HitType.Danger }
            });

            spells.Add(new spelldata
            {
                SDataName = "lissandrar",
                MenuName = "Frozen Tomb | R",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 5,
                HitType =
                    new[]
                    {
                        global::Activator.HitType.CrowdControl, global::Activator.HitType.Danger,
                        global::Activator.HitType.Ultimate
                    }
            });

            spells.Add(new spelldata
            {
                SDataName = "lucianq",
                MenuName = "Piercing Light | Q",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 2,
                HitType = new[] { global::Activator.HitType.Danger }
            });

            spells.Add(new spelldata
            {
                SDataName = "luluq",
                MenuName = "Glitterlance | Q",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 3,
                HitType = new[] { global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "luluqmissle",
                MenuName = "Glitterlance | Q",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 2,
                HitType = new[] { global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "lulue",
                MenuName = "Help Pix! | E",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 3,
                HitType = new[] { global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "luxlightbinding",
                MenuName = "Light Binding | Q",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 3,
                HitType = new[] { global::Activator.HitType.CrowdControl, global::Activator.HitType.Danger }
            });

            spells.Add(new spelldata
            {
                SDataName = "luxlightstriketoggle",
                MenuName = "Lucent Singularity | E",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 2,
                HitType = new[] { global::Activator.HitType.Danger }
            });

            //spells.Add(new spelldata
            //{
            //    SDataName = "luxmalicecannon",
            //    MenuName = "Final Spark | R",
            //    Wait = false,
            //    Speed = int.MaxValue,
            //    WaitDelay = 0,
            //    DangerLevel = 5,
            //    HitType = new[] { global::Activator.HitType.Danger, global::Activator.HitType.Ultimate }
            //});

            spells.Add(new spelldata
            {
                SDataName = "seismicshard",
                MenuName = "Seismic Shard | Q",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 1,
                HitType = new[] { global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "landslide",
                MenuName = "Ground Slam | E",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 3,
                HitType = new[] { global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "ufslash",
                MenuName = "Unstoppable Force | R",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 5,
                HitType =
                    new[]
                    {
                        global::Activator.HitType.CrowdControl, global::Activator.HitType.Danger,
                        global::Activator.HitType.Ultimate
                    }
            });

            spells.Add(new spelldata
            {
                SDataName = "alzaharnethergrasp",
                MenuName = "Nether Grasp | R",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 5,
                HitType =
                    new[]
                    {
                        global::Activator.HitType.CrowdControl, global::Activator.HitType.Ultimate,
                        global::Activator.HitType.Danger
                    }
            });

            spells.Add(new spelldata
            {
                SDataName = "maokaitrunkline",
                MenuName = "Arcane Smash | Q",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 3,
                HitType = new[] { global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "maokaiunstablegrowth",
                MenuName = "Twisted Advance | W",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 4,
                HitType = new[] { global::Activator.HitType.CrowdControl, global::Activator.HitType.Danger }
            });

            spells.Add(new spelldata
            {
                SDataName = "alphastrike",
                MenuName = "Alpha Strike | Q",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 2,
                HitType = new[] { global::Activator.HitType.Danger }
            });

            spells.Add(new spelldata
            {
                SDataName = "monkeykingdoubleattack",
                MenuName = "Crushing Blow | Q",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 3,
                HitType = new[] { global::Activator.HitType.Danger }
            });

            spells.Add(new spelldata
            {
                SDataName = "monkeykingdecoy",
                MenuName = "Decoy | W",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 1,
                HitType = new[] { global::Activator.HitType.Stealth }
            });

            spells.Add(new spelldata
            {
                SDataName = "monkeykingspintowin",
                MenuName = "Cyclone | R",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 5,
                HitType =
                    new[]
                    {
                        global::Activator.HitType.CrowdControl, global::Activator.HitType.Ultimate,
                        global::Activator.HitType.Danger,
                    }
            });

            spells.Add(new spelldata
            {
                SDataName = "mordekaisersyphoneofdestruction",
                MenuName = "Siphon of Destruction | E",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 2,
                HitType = new[] { global::Activator.HitType.Danger }
            });

            spells.Add(new spelldata
            {
                SDataName = "mordekaiserchildrenofthegrave",
                MenuName = "Children of the Grave | R",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 5,
                HitType =
                    new[]
                    {
                        global::Activator.HitType.Danger, global::Activator.HitType.Ultimate,
                        global::Activator.HitType.CrowdControl,
                    }
            });

            spells.Add(new spelldata
            {
                SDataName = "darkbindingmissile",
                MenuName = "Dark Binding | Q",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 4,
                HitType = new[] { global::Activator.HitType.Danger, global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "soulshackles",
                MenuName = "Soul Shackles | R",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 5,
                HitType = new[] { global::Activator.HitType.Danger, global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "namiq",
                MenuName = "Auqa Prison | Q",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 5,
                HitType = new[] { global::Activator.HitType.Danger, global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "namir",
                MenuName = "Tidal Wave | R",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 2,
                HitType = new[] { global::Activator.HitType.Danger, global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "nasusq",
                MenuName = "Siphoning Strike| Q",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 2,
                HitType = new[] { global::Activator.HitType.Danger }
            });

            spells.Add(new spelldata
            {
                SDataName = "nasusw",
                MenuName = "Wither | W",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 5,
                HitType = new[] { global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "nautilusanchordrag",
                MenuName = "Dredge Line | Q",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 5,
                HitType = new[] { global::Activator.HitType.CrowdControl, global::Activator.HitType.Danger }
            });

            spells.Add(new spelldata
            {
                SDataName = "javelintoss",
                MenuName = "Javlin Toss | Q",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 4,
                HitType = new[] { global::Activator.HitType.Danger }
            });

            spells.Add(new spelldata
            {
                SDataName = "takedown",
                MenuName = "Takedown | Q",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 3,
                HitType = new[] { global::Activator.HitType.Danger }
            });

            spells.Add(new spelldata
            {
                SDataName = "nocturneunspeakablehorror",
                MenuName = "Unspeakable Horror | E",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 3,
                HitType = new[] { global::Activator.HitType.Danger, global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "iceblast",
                MenuName = "Ice Blast | E",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 3,
                HitType = new[] { global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "olafrecklessstrike",
                MenuName = "Reckless Swing | E",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 3,
                HitType = new[] { global::Activator.HitType.Danger }
            });

            spells.Add(new spelldata
            {
                SDataName = "orianadissonancecommand",
                MenuName = "Command: Dissonance | W",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 3,
                HitType = new[] { global::Activator.HitType.CrowdControl }
            });


            spells.Add(new spelldata
            {
                SDataName = "orianadetonatecommand",
                MenuName = "Command: Shockwave | R",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 3,
                HitType =
                    new[]
                    {
                        global::Activator.HitType.CrowdControl, global::Activator.HitType.Danger,
                        global::Activator.HitType.Ultimate
                    }
            });

            spells.Add(new spelldata
            {
                SDataName = "pantheonw",
                MenuName = "Aegis of Zeonia | W",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 3,
                HitType = new[] { global::Activator.HitType.CrowdControl, global::Activator.HitType.Danger,  }
            });

            spells.Add(new spelldata
            {
                SDataName = "poppydevastatingblow",
                MenuName = "Devastating Blow | Q",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 1,
                HitType = new[] { global::Activator.HitType.Danger }
            });

            spells.Add(new spelldata
            {
                SDataName = "poppyheroiccharge",
                MenuName = "Heroic Charge | E",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 1,
                HitType = new[] { global::Activator.HitType.CrowdControl, global::Activator.HitType.Danger }
            });

            spells.Add(new spelldata
            {
                SDataName = "quinnq",
                MenuName = "Blinding Assault | Q",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 2,
                HitType = new[] { global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "quinne",
                MenuName = "Vault | E",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 3,
                HitType = new[] { global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "puncturingtaunt",
                MenuName = "Taunt | E",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 3,
                HitType = new[] { global::Activator.HitType.CrowdControl, global::Activator.HitType.Danger }
            });

            spells.Add(new spelldata
            {
                SDataName = "renektonpreexecute",
                MenuName = "Ruthless Predator | W",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 3,
                HitType = new[] { global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "rengare",
                MenuName = "Bola | E",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 3,
                HitType = new[] { global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "rengarefinal",
                MenuName = "Bola Empowered | E",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 4,
                HitType = new[] { global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "reksaiwburrowed",
                MenuName = "Unburrow | W",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 3,
                HitType = new[] { global::Activator.HitType.CrowdControl, global::Activator.HitType.Danger }
            });

            spells.Add(new spelldata
            {
                SDataName = "reksaie",
                MenuName = "Furious Bite | E",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 2,
                HitType = new[] { global::Activator.HitType.Danger }
            });

            spells.Add(new spelldata
            {
                SDataName = "rivenmartyr",
                MenuName = "Ki Burst | W",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 3,
                HitType = new[] { global::Activator.HitType.CrowdControl, global::Activator.HitType.Danger }
            });

            spells.Add(new spelldata
            {
                SDataName = "rivenizunablade",
                MenuName = "Windslash | R",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 5,
                HitType = new[] { global::Activator.HitType.Ultimate, global::Activator.HitType.Danger }
            });

            spells.Add(new spelldata
            {
                SDataName = "rumbegrenade",
                MenuName = "Electro-Harpoon | E",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 1,
                HitType = new[] { global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "overload",
                MenuName = "Overload | Q",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 2,
                HitType = new[] { global::Activator.HitType.Danger }
            });

            spells.Add(new spelldata
            {
                SDataName = "runeprison",
                MenuName = "Rune Prison | W",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 3,
                HitType = new[] { global::Activator.HitType.Danger, global::Activator.HitType.CrowdControl,  }
            });

            spells.Add(new spelldata
            {
                SDataName = "sejuaniarcticassault",
                MenuName = "",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 3,
                HitType = new[] { global::Activator.HitType.Danger, global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "sejuaniglacialprisonstart",
                MenuName = "Glacial Prision | R",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 5,
                HitType =
                    new[]
                    {
                        global::Activator.HitType.Danger, global::Activator.HitType.Ultimate,
                        global::Activator.HitType.CrowdControl
                    }
            });

            spells.Add(new spelldata
            {
                SDataName = "twoshivpoison",
                MenuName = "",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 2,
                HitType = new[] { global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "shenfeint",
                MenuName = "",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 2,
                HitType = new[] { global::Activator.HitType.CrowdControl, global::Activator.HitType.Danger,  }
            });

            spells.Add(new spelldata
            {
                SDataName = "shyvanatransformcast",
                MenuName = "",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 2,
                HitType =
                    new[]
                    {
                        global::Activator.HitType.CrowdControl, global::Activator.HitType.Danger,
                        global::Activator.HitType.Ultimate,
                    }
            });

            spells.Add(new spelldata
            {
                SDataName = "shyvanatransformleap",
                MenuName = "",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 2,
                HitType =
                    new[]
                    {
                        global::Activator.HitType.CrowdControl, global::Activator.HitType.Danger,
                        global::Activator.HitType.Ultimate,
                    }
            });

            spells.Add(new spelldata
            {
                SDataName = "fling",
                MenuName = "Fling | E",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 2,
                HitType = new[] { global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "sionq",
                MenuName = "",
                Wait = true,
                WaitDelay = 2000,
                DangerLevel = 4,
                HitType = new[] { global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "sione",
                MenuName = "",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 2,
                HitType = new[] { global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "skarnerfracture",
                MenuName = "",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 3,
                HitType = new[] { global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "skarnerfracturemissile",
                MenuName = "",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 3,
                HitType = new[] { global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "skarnerimpale",
                MenuName = "",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 5,
                HitType =
                    new[]
                    {
                        global::Activator.HitType.CrowdControl, global::Activator.HitType.Danger,
                        global::Activator.HitType.Ultimate
                    }
            });

            spells.Add(new spelldata
            {
                SDataName = "sonaq",
                MenuName = "",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 2,
                HitType = new[] { global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "sonae",
                MenuName = "",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 3,
                HitType = new[] { global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "sonar",
                MenuName = "",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 5,
                HitType =
                    new[]
                    {
                        global::Activator.HitType.Danger, global::Activator.HitType.Ultimate,
                        global::Activator.HitType.CrowdControl
                    }
            });

            spells.Add(new spelldata
            {
                SDataName = "swaintorment",
                MenuName = "",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 2,
                HitType = new[] { global::Activator.HitType.Danger }
            });

            spells.Add(new spelldata
            {
                SDataName = "swainshadowgrasp",
                MenuName = "",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 3,
                HitType = new[] { global::Activator.HitType.Danger, global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "syndrae",
                MenuName = "",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 3,
                HitType = new[] { global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "syndraw",
                MenuName = "",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 2,
                HitType = new[] { global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "syndrar",
                MenuName = "",
                Wait = true,
                WaitDelay = 0,
                DangerLevel = 5,
                HitType = new[] { global::Activator.HitType.Danger, global::Activator.HitType.Ultimate }
            });

            spells.Add(new spelldata
            {
                SDataName = "talonrake",
                MenuName = "",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 2,
                HitType = new[] { global::Activator.HitType.CrowdControl, global::Activator.HitType.Danger }
            });

            spells.Add(new spelldata
            {
                SDataName = "dazzle",
                MenuName = "",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 2,
                HitType = new[] { global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "blindingdart",
                MenuName = "",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 3,
                HitType = new[] { global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "threshq",
                MenuName = "",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 3,
                HitType = new[] { global::Activator.HitType.CrowdControl, global::Activator.HitType.Danger }
            });

            spells.Add(new spelldata
            {
                SDataName = "threshe",
                MenuName = "",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 3,
                HitType = new[] { global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "tristanar",
                MenuName = "",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 4,
                HitType = new[] { global::Activator.HitType.CrowdControl, global::Activator.HitType.Danger }
            });


            spells.Add(new spelldata
            {
                SDataName = "tristanaw",
                MenuName = "",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 4,
                HitType = new[] { global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "tristanaw",
                MenuName = "",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 4,
                HitType = new[] { global::Activator.HitType.CrowdControl, global::Activator.HitType.Danger }
            });


            spells.Add(new spelldata
            {
                SDataName = "mockingshout",
                MenuName = "",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 2,
                HitType = new[] { global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "expunge",
                MenuName = "",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 4,
                HitType = new[] { global::Activator.HitType.CrowdControl, global::Activator.HitType.Danger }
            });

            spells.Add(new spelldata
            {
                SDataName = "twitchvenomcask",
                MenuName = "",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 2,
                HitType = new[] { global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "twitchvenomcaskmissle",
                MenuName = "",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 2,
                HitType = new[] { global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "goldcardpreattack",
                MenuName = "",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 3,
                HitType = new[] { global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "redcardpreattack",
                MenuName = "",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 2,
                HitType = new[] { global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "udyrbearstance",
                MenuName = "",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 2,
                HitType = new[] { global::Activator.HitType.CrowdControl, global::Activator.HitType.Danger,  }
            });

            spells.Add(new spelldata
            {
                SDataName = "urgotswap2",
                MenuName = "",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 5,
                HitType = new[] { global::Activator.HitType.CrowdControl, global::Activator.HitType.Danger }
            });

            spells.Add(new spelldata
            {
                SDataName = "urgotheatseekingmissile",
                MenuName = "",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 2,
                HitType = new[] { global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "urgotplasmagrenade",
                MenuName = "",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 3,
                HitType = new[] { global::Activator.HitType.CrowdControl, global::Activator.HitType.Danger  }
            });

            spells.Add(new spelldata
            {
                SDataName = "urgotplasmagrenadeboom",
                MenuName = "",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 3,
                HitType = new[] { global::Activator.HitType.CrowdControl, global::Activator.HitType.Danger }
            });

            spells.Add(new spelldata
            {
                SDataName = "varusq",
                MenuName = "",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 2,
                HitType = new[] { global::Activator.HitType.Danger }
            });

            spells.Add(new spelldata
            {
                SDataName = "varuse",
                MenuName = "",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 2,
                HitType = new[] { global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "varusr",
                MenuName = "",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 5,
                HitType =
                    new[]
                    {
                        global::Activator.HitType.Danger, global::Activator.HitType.Ultimate,
                        global::Activator.HitType.CrowdControl,
                    }
            });

            spells.Add(new spelldata
            {
                SDataName = "vaynecondemnmissile",
                MenuName = "",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 4,
                HitType = new[] { global::Activator.HitType.CrowdControl, global::Activator.HitType.Danger  }
            });

            spells.Add(new spelldata
            {
                SDataName = "veigarbalefulstrike",
                MenuName = "",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 3,
                HitType = new[] { global::Activator.HitType.Danger }
            });

            spells.Add(new spelldata
            {
                SDataName = "veigarprimordialburst",
                MenuName = "",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 5,
                HitType = new[] { global::Activator.HitType.Danger, global::Activator.HitType.Ultimate  }
            });


            spells.Add(new spelldata
            {
                SDataName = "velkozq",
                MenuName = "",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 3,
                HitType = new[] { global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "velkozqmissile",
                MenuName = "",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 3,
                HitType = new[] { global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "velkozqplitactive",
                MenuName = "",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 3,
                HitType = new[] { global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "viq",
                MenuName = "",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 3,
                HitType = new[] { global::Activator.HitType.CrowdControl, global::Activator.HitType.Danger }
            });

            spells.Add(new spelldata
            {
                SDataName = "vir",
                MenuName = "",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 2,
                HitType = new[] { global::Activator.HitType.CrowdControl, global::Activator.HitType.Danger }
            });

            spells.Add(new spelldata
            {
                SDataName = "velkozqplitactive",
                MenuName = "",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 3,
                HitType = new[] { global::Activator.HitType.CrowdControl }
            });


            spells.Add(new spelldata
            {
                SDataName = "viktorchaosstorm",
                MenuName = "",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 5,
                HitType = new[] { global::Activator.HitType.CrowdControl, global::Activator.HitType.Danger, global::Activator.HitType.Ultimate }
            });


            spells.Add(new spelldata
            {
                SDataName = "vladimirhemoplague",
                MenuName = "",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 3,
                HitType = new[] { global::Activator.HitType.CrowdControl, global::Activator.HitType.Danger }
            });

            spells.Add(new spelldata
            {
                SDataName = "volibearw",
                MenuName = "",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 3,
                HitType = new[] { global::Activator.HitType.Danger }
            });

            spells.Add(new spelldata
            {
                SDataName = "volibeare",
                MenuName = "",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 3,
                HitType = new[] { global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "infiniteduress",
                MenuName = "",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 5,
                HitType = new[] { global::Activator.HitType.CrowdControl, global::Activator.HitType.Danger }
            });

            spells.Add(new spelldata
            {
                SDataName = "xerathmagespear",
                MenuName = "",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 3,
                HitType = new[] { global::Activator.HitType.CrowdControl }
            });


            spells.Add(new spelldata
            {
                SDataName = "xeratharcanebarrage2",
                MenuName = "",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 2,
                HitType = new[] { global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "xenzhaosweep",
                MenuName = "",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 3,
                HitType = new[] { global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "xenzhaoparry",
                MenuName = "",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 4,
                HitType = new[] { global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "yasuoq3w",
                MenuName = "",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 4,
                HitType = new[] { global::Activator.HitType.CrowdControl, global::Activator.HitType.Danger }
            });

            spells.Add(new spelldata
            {
                SDataName = "yasuoqw",
                MenuName = "",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 2,
                HitType = new[] { global::Activator.HitType.Danger }
            });

            spells.Add(new spelldata
            {
                SDataName = "yasuoqw2",
                MenuName = "",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 2,
                HitType = new[] { global::Activator.HitType.Danger }
            });

            spells.Add(new spelldata
            {
                SDataName = "yasuorknockupcombow",
                MenuName = "",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 5,
                HitType =
                    new[]
                    {
                        global::Activator.HitType.Danger, global::Activator.HitType.Ultimate,
                        global::Activator.HitType.CrowdControl,
                    }
            });

            spells.Add(new spelldata
            {
                SDataName = "yorickdecayed",
                MenuName = "",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 2,
                HitType = new[] { global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "zacr",
                MenuName = "",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 5,
                HitType =
                    new[]
                    {
                        global::Activator.HitType.CrowdControl, global::Activator.HitType.Danger,
                        global::Activator.HitType.Ultimate
                    }
            });

            spells.Add(new spelldata
            {
                SDataName = "zedshuriken",
                MenuName = "",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 2,
                HitType = new[] { global::Activator.HitType.CrowdControl, global::Activator.HitType.Danger }
            });

            spells.Add(new spelldata
            {
                SDataName = "zedpbaoedummy",
                MenuName = "",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 2,
                HitType = new[] { global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "zedult",
                MenuName = "",
                Wait = true,
                WaitDelay = 700,
                DangerLevel = 5,
                HitType = new[] { global::Activator.HitType.CrowdControl, global::Activator.HitType.Danger }
            });

            spells.Add(new spelldata
            {
                SDataName = "ziggsr",
                MenuName = "",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 5,
                HitType = new[] { global::Activator.HitType.Danger, global::Activator.HitType.Ultimate }
            });

            spells.Add(new spelldata
            {
                SDataName = "zyragraspingroots",
                MenuName = "",
                Wait = false,
                WaitDelay = 0,
                DangerLevel = 2,
                HitType = new[] { global::Activator.HitType.CrowdControl }
            });

            spells.Add(new spelldata
            {
                SDataName = "zyrabramblezone",
                MenuName = "",
                Wait = true,
                WaitDelay = 1000,
                DangerLevel = 5,
                HitType =
                    new[]
                    {
                        global::Activator.HitType.CrowdControl, global::Activator.HitType.Ultimate,
                        global::Activator.HitType.Danger
                    }
            });
        }
    }
}
