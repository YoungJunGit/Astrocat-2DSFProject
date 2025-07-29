using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitPositioner", menuName = "GameScene/UnitPositioner", order = 5)]
class UnitPositioner : ScriptableObject
{
    [Serializable] private class UnitPositionBox
    {
        public Vector2 pointStart;
        public Vector2 pointEnd;
    }

    [SerializeField] private ScriptableListBaseUnit unitList = null;
    [SerializeField] private UnitPositionBox box;

    public void Prepare()
    {
        foreach(BaseUnit unit in unitList)
        {
            unit.GetStat().OnDie += OnCharacterDie;
        }
    }

    public void SetPositionForUnits(List<PlayerUnit> playerUnits, List<EnemyUnit> enemyUnits)
    {
        Vector2 pos;
        for (int i = 0; i < playerUnits.Count; i++)
        {
            pos = PlayerPositionCaculate(i, playerUnits.Count);
            playerUnits[i].transform.position = pos;
        }

        for(int i = 0;i < enemyUnits.Count;i++)
        {
            pos = EnemyPositionCaculate(i, enemyUnits.Count);
            enemyUnits[i].transform.position = pos;
        }
    }

    public void OnCharacterDie(UnitStat stat)
    {
        List<PlayerUnit> playerUnits = unitList.GetPlayerUnits();
        List<EnemyUnit> enemyUnits = unitList.GetEnemyUnits();
        SetPositionForUnits(playerUnits, enemyUnits);
    }

    public void OnCharacterRevivel()
    {
        // To be added later
    }

    private Vector2 PlayerPositionCaculate(int index, int count)
    {
        Vector2 direction = box.pointEnd - box.pointStart;
        Vector2 targetPosition = box.pointStart + (direction / (count + 1)) * (index + 1);
        return targetPosition;
    }

    private Vector2 EnemyPositionCaculate(int index, int count)
    {
        Vector2 direction = box.pointEnd - box.pointStart;
        Vector2 targetPosition = box.pointStart + (direction / (count + 1)) * (index + 1);
        targetPosition.x = -targetPosition.x;
        return targetPosition;
    }
}