using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EntitySpawner : MonoBehaviour
{
    public bool isPlayerSide;
    public List<GameObject> spawnPos;

    public void CreateUnit(List<EntityData> entityDataList)
    {
        GameObject prefab;
        foreach (var entityData in entityDataList.Select((value, index) => (value, index)))
        {
            prefab = isPlayerSide == true ? AssetLoader.LoadCharacterPrefabAsset(entityData.value.Asset_File) : AssetLoader.LoadMonsterPrefabAsset(entityData.value.Asset_File);
            GameObject unitObj = Instantiate(prefab, Vector2.zero, Quaternion.identity);
            unitObj.transform.SetParent(spawnPos[entityData.index].transform, false);
            BaseUnit unit = unitObj.GetComponent<BaseUnit>();
            if (unit != null)
            {
                unitObj.GetComponent<BaseUnit>().Init(entityData.value);
            }
            else
            {
                Debug.LogWarning("유닛에 스크립트 할당이 되어있는지 확인하십시오!!!");
            }
        }
    }
}