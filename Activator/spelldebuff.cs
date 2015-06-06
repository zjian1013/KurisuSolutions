#region Copyright © 2015 Kurisu Solutions
// All rights are reserved. Transmission or reproduction in part or whole,
// any form or by any means, mechanical, electronical or otherwise, is prohibited
// without the prior written consent of the copyright owner.
// 
// Document:	activator/spelldebuff.cs
// Date:		06/06/2015
// Author:		Robin Kurisu
#endregion

using LeagueSharp;
using System.Collections.Generic;

namespace Activator
{
    public class spelldebuff
    {
        public string Name { get; set; }
        public bool Evade { get; set; }
        public bool Damage { get; set; }
        public int EvadeTimer { get; set; }
        public bool Cleanse { get; set; }
        public int CleanseTimer { get; set; }
        public SpellSlot Slot { get; set; }

        public static List<spelldebuff> debuffs = new List<spelldebuff>();

        static spelldebuff()
        {
            debuffs.Add(new spelldebuff
            {
                Name = "bruammark",
                Evade = false,
                Damage = false,
                EvadeTimer = 0,
                Cleanse = true,
                CleanseTimer = 200,
                Slot = SpellSlot.Q
            });

            debuffs.Add(new spelldebuff
            {
                Name = "zedulttargetmark",
                Evade = true,
                Damage = true,
                EvadeTimer = 2800,
                Cleanse = true,
                CleanseTimer = 1800,
                Slot = SpellSlot.R
            });

            debuffs.Add(new spelldebuff
            {
                Name = "fallenonetarget",
                Evade = true,
                Damage = true,
                EvadeTimer = 2600,
                Cleanse = false,
                CleanseTimer = 0,
                Slot = SpellSlot.R
            });

            debuffs.Add(new spelldebuff
            {
                Name = "fizzmarinerdoombomb",
                Evade = false,
                Damage = true,
                EvadeTimer = 0,
                Cleanse = true,
                CleanseTimer = 0,
                Slot = SpellSlot.R
            });

            debuffs.Add(new spelldebuff
            {
                Name = "soulshackles",
                Evade = false,
                Damage = false,
                EvadeTimer = 2600,
                Cleanse = true,
                CleanseTimer = 1100,
                Slot = SpellSlot.R
            });

            debuffs.Add(new spelldebuff
            {
                Name = "varusrsecondary",
                Evade = false,
                Damage = true,
                EvadeTimer = 0,
                Cleanse = true,
                CleanseTimer = 0,
                Slot = SpellSlot.R
            });

            debuffs.Add(new spelldebuff
            {
                Name = "caitlynaceinthehole",
                Evade = true,
                Damage = true,
                EvadeTimer = 800,
                Cleanse = false,
                CleanseTimer = 0,
                Slot = SpellSlot.R
            });

            debuffs.Add(new spelldebuff
            {
                Name = "vladimirhemoplague",
                Evade = true,
                Damage = true,
                EvadeTimer = 4500,
                Cleanse = true,
                CleanseTimer = 0,
                Slot = SpellSlot.R
            });

            debuffs.Add(new spelldebuff
            {
                Name = "urgotswap2",
                Evade = false,
                Damage = false,
                EvadeTimer = 0,
                Cleanse = true,
                CleanseTimer = 0,
                Slot = SpellSlot.R
            });

            debuffs.Add(new spelldebuff
            {
                Name = "skarnerimpale",
                Evade = false,
                Damage = false,
                EvadeTimer = 0,
                Cleanse = true,
                CleanseTimer = 1000,
                Slot = SpellSlot.R
            });


            debuffs.Add(new spelldebuff
            {
                Name = "poppydiplomaticimmunity",
                Evade = false,
                Damage = false,
                EvadeTimer = 0,
                Cleanse = true,
                CleanseTimer = 0,
                Slot = SpellSlot.R
            });

            debuffs.Add(new spelldebuff
            {
                Name = "blindmonkqonechaos",
                Evade = false,
                Damage = false,
                EvadeTimer = 0,
                Cleanse = true,
                CleanseTimer = 0,
                Slot = SpellSlot.Q
            });

            debuffs.Add(new spelldebuff
            {
                Name = "leblancsoulshackle",
                Evade = false,
                Damage = false,
                EvadeTimer = 2000,
                Cleanse = true,
                CleanseTimer = 0,
                Slot = SpellSlot.Q
            });

            debuffs.Add(new spelldebuff
            {
                Name = "leblancsoulshacklem",
                Evade = true,
                Damage = false,
                EvadeTimer = 2000,
                Cleanse = true,
                CleanseTimer = 0,
                Slot = SpellSlot.Q
            });

            debuffs.Add(new spelldebuff
            {
                Name = "defile",
                Evade = false,
                Damage = true,
                EvadeTimer = 0,
                Cleanse = false,
                CleanseTimer = 0,
                Slot = SpellSlot.E
            });

            debuffs.Add(new spelldebuff
            {
                Name = "suppression",
                Evade = false,
                Damage = true,
                EvadeTimer = 0,
                Cleanse = true,
                CleanseTimer = 0,
                Slot = SpellSlot.R
            });
        }

        public static List<string> excludedbuffs = new List<string>
        {
            "vir",
            "virknockup",
            "yasuorknockupcombo",
            "yasuorknockupcombotar",
            "zyrabramblezoneknockup",
            "frozenheartaura",
            "dariusaxebrabcone",
            "frozenheartauracosmetic",
            "sunfirecapeaura",
            "fizzmoveback",
            "blessingofthelizardelderslow",
            "dragonburning",
            "rocketgrab2",
            "monkeykingspinknockup",
            "frostarrow",
        };
    }
}
