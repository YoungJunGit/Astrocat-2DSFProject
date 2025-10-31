using UnityEngine;
using DataEntity;
using DataEnum;
using System;

public class UnitStat
{
    private readonly EntityData _baseData;
    public SIDE Side            => _baseData.Side;
    public string Name          => _baseData.Name;
    public string AssetFileName => _baseData.Asset_File;
    public string[] SkillsID    => _baseData.Skill_ID;

    public enum BUFF_TYPE
    {
        NONE = 0,
        Max_HP,
        Max_SP,
        Attack,
        Defense,
        Speed,
        CriticalChance,
        CriticalDamageRate,
        CounterDamageRate
    }

    private float _curHp;
    private int   _curAp;
    private int   _priority;
    public float HP     { get => _curHp; }
    public int SP       { get => _curAp; }
    public int Priority { get => _priority; }

    private readonly StatsMediator _mediator;
    public StatsMediator Mediator => _mediator;

    public float MaxHP
    {
        get
        {
            return (float)_baseData.Max_HP;
        }
    }

    public int MaxSP
    {
        get
        {
            return _baseData.Max_SP;
        }
    }

    public float Attack 
    {
        get 
        {
            var q = new Query<float>(BUFF_TYPE.Attack, (float)_baseData.Default_Attack);
            _mediator.PerformQuery(this, q);
            return q.Value;
        }
    }

    public float Defense
    {
        get
        {
            return (float)_baseData.Default_Defense;
        }
    }

    public float Speed
    {
        get
        {
            return (float)_baseData.Default_Speed;
        }
    }

    public float CriticalChance
    {
        get
        {
            return (float)_baseData.Critical_Chance;
        }
    }

    public float CriticalDamageRate
    {
        get
        {
            return (float)_baseData.Critical_Damage_Rate;
        }
    }

    public float CounterDamageRate
    {
        get
        {
            return (float)_baseData.Counter_Damage_Rate;
        }
    }

    public Action<float, float> OnHPChanged;
    public Action<int, int> OnAPChanged;
    public Action<float> OnDamaged;
    public Action<float> OnHealed;
    public Action OnDie;

    public UnitStat(EntityData baseData, int index)
    {
        _mediator = new StatsMediator();
        _baseData = baseData;
        _priority = index;
        
        _curHp = (float)baseData.Max_HP;
        _curAp = baseData.Default_SP;
    }

    public void OnPrepareCombat()
    {
        OnHPChanged.Invoke(_curHp, MaxHP);
        OnAPChanged?.Invoke(_curAp, MaxSP);
    }

    public void GetDamaged(float value)     
    {
        _curHp = Mathf.Clamp(_curHp - value, 0f, MaxHP);
        OnHPChanged.Invoke(_curHp, MaxHP);
        OnDamaged.Invoke(value);

        if (_curHp <= 0f)
        {
            OnDie.Invoke();
        }
    }

    public void GetHealed(float value)
    {
        _curHp = Mathf.Clamp(_curHp + value, 0f, MaxHP);
        OnHPChanged.Invoke(_curHp, MaxHP);
        OnHealed.Invoke(value);
    }

    public void OnNormalAttack()
    {
        _curAp = Mathf.Clamp(_curAp + 1, 0, MaxSP);
        OnAPChanged.Invoke(_curAp, MaxSP);
    }

    public void OnSkillAttack(int value)
    {
        _curAp = Mathf.Clamp(_curAp - value, 0, MaxSP);
        OnAPChanged.Invoke(_curAp, MaxSP);
    }

    public int CompareTo(UnitStat other)
    {
        if (this.Speed > other.Speed) { return -1; }
        else if (this.Speed < other.Speed) { return 1; }
        else
        {
            if (this._baseData.Side < other._baseData.Side) { return -1; }
            else if (this._baseData.Side > other._baseData.Side) { return 1; }
            else
            {
                if (this._priority < other._priority) { return -1; }
                else return 1;
            }
        }
    }
}
