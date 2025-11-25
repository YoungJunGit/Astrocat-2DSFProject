using DataEnum;
using System;

namespace DataEntity
{
    #region[DT]
    [Serializable]
    public class EntityData
    {
        public string   code;
        public string   table;
        public string   kind;
        public string   index;
        public SIDE     Side                    = SIDE.NONE;
        public string   Name;
        public double   Max_HP                  = 0.0f;
        public int      Max_SP                  = 0;
        public double   Default_Attack          = 0.0f;
        public double   Default_Defense         = 0.0f;
        public int      Default_SP              = 0;
        public double   Default_Speed           = 0.0f;
        public double   Critical_Chance         = 0.0f;
        public double   Critical_Damage_Rate    = 0.0f;
        public double   Counter_Damage_Rate     = 0.0f;
        public string[] Skill_ID;
        public double[] Element_Charge_Rate;
        public double[] Element_Charge_Resist;
        public double[] Overload_Rate;
        public string   Asset_File;
    }

    [Serializable]
    public class SkillData
    {
        public string               Skill_ID;
        public string               table;
        public string               kind;
        public string               index;
        public string               Skill_Name;
        public string               Skill_Desc;
        public ELEMENT_TYPE         Element_Type        = ELEMENT_TYPE.NONE;
        public SKILL_TYPE           Skill_Type          = SKILL_TYPE.NONE;
        public SIDE                 Skill_Side          = SIDE.NONE;
        public TARGET_TYPE          Skill_Target_Type   = TARGET_TYPE.NONE;
        public int                  Skill_Cost_SP       = 0;
        public double               Skill_ATK_Rate      = 0.0f;
        public int                  Skill_Hit_Count     = 0;
        public int                  Skill_Duration_Turn = 0;
        public SKILL_ELEMENT_RATE   Skill_Element_Rate  = SKILL_ELEMENT_RATE.NONE;
        public string[]             Buff_ID;
        public double[]             Buff_Value;
        public string               QTE_ID;
    }

    [Serializable]
    public class BuffData
    {
        public string    Buff_ID;
        public string    table;
        public string    kind;
        public string    index;
        public BUFF_TYPE Buff_Type = BUFF_TYPE.NONE;
    }

    [Serializable]
    public class ElementStatusData
    {
        public string   Element_Status_ID;
        public string   table;
        public string   kind;
        public string   index;
        public string   Buff_Table_ID;
        public double[] Element_Status_Value;
        public int      Element_Status_Duration_Turn = 0;
        public string   Element_Status_Name;
    }

    [Serializable]
    public class StringData
    {
        public string String_ID;
        public string table;
        public string kind;
        public string index;
        public string String_Key_Value;
        public string Korean;
        public string English;
    }

    #endregion
}