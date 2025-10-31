using System;
using UnityEngine;
using UnityEngine.UI;
using DataEnum;

public class CombatTester : MonoBehaviour
{
    [SerializeField] private CombatManager combatManager;
    [SerializeField] private UnitManager unitManager;

    [SerializeField] private Button AddSpeedBtn;
    [SerializeField] private Button DieBtn;
    [SerializeField] private Button Fainting;
    [SerializeField] private HUDManager hudManager;

    [Header("Buff Test")]
    [Space(10f)]
    public SIDE buffSide;
    [Range(1, 3)] public int buffCharacterNumber;
    [Range(1, 10)] public int durationRound;
    public double addSpeedValue;

    [Header("Die Test")]
    [Space(10f)]
    public SIDE dieSide;
    [Range(1, 3)] public int dieCharacterNumber;

    private void Awake()
    {
        //AddSpeedBtn.onClick.AddListener(() => combatManager.BuffCharacter(buffSide, buffCharacterNumber, speedBuff));
        //DieBtn.onClick.AddListener(() => combatManager.DieCharacter(dieSide, dieCharacterNumber));
    }

    public void OnDieButton()
    {
        //var currentUnit = combatManager.GetCurrentTurnUnit();
        ////combatManager.OnCharacterDie(currentUnit.GetStat());

        //if (currentUnit is PlayerUnit)
        //{
        //    //hudManager.DeletePlayerHUD((PlayerUnit)currentUnit);
        //    //unitManager.DeletePlayerUnit((PlayerUnit)currentUnit);
        //}
        //else { 
        //    //hudManager.DeleteEnemyHUD((EnemyUnit)currentUnit);
        //    //unitManager.DeleteEnemyUnit((EnemyUnit)currentUnit);
        //}
    }
    public void OnFaintingButton()
    {
        combatManager.OnFainting();
    }

    public void OnExtraButton()
    {
        combatManager.OnExtraTurn();
    }
}
