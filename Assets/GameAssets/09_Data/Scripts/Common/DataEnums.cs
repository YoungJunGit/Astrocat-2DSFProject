namespace DataEnum
{
    #region[DT ����]

    public enum SIDE
    {
        NONE = 0,
        PLAYER,
        ENEMY
    }

    public enum TARGET_TYPE
    {
        NONE = 0,
        SINGLE,
        ALL,
        RANDOM,
        SPLASH
    }

    public enum ELEMENT_TYPE
    {
        NONE = 0,
        PHYSICAL,
        FIRE,
        RADIATION,
        GRAVITY,
        VOID,
        HOLY,
        ETC
    }

    public enum ELEMENT_STATUS_CATEGORY
    {
        NONE = 0,
        STUN,
        BURN,
        CONTAMINATION,
        SUPPRESS,
        STRANGE,
        SILENCE,
        WEAKNESS,
        OVERHEAT,
        EXPOSURE,
        BIND,
        CORRODE,
        DOMINATE,
        CHAOS
    }

    public enum SKILL_TYPE
    {
        NONE = 0,
        PASSIVE_SKILL,
        ATTACK_SKILL,
        HEAL_SKILL,
        SUPPORT_SKILL
    }

    public enum SKILL_ELEMENT_RATE
    {
        NONE = 0,
        MINOR,
        STANDARD,
        GRAND
    }

    public enum BUFF_TYPE
    {
        NONE = 0,
        MAX_HP,
        MAX_SP,
        ATTACK,
        DEFENSE,
        SPEED,
        CRITICAL_CHANCE,
        CRITICAL_DAMAGE_RATE,
        COUNTER_DAMAGE_RATE,
        PHYSICAL_GAUGE_EFFICIENCY,
        FIRE_GAUGE_EFFICIENCY,
        RADIATION_GAUGE_EFFICIENCY,
        GRAVITY_GAUGE_EFFICIENCY,
        VOID_GAUGE_EFFICIENCY,
        HOLY_GAUGE_EFFICIENCY,
        PHYSICAL_GAUGE_RESISTANCE,
        FIRE_GAUGE_RESISTANCE,
        RADIATION_GAUGE_RESISTANCE,
        GRAVITY_GAUGE_RESISTANCE,
        VOID_GAUGE_RESISTANCE,
        HOLY_GAUGE_RESISTANCE,
        PHYSICAL_OVERLOAD_RATE,
        FIRE_OVERLOAD_RATE,
        RADIATION_OVERLOAD_RATE,
        GRAVITY_OVERLOAD_RATE,
        VOID_OVERLOAD_RATE,
        HOLY_OVERLOAD_RATE,
        DAMAGE_MULTIPLIER,
        DAMAGE_TAKEN_MULTIPLIER,
        SP_CONSUMPTION_RATE,
        DAMAGE_EACH_TURN,
        HEAL_EACH_TURN,
        STUN,
        STRANGE,
        SILENCE
    }

    /*�߰�*/
    #endregion

    #region[�⺻]

    public enum UNIT_TYPE
    {
        NONE = 0,
        MELEE,
        RANGE
    }

    public enum UNIT_STATE
    {
        NONE = 0,
        ATTACK,
        MOVE
    }

    public enum UPDATE_TYPE
    {
        NONE,
        ROUND,
        TURN
    }

    public enum ANIMATION
    {
        NONE = 0,
        IDLE,
        ATTACK,
        HIT,
        DEATH,
        MOVE,
        RETREAT,
        SKILL,
        PARRY
    }

    public enum ANIMATION_EVENT
    {
        NONE = 0,
        ATTACK,
        MOVE,
        SKILL,
        PARRY_START,
        PARRY_END
    }

    /*�߰�*/
    #endregion
}