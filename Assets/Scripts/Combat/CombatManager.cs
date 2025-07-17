using DataEntity;
using DataEnum;
using Obvious.Soap;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "CombatManager", menuName = "CombatManager", order = 1)]
public class CombatManager : ScriptableObject
{
    private TurnTimelineSystem timelineSystem;

    [SerializeField] private ScriptableDictionaryUnit_HUD unit_HUD_Dic = null;

    public void Initialize(List<EntityData> dataList, TurnTimelineSystem timelineSystem, EntitySpawner playerCharacterSpawner, EntitySpawner enemyCharacterSpawner, HUDManager hudManager)
    {
        this.timelineSystem = timelineSystem;
        
        timelineSystem.Initialize(dataList);

        List<EntityData> playerCharacterDataList = dataList.FindAll(element => element.Side == SIDE.PLAYER);
        List<EntityData> enemyCharacterDataList = dataList.FindAll(element => element.Side == SIDE.ENEMY);

        foreach(var entityData in playerCharacterDataList.Select((value, index)=>(value,index)))
        {
            BaseUnit unit = playerCharacterSpawner.CreateUnit(entityData.value, hudManager, entityData.index);
        }

        foreach (var entityData in enemyCharacterDataList.Select((value, index) => (value, index)))
        {
            BaseUnit unit = enemyCharacterSpawner.CreateUnit(entityData.value, hudManager, entityData.index);
        }
    }
}
