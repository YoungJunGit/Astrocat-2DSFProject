using UnityEngine;
using DataEntity;
using DataEnum;
using Utils;
using System.Collections.Generic;

public class ModifierStat
{
    private readonly EntityData _baseData;
    public ModifierStat(EntityData baseData)
    {
        _baseData = baseData;
        _mediator = new StatsMediator();
    }

    private StatsMediator _mediator;
    public StatsMediator Mediator => _mediator;

    private float PercentageValue(BUFF_TYPE type, float defaultValue = 1.0f)
    {
        var q = new Query<float>(type, defaultValue);
        _mediator.PerformQuery(this, q);
        return Mathf.Max(0.0f, q.Value);
    }

    public float DamageHealValue(BUFF_TYPE type)
    {
        var q = new Query<float>(type, 0.0f);
        _mediator.PerformQuery(this, q);
        return Mathf.Max(0.0f, q.Value);
    }

    public void ResetStatMediator()
    {
        _mediator = new StatsMediator();
    }

    public float MaxEG => 100.0f;

    #region [DT-Value]
    public float MaxHP
    {
        get
        {
            float value = (float)_baseData.Max_HP * PercentageValue(BUFF_TYPE.MAX_HP);
            return Mathf.Max(1.0f, value);
        }
    }

    public int MaxSP
    {
        get
        {
            float value = _baseData.Max_SP * PercentageValue(BUFF_TYPE.MAX_SP);
            return Mathf.Max(1, Mathf.RoundToInt(value));
        }
    }

    public float Attack
    {
        get
        {
            float value = (float)_baseData.Default_Attack * PercentageValue(BUFF_TYPE.ATTACK);
            return Mathf.Max(1.0f, value);
        }
    }

    public float Defense
    {
        get
        {
            float value = (float)_baseData.Default_Defense * PercentageValue(BUFF_TYPE.DEFENSE);
            return Mathf.Max(1.0f, value);
        }
    }

    public float Speed
    {
        get
        {
            return (float)_baseData.Default_Speed * PercentageValue(BUFF_TYPE.SPEED);
        }
    }

    public float CriticalChance
    {
        get
        {
            return PercentageValue(BUFF_TYPE.CRITICAL_CHANCE, (float)_baseData.Critical_Chance);
        }
    }

    public float CriticalDamageRate
    {
        get
        {
            float value = PercentageValue(BUFF_TYPE.CRITICAL_DAMAGE_RATE, (float)_baseData.Critical_Damage_Rate);
            return Mathf.Max(1.0f, value);
        }
    }

    public float CounterDamageRate
    {
        get
        {
            float value = PercentageValue(BUFF_TYPE.COUNTER_DAMAGE_RATE, (float)_baseData.Counter_Damage_Rate);
            return Mathf.Max(1.0f, value);
        }
    }

    public float DamageMultiplier
    {
        get
        {
            return PercentageValue(BUFF_TYPE.DAMAGE_MULTIPLIER);
        }
    }

    public float DamageTakenMultiplier
    {
        get
        {
            return PercentageValue(BUFF_TYPE.DAMAGE_TAKEN_MULTIPLIER);
        }
    }

    public float SPConsumptionRate
    {
        get
        {
            return PercentageValue(BUFF_TYPE.SP_CONSUMPTION_RATE);
        }
    }

    public float ElementChargeRate(ELEMENT_TYPE type)
    {
        if (type == ELEMENT_TYPE.NONE || type == ELEMENT_TYPE.ETC) return -1;
        float element_charge_rate = (float)FunctionUtils.SafeGet(_baseData.Element_Charge_Rate, (int)(type - 1));
        return PercentageValue((BUFF_TYPE)((int)BUFF_TYPE.PHYSICAL_GAUGE_EFFICIENCY + (int)type) - 1, element_charge_rate);
    }

    public float ElementChargeResist(ELEMENT_TYPE type)
    {
        if (type == ELEMENT_TYPE.NONE || type == ELEMENT_TYPE.ETC) return -1;
        float element_charge_resist = (float)FunctionUtils.SafeGet(_baseData.Element_Charge_Resist, (int)(type - 1));
        return PercentageValue((BUFF_TYPE)((int)BUFF_TYPE.PHYSICAL_GAUGE_RESISTANCE + (int)type) - 1, element_charge_resist);
    }

    public float OverloadRate(ELEMENT_TYPE type)
    {
        if (type == ELEMENT_TYPE.NONE || type == ELEMENT_TYPE.ETC) return -1;
        float overload_rate = (float)FunctionUtils.SafeGet(_baseData.Overload_Rate, (int)(type - 1));
        return PercentageValue((BUFF_TYPE)((int)BUFF_TYPE.PHYSICAL_OVERLOAD_RATE + (int)type) - 1, overload_rate);
    }
    #endregion

    #region [Skill Related]

    public class TauntInfo
    {
        public int Count = 0;
        public string ForcedTargetID = string.Empty;
    }

    private TauntInfo tauntInfo = new();
    public string TauntID => tauntInfo.ForcedTargetID;
    public int TauntCount => tauntInfo.Count;

    public void SetTaunt(int count, string id)
    {
        tauntInfo.Count = count;
        tauntInfo.ForcedTargetID = id;
    }

    public void DecreaseTauntCount() => tauntInfo.Count--;

    #endregion
}