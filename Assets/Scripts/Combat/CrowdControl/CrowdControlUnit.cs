using System.Collections.Generic;
using UnityEngine;
using static CrowdControlManager;

public class CrowdControlUnit
{
    private readonly HashSet<ICrowdControl> _effectList;
    public IReadOnlyCollection<ICrowdControl> EffectList => _effectList;

    public CrowdControlUnit() 
    { 
        _effectList = new HashSet<ICrowdControl>();
    }

    public bool Add(ICrowdControl c)
    {
        if(c != null)
            return _effectList.Add(c);
        return false;
    }

    public bool Remove<T>() where T : ICrowdControl
    {
        if (_effectList.RemoveWhere(element => element is T) > 0)
            return true;
        return false;
    }
}
