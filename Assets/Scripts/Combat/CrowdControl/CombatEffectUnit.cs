using ObservableCollections;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using R3;

public class CombatEffectUnit
{
    public enum COMBAT_EFFECT_TYPE
    {
        NORMAL,
        CC
    }

    private readonly ObservableList<IEffectable> _ccEffectList = new();
    private readonly ObservableList<IEffectable> _normalEffectList = new();

    public void Add(COMBAT_EFFECT_TYPE type, IEffectable combatEffect)
    {
        if(type == COMBAT_EFFECT_TYPE.NORMAL)
        {
            _normalEffectList.Add(combatEffect);
            combatEffect.OnDispose += () =>
            {
                _normalEffectList.Remove(combatEffect);
            };
        }
        else
        {
            _ccEffectList.Add(combatEffect);
            combatEffect.OnDispose += () =>
            {
                _ccEffectList.Remove(combatEffect);
            };
        }
    }
}