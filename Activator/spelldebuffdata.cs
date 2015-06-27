#region Copyright © 2015 Kurisu Solutions
// All rights are reserved. Transmission or reproduction in part or whole,
// any form or by any means, mechanical, electronical or otherwise, is prohibited
// without the prior written consent of the copyright owner.
// 
// Document:	activator/spelldebuffdata.cs
// Date:		06/06/2015
// Author:		Robin Kurisu
#endregion

using LeagueSharp;
using System.Collections.Generic;

namespace Activator
{
    public class spelldebuffdata
    {
        public string Name { get; set; }
        public bool Evade { get; set; }
        public bool Damage { get; set; }
        public int EvadeTimer { get; set; }
        public bool Cleanse { get; set; }
        public int CleanseTimer { get; set; }
        public SpellSlot Slot { get; set; }
        public bool QSSIgnore { get; set; }

        public bool Included { get; set; }
        public Obj_AI_Base Sender { get; set; }

        public static List<spelldebuffdata> debuffs = new List<spelldebuffdata>();

        static spelldebuffdata()
        {
            debuffs.Add(new spelldebuffdata
            {
                Name = "defile",
                Evade = false,
                Damage = true,
                EvadeTimer = 0,
                Cleanse = false,
                CleanseTimer = 0,
                Slot = SpellSlot.E
            });

            debuffs.Add(new spelldebuffdata
            {
                Name = "suppression",
                Evade = false,
                Damage = true,
                EvadeTimer = 0,
                Cleanse = true,
                CleanseTimer = 0,
                Slot = SpellSlot.R
            });

            debuffs.Add(new spelldebuffdata
            {
                Name = "jaxcounterstrike",
                Evade = false,
                Damage = true,
                EvadeTimer = 0,
                Cleanse = false,
                CleanseTimer = 0,
                Slot = SpellSlot.E
            });

            debuffs.Add(new spelldebuffdata
            {
                Name = "kennenlightningrush",
                Evade = false,
                Damage = true,
                EvadeTimer = 0,
                Cleanse = false,
                CleanseTimer = 0,
                Slot = SpellSlot.E
            });

            debuffs.Add(new spelldebuffdata
            {
                Name = "tremors2",
                Evade = false,
                Damage = true,
                EvadeTimer = 0,
                Cleanse = false,
                CleanseTimer = 0,
                Slot = SpellSlot.R
            });

            debuffs.Add(new spelldebuffdata
            {
                Name = "shyvanaimmolationaura",
                Evade = false,
                Damage = true,
                EvadeTimer = 0,
                Cleanse = false,
                CleanseTimer = 0,
                Slot = SpellSlot.W
            });

            debuffs.Add(new spelldebuffdata
            {
                Name = "swainmetamorphism",
                Evade = false,
                Damage = true,
                EvadeTimer = 0,
                Cleanse = false,
                CleanseTimer = 0,
                Slot = SpellSlot.E
            });

            debuffs.Add(new spelldebuffdata
            {
                Name = "monkeykingspintowin",
                Evade = false,
                Damage = true,
                EvadeTimer = 0,
                Cleanse = false,
                CleanseTimer = 0,
                Slot = SpellSlot.R
            });

            debuffs.Add(new spelldebuffdata
            {
                Name = "zacr",
                Evade = true,
                Damage = true,
                EvadeTimer = 150,
                Cleanse = false,
                CleanseTimer = 0,
                Slot = SpellSlot.R
            });

            debuffs.Add(new spelldebuffdata
            {
                Name = "powerball",
                Evade = false,
                Damage = true,
                EvadeTimer = 0,
                Cleanse = false,
                CleanseTimer = 0,
                Slot = SpellSlot.Q
            });

            debuffs.Add(new spelldebuffdata
            {
                Name = "volibearq",
                Evade = false,
                Damage = true,
                EvadeTimer = 0,
                Cleanse = false,
                CleanseTimer = 0,
                Slot = SpellSlot.Q
            });

            debuffs.Add(new spelldebuffdata
            {
                Name = "defensiveballcurl",
                Evade = false,
                Damage = true,
                EvadeTimer = 0,
                Cleanse = false,
                CleanseTimer = 0,
                Slot = SpellSlot.W
            });

            debuffs.Add(new spelldebuffdata
            {
                Name = "mordekaisermaceofspades",
                Evade = false,
                Damage = true,
                EvadeTimer = 0,
                Cleanse = true,
                CleanseTimer = 0,
                Slot = SpellSlot.R
            });

            debuffs.Add(new spelldebuffdata
            {
                Name = "nasusq",
                Evade = false,
                Damage = true,
                EvadeTimer = 0,
                Cleanse = false,
                CleanseTimer = 0,
                Slot = SpellSlot.Q
            });

            debuffs.Add(new spelldebuffdata
            {
                Name = "sheen",
                Evade = false,
                Damage = true,
                EvadeTimer = 0,
                Cleanse = false,
                CleanseTimer = 0,
                Slot = SpellSlot.Unknown
            });

            debuffs.Add(new spelldebuffdata
            {
                Name = "lichbane",
                Evade = false,
                Damage = true,
                EvadeTimer = 0,
                Cleanse = false,
                CleanseTimer = 0,
                Slot = SpellSlot.Unknown
            });

            debuffs.Add(new spelldebuffdata
            {
                Name = "summonerdot",
                Evade = false,
                Damage = true,
                EvadeTimer = 0,
                Cleanse = false,
                CleanseTimer = 0,
                Slot = SpellSlot.Unknown
            });

            debuffs.Add(new spelldebuffdata
            {
                Name = "burningagony",
                Evade = false,
                Damage = true,
                EvadeTimer = 0,
                Cleanse = false,
                CleanseTimer = 0,
                Slot = SpellSlot.Unknown
            });

            debuffs.Add(new spelldebuffdata
            {
                Name = "dianaorbs",
                Evade = false,
                Damage = true,
                EvadeTimer = 0,
                Cleanse = false,
                CleanseTimer = 0,
                Slot = SpellSlot.W
            });

            debuffs.Add(new spelldebuffdata
            {
                Name = "garene",
                Evade = false,
                Damage = true,
                EvadeTimer = 0,
                Cleanse = false,
                CleanseTimer = 0,
                Slot = SpellSlot.E
            });

            debuffs.Add(new spelldebuffdata
            {
                Name = "auraofdespair",
                Evade = false,
                Damage = true,
                EvadeTimer = 0,
                Cleanse = false,
                CleanseTimer = 0,
                Slot = SpellSlot.W
            });

            debuffs.Add(new spelldebuffdata
            {
                Name = "hecarimw",
                Evade = false,
                Damage = true,
                EvadeTimer = 0,
                Cleanse = false,
                CleanseTimer = 0,
                Slot = SpellSlot.W
            });

            debuffs.Add(new spelldebuffdata
            {
                Name = "bruammark",
                Evade = false,
                Damage = false,
                EvadeTimer = 0,
                Cleanse = true,
                CleanseTimer = 200,
                Slot = SpellSlot.Q
            });

            debuffs.Add(new spelldebuffdata
            {
                Name = "zedulttargetmark",
                Evade = true,
                Damage = true,
                EvadeTimer = 2800,
                Cleanse = true,
                CleanseTimer = 1800,
                Slot = SpellSlot.R
            });

            debuffs.Add(new spelldebuffdata
            {
                Name = "fallenonetarget",
                Evade = true,
                Damage = true,
                EvadeTimer = 2600,
                Cleanse = false,
                CleanseTimer = 0,
                Slot = SpellSlot.R
            });

            debuffs.Add(new spelldebuffdata
            {
                Name = "fizzmarinerdoombomb",
                Evade = false,
                Damage = true,
                EvadeTimer = 0,
                Cleanse = true,
                CleanseTimer = 0,
                Slot = SpellSlot.R
            });

            debuffs.Add(new spelldebuffdata
            {
                Name = "soulshackles",
                Evade = true,
                Damage = true,
                EvadeTimer = 2600,
                Cleanse = true,
                CleanseTimer = 1100,
                Slot = SpellSlot.R
            });

            debuffs.Add(new spelldebuffdata
            {
                Name = "varusrsecondary",
                Evade = false,
                Damage = false,
                EvadeTimer = 0,
                Cleanse = true,
                CleanseTimer = 0,
                Slot = SpellSlot.R
            });

            debuffs.Add(new spelldebuffdata
            {
                Name = "caitlynaceinthehole",
                Evade = true,
                Damage = true,
                EvadeTimer = 900,
                Cleanse = false,
                CleanseTimer = 0,
                Slot = SpellSlot.R
            });

            debuffs.Add(new spelldebuffdata
            {
                Name = "vladimirhemoplague",
                Evade = true,
                Damage = true,
                EvadeTimer = 4500,
                Cleanse = true,
                CleanseTimer = 0,
                Slot = SpellSlot.R
            });

            debuffs.Add(new spelldebuffdata
            {
                Name = "urgotswap2",
                Evade = false,
                Damage = false,
                EvadeTimer = 0,
                Cleanse = true,
                CleanseTimer = 0,
                Slot = SpellSlot.R
            });

            debuffs.Add(new spelldebuffdata
            {
                Name = "skarnerimpale",
                Evade = false,
                Damage = false,
                EvadeTimer = 0,
                Cleanse = true,
                CleanseTimer = 500,
                Slot = SpellSlot.R
            });


            debuffs.Add(new spelldebuffdata
            {
                Name = "poppydiplomaticimmunity",
                Evade = false,
                Damage = false,
                EvadeTimer = 0,
                Cleanse = true,
                CleanseTimer = 0,
                Slot = SpellSlot.R
            });

            debuffs.Add(new spelldebuffdata
            {
                Name = "blindmonkqonechaos",
                Evade = false,
                Damage = false,
                EvadeTimer = 0,
                Cleanse = true,
                CleanseTimer = 0,
                Slot = SpellSlot.Q
            });

            debuffs.Add(new spelldebuffdata
            {
                Name = "leblancsoulshackle",
                Evade = false,
                Damage = false,
                EvadeTimer = 2000,
                Cleanse = true,
                CleanseTimer = 0,
                Slot = SpellSlot.E
            });

            debuffs.Add(new spelldebuffdata
            {
                Name = "leblancsoulshacklem",
                Evade = true,
                Damage = false,
                EvadeTimer = 2000,
                Cleanse = true,
                CleanseTimer = 0,
                Slot = SpellSlot.E
            });

            debuffs.Add(new spelldebuffdata
            {
                Name = "vir",
                Evade = false,
                Damage = false,
                Cleanse = false,
                CleanseTimer = 0,
                EvadeTimer = 0,
                QSSIgnore = true,
                Slot = SpellSlot.Unknown
            });

            debuffs.Add(new spelldebuffdata
            {
                Name = "virknockup",
                Evade = false,
                Damage = false,
                Cleanse = false,
                CleanseTimer = 0,
                EvadeTimer = 0,
                QSSIgnore = true,
                Slot = SpellSlot.Unknown
            });

            debuffs.Add(new spelldebuffdata
            {
                Name = "yasuorknockupcombo",
                Evade = false,
                Damage = false,
                Cleanse = false,
                CleanseTimer = 0,
                EvadeTimer = 0,
                QSSIgnore = true,
                Slot = SpellSlot.Unknown
            });

            debuffs.Add(new spelldebuffdata
            {
                Name = "yasuorknockupcombotar",
                Evade = false,
                Damage = false,
                Cleanse = false,
                CleanseTimer = 0,
                EvadeTimer = 0,
                QSSIgnore = true,
                Slot = SpellSlot.Unknown
            });

            debuffs.Add(new spelldebuffdata
            {
                Name = "zyrabramblezoneknockup",
                Evade = false,
                Damage = false,
                Cleanse = false,
                CleanseTimer = 0,
                EvadeTimer = 0,
                QSSIgnore = true,
                Slot = SpellSlot.Unknown
            });

            debuffs.Add(new spelldebuffdata
            {
                Name = "frozenheartaura",
                Evade = false,
                Damage = false,
                Cleanse = false,
                CleanseTimer = 0,
                EvadeTimer = 0,
                QSSIgnore = true,
                Slot = SpellSlot.Unknown
            });

            debuffs.Add(new spelldebuffdata
            {
                Name = "dariusaxebrabcone",
                Evade = false,
                Damage = false,
                Cleanse = false,
                CleanseTimer = 0,
                EvadeTimer = 0,
                QSSIgnore = true,
                Slot = SpellSlot.Unknown
            });

            debuffs.Add(new spelldebuffdata
            {
                Name = "frozenheartauracosmetic",
                Evade = false,
                Damage = false,
                Cleanse = false,
                CleanseTimer = 0,
                EvadeTimer = 0,
                QSSIgnore = true,
                Slot = SpellSlot.Unknown
            });

            debuffs.Add(new spelldebuffdata
            {
                Name = "sunfirecapeaura",
                Evade = false,
                Damage = true,
                Cleanse = false,
                CleanseTimer = 0,
                EvadeTimer = 0,
                QSSIgnore = true,
                Slot = SpellSlot.Unknown
            });

            debuffs.Add(new spelldebuffdata
            {
                Name = "fizzmoveback",
                Evade = false,
                Damage = false,
                Cleanse = false,
                CleanseTimer = 0,
                EvadeTimer = 0,
                QSSIgnore = true,
                Slot = SpellSlot.Unknown
            });

            debuffs.Add(new spelldebuffdata
            {
                Name = "blessingofthelizardelderslow",
                Evade = false,
                Damage = true,
                Cleanse = false,
                CleanseTimer = 0,
                EvadeTimer = 0,
                QSSIgnore = true,
                Slot = SpellSlot.Unknown
            });

            debuffs.Add(new spelldebuffdata
            {
                Name = "dragonburning",
                Evade = false,
                Damage = false,
                Cleanse = false,
                CleanseTimer = 0,
                EvadeTimer = 0,
                QSSIgnore = true,
                Slot = SpellSlot.Unknown
            });

            debuffs.Add(new spelldebuffdata
            {
                Name = "rocketgrab2",
                Evade = false,
                Damage = false,
                Cleanse = false,
                CleanseTimer = 0,
                EvadeTimer = 0,
                QSSIgnore = true,
                Slot = SpellSlot.Unknown
            });

            debuffs.Add(new spelldebuffdata
            {
                Name = "monkeykingspinknockup",
                Evade = false,
                Damage = false,
                Cleanse = false,
                CleanseTimer = 0,
                EvadeTimer = 0,
                QSSIgnore = true,
                Slot = SpellSlot.Unknown
            });

            debuffs.Add(new spelldebuffdata
            {
                Name = "frostarrow",
                Evade = false,
                Damage = false,
                Cleanse = false,
                CleanseTimer = 0,
                EvadeTimer = 0,
                QSSIgnore = true,
                Slot = SpellSlot.Unknown
            });
        }
    }
}
