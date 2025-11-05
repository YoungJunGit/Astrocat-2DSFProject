using System;
using UnityEngine;
using DataEnum;
using Random = UnityEngine.Random;
using System.Collections.Generic;
using UnityEngine.UI;

public class CCTester : MonoBehaviour
{
    [SerializeField] Button btn;
    [SerializeField] ELEMENT_STATUS_CATEGORY TestType;

    private static readonly Dictionary<ELEMENT_STATUS_CATEGORY,
       Action<CrowdControlManager, BaseUnit, BaseUnit>> _table =
       new()
       {
            { ELEMENT_STATUS_CATEGORY.STUN, static (mgr, t, c) => mgr.AddCrowdControl<Stun>(t, c) },
            { ELEMENT_STATUS_CATEGORY.BURN, static (mgr, t, c) => mgr.AddCrowdControl<Burn>(t, c) },
       };

    private void Awake()
    {
        btn.onClick.AddListener(() => SetCCOnRandomUnit());
    }

    public void SetCCOnRandomUnit()
    {
        CrowdControlManager crowdControlManager;
        UnitManager unitManager;
        ServiceLocator.For(this)
            .Get(out unitManager)
            .Get(out crowdControlManager);
        
        var enemyUnits = unitManager.GetEnemyUnits();
        var playerUnits = unitManager.GetPlayerUnits();

        var target = enemyUnits[Random.Range(0, enemyUnits.Count)];
        var caster = playerUnits[Random.Range(0, playerUnits.Count)];

        if (!_table.TryGetValue(TestType, out var call))
        {
            Debug.LogWarning($"No mapping for {TestType}");
            return;
        }

        call(crowdControlManager, target, caster);
    }
}
