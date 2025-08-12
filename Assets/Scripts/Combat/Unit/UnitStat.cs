using UnityEngine;
using DataEntity;
using DataEnum;
using System;

public class UnitStat
{
    private EntityData _baseData;

    private float _curHp;
    private int _curAp;
    private float _curSpeed;
    private int priority;

    public Action<float, float> OnHPChanged;
    public Action<int, int> OnAPChanged;
    public Action<UnitStat> OnDie;
    
    public string Name { get => _baseData.Name; }
    public ELEMENT_TYPE WeakType { get => _baseData.Weak_Type; }
    public ELEMENT_TYPE ResistType { get => _baseData.Resist_Type; }

    public ELEMENT_TYPE _currentCondition = ELEMENT_TYPE.NONE;

    public double DefaultAttack  { get => _baseData.Default_Attack; }
    public float Max_HP         { get => (float)_baseData.Default_HP; }
    public int Max_AP           { get => 9; }
    public float Default_Speed  { get => (float)_baseData.Default_Speed; }
    public float Cur_HP         { get => _curHp; }
    public int Cur_AP           { get => _curAp; }
    public float Cur_Speed      { get => _curSpeed; }
    public int Priority         { get => priority; }  

    public int radiantStack = 0 ;
    public int fireStack = 0 ;
    public int gravityStack = 0 ;
    public int forbiddenStack = 0;


    public UnitStat(EntityData baseData, int index)
    {
        _baseData = baseData;
        priority = index;
        
        _curHp = (float)baseData.Default_HP;
        _curAp = baseData.Default_AP;
        _curSpeed = (float)baseData.Default_Speed;
    }

    public void OnPrepareCombat()
    {
        OnHPChanged.Invoke(_curHp, Max_HP);
        OnAPChanged?.Invoke(_curAp, Max_AP);
    }

    public void GetDamaged(float value, float Cur_HP)     
    {
        _curHp = Cur_HP;
        _curHp = Mathf.Clamp(_curHp - value, 0f, Max_HP);
        OnHPChanged.Invoke(_curHp, Max_HP);

        if (Cur_HP <= 0f)
        {
            OnDie.Invoke(this);
        }
    }

    public void GetHealed(float value)
    {
        _curHp = Mathf.Clamp(_curHp + value, 0f, Max_HP);
        OnHPChanged.Invoke(_curHp, Max_HP);
    }

    public void OnNormalAttack()
    {
        _curAp = Mathf.Clamp(_curAp + 1, 0, Max_AP);
        OnAPChanged.Invoke(_curAp, Max_AP);
    }

    public void OnSkillAttack(int value)
    {
        _curAp = Mathf.Clamp(_curAp - value, 0, Max_AP);
        OnAPChanged.Invoke(_curAp, Max_AP);
    }

    public void AddSpeed(float value)
    {
        _curSpeed += value;
    }

    public int CompareTo(UnitStat other)
    {
        if (this.Cur_Speed > other.Cur_Speed) { return -1; }
        else if (this.Cur_Speed < other.Cur_Speed) { return 1; }
        else
        {
            if (this._baseData.Side < other._baseData.Side) { return -1; }
            else if (this._baseData.Side > other._baseData.Side) { return 1; }
            else
            {
                if (this.priority < other.priority) { return -1; }
                else return 1;
            }
        }
    }

    public EntityData GetData() { return _baseData; }
}
