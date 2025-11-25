using System.Collections.Generic;
using UnityEngine;
using DataEntity;
using DataEnum;

public class EntityDataCreator
{
    public List<EntityData> CreateEntityData(DataHandler dataHandler, List<string> playerCharacterID, List<string> enemyCharacterID)
    {
        List<EntityData> poolData = new List<EntityData>();
        
        foreach (string id in playerCharacterID)
        {
            EntityData entity = dataHandler.FindEntityData(id);
            if (entity != null)
            {
                poolData.Add(entity);
            }
        }
        foreach (string id in enemyCharacterID)
        {
            EntityData entity = dataHandler.FindEntityData(id);
            if (entity != null)
            {
                poolData.Add(entity);
            }
        }

        return poolData;
    }
}
