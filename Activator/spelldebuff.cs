using System.Collections.Generic;
using LeagueSharp;

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
                Name = "caitlynaceinthehole",
                Evade = true,
                Damage = true,
                EvadeTimer = 1000,
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
        }
    }
}
