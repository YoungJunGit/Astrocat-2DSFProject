using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DataEntity;
using DataEnum;
using Unity.VisualScripting;

public class EntitySpawner : MonoBehaviour
{
    public bool isPlayerSide;
    public List<GameObject> spawnPos;

    public BaseUnit CreateUnit(EntityData entityData, HUDManager hudManager, int index)
    {
        GameObject prefab;
        prefab = isPlayerSide == true ? AssetLoader.LoadCharacterPrefabAsset(entityData.Asset_File) : AssetLoader.LoadMonsterPrefabAsset(entityData.Asset_File);
        BaseUnit unit = Instantiate(prefab, Vector2.zero, Quaternion.identity).GetComponent<BaseUnit>();
        unit.transform.SetParent(spawnPos[index].transform, false);

        if (unit != null)
        {
            unit.Initialize(entityData, hudManager);
        }
        else
        {
            Debug.LogWarning("���ֿ� ��ũ��Ʈ �Ҵ��� �Ǿ��ִ��� Ȯ���Ͻʽÿ�!!!");
        }

        return unit;
    }
}