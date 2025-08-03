using System;
using UnityEngine;
using UnityEngine.UI;
using DataEnum;

public class Buff
{
    public string Buff_Name;
    public int Buff_Duration = 0;
    public double HP_Value = 0.0f;
    public double Attack_Value = 0.0f;
    public int AP_Value = 0;
    public double Speed_Value = 0.0f;

    // Temp Constructor
    public Buff(string buff_Name, int buff_Duration, double hp_Value, double attack_Value, int ap_Value, double speed_Value)
    {
        Buff_Name = buff_Name;
        Buff_Duration = buff_Duration;
        HP_Value = hp_Value;
        Attack_Value = attack_Value;
        AP_Value = ap_Value;
        Speed_Value = speed_Value;
    }
}

public class CombatTester : MonoBehaviour
{
    [SerializeField] private CombatManager combatManager;
    [SerializeField] private UnitManager unitManager;

    [SerializeField] private Button AddSpeedBtn;
    [SerializeField] private Button DieBtn;
    [SerializeField] private HUDManager hudManager;

    [Header("Buff Test")] [Space(10f)]
    public SIDE buffSide;
    [Range(1, 3)]   public int buffCharacterNumber;
    [Range(1, 10)]  public int durationRound;
    public double addSpeedValue;

    [Header("Die Test")] [Space(10f)]
    public SIDE dieSide;
    [Range(1, 3)]   public int dieCharacterNumber;

    private void Awake()
    {
        Buff speedBuff = new Buff("Speed Buff", durationRound, 0, 0, 0, addSpeedValue);
        /*AddSpeedBtn.onClick.AddListener(() => combatManager.BuffCharacter(buffSide, buffCharacterNumber, speedBuff));
        DieBtn.onClick.AddListener(() => combatManager.DieCharacter(dieSide, dieCharacterNumber));*/
    }

    public void OnDieButton()
    {
        var currentUnit = combatManager.GetCurrentTurnUnit();
        combatManager.OnCharacterDie(currentUnit.GetStat());

        if (currentUnit is PlayerUnit)
        {
            hudManager.DeletePlayerHUD((PlayerUnit)currentUnit);
            unitManager.DeletePlayerUnit((PlayerUnit)currentUnit);
        }
        else { 
            hudManager.DeleteEnemyHUD((EnemyUnit)currentUnit);
            unitManager.DeleteEnemyUnit((EnemyUnit)currentUnit);
        }
    }
}
