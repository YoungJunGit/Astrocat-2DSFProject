using UnityEngine;
using DataEntity;
using DataEnum;
using System;
using Utils;
using ObservableCollections;
using R3;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

#region [Core Stat]
public class CoreStat
{
    private readonly EntityData _baseData;
    public CoreStat(EntityData baseData)
    {
        _baseData = baseData;
    }

    public SIDE Side => _baseData.Side;
    public string Name => _baseData.Name;
    public string AssetFileName => _baseData.Asset_File;
    public string[] SkillsID => _baseData.Skill_ID;
}
#endregion

#region [Modifier Stat]
public class ModifierStat
{
    private readonly EntityData _baseData;
    public ModifierStat(EntityData baseData)
    {
        _baseData = baseData;
        _mediator = new StatsMediator();
    }

    private readonly StatsMediator _mediator;
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
}
#endregion

public class UnitStat
{
    public readonly CoreStat CoreStat;
    public readonly ModifierStat ModifierStat;

    private readonly ObservableDictionary<BUFF_TYPE, IconInfo> _buffIconInfoDic = new();
    private readonly ObservableDictionary<ELEMENT_TYPE, IconInfo> _elementIconInfoDic = new();

    private float _curHp;
    private int   _curSP;
    private int   _priority;
    public float HP     { get => _curHp; }
    public int SP       { get => _curSP; }
    public int Priority { get => _priority; }

    public Action<float, float> OnHPChanged;
    public Action<int, int> OnSPChanged;
    public Action<IDamage> OnDamaged;
    public Action<float> OnHealed;
    public Action OnDie;

    public UnitStat(EntityData baseData, int priority)
    {
        CoreStat = new CoreStat(baseData);
        ModifierStat = new ModifierStat(baseData);

        _curHp = (float)baseData.Max_HP;
        _curSP = baseData.Default_SP;
        _priority = priority;
    }

    public void OnPrepareCombat()
    {
        OnHPChanged.Invoke(_curHp, ModifierStat.MaxHP);
        OnSPChanged?.Invoke(_curSP, ModifierStat.MaxSP);
    }

    public void GetDamaged(IDamage damage)
    {
        _curHp = Mathf.Clamp(_curHp - damage.Value, 0f, ModifierStat.MaxHP);
        OnHPChanged.Invoke(_curHp, ModifierStat.MaxHP);
        OnDamaged.Invoke(damage);

        if (_curHp <= 0f)
        {
            OnDie.Invoke();
        }
    }

    public void GetHealed(float value)
    {
        _curHp = Mathf.Clamp(_curHp + value, 0f, ModifierStat.MaxHP);
        OnHPChanged.Invoke(_curHp, ModifierStat.MaxHP);
        OnHealed.Invoke(value);
    }

    public void OnNormalAttack()
    {
        _curSP = Mathf.Clamp(_curSP + 1, 0, ModifierStat.MaxSP);
        OnSPChanged.Invoke(_curSP, ModifierStat.MaxSP);
    }

    public void OnSkillAttack(int value)
    {
        _curSP = Mathf.Clamp(_curSP - value, 0, ModifierStat.MaxSP);
        OnSPChanged.Invoke(_curSP, ModifierStat.MaxSP);
    }

    public int CompareTo(UnitStat other)
    {
        if (this.ModifierStat.Speed > other.ModifierStat.Speed) { return -1; }
        else if (this.ModifierStat.Speed < other.ModifierStat.Speed) { return 1; }
        else
        {
            if (this.CoreStat.Side < other.CoreStat.Side) { return -1; }
            else if (this.CoreStat.Side > other.CoreStat.Side) { return 1; }
            else
            {
                if (this._priority < other._priority) { return -1; }
                else return 1;
            }
        }
    }
}
