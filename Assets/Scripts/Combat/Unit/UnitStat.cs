using UnityEngine;
using DataEntity;
using DataEnum;
using System;
using Utils;

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

    public float PercentageValue(BUFF_TYPE type)
    {
        var q = new Query<float>(type, 1.0f);
        _mediator.PerformQuery(this, q);
        return Mathf.Max(0.0f, q.Value);
    }

    #region [Final Value]
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
            float value = (float)_baseData.Default_Speed * PercentageValue(BUFF_TYPE.SPEED);
            return Mathf.Max(0.0f, value);
        }
    }

    public float CriticalChance
    {
        get
        {
            float value = (float)_baseData.Critical_Chance * PercentageValue(BUFF_TYPE.CRITICAL_CHANCE);
            return Mathf.Max(0.0f, value);
        }
    }

    public float CriticalDamageRate
    {
        get
        {
            float value = (float)_baseData.Critical_Damage_Rate * PercentageValue(BUFF_TYPE.CRITICAL_DAMAGE_RATE);
            return Mathf.Max(1.0f, value);
        }
    }

    public float CounterDamageRate
    {
        get
        {
            float value = (float)_baseData.Counter_Damage_Rate * PercentageValue(BUFF_TYPE.COUNTER_DAMAGE_RATE);
            return Mathf.Max(1.0f, value);
        }
    }

    public float Element_Charge_Resist
    {
        get
        {
            float value = (float)_baseData.Element_Charge_Resist * PercentageValue(BUFF_TYPE.ELEMENT_GAUGE_RESISTANCE);
            return Mathf.Min(value, 1.0f);
        }
    }

    public float Element_Charge_Rate(ELEMENT_TYPE type)
    {
        if(type == ELEMENT_TYPE.NONE) return -1;
        float value = (float)FunctionUtils.SafeGet(_baseData.Element_Charge_Rate, (int)(type - 1)) * PercentageValue((BUFF_TYPE)((int)BUFF_TYPE.PHYSICAL_GAUGE_EFFICIENCY + (int)type) - 1);
        return Mathf.Max(0.0f, value);
    }

    public float Overload_Rate(ELEMENT_TYPE type)
    {
        if (type == ELEMENT_TYPE.NONE) return -1;
        float value = (float)FunctionUtils.SafeGet(_baseData.Overload_Rate, (int)(type - 1)) * PercentageValue((BUFF_TYPE)((int)BUFF_TYPE.PHYSICAL_OVERLOAD_RATE + (int)type) - 1);
        return Mathf.Max(0.0f, value);
    }


    #endregion
}

public class UnitStat
{
    public readonly CoreStat coreStat;
    public readonly ModifierStat modifierStat;

    private float _curHp;
    private int   _curAp;
    private int   _priority;
    public float HP     { get => _curHp; }
    public int SP       { get => _curAp; }
    public int Priority { get => _priority; }

    public Action<float, float> OnHPChanged;
    public Action<int, int> OnAPChanged;
    public Action<float> OnDamaged;
    public Action<float> OnHealed;
    public Action OnDie;

    public UnitStat(EntityData baseData, int index)
    {
        coreStat = new CoreStat(baseData);
        modifierStat = new ModifierStat(baseData);
        
        _curHp = (float)baseData.Max_HP;
        _curAp = baseData.Default_SP;
        _priority = index;
    }

    public void OnPrepareCombat()
    {
        OnHPChanged.Invoke(_curHp, modifierStat.MaxHP);
        OnAPChanged?.Invoke(_curAp, modifierStat.MaxSP);
    }

    public void GetDamaged(float value)     
    {
        _curHp = Mathf.Clamp(_curHp - value, 0f, modifierStat.MaxHP);
        OnHPChanged.Invoke(_curHp, modifierStat.MaxHP);
        OnDamaged.Invoke(value);

        if (_curHp <= 0f)
        {
            OnDie.Invoke();
        }
    }

    public void GetHealed(float value)
    {
        _curHp = Mathf.Clamp(_curHp + value, 0f, modifierStat.MaxHP);
        OnHPChanged.Invoke(_curHp, modifierStat.MaxHP);
        OnHealed.Invoke(value);
    }

    public void OnNormalAttack()
    {
        _curAp = Mathf.Clamp(_curAp + 1, 0, modifierStat.MaxSP);
        OnAPChanged.Invoke(_curAp, modifierStat.MaxSP);
    }

    public void OnSkillAttack(int value)
    {
        _curAp = Mathf.Clamp(_curAp - value, 0, modifierStat.MaxSP);
        OnAPChanged.Invoke(_curAp, modifierStat.MaxSP);
    }

    public int CompareTo(UnitStat other)
    {
        if (this.modifierStat.Speed > other.modifierStat.Speed) { return -1; }
        else if (this.modifierStat.Speed < other.modifierStat.Speed) { return 1; }
        else
        {
            if (this.coreStat.Side < other.coreStat.Side) { return -1; }
            else if (this.coreStat.Side > other.coreStat.Side) { return 1; }
            else
            {
                if (this._priority < other._priority) { return -1; }
                else return 1;
            }
        }
    }
}
