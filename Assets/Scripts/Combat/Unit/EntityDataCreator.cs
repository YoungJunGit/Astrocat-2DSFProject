using System.Collections.Generic;
using UnityEngine;
using DataEntity;
using DataEnum;

public class EntityDataCreator
{
    public List<EntityData> CreateEntityData(List<EntityData> entityData, List<string> playerCharacterID, List<string> enemyCharacterID)
    {
        List<EntityData> poolData = new List<EntityData>();
        
        foreach (string id in playerCharacterID)
        {
            EntityData entity = entityData.Find(element => element.code == id);
            if (entity != null)
            {
                poolData.Add(entity);
            }
        }
        foreach (string id in enemyCharacterID)
        {
            EntityData entity = entityData.Find(element => element.code == id);
            if (entity != null)
            {
                poolData.Add(entity);
            }
        }

        return poolData;
    }
}
