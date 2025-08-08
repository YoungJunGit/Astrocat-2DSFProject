
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
    
    private LinkedList<ICrowdControl> _crowdControlList = new();
    
    public void AddCrowdControl(CrowdControlType crowdControlType)
    {
        _crowdControlList.AddLast(CrowdControlFactory.CreateCrowdControl(crowdControlType));
    }
    
    public void ApplyCrowdControl(BaseUnit target)
    {
        foreach (var crowdControl in _crowdControlList)
        {
            crowdControl.ApplyCrowdControl(target);
        }
    }
}