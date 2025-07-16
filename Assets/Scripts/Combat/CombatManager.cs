using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CombatManager", menuName = "CombatManager", order = 1)]
public class CombatManager : ScriptableObject
{
    public TurnTimelineSystem timelineSystem;
    public EntitySpawner playerCharacterSpawner;
    public EntitySpawner enemyCharacterSpawner;
    [SerializeField] private EntityDataCreator entityDataCreator;

    private List<EntityData> entityDataList;

    [SerializeField]
    private List<string> playerCharacterID = new List<string>();

    [SerializeField]
    private List<string> enemyCharacterID = new List<string>();

    public void Init(TurnTimelineSystem timelineSystem)
    {
        this.timelineSystem = timelineSystem;
        
        entityDataList = entityDataCreator.CreateEntityDataWithID(playerCharacterID, enemyCharacterID);
        timelineSystem.OnCombatStart(entityDataList);

        List<EntityData> playerCharacterDataList = entityDataList.FindAll(element => element.Side == SIDE.PLAYER);
        List<EntityData> enemyCharacterDataList = entityDataList.FindAll(element => element.Side == SIDE.ENEMY);

        playerCharacterSpawner.CreateEntity(playerCharacterDataList);
        enemyCharacterSpawner.CreateEntity(enemyCharacterDataList);

        Debug.Log("���� ����");
    }
}
