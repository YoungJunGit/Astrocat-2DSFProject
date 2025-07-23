using UnityEngine;
using DataEntity;
using System;

public class UnitStat
{
    private EntityData _baseData;

    public Action<float, float> OnHPChanged;
    public Action<int, int> OnAPChanged;
    public Action OnDie;

    public string Name { get => _baseData.Name; }
    public float Max_HP { get => (float)_baseData.Default_HP; }
    
    private float _curHp;       
    public float Cur_HP { get { return _curHp; } }
    
    public int Max_AP { get => _baseData.Default_AP; }
    
    private int _curAp;         
    public int Cur_AP {  get { return _curAp; } }

    public UnitStat(EntityData baseData)
    {
        _baseData = baseData;
        
        _curHp = (float)baseData.Default_HP;
        _curAp = baseData.Default_AP;
    }

    public void GetDamaged(float value)
    {
        _curHp = Mathf.Clamp(_curHp - value, 0f, Max_HP);
        OnHPChanged.Invoke(_curHp, Max_HP);

        if (Cur_HP <= 0f)
            OnDie.Invoke();
    }

    public void GetHealed(float value)
    {
        _curHp = Mathf.Clamp(_curHp + value, 0f, Max_HP);
        OnHPChanged.Invoke(_curHp, Max_HP);
    }

    public EntityData GetData() { return _baseData; }
}
