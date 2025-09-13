using System;
using UnityEngine;
using static CrowdControlManager;
using Random = UnityEngine.Random;

public class CCTester : MonoBehaviour
{
    UnitManager unitManager;

    public void SetCCOnRendomUnit()
    {
        if (unitManager == null)
            ServiceLocator.For(this).Get(out unitManager);
        
        Debug.Log($"Set CC");
        
        var units = unitManager.GetAllUnit();
        
        units[Random.Range(0, units.Count)].GetCrowdControlManager()
            .AddCrowdControl((CrowdControlType)Random.Range(0, Enum.GetValues(typeof(CrowdControlType)).Length));
    }
}
