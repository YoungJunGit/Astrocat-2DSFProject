using ObservableCollections;
using System.Collections.Generic;
using UnityEngine;
using static CrowdControlManager;

public class CrowdControlUnit
{
    public readonly ObservableHashSet<ICrowdControl> _effectList;

    public CrowdControlUnit() 
    { 
        _effectList = new ObservableHashSet<ICrowdControl>();
    }

    public bool Add(ICrowdControl c)
    {
        if(c != null)
            return _effectList.Add(c);
        return false;
    }

    public bool Remove(ICrowdControl c)
    {
        if (c != null)
            return _effectList.Remove(c);
        return false;
    }
}
