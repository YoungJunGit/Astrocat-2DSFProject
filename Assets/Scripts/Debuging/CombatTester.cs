using System;
using UnityEngine;
using UnityEngine.UI;
using DataEnum;
using R3;

public class CombatTester : MonoBehaviour
{
    private IUnitManager unitManager;
    private bool btnOn = false;

    private void Awake()
    {
        ServiceLocator.For(this).Get(out unitManager);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.F1) && !btnOn)
        {
            btnOn = true;
            var list = unitManager.GetEnemyUnits();

            foreach(var unit in list)
            {
                IDamage damage = new Damage(99999f, true);
                unit.GetStat().GetDamaged(damage);
            }
        }
    }
}
