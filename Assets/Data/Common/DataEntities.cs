using DataEnum;
using System;

namespace DataEntity
{
    [Serializable]
    public class CharacterDataEntity
    {
        public string       Character_ID;
        public string       Char_Name;
        public double       Char_Default_HP         = 0.0f;
        public double       Char_Default_Attack     = 0.0f;
        public int          Char_Default_AP         = 0;
        public double       Char_Default_Speed      = 0.0f;
        public string       Skill1_ID;
        public string       Skill2_ID;
        public string       Skill3_ID;
        public ELEMENT_TYPE Weak_Type               = ELEMENT_TYPE.NONE;
        public ELEMENT_TYPE Resist_Type             = ELEMENT_TYPE.NONE;
    }

    [Serializable]
    public class MonsterDataEntity
    {
        public string       Mob_ID;
        public string       Mob_Name;
        public double       Mob_Default_HP          = 0.0f;
        public double       Mob_Default_Attack      = 0.0f;
        public int          Mob_Default_AP          = 0;
        public double       Mob_Default_Speed       = 0.0f; 
        public string       Skill1_ID;
        public string       Skill2_ID;
        public string       Skill3_ID;
        public ELEMENT_TYPE Weak_Type               = ELEMENT_TYPE.NONE;
        public ELEMENT_TYPE Resist_Type             = ELEMENT_TYPE.NONE;
    }

    [Serializable]
    public class SkillDataEntity
    {
        public string       Skill_ID;
        public string       Skill_Name;
        public int          Skill_CostAP             = 0;
        public string       Skill_EffectDesc;
        public EFFECT_TYPE  Effect1_Type             = EFFECT_TYPE.NONE;
        public double       Effect1_Chance           = 0.0f;
        public int          Effect1_Duration_Turns   = 0;
        public TARGET_TYPE  Effect1_Target_Type      = TARGET_TYPE.NONE;
        public double       Effect1_Value            = 0.0f;
        public EFFECT_TYPE  Effect2_Type             = EFFECT_TYPE.NONE;
        public double       Effect2_Chance           = 0.0f;
        public int          Effect2_Duration_Turns   = 0;
        public TARGET_TYPE  Effect2_Target_Type      = TARGET_TYPE.NONE;
        public double       Effect2_Value            = 0.0f;
    }

    [Serializable]
    public class StringDataEntity
    {
        public string String_ID;
        public string Korean;
        public string English;
    }
    /*Ãß°¡*/
}