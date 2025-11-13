using UnityEngine;
using DataEntity;
using DataEnum;
using System;
using Utils;
using ObservableCollections;

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

    public float PercentageValue(BUFF_TYPE type, float defaultValue = 1.0f)
    {
        var q = new Query<float>(type, defaultValue);
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
            float value = PercentageValue(BUFF_TYPE.CRITICAL_CHANCE, (float)_baseData.Critical_Chance);
            return Mathf.Max(0.0f, value);
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
            float value = PercentageValue(BUFF_TYPE.DAMAGE_MULTIPLIER);
            return Mathf.Max(0.0f, value);
        }
    }

    public float DamageTakenMultiplier
    {
        get
        {
            float value = PercentageValue(BUFF_TYPE.DAMAGE_TAKEN_MULTIPLIER);
            return Mathf.Max(0.0f, value);
        }
    }

    public float SPConsumptionRate
    {
        get
        {
            float value = PercentageValue(BUFF_TYPE.SP_CONSUMPTION_RATE);
            return Mathf.Max(0.0f, value);
        }
    }

    public float ElementChargeResist
    {
        get
        {
            float value = PercentageValue(BUFF_TYPE.ELEMENT_GAUGE_RESISTANCE, (float)_baseData.Element_Charge_Resist);
            return Mathf.Min(value, 1.0f);
        }
    }

    public float ElementChargeRate(ELEMENT_TYPE type)
    {
        float element_charge_rate = (float)FunctionUtils.SafeGet(_baseData.Element_Charge_Rate, (int)(type - 1));
        if (type == ELEMENT_TYPE.NONE || element_charge_rate == 0f) return -1;
        float value = PercentageValue((BUFF_TYPE)((int)BUFF_TYPE.PHYSICAL_GAUGE_EFFICIENCY + (int)type) - 1, element_charge_rate);
        return Mathf.Max(0.0f, value);
    }

    public float OverloadRate(ELEMENT_TYPE type)
    {
        float overload_rate = (float)FunctionUtils.SafeGet(_baseData.Overload_Rate, (int)(type - 1));
        if (type == ELEMENT_TYPE.NONE || overload_rate == 0f) return -1;
        float value = PercentageValue((BUFF_TYPE)((int)BUFF_TYPE.PHYSICAL_OVERLOAD_RATE + (int)type) - 1, overload_rate);
        return Mathf.Max(0.0f, value);
    }


    #endregion
}

public class UnitStat
{
    public readonly CoreStat CoreStat;
    public readonly ModifierStat ModifierStat;

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

    #region[Observable]
    private ObservableDictionary<ELEMENT_TYPE, int> ceList = new()
    {
        { ELEMENT_TYPE.PHYSICAL,  0 },
        { ELEMENT_TYPE.FIRE,      0 },
        { ELEMENT_TYPE.RADIATION, 0 },
        { ELEMENT_TYPE.GRAVITY,   0 },
        { ELEMENT_TYPE.VOID,      0 },
        { ELEMENT_TYPE.HOLY,      0 },
        { ELEMENT_TYPE.ETC,       0 },
    };
    public IReadOnlyObservableDictionary<ELEMENT_TYPE, int> CEList => ceList;

    public void OnAddCE(ELEMENT_TYPE type) => ceList[type]++;
    public void OnSetCE(ELEMENT_TYPE type, int value) => ceList[type] = value;
    #endregion
}
