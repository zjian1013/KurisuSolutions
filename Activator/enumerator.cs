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
        EnemyLowHP,
        TurretHP
    }
}
