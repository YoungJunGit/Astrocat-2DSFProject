using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitPositioner", menuName = "GameScene/UnitPositioner", order = 5)]
class UnitPositioner : ScriptableObject
{
    [HideInInspector] public int playerDataCount;
    [HideInInspector] public int EnemyDataCount;
    [HideInInspector] public List<Vector2> playerPositions = new List<Vector2>();
    [HideInInspector] public List<Vector2> enemyPositions = new List<Vector2>();

    // TODO : PointStart, PointEnd를 인스펙터창에서 조절할 수 있도록 수정해야 함
    private Vector2 pointStart = new Vector2(2f, 0f);
    private Vector2 pointEnd = new Vector2(8f, -4f);
    private Vector2 direction;

    private Vector2 targetPosition;
    private StatusCanvas statuesCanvas;

    public void Init()
    {
        direction = pointEnd - pointStart;
    }

    public Vector2 playerPositionCaculate(int index) {

        Vector2 step = direction / playerDataCount;
        targetPosition = pointStart + (step * index);
        playerPositions[index - 1] = targetPosition;
        return targetPosition;
    }

    public Vector2 enemyPositionCaculate(int index)
    {
        Vector2 step = direction / EnemyDataCount;
        targetPosition = pointStart + (step * index);
        targetPosition.x = -targetPosition.x;
        enemyPositions.Add(targetPosition);  // 가장 안전한 방식
        return targetPosition;
    }

    public void SetPositionForUnits(List<PlayerUnit> playerUnits, List<EnemyUnit> enemyUnits)
    {
        // Implement Method
    }

    public void OnCharacterDie(UnitStat stat)
    {
        
    }
}