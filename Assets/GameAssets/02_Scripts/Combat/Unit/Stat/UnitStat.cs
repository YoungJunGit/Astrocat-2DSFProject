using UnityEngine;
using DataEntity;
using DataEnum;
using System;

public class UnitStat
{
    public readonly CoreStat CoreStat;
    public readonly ModifierStat ModifierStat;

    private ELEMENT_TYPE _currentElementType = ELEMENT_TYPE.NONE;
    private float _curHp;
    private int   _curSP;
    private float _curEG;
    private int   _priority;
    public float HP     { get => _curHp; }
    public int SP       { get => _curSP; }
    public float EG { get => _curEG; }
    public int Priority { get => _priority; }

    public Action<float, float> OnHPChanged = delegate { };
    public Action<int, int> OnSPChanged = delegate { };
    public Action<ELEMENT_TYPE, bool, int, float, float> OnEGChanged = delegate { };

    public UnitStat(EntityData baseData, int priority)
    {
        CoreStat = new CoreStat(baseData);
        ModifierStat = new ModifierStat(baseData);

        _curHp = (float)baseData.Max_HP;
        _curSP = baseData.Default_SP;
        _curEG = 0.0f;
        _priority = priority;
    }

    // temp
    public void ApplySaveData(float normalizedHp, float normalizedSp)
    {
        _curHp = normalizedHp * ModifierStat.MaxHP;
        _curSP = (int)(normalizedSp * ModifierStat.MaxSP);
    }

    public void OnPrepareCombat()
    {
        OnHPChanged.Invoke(_curHp, ModifierStat.MaxHP);
        OnSPChanged?.Invoke(SP, ModifierStat.MaxSP);
    }

    public float GetDamaged(float value) 
    {
        float previous = _curHp;
        _curHp = Mathf.Clamp(_curHp - value, 0f, ModifierStat.MaxHP);
        OnHPChanged.Invoke(_curHp, ModifierStat.MaxHP);
        return previous - _curHp;
    }

    public float GetHealed(float value) 
    { 
        float previous = _curHp;
        _curHp = Mathf.Clamp(_curHp + value, 0f, ModifierStat.MaxHP);
        OnHPChanged.Invoke(_curHp, ModifierStat.MaxHP);
        return _curHp - previous;
    }

    public void AddSP(int value) 
    { 
        _curSP = Mathf.Clamp(_curSP + value, 0, ModifierStat.MaxSP);
        OnSPChanged.Invoke(_curSP, ModifierStat.MaxSP);
    }

    public void IncreaseElementGauge(ELEMENT_TYPE elementType, float value)
    {
        var finalValue = _curEG + value;
        var isReset = _currentElementType != elementType;

        if (isReset)
        {
            _currentElementType = elementType;
            finalValue = value;
        }
        int fillCount = (int)finalValue / (int)ModifierStat.MaxEG;
        _curEG = finalValue % ModifierStat.MaxEG;
        OnEGChanged.Invoke(elementType, isReset, fillCount, _curEG, ModifierStat.MaxEG);
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
