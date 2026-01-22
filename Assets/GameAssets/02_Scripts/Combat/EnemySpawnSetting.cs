using MoreMountains.Feedbacks;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemySpawnSetting", menuName = "SO/Combat/Spawn/EnemySpawnSetting")]
public class EnemySpawnSetting : ScriptableObject
{
    [SerializeField] private string[] fixedEnemyID;
    [SerializeField] private string[] nominateEnemyID;
    [SerializeField, Range(1, 3)] private int spawnCount = 3;

    public string[] FixedEnemyID => fixedEnemyID;
    public string[] NominateEnemyID => nominateEnemyID;
    public int SpawnCount => spawnCount;

    public void SetSpawnCount(int count) => spawnCount = count;
}
