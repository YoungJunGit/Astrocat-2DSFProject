using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DataEntity;
using DataEnum;
using Unity.VisualScripting;

[CreateAssetMenu(fileName = "EntitySpawner", menuName = "GameScene/EntitySpawner", order = 4)]
public class EntitySpawner : ScriptableObject
{
    private Transform _entityRoot;

    public void Init()
    {
        _entityRoot = new GameObject("EntityRoot").transform;
        _entityRoot.SetParent(null);
    }

    public PlayerUnit CreatePlayerUnit(EntityData entityData, int index)
    {
        GameObject go = AssetLoader.LoadCharacterPrefabAsset(entityData.Asset_File);
        BaseUnit unit = Instantiate(go, Vector2.zero, Quaternion.identity).GetComponent<BaseUnit>();
        unit.Initialize(entityData, index);

        // TODO : 캐릭터 생성 타이밍에 포지션 설정X
        //BaseUnit unit = Instantiate(go, position, Quaternion.identity).GetComponent<BaseUnit>();
        
        unit.transform.SetParent(_entityRoot);
        
        return unit as PlayerUnit;
    }

    public EnemyUnit CreateEnemyUnit(EntityData entityData, int index)
    {
        GameObject go = AssetLoader.LoadMonsterPrefabAsset(entityData.Asset_File);
        BaseUnit unit = Instantiate(go, Vector2.zero, Quaternion.identity).GetComponent<BaseUnit>();
        unit.Initialize(entityData, index);

        // TODO : 캐릭터 생성 타이밍에 포지션 설정X
        //BaseUnit unit = Instantiate(go, position, Quaternion.identity).GetComponent<BaseUnit>();

        unit.transform.SetParent(_entityRoot);
        
        return unit as EnemyUnit;
    }
}