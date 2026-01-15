using System;
using UnityEngine;
using UnityEngine.UI;
using DataEnum;
using R3;

public class CombatTester : MonoBehaviour
{
    [SerializeField] GameObject[] debuggingObjs;
    private IUnitManager unitManager;
    private ISoundService soundService;
    private bool btnOn = false;

    private bool toggleMute = false;

    private void Awake()
    {
        ServiceLocator.For(this)
            .Get(out unitManager)
            .Get(out soundService);

        foreach(var obj in debuggingObjs)
        {
            obj.SetActive(false);
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.F1) && !btnOn)
        {
            btnOn = true;
            var enemyList = unitManager.GetEnemyUnits();
            var playerList = unitManager.GetPlayerUnits();

            foreach(var unit in enemyList)
            {
                IDamageInfo damage = new DamageInfo(playerList[0], 99999f, true);
                unit.GetDamage(damage);
            }
        }
        
        if(Input.GetKeyDown(KeyCode.Minus))
        {
            if(toggleMute)
            {
                soundService.PlayBackGround("Title_Background", true);
                toggleMute = false;
            }
            else
            {
                soundService.StopBackGround();
                toggleMute = true;
            }
        }
    }
}
