using System;
using LeagueSharp;
using LeagueSharp.Common;
using System.Collections.Generic;

namespace KurisuMorgana
{
    public enum Skilltype
    {
        Unknown = 0,
        Line = 1,
        Circle = 2,
        Cone = 3
    }

    public class KurisuLib
    {
        public string HeroName { get; set; }
        public string SpellMenuName { get; set; }
        public SpellSlot Slot { get; set; }
        public Skilltype Type { get; set; }
        public float Radius { get; set; }
        public string SDataName { get; set; }
        public int DangerLevel { get; set; }


        public static List<KurisuLib> CcList = new List<KurisuLib>();
        public static List<KurisuLib> SilenceList = new List<KurisuLib>();

        static KurisuLib()
        {
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Aatorx",
                    SpellMenuName = "Dark Flight",
                    Slot = SpellSlot.Q,
                    Type = Skilltype.Circle,
                    SDataName = "AatroxQ",
                    DangerLevel = 5
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Aatorx",
                    SpellMenuName = "Blades of Torment",
                    Slot = SpellSlot.E,
                    Type = Skilltype.Cone,
                    SDataName = "AatroxE",
                    DangerLevel = 3
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Ahri",
                    SpellMenuName = "Charm",
                    Slot = SpellSlot.E,
                    Type = Skilltype.Line,
                    SDataName = "AhriSeduce",
                    DangerLevel = 5
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Alistar",
                    SpellMenuName = "Pulverize",
                    Slot = SpellSlot.Q,
                    Type = Skilltype.Circle,
                    SDataName = "Pulverize",
                    DangerLevel = 5
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Alistar",
                    SpellMenuName = "Headbutt",
                    Slot = SpellSlot.W,
                    SDataName = "Headbutt",
                    DangerLevel = 3
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Amumu",
                    SpellMenuName = "Bandage Toss",
                    Slot = SpellSlot.Q,
                    Type = Skilltype.Circle,
                    SDataName = "BandageToss",
                    DangerLevel = 5
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Amumu",
                    SpellMenuName = "Curse of the Sad Mummy",
                    Slot = SpellSlot.R,
                    Type = Skilltype.Circle,
                    SDataName = "CurseoftheSadMummy",
                    DangerLevel = 5
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Anivia",
                    SpellMenuName = "Flash Frost",
                    Slot = SpellSlot.Q,
                    Type = Skilltype.Line,
                    SDataName = "FlashFrost",
                    DangerLevel = 5
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Anivia",
                    SpellMenuName = "Glacial Storm",
                    Slot = SpellSlot.Q,
                    Type = Skilltype.Circle,
                    SDataName = "GlacialStorm",
                    DangerLevel = 3
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Annie",
                    SpellMenuName = "Tibbers",
                    Slot = SpellSlot.Q,
                    Type = Skilltype.Circle,
                    SDataName = "InfernalGuardian",
                    DangerLevel = 5
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Ashe",
                    SpellMenuName = "Crystal Arrow",
                    Slot = SpellSlot.W,
                    Type = Skilltype.Line,
                    SDataName = "EnchantedCrystalArrow",
                    DangerLevel = 5
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Ashe",
                    SpellMenuName = "Volley",
                    Slot = SpellSlot.W,
                    Type = Skilltype.Cone,
                    SDataName = "Volley",
                    DangerLevel = 3
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Azir",
                    SpellMenuName = "ShiftingSands",
                    Slot = SpellSlot.E,
                    Type = Skilltype.Circle,
                    SDataName = "AzirE",
                    DangerLevel = 3
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Azir",
                    SpellMenuName = "Emperor's Divide",
                    Slot = SpellSlot.R,
                    Type = Skilltype.Circle,
                    SDataName = "AzirR",
                    DangerLevel = 5
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Blitzcrank",
                    SpellMenuName = "Rocket Grab",
                    Slot = SpellSlot.Q,
                    Type = Skilltype.Line,
                    SDataName = "RocketGrab",
                    DangerLevel = 5
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Blitzcrank",
                    SpellMenuName = "Power Fist",
                    Slot = SpellSlot.E,
                    SDataName = "PowerFist",
                    DangerLevel = 3
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Brand",
                    SpellMenuName = "Sear",
                    Slot = SpellSlot.Q,
                    Type = Skilltype.Line,
                    SDataName = "BrandBlazeMissile",
                    DangerLevel = 3
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Bruam",
                    SpellMenuName = "Winter's Bite",
                    Slot = SpellSlot.Q,
                    Type = Skilltype.Line,
                    SDataName = "BraumQ",
                    DangerLevel = 3
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Bruam",
                    SpellMenuName = "Glacial Fissure",
                    Slot = SpellSlot.R,
                    Type = Skilltype.Line,
                    SDataName = "BraumR",
                    DangerLevel = 5
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Caitlyn",
                    SpellMenuName = "90 Caliber Net",
                    Slot = SpellSlot.Q,
                    Type = Skilltype.Line,
                    SDataName = "CaitlynEntrapment",
                    DangerLevel = 3
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Cassiopeia",
                    SpellMenuName = "Petrifying Gaze",
                    Slot = SpellSlot.R,
                    Type = Skilltype.Cone,
                    SDataName = "CassiopeiaPetrifyingGaze",
                    DangerLevel = 5
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Cho'gath",
                    SpellMenuName = "Rupture",
                    Slot = SpellSlot.Q,
                    Type = Skilltype.Circle,
                    SDataName = "Rupture",
                    DangerLevel = 5
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Darius",
                    SpellMenuName = "Aprehend",
                    Slot = SpellSlot.E,
                    Type = Skilltype.Cone,
                    SDataName = "DariusAxeGrabCone",
                    DangerLevel = 3
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Diana",
                    SpellMenuName = "Moonfall",
                    Slot = SpellSlot.E,
                    Type = Skilltype.Circle,
                    SDataName = "DianaVortex",
                    DangerLevel = 3
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "DrMundo",
                    SpellMenuName = "Cleaver",
                    Slot = SpellSlot.Q,
                    Type = Skilltype.Line,
                    SDataName = "InfectedCleaverMissileCast",
                    DangerLevel = 3
                });

            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Draven",
                    SpellMenuName = "Stand Aside",
                    Slot = SpellSlot.E,
                    Type = Skilltype.Line,
                    SDataName = "DravenDoubleShot",
                    DangerLevel = 3
                });

            CcList.Add(
               new KurisuLib
               {
                   HeroName = "Elise",
                   SpellMenuName = "Cocoon",
                   Slot = SpellSlot.E,
                   Type = Skilltype.Line,
                   SDataName = "DravenDoubleShot",
                   DangerLevel = 3
               });

            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Evelynn",
                    SpellMenuName = "Agony's Embrace",
                    Slot = SpellSlot.R,
                    Type = Skilltype.Circle,
                    DangerLevel = 5,
                    SDataName = "EvelynnR",
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Fizz",
                    SpellMenuName = "Chum the Waters",
                    Slot = SpellSlot.R,
                    Type = Skilltype.Line,
                    DangerLevel = 5,
                    SDataName = "FizzMarinerDoomMissile",
                });
            CcList.Add(
                 new KurisuLib
                 {
                     HeroName = "Fizz",
                     SpellMenuName = "Playful Trickster",
                     Slot = SpellSlot.E,
                     Type = Skilltype.Line,
                     DangerLevel = 3,
                     SDataName = "FizzJump",
                 });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Galio",
                    SpellMenuName = "Resolute Smite",
                    Slot = SpellSlot.Q,
                    Type = Skilltype.Circle,
                    DangerLevel = 2,
                    SDataName = "GalioResoluteSmite",
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Galio",
                    SpellMenuName = "Idol Of Durand",
                    Slot = SpellSlot.R,
                    Type = Skilltype.Circle,
                    DangerLevel = 5,
                    SDataName = "GalioIdolOfDurand",
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Gnar",
                    SpellMenuName = "Boomerang Throw",
                    Slot = SpellSlot.Q,
                    Type = Skilltype.Line,
                    DangerLevel = 2,
                    SDataName = "GnarQ",
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Gnar",
                    SpellMenuName = "Bouldar Toss",
                    Slot = SpellSlot.Q,
                    Type = Skilltype.Line,
                    DangerLevel = 2,
                    SDataName = "GnarBigQ",
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Gnar",
                    SpellMenuName = "Wallop",
                    Slot = SpellSlot.W,
                    Type = Skilltype.Line,
                    DangerLevel = 2,
                    SDataName = "GnarBigW",
                });

            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Gnar",
                    SpellMenuName = "GNAR!",
                    Slot = SpellSlot.R,
                    Type = Skilltype.Circle,
                    DangerLevel = 5,
                    SDataName = "GnarR",
                });

            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Gragas",
                    SpellMenuName = "Barrel Roll",
                    Slot = SpellSlot.Q,
                    Type = Skilltype.Circle,
                    DangerLevel = 2,
                    SDataName = "GragasQ",
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Gragas",
                    SpellMenuName = "Body Slam",
                    Slot = SpellSlot.E,
                    Type = Skilltype.Line,
                    DangerLevel = 2,
                    SDataName = "GragasE",
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Gragas",
                    SpellMenuName = "Explosive Cask",
                    Slot = SpellSlot.R,
                    Type = Skilltype.Circle,
                    DangerLevel = 5,
                    SDataName = "GragasR",
                });

            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Heimerdinger",
                    SpellMenuName = "Electron Storm Grenade",
                    Slot = SpellSlot.E,
                    Type = Skilltype.Circle,
                    DangerLevel = 3,
                    SDataName = "HeimerdingerE",
                });
            CcList.Add(
                 new KurisuLib
                 {
                     HeroName = "Hecarim",
                     SpellMenuName = "Onslaught of Shadows",
                     Slot = SpellSlot.R,
                     Type = Skilltype.Circle,
                     DangerLevel = 5,
                     SDataName = "HecarimUlt",
                 });
            CcList.Add(
                  new KurisuLib
                  {
                      HeroName = "Hecarim",
                      SpellMenuName = "Devestating Charge",
                      Slot = SpellSlot.E,
                      Type = Skilltype.Circle,
                      DangerLevel = 3,
                      SDataName = "HecarimRamp",
                  });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Janna",
                    SpellMenuName = "Howling Gale",
                    Slot = SpellSlot.Q,
                    Type = Skilltype.Line,
                    DangerLevel = 3,
                    SDataName = "HowlingGale",
                });
            CcList.Add(
                 new KurisuLib
                 {
                     HeroName = "Janna",
                     SpellMenuName = "Zephyr",
                     Slot = SpellSlot.W,
                     DangerLevel = 3,
                     SDataName = "ReapTheWhirlwind",
                 });
            CcList.Add(
                 new KurisuLib
                 {
                     HeroName = "Jax",
                     SpellMenuName = "Counter Strike",
                     Slot = SpellSlot.E,
                     Type = Skilltype.Line,
                     DangerLevel = 5,
                     SDataName = "JaxCounterStrike",
                 });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "JarvanIV",
                    SpellMenuName = "Dragon Strike",
                    Slot = SpellSlot.Q,
                    Type = Skilltype.Line,
                    DangerLevel = 3,
                    SDataName = "JarvanIVDragonStrike",
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Jayce",
                    SpellMenuName = "Thundering Blow",
                    Slot = SpellSlot.E,
                    DangerLevel = 3,
                    SDataName = "JayceThunderingBlow",
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Jinx",
                    SpellMenuName = "Zap!",
                    Slot = SpellSlot.W,
                    Type = Skilltype.Line,
                    DangerLevel = 3,
                    SDataName = "JinxW",
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Jinx",
                    SpellMenuName = "Chompers!",
                    Slot = SpellSlot.E,
                    Type = Skilltype.Line,
                    DangerLevel = 4,
                    SDataName = "JinxE",
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Karma",
                    SpellMenuName = "Inner Flame (Mantra)",
                    Slot = SpellSlot.Q,
                    Type = Skilltype.Circle,
                    DangerLevel = 2,
                    SDataName = "KarmaQMantra",
                });
            CcList.Add(
                 new KurisuLib
                 {
                     HeroName = "Karma",
                     SpellMenuName = "Sprit Bond",
                     Slot = SpellSlot.W,
                     DangerLevel = 3,
                     SDataName = "KarmaQMantra",
                 });

            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Kassadin",
                    SpellMenuName = "Force Pulse",
                    Slot = SpellSlot.E,
                    Type = Skilltype.Cone,
                    DangerLevel = 3,
                    SDataName = "ForcePulse",
                });

            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Khazix",
                    SpellMenuName = "Void Spikes",
                    Slot = SpellSlot.W,
                    Type = Skilltype.Line,
                    DangerLevel = 2,
                    SDataName = "KhazixW",
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Kayle",
                    SpellMenuName = "Reckoning",
                    Slot = SpellSlot.Q,
                    DangerLevel = 3,
                    SDataName = "JudicatorReckoning",
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "KogMaw",
                    SpellMenuName = "Void Ooze",
                    Slot = SpellSlot.E,
                    Type = Skilltype.Line,
                    DangerLevel = 2,
                    SDataName = "KogMawVoidOoze",
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Leblanc",
                    SpellMenuName = "Soul Shackle",
                    Slot = SpellSlot.E,
                    Type = Skilltype.Line,
                    DangerLevel = 3,
                    SDataName = "LeblancSoulShackle",
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Leblanc",
                    SpellMenuName = "Soul Shackle (Mimic)",
                    Slot = SpellSlot.E,
                    Type = Skilltype.Line,
                    DangerLevel = 3,
                    SDataName = "LeblancSoulShackleM",
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "LeeSin",
                    SpellMenuName = "Dragon's Rage",
                    Slot = SpellSlot.R,
                    DangerLevel = 5,
                    SDataName = "BlindMonkRKick",
                });
                            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Leona",
                    SpellMenuName = "Zenith Blade",
                    Slot = SpellSlot.E,
                    Type = Skilltype.Line,
                    DangerLevel = 3,
                    SDataName = "LeonaZenithBlade",
                });
            CcList.Add(
                 new KurisuLib
                 {
                     HeroName = "Leona",
                     SpellMenuName = "Shield of Daybreak",
                     Slot = SpellSlot.Q,
                     DangerLevel = 3,
                     SDataName = "LeonaShieldOfDaybreak",
                 });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Leona",
                    SpellMenuName = "Solar Flare",
                    Slot = SpellSlot.R,
                    Type = Skilltype.Circle,
                    DangerLevel = 5,
                    SDataName = "LeonaSolarFlare",
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Lissandra",
                    SpellMenuName = "Ice Shard",
                    Slot = SpellSlot.Q,
                    Type = Skilltype.Line,
                    DangerLevel = 2,
                    SDataName = "LissandraQ",
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Lissandra",
                    SpellMenuName = "Ring of Frost",
                    Slot = SpellSlot.W,
                    Type = Skilltype.Line,
                    DangerLevel = 3,
                    SDataName = "LissandraW",
                });

            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Lulu",
                    SpellMenuName = "Glitterlance",
                    Slot = SpellSlot.Q,
                    Type = Skilltype.Line,
                    DangerLevel = 2,
                    SDataName = "LuluQ"
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Lulu",
                    SpellMenuName = "Glitterlance: Extended",
                    Slot = SpellSlot.Q,
                    Type = Skilltype.Line,
                    DangerLevel = 3,
                    SDataName = "LuluQMissileTwo"
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Lux",
                    SpellMenuName = "Light Binding",
                    Slot = SpellSlot.Q,
                    Type = Skilltype.Line,
                    DangerLevel = 5,
                    SDataName = "LuxLightBinding",
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Lux",
                    SpellMenuName = "Lucent Singularity",
                    Slot = SpellSlot.E,
                    Type = Skilltype.Circle,
                    DangerLevel = 3,
                    SDataName = "LuxLightStrikeKugel",
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Lux",
                    SpellMenuName = "Final Spark",
                    Slot = SpellSlot.R,
                    Type = Skilltype.Line,
                    DangerLevel = 5,
                    SDataName = "LuxMaliceCannon",
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Malphite",
                    SpellMenuName = "Unstoppable Force",
                    Slot = SpellSlot.R,
                    Type = Skilltype.Circle,
                    DangerLevel = 5,
                    SDataName = "UFSlash",
                });
            CcList.Add(
                 new KurisuLib
                 {
                     HeroName = "Malphite",
                     SpellMenuName = "Sismic Shard",
                     Slot = SpellSlot.Q,
                     Type = Skilltype.Circle,
                     DangerLevel = 3,
                     SDataName = "SismicShard",
                 });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Malzahar",
                    SpellMenuName = "Nether Grasp",
                    Slot = SpellSlot.R,
                    DangerLevel = 5,
                    SDataName = "AlZaharNetherGrasp",
                });
            CcList.Add(
                 new KurisuLib
                 {
                     HeroName = "Maokai",
                     SpellMenuName = "Twisted Advance",
                     Slot = SpellSlot.W,
                     DangerLevel = 3,
                     SDataName = "MaokaiUnstableGrowth",
                 });
            CcList.Add(
                 new KurisuLib
                 {
                     HeroName = "Maokai",
                     SpellMenuName = "Arcane Smash",
                     Slot = SpellSlot.Q,
                     DangerLevel = 3,
                     SDataName = "MaokaiTrunkLine",
                 });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Morgana",
                    SpellMenuName = "Dark Binding",
                    Slot = SpellSlot.Q,
                    Type = Skilltype.Line,
                    DangerLevel = 5,
                    SDataName = "DarkBindingMissile",
                });
            CcList.Add(
                 new KurisuLib
                 {
                     HeroName = "Mordekaiser",
                     SpellMenuName = "Children of the Grave",
                     Slot = SpellSlot.Q,
                     DangerLevel = 5,
                     SDataName = "MordekaiserChildrenOfTheGrave",
                 });
            CcList.Add(
                 new KurisuLib
                 {
                     HeroName = "Wukong",
                     SpellMenuName = "Cyclone",
                     Slot = SpellSlot.R,
                     Type = Skilltype.Circle,
                     DangerLevel = 5,
                     SDataName = "MonkeyKingSpinToWin",
                 });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Nami",
                    SpellMenuName = "Aqua Prision",
                    Slot = SpellSlot.Q,
                    Type = Skilltype.Circle,
                    DangerLevel = 3,
                    SDataName = "NamiQ",
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Nasus",
                    SpellMenuName = "Wither",
                    Slot = SpellSlot.Q,
                    DangerLevel = 3,
                    SDataName = "NasusW",
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Karthus",
                    SpellMenuName = "Wall of Pain",
                    Slot = SpellSlot.Q,
                    Type = Skilltype.Circle,
                    DangerLevel = 3,
                    SDataName = "KarthusWallOfPain",
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Nami",
                    SpellMenuName = "Tidal Wave",
                    Slot = SpellSlot.R,
                    Type = Skilltype.Line,
                    DangerLevel = 2,
                    SDataName = "NamiR",
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Nautilus",
                    SpellMenuName = "Dredge Line",
                    Slot = SpellSlot.Q,
                    Type = Skilltype.Line,
                    DangerLevel = 3,
                    SDataName = "NautilusAnchorDragMissile",
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Nautilus",
                    SpellMenuName = "Riptide",
                    Slot = SpellSlot.E,
                    Type = Skilltype.Circle,
                    DangerLevel = 2,
                    SDataName = "NautilusSplashZone",
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Nautilus",
                    SpellMenuName = "Depth Charge",
                    Slot = SpellSlot.R,
                    DangerLevel = 5,
                    SDataName = "NautilusGrandLine",
                });

            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Nidalee",
                    SpellMenuName = "Javelin Toss",
                    Slot = SpellSlot.Q,
                    Type = Skilltype.Line,
                    DangerLevel = 3,
                    SDataName = "JavelinToss",
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Olaf",
                    SpellMenuName = "Undertow",
                    Slot = SpellSlot.Q,
                    Type = Skilltype.Line,
                    DangerLevel = 2,
                    SDataName = "OlafAxeThrowCast",
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Orianna",
                    SpellMenuName = "Command: Dissonance ",
                    Slot = SpellSlot.W,
                    Type = Skilltype.Circle,
                    DangerLevel = 3,
                    SDataName = "OrianaDissonanceCommand",
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Orianna",
                    SpellMenuName = "OrianaDetonateCommand",
                    Slot = SpellSlot.R,
                    Type = Skilltype.Circle,
                    DangerLevel = 5,
                    SDataName = "OrianaDetonateCommand",
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Quinn",
                    SpellMenuName = "Blinding Assault",
                    Slot = SpellSlot.Q,
                    Type = Skilltype.Line,
                    DangerLevel = 2,
                    SDataName = "QuinnQ",
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Rammus",
                    SpellMenuName = "Puncturing Taunt",
                    Slot = SpellSlot.Q,
                    Type = Skilltype.Line,
                    DangerLevel = 2,
                    SDataName = "PuncturingTaunt",
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Rengar",
                    SpellMenuName = "Bola Strike (Emp)",
                    Slot = SpellSlot.E,
                    Type = Skilltype.Line,
                    DangerLevel = 3,
                    SDataName = "RengarEFinal",
                });

            CcList.Add(
                 new KurisuLib
                 {
                     HeroName = "Fiddlesticks",
                     SpellMenuName = "Terrify",
                     Slot = SpellSlot.Q,
                     DangerLevel = 3,
                     SDataName = "Terrify",
                 });
            CcList.Add(
                 new KurisuLib
                 {
                     HeroName = "Renekton",
                     SpellMenuName = "Ruthless Predator",
                     Slot = SpellSlot.W,
                     DangerLevel = 3,
                     SDataName = "RenektonPreExecute",
                 });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Riven",
                    SpellMenuName = "Ki Burst",
                    Slot = SpellSlot.W,
                    Type = Skilltype.Line,
                    DangerLevel = 5,
                    SDataName = "RivenMartyr"
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Rumble",
                    SpellMenuName = "RumbleGrenade",
                    Slot = SpellSlot.E,
                    Type = Skilltype.Line,
                    DangerLevel = 2,
                    SDataName = "RumbleGrenade",
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Rumble",
                    SpellMenuName = "RumbleCarpetBombM",
                    Slot = SpellSlot.R,
                    Type = Skilltype.Line,
                    DangerLevel = 4,
                    SDataName = "RumbleCarpetBombMissile",
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Ryze",
                    SpellMenuName = "Rune Prision",
                    Slot = SpellSlot.W,
                    DangerLevel = 3,
                    SDataName = "RunePrison",
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Sejuani",
                    SpellMenuName = "Arctic Assault",
                    Slot = SpellSlot.Q,
                    Type = Skilltype.Line,
                    DangerLevel = 3,
                    SDataName = "SejuaniArcticAssault",
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Sejuani",
                    SpellMenuName = "Glacial Prision",
                    Slot = SpellSlot.R,
                    Type = Skilltype.Line,
                    DangerLevel = 5,
                    SDataName = "SejuaniGlacialPrisonStart",
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Singed",
                    SpellMenuName = "Mega Adhesive",
                    Slot = SpellSlot.W,
                    Type = Skilltype.Circle,
                    DangerLevel = 2,
                    SDataName = "MegaAdhesive",
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Singed",
                    SpellMenuName = "Fling",
                    Slot = SpellSlot.E,
                    DangerLevel = 2,
                    SDataName = "Fling",
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Nocturne",
                    SpellMenuName = "Unspeakable Horror",
                    Slot = SpellSlot.E,
                    DangerLevel = 3,
                    SDataName = "NocturneUnspeakableHorror",
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Shen",
                    SpellMenuName = "ShenShadowDash",
                    Slot = SpellSlot.E,
                    Type = Skilltype.Line,
                    DangerLevel = 3,
                    SDataName = "ShenShadowDash",
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Shyvana",
                    SpellMenuName = "ShyvanaTransformCast",
                    Slot = SpellSlot.R,
                    Type = Skilltype.Line,
                    DangerLevel = 3,
                    SDataName = "ShyvanaTransformCast",
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Skarner",
                    SpellMenuName = "Fracture",
                    Slot = SpellSlot.E,
                    Type = Skilltype.Line,
                    DangerLevel = 3,
                    SDataName = "SkarnerFractureMissile",
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Skarner",
                    SpellMenuName = "Impale",
                    Slot = SpellSlot.R,
                    Type = Skilltype.Line,
                    DangerLevel = 5,
                    SDataName = "SkarnerFractureMissile",
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Pantheon",
                    SpellMenuName = "Aegis of Zeonia",
                    Slot = SpellSlot.W,
                    DangerLevel = 3,
                    SDataName = "PantheonW",
                });
            CcList.Add(
                 new KurisuLib
                 {
                     HeroName = "Pantheon",
                     SpellMenuName = "Heroic Charge",
                     Slot = SpellSlot.W,
                     DangerLevel = 3,
                     SDataName = "PoppyHeroicCharge",
                 });
            CcList.Add(
                 new KurisuLib
                 {
                     HeroName = "Nunu",
                     SpellMenuName = "Ice Blast",
                     Slot = SpellSlot.E,
                     DangerLevel = 3,
                     SDataName = "Ice Blast",
                 });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Sona",
                    SpellMenuName = "Crescendo",
                    Slot = SpellSlot.R,
                    Type = Skilltype.Line,
                    DangerLevel = 5,
                    SDataName = "SonaCrescendo",
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Swain",
                    SpellMenuName = "Nevermove",
                    Slot = SpellSlot.W,
                    Type = Skilltype.Circle,
                    DangerLevel = 3,
                    SDataName = "SwainShadowGrasp",
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Syndra",
                    SpellMenuName = "Scatter the Weak",
                    Slot = SpellSlot.E,
                    Type = Skilltype.Cone,
                    DangerLevel = 5,
                    SDataName = "SyndraE",
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Thresh",
                    SpellMenuName = "Death Sentence",
                    Slot = SpellSlot.Q,
                    Type = Skilltype.Line,
                    DangerLevel = 3,
                    SDataName = "ThreshQ",
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Thresh",
                    SpellMenuName = "Flay",
                    Slot = SpellSlot.E,
                    Type = Skilltype.Line,
                    DangerLevel = 3,
                    SDataName = "ThreshEFlay",
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Tristana",
                    SpellMenuName = "Buster Shot",
                    Slot = SpellSlot.R,
                    DangerLevel = 5,
                    SDataName = "BusterShot",
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Trundle",
                    SpellMenuName = "Pillar of Ice",
                    Slot = SpellSlot.E,
                    DangerLevel = 3,
                    SDataName = "TrundleCircle",
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Trundle",
                    SpellMenuName = "Subjugate",
                    Slot = SpellSlot.R,
                    DangerLevel = 5,
                    SDataName = "TrundlePain",
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Tryndamere",
                    SpellMenuName = "Mocking Shout",
                    Slot = SpellSlot.W,
                    DangerLevel = 3,
                    SDataName = "MockingShout",
                });

            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Twitch",
                    SpellMenuName = "Venom Cask",
                    Slot = SpellSlot.W,
                    Type = Skilltype.Circle,
                    DangerLevel = 2,
                    SDataName = "TwitchVenomCaskMissile",
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Urgot",
                    SpellMenuName = "Corrosive Charge",
                    Slot = SpellSlot.E,
                    Type = Skilltype.Circle,
                    DangerLevel = 3,
                    SDataName = "UrgotPlasmaGrenadeBoom",
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Varus",
                    SpellMenuName = "Hail of Arrowws",
                    Slot = SpellSlot.E,
                    Type = Skilltype.Circle,
                    DangerLevel = 2,
                    SDataName = "VarusE",
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Varus",
                    SpellMenuName = "Chain of Corruption",
                    Slot = SpellSlot.R,
                    Type = Skilltype.Line,
                    DangerLevel = 5,
                    SDataName = "VarusR",
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Veigar",
                    SpellMenuName = "Event Horizon",
                    Slot = SpellSlot.E,
                    Type = Skilltype.Circle,
                    DangerLevel = 5,
                    SDataName = "VeigarEventHorizon",
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Velkoz",
                    SpellMenuName = "VelkozQ",
                    Slot = SpellSlot.Q,
                    Type = Skilltype.Line,
                    DangerLevel = 3,
                    SDataName = "VelkozQ",
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Velkoz",
                    SpellMenuName = "Plasma Fission",
                    Slot = SpellSlot.Q,
                    Type = Skilltype.Line,
                    DangerLevel = 3,
                    SDataName = "VelkozQSplit",
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Velkoz",
                    SpellMenuName = "Tectonic Disruption",
                    Slot = SpellSlot.E,
                    Type = Skilltype.Circle,
                    DangerLevel = 3,
                    SDataName = "VelkozE",
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Vi",
                    SpellMenuName = "Vault Breaker",
                    Slot = SpellSlot.Q,
                    Type = Skilltype.Line,
                    DangerLevel = 3,
                    SDataName = "ViQ",
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Vi",
                    SpellMenuName = "Assault and Battery",
                    Slot = SpellSlot.R,
                    DangerLevel = 5,
                    SDataName = "ViR",
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Viktor",
                    SpellMenuName = "Gravity Field",
                    Slot = SpellSlot.W,
                    Type = Skilltype.Circle,
                    DangerLevel = 5,
                    SDataName = "ViktorGravitonField",
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Vayne",
                    SpellMenuName = "Condemn",
                    Slot = SpellSlot.E,
                    DangerLevel = 3,
                    SDataName = "Vayne Condemn",
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Warwick",
                    SpellMenuName = "Infinite Duress",
                    Slot = SpellSlot.R,
                    DangerLevel = 5,
                    SDataName = "InfiniteDuress",
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Xerath",
                    SpellMenuName = "Eye of Destruction",
                    Slot = SpellSlot.W,
                    Type = Skilltype.Circle,
                    DangerLevel = 2,
                    SDataName = "XerathArcaneBarrage2",
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Xerath",
                    SpellMenuName = "Shocking Orb",
                    Slot = SpellSlot.E,
                    Type = Skilltype.Line,
                    DangerLevel = 2,
                    SDataName = "XerathMageSpearMissile",
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "XinZhao",
                    SpellMenuName = "Three Talon Strike",
                    Slot = SpellSlot.Q,
                    DangerLevel = 3,
                    SDataName = "XenZhaoComboTarget",
                });
            CcList.Add(
                 new KurisuLib
                 {
                     HeroName = "XinZhao",
                     SpellMenuName = "Audacious Charge",
                     Slot = SpellSlot.E,
                     DangerLevel = 4,
                     SDataName = "XenZhaoSweep",
                 });
           CcList.Add(
                 new KurisuLib
                 {
                     HeroName = "XinZhao",
                     SpellMenuName = "Crescent Sweep",
                     Slot = SpellSlot.R,
                     Type = Skilltype.Circle,
                     DangerLevel = 5,
                     SDataName = "XenZhaoParry",
                 });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Yasuo",
                    SpellMenuName = "yasuoq2",
                    Slot = SpellSlot.Q,
                    Type = Skilltype.Line,
                    DangerLevel = 2,
                    SDataName = "yasuoq2",
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Yasuo",
                    SpellMenuName = "yasuoq3w",
                    Slot = SpellSlot.Q,
                    Type = Skilltype.Line,
                    DangerLevel = 3,
                    SDataName = "yasuoq3w",
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Yasuo",
                    SpellMenuName = "yasuoq",
                    Slot = SpellSlot.Q,
                    Type = Skilltype.Line,
                    DangerLevel = 2,
                    SDataName = "yasuoq",
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Zac",
                    SpellMenuName = "Stretching Strike",
                    Slot = SpellSlot.Q,
                    Type = Skilltype.Line,
                    DangerLevel = 2,
                    SDataName = "ZacQ",
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Zac",
                    SpellMenuName = "Lets Bounce!",
                    Slot = SpellSlot.R,
                    DangerLevel = 5,
                    SDataName = "ZacR",
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Zed",
                    SpellMenuName = "Death Mark",
                    Slot = SpellSlot.R,
                    DangerLevel = 5,
                    SDataName = "ZedUlt",
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Ziggs",
                    SpellMenuName = "Satchel Charge",
                    Slot = SpellSlot.W,
                    Type = Skilltype.Circle,
                    DangerLevel = 2,
                    SDataName = "ZiggsW",
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Zyra",
                    SpellMenuName = "Grasping Roots",
                    Slot = SpellSlot.E,
                    Type = Skilltype.Line,
                    DangerLevel = 5,
                    SDataName = "ZyraGraspingRoots",
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Zyra",
                    SpellMenuName = "Stranglethorns",
                    Slot = SpellSlot.R,
                    Type = Skilltype.Line,
                    DangerLevel = 3,
                    SDataName = "ZyraBrambleZone",
                });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Taric",
                    SpellMenuName = "Dazzle",
                    Slot = SpellSlot.E,
                    SDataName = "Dazzle",
                });
            CcList.Add(
                 new KurisuLib
                 {
                     HeroName = "Yoric",
                     SpellMenuName = "Omen of Pestilence",
                     Slot = SpellSlot.W,
                     DangerLevel = 3,
                     SDataName = "YorickDecayed",
                 });
            CcList.Add(
                new KurisuLib
                {
                    HeroName = "Yasuo",
                    SpellMenuName = "Steel Tempest (3)",
                    Slot = SpellSlot.W,
                    DangerLevel = 3,
                    SDataName = "YasuoQ3",
                });

            // SilenceList
            SilenceList.Add(
                new KurisuLib
                {
                    HeroName = "Fiddlesticks",
                    SpellMenuName = "Dark Wind",
                    Slot = SpellSlot.E,
                    DangerLevel = 3,
                    SDataName = "FiddlesticksDarkWind",
                });
            SilenceList.Add(
                new KurisuLib
                {
                    HeroName = "Blitzcrank",
                    SpellMenuName = "Static Field",
                    Slot = SpellSlot.R,
                    Type = Skilltype.Circle,
                    DangerLevel = 3,
                    SDataName = "StaticField",

                });
            SilenceList.Add(
                new KurisuLib
                {
                    HeroName = "Chogath",
                    SpellMenuName = "Feral Scream",
                    Slot = SpellSlot.W,
                    Type = Skilltype.Cone,
                    DangerLevel = 3,
                    SDataName = "FeralScream",

                });
            SilenceList.Add(
                new KurisuLib
                {
                    HeroName = "Malzahar",
                    SpellMenuName = "Call of the Void",
                    Slot = SpellSlot.Q,
                    Type = Skilltype.Line,
                    DangerLevel = 3,
                    SDataName = "AlZaharCalloftheVoid",
                });
            SilenceList.Add(
                new KurisuLib
                {
                    HeroName = "Talon",
                    SpellMenuName = "Cutthroat",
                    Slot = SpellSlot.E,
                    DangerLevel = 3,
                    SDataName = "TalonCutthroat",
                });
           SilenceList.Add(
                new KurisuLib
                {
                    HeroName = "Garen",
                    SpellMenuName = "Decisive Strike",
                    Slot = SpellSlot.Q,
                    DangerLevel = 3,
                    SDataName = "GarenQ",
                });
          SilenceList.Add(
                new KurisuLib
                {
                    HeroName = "Viktor",
                    SpellMenuName = "Chaos Storm",
                    Slot = SpellSlot.R,
                    DangerLevel = 3,
                    SDataName = "ViktorChaosStorm",
                });
         SilenceList.Add(
                new KurisuLib
                {
                    HeroName = "Soraka",
                    SpellMenuName = "Equinox",
                    Slot = SpellSlot.E,
                    Type = Skilltype.Circle,
                    DangerLevel = 2,
                    SDataName = "SorakaE",
                });
        }        
    }
}