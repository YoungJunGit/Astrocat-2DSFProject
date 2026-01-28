using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DataEntity;
using DataEnum;
using Unity.VisualScripting;

[CreateAssetMenu(fileName = "EntitySpawner", menuName = "SO/Combat/Unit/EntitySpawner", order = 1)]
public class EntitySpawner : ScriptableObject
{
    private Transform _entityRoot;

    public void Init()
    {
        _entityRoot = new GameObject("EntityRoot").transform;
        _entityRoot.SetParent(null);
    }

    public PlayerUnit CreatePlayerUnit(EntityData entityData, int index, PlayerSaveData saveData)
    {
        GameObject obj = AssetLoader.LoadCharacterPrefabAsset("player_" + entityData.Asset_File);
        PlayerUnit unit = Instantiate(obj).GetComponent<PlayerUnit>();
        unit.Initialize(entityData, index);
        
        if(saveData != null)
            unit.ApplySaveData(saveData);
        
        unit.transform.SetParent(_entityRoot);

        return unit;
    }

    public EnemyUnit CreateEnemyUnit(EntityData entityData, int index)
    {
        GameObject obj = AssetLoader.LoadCharacterPrefabAsset("enemy_" + entityData.Asset_File);
        EnemyUnit unit = Instantiate(obj).GetComponent<EnemyUnit>();
        unit.Initialize(entityData, index);

        unit.transform.SetParent(_entityRoot);
        
        return unit;
    }
}