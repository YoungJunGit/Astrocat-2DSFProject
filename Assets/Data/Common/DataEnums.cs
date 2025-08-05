namespace DataEnum
{
    #region[DT 관련]
    public enum ELEMENT_TYPE
    {
        NONE = 0,
        FIRE,
        GRAVITY,
        RADIATION,
        HOLY,
        VOID
    }

    public enum TARGET_TYPE
    {
        NONE = 0,
        ENEMY_SINGLE,
        ENEMY_ALL,
        SELF,
        ALLY_SINGLE,
        ALLY_ALL
    }

    public enum EFFECT_TYPE
    {
        NONE = 0,
        DAMAGE,
        OFFENSIVE_POWER,
        DEFENSIVE_POWER,
        CLEANSE
    }

    /*추가*/
    #endregion

    #region[기본]
    public enum SIDE
    {
        NONE = 0,
        PLAYER,
        ENEMY
    }

    public enum  UNIT_STATE
    {
        NONE = 0,
        ATTACK,
        MOVE
    }

    /*추가*/
    #endregion
}