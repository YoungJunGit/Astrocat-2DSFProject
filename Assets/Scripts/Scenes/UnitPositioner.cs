using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitPositioner", menuName = "GameScene/UnitPositioner", order = 1)]
class UnitPositioner : ScriptableObject
{
    [HideInInspector] public int playerDataCount;
    [HideInInspector] public int EnemyDataCount;
    [HideInInspector] public List<Vector2> playerPositions = new List<Vector2>();
    [HideInInspector] public List<Vector2> enemyPositions = new List<Vector2>();

    private Vector2 pointStart = new Vector2(2f, 0f);
    private Vector2 pointEnd = new Vector2(8f, -4f);
    private Vector2 direction;

    private Vector2 targetPosition;
    private StatusCanvas statuesCanvas;

    public void Init()
    {
        direction = pointEnd - pointStart;

        // �÷��̾� ��ġ �ʱ�ȭ
        playerPositions.Clear();
        for (int i = 0; i < playerDataCount; i++)
        {
            playerPositions.Add(Vector2.zero);  // �⺻������ ä�� �ֱ�
        }

        // �� ��ġ �ʱ�ȭ
        enemyPositions.Clear();
        for (int i = 0; i < EnemyDataCount; i++)
        {
            enemyPositions.Add(Vector2.zero);
        }
    }


    public Vector2 playerPositionCaculate(int index) {

        Vector2 step = direction / playerDataCount;
        targetPosition = pointStart + (step * (index + 1));
        playerPositions[index] = targetPosition;
        return targetPosition;
    }

    public Vector2 enemyPositionCaculate(int index)
    {
        Vector2 step = direction / playerDataCount;
        targetPosition = pointStart + (step * (index + 1));
        targetPosition.x = -targetPosition.x;
        enemyPositions.Add(targetPosition);  // ���� ������ ���
        return targetPosition;
    }

}