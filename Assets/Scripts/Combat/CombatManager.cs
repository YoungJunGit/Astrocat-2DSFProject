using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "CombatManager", menuName = "GameScene/CombatManager", order = 1)]
public class CombatManager : ScriptableObject
{
    [SerializeField] private int currentTernIdx;
    
    [SerializeField] private List<BaseUnit> timeLineOrderdUnits = new();
    private List<PlayerUnit> playerUnits = null;
    private List<EnemyUnit> enemyUnits = null;
    
    [SerializeField] private ActionSelector actionSelector;
    private UnitSelector unitSelector = new();
    
    private bool isCombatStarted = false;
    
    public Action<List<BaseUnit>> OnTimeLineChanged;
    public Action<int> OnTurnChanged;
    
    public void Init(List<PlayerUnit> playerUnits, List<EnemyUnit> enemyUnits)
    {
        this.playerUnits = playerUnits;
        this.enemyUnits = enemyUnits;
        timeLineOrderdUnits = new List<BaseUnit>();
        
        // TODO : Sort by speed in timeLineOrderdUnits
        foreach (var playerUnit in playerUnits)
        {
            timeLineOrderdUnits.Add(playerUnit);
        }
        foreach (var enemyUnit in enemyUnits)
        {
            timeLineOrderdUnits.Add(enemyUnit);
        }
        
        OnTimeLineChanged?.Invoke(timeLineOrderdUnits);
        OnTurnChanged?.Invoke(currentTernIdx);
        
        currentTernIdx = 0;
    }
    
    public async UniTask StartCombat()
    {
        while (true)
        {
            Debug.Log("RoundStart");
        
            if (timeLineOrderdUnits[currentTernIdx] is PlayerUnit)
            {
                UnitAction selectedAction = await actionSelector.SelectAction(timeLineOrderdUnits[currentTernIdx] as PlayerUnit);
                EnemyUnit selectedUnit = await unitSelector.SelectEnemyUnit();
                
                selectedAction.Execute();
            }
            else
            {
                // TODO : AI Logic for Enemy Unit
            }
        
            // TODO : Execute Action
            
            
            // TODO : Check is finish
            //if ()

            Debug.Log(currentTernIdx);
            currentTernIdx = ++currentTernIdx % timeLineOrderdUnits.Count;
        }
        
    }
}