using UnityEngine;
using System.Collections.Generic;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "EnemyPartyData", menuName = "SO/Combat/SaveData/EnemyPartyData", order = 1)]
public class EnemyPartyData : ScriptableObject
{
    // 적 스폰 흐름 순서
    // 1. 전투 씬 진입 전, Node관련 클래스에서 DT에서 가져온 Setting을 해당 SO에 주입
    // 2. 전투 씬 진입 후 MakeEnemyIDWithSetting을 통해 finalSpawnEnemyID에 들어갈 적 최종 스폰 ID를 계산
    // 3. Entity생성 시점에 완성된 FinalSpawnEnemyID를 가져와서 적 생성

    // Temporary
    [SerializeField, Expandable]
    private EnemySpawnSetting enemySpawnSetting;

    private string[] finalSpawnEnemyID;
    public string[] FinalSpawnEnemyID => finalSpawnEnemyID;

    // enemySpawnSetting을 통해 finalSpawnEnemyID를 계산
    //
    // <입력과 출력 과정>
    // spawnCount = 5, fixed = 1
    // [_, _, A, _, _] 
    // 
    // spawnCount = 5, fixed = 2
    // [_, A, B, _, _] 
    // 
    // spawnCount = 5, fixed = 3
    // [_, A, B, C, _] 
    // 
    // spawnCount = 4, fixed = 2
    // [_, A, B, _] 
    // 
    // spawnCount = 3, fixed = 2
    // [A, B, _]
    //
    // 적 최대 스폰 수가 언제든 바뀔 가능성에 따라 확장성 있게 구현
    public void MakeEnemyIDWithSetting()
    {
        // 0. Validation (B + C)
        if (enemySpawnSetting.SpawnCount > enemySpawnSetting.FixedEnemyID.Length && (enemySpawnSetting.NominateEnemyID == null || enemySpawnSetting.NominateEnemyID.Length == 0))
        {
            Debug.LogWarning
            (
                $"[EnemySpawnSetting] Invalid SpawnSetting detected.\n" +
                $"spawnCount({enemySpawnSetting.SpawnCount}) > fixedEnemyID({enemySpawnSetting.FixedEnemyID.Length}) but nominateEnemyID is empty.\n" +
                $"spawnCount will be clamped."
            );
            enemySpawnSetting.SetSpawnCount(enemySpawnSetting.FixedEnemyID.Length);
        }

        // 1. Slot 준비
        string[] slots = new string[enemySpawnSetting.SpawnCount];

        int fixedCount = Mathf.Min(enemySpawnSetting.FixedEnemyID.Length, enemySpawnSetting.SpawnCount);
        int startIndex = (enemySpawnSetting.SpawnCount - fixedCount) / 2;

        // 2. Fixed 중앙 배치
        for (int i = 0; i < fixedCount; i++)
        {
            slots[startIndex + i] = enemySpawnSetting.FixedEnemyID[i];
        }

        // 3. Random 후보 준비 (fixed랑 겹침 허용)
        List<string> candidates = new List<string>();
        if (enemySpawnSetting.NominateEnemyID != null)
        {
            foreach (var id in enemySpawnSetting.NominateEnemyID)
            {
                candidates.Add(id);
            }
        }

        // 4. 빈 슬롯 채우기
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] != null)
                continue;

            if (candidates.Count > 0)
            {
                slots[i] = candidates[Random.Range(0, candidates.Count)];
            }
        }

        finalSpawnEnemyID = slots;
    }

    // 추후 NodeData + DT 연동 후 적용 (NodeData에 해당 SO를 포함시킬 것)
    public void SetEnemySpawnSetting(EnemySpawnSetting setting)
    {
        if(setting != null)
            enemySpawnSetting = setting;
    }
}