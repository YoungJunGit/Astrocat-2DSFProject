
using System.Collections.Generic;

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
    
    public void AddCrowdControl(CrowdControlType crowdControlType)
    {
        var crowdControl = CrowdControlFactory.CreateCrowdControl(crowdControlType);
        if (crowdControl == null)
            return;

        if (TryUpdateExistingCrowdControl(crowdControl))
            return;

        crowdControl.Count = 1;
        _crowdControlList.Add(crowdControl);
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
    
    public void ApplyCrowdControl(BaseUnit target)
    {
        foreach (var crowdControl in _crowdControlList)
        {
            crowdControl.ApplyCrowdControl(target);
        }
    }
}