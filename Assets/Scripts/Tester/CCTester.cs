using System;
using UnityEngine;
using static CrowdControlManager;
using Random = UnityEngine.Random;

public class CCTester : MonoBehaviour
{
    [SerializeField] CrowdControlType TestType;
    public void SetCCOnRandomUnit()
    {
        UnitManager unitManager;
        ServiceLocator.For(this).Get(out unitManager);
        
        var units = unitManager.GetAllUnits();
        
        //units[Random.Range(0, units.Count)].GetCrowdControlManager().AddCrowdControl(TestType);
    }
}
