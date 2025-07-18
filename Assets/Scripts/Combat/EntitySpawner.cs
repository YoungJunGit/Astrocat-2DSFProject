using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DataEntity;
using DataEnum;
using Unity.VisualScripting;

public class EntitySpawner : MonoBehaviour
{
    public PlayerUnit CreatePlayerUnit(EntityData entityData)
    {
        GameObject go = AssetLoader.LoadCharacterPrefabAsset(entityData.Asset_File);
        BaseUnit unit = Instantiate(go, Vector2.zero, Quaternion.identity).GetComponent<BaseUnit>();
        
        return unit as PlayerUnit;
    }
    public EnemyUnit CreateEnemyUnit(EntityData entityData)
    {
        GameObject go = AssetLoader.LoadCharacterPrefabAsset(entityData.Asset_File);
        BaseUnit unit = Instantiate(go, Vector2.zero, Quaternion.identity).GetComponent<BaseUnit>();
        
        return unit as EnemyUnit;
    }
}