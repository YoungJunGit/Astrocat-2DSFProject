using UnityEngine;
using DataEntity;
using System;

public class UnitStat
{
    private EntityData _baseData;

    public Action<float, float> OnHPChanged;
    public Action<int, int> OnAPChanged;
    public Action<UnitStat> OnDie;

    public float Max_HP         { get => (float)_baseData.Default_HP; }
    private float _curHp;       
    public float Cur_HP         { get { return _curHp; } }
    
    public int Max_AP           { get => 9; }
    private int _curAp;         
    public int Cur_AP           {  get { return _curAp; } }

    public float Default_Speed  { get => (float)_baseData.Default_Speed; }
    private float _curSpeed; 
    public float Cur_Speed      { get { return _curSpeed; } }

    private int priority;
    public int Priority         { get { return priority; } }

    public UnitStat(EntityData baseData, int index)
    {
        _baseData = baseData;
        priority = index;
        
        _curHp = (float)baseData.Default_HP;
        _curAp = baseData.Default_AP;
        _curSpeed = (float)baseData.Default_Speed;
    }

    public void GetDamaged(float value)
    {
        _curHp = Mathf.Clamp(_curHp - value, 0f, Max_HP);
        OnHPChanged.Invoke(_curHp, Max_HP);

        if (Cur_HP <= 0f)
            OnDie.Invoke(this);
    }

    public void GetHealed(float value)
    {
        _curHp = Mathf.Clamp(_curHp + value, 0f, Max_HP);
        OnHPChanged.Invoke(_curHp, Max_HP);
    }

    public void AddSpeed(float value)
    {
        _curSpeed += value;
    }

    /// <summary>
    /// 정렬을 위한 커스텀 Compare함수
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
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
