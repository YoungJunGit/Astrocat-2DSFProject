using DataEnum;
using ObservableCollections;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using R3;
using static CrowdControlManager;
using Unity.VisualScripting;

public class CrowdControlUnit
{
    private readonly Dictionary<ELEMENT_TYPE, ObservableList<ICrowdControl>> _effectDictionary = new()
        {
            {ELEMENT_TYPE.PHYSICAL, new ObservableList<ICrowdControl>() },
            {ELEMENT_TYPE.FIRE, new ObservableList<ICrowdControl>() },
            {ELEMENT_TYPE.RADIATION, new ObservableList<ICrowdControl>() },
            {ELEMENT_TYPE.GRAVITY, new ObservableList<ICrowdControl>() },
            {ELEMENT_TYPE.VOID, new ObservableList<ICrowdControl>() },
            {ELEMENT_TYPE.HOLY, new ObservableList<ICrowdControl>() },
            {ELEMENT_TYPE.ETC, new ObservableList<ICrowdControl>() }
        };

    public IReadOnlyDictionary<ELEMENT_TYPE, IReadOnlyObservableList<ICrowdControl>> EffectDictionary =>
        _effectDictionary.ToDictionary(kv => kv.Key, kv => (IReadOnlyObservableList<ICrowdControl>)kv.Value);

    public ELEMENT_TYPE Previous_Element_Type { get; set; } = ELEMENT_TYPE.NONE;

    public void Add(ELEMENT_TYPE elementType, ICrowdControl c)
    {
        if(elementType != ELEMENT_TYPE.ETC)
            Previous_Element_Type = elementType;
    
        _effectDictionary[elementType].Add(c);
    }
    
    public void Replace(ELEMENT_TYPE elementType, ICrowdControl oldValue, ICrowdControl newValue)
    {
        if (elementType != ELEMENT_TYPE.ETC)
            Previous_Element_Type = elementType;

        int index = _effectDictionary[elementType].IndexOf(oldValue);
        _effectDictionary[elementType][index] = newValue;

        oldValue.Dispose();
    }

    public void Remove(ELEMENT_TYPE elementType, ICrowdControl c)
    {
        _effectDictionary[elementType].Remove(c);

        c.Dispose();
    }
}