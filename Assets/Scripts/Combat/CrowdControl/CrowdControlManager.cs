
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CrowdControlManager
{
    public enum CrowdControlType
    {
        Burn,
        Oppression,
        Expose,
        Flood,
        Confusion,
    }

    
    private HashSet<ICrowdControl> _crowdControlList = new();
    private BaseUnit _target;
    private DamageFactory _damageFactory;
    
    public void Init(BaseUnit target, DamageFactory damageFactory)
    {
        _target = target;
        _damageFactory = damageFactory;
    }
    
    public void AddCrowdControl(CrowdControlType crowdControlType)
    {
        var crowdControl = CrowdControlFactory.CreateCrowdControl(crowdControlType);
        if (crowdControl == null)
            return;

        _crowdControlList.Add(crowdControl);
        
        Debug.Log($"{_target} get {crowdControlType}\nCrowdControl List: \n{ToString()}");
    }

    public void ApplyCrowdControl()
    {
        if (_target == null || _crowdControlList.Count <= 0)
            return;
        
        foreach (var crowdControl in _crowdControlList)
        {
            // TODO : Calculate CC damage
            crowdControl.ApplyCrowdControl(_target);
        }
    }

    private string ToString()
    {
        string result = $"";
        foreach (var crowdControl in _crowdControlList)
        {
            result += $"\n{crowdControl.GetType().Name}";
        }
        return result;
    }
}