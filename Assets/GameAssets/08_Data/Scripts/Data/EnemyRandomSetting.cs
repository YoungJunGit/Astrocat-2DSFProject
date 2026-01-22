using MoreMountains.Feedbacks;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyRandomSetting", menuName = "EnemyRandomSetting")]
public class EnemyRandomSetting : ScriptableObject
{
    [SerializeField]
    private List<string> fixedEnemiesID;
    [SerializeField]
    private List<string> randomEnemiesID;

    public List<string> SetEnemies()
    {
        List<string> enemies = new List<string>();

        // 고정 출연하는 적들
        for(int i = 0;i < 2;i++)
        {
            enemies.Add(fixedEnemiesID[Random.Range(0, fixedEnemiesID.Count)]);
        }

        int count = Random.Range(0, 2);

        // 랜덤으로 출연하는 적들
        for(int i = 0;i < count;i++)
        {
            enemies.Add(randomEnemiesID[Random.Range(0, randomEnemiesID.Count)]);
        }

        return enemies;
    }
}
