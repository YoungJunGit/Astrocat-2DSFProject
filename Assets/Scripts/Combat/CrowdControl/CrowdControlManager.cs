
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
    
    public void Init(BaseUnit target)
    {
        _target = target;
    }
    
    public void AddCrowdControl(CrowdControlType crowdControlType)
    {
        var crowdControl = CrowdControlFactory.CreateCrowdControl(crowdControlType);
        if (crowdControl == null)
            return;

        if (TryUpdateExistingCrowdControl(crowdControl))
            return;

        crowdControl.Count = 1;
        _crowdControlList.Add(crowdControl);
        
        Debug.Log($"{_target} get {crowdControlType}\nCrowdControl List: \n{ToString()}");
    }

    public void ApplyCrowdControl()
    {
        if (_target == null || _crowdControlList.Count <= 0)
            return;
        
        foreach (var crowdControl in _crowdControlList)
        {
            crowdControl.ApplyCrowdControl(_target);
        }
    }
    
    private bool TryUpdateExistingCrowdControl(ICrowdControl crowdControl)
    {
        if (_crowdControlList.TryGetValue(crowdControl, out ICrowdControl existing))
        {
            existing.Count += 1;
            return true;
        }
        return false;
    }

    private string ToString()
    {
        string result = $"";
        foreach (var crowdControl in _crowdControlList)
        {
            result += $"\n{crowdControl.GetType().Name} : {crowdControl.Count}";
        }
        return result;
    }
}