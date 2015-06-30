#region Copyright © 2015 Kurisu Solutions
// All rights are reserved. Transmission or reproduction in part or whole,
// any form or by any means, mechanical, electronical or otherwise, is prohibited
// without the prior written consent of the copyright owner.
// 
// Document:	activator/enumerators.cs
// Date:		01/07/2015
// Author:		Robin Kurisu
#endregion

namespace Activator
{
    public enum HitType
    {
        None = 0,
        AutoAttack = 1,
        MinionAttack = 2,
        TurretAttack = 3,
        Spell = 4,
        Danger = 5,
        Ultimate = 6,
        CrowdControl = 7,
        Stealth = 8
    }

    public enum MapType
    {        
        Common = 0,
        SummonersRift = 1,
        CrystalScar = 2,
        TwistedTreeline = 3,
        HowlingAbyss = 4,
    }

    public enum MenuType
    {
        Zhonyas,
        Stealth,
        Cleanse,
        SlowRemoval,
        SpellShield,
        ActiveCheck,
        SelfCount,
        SelfMuchHP,
        SelfLowHP,
        SelfLowMP,
        SelfMinMP,
        SelfMinHP,
        EnemyLowHP
    }
}
