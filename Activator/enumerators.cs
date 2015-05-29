namespace Activator
{
    public enum HitType
    {
        None = 0,
        AutoAttack,
        MinionAttack,
        TurretAttack,
        Spell,
        Danger,
        Ultimate,
        CrowdControl,
        Stealth
    }

    public enum MapType
    {
        SummonersRift,
        CrystalScar,
        HowlingAbyss,
        TwistedTreeline
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
