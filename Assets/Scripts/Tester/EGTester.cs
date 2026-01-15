using UnityEngine;
using DataEnum;
using Random = UnityEngine.Random;
using System.Collections.Generic;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class EGTester : MonoBehaviour
{
    enum EGSide
    {
        PLAYER_TO_ENEMY,
        ENEMY_TO_PLAYER
    }

    [SerializeField] private Button increaseEGBtn;
    [SerializeField] private EGSide targetSide;
    [SerializeField] private ELEMENT_TYPE elementType;
    [SerializeField] private SKILL_ELEMENT_RATE elementRate;
    [SerializeField] private int playerIndex;
    [SerializeField] private int enemyIndex;

    private IUnitManager unitManager;

    void Start()
    {
        ServiceLocator.For(this).Get(out unitManager);

        increaseEGBtn.onClick.AddListener(() => IncreaseElementGauge());
    }

    private void IncreaseElementGauge()
    {
        var enemyList = unitManager.GetEnemyUnits();
        var playerList = unitManager.GetPlayerUnits();

        var target = enemyList[enemyIndex];
        var caster = playerList[playerIndex];

        if(targetSide == EGSide.PLAYER_TO_ENEMY)
        {
            // DamageResult damageInfo = DamageFactory.CreateDamage<DamageCalculatorTest, ElementGaugeCalculator>(caster, target, elementType, elementRate, 1.0f);
            // target.GetDamage(damageInfo);
        }
        else
        {
            // DamageResult damageInfo = DamageFactory.CreateDamage<DamageCalculatorTest, ElementGaugeCalculator>(target, caster, elementType, elementRate, 1.0f);
            // caster.GetDamage(damageInfo);
        }
    } 
}