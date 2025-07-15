using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public TurnTimelineSystem timelineSystem;
    public EntitySpawner playerCharacterSpawner;
    public EntitySpawner enemyCharacterSpawner;
    private EntityDataCreator entityDataCreator;

    private List<EntityData> entityDataList;

    [SerializeField]
    private List<string> playerCharacterID = new List<string>();

    [SerializeField]
    private List<string> enemyCharacterID = new List<string>();

    /// <summary>
    /// 변수 할당 과정
    /// </summary>
    private void Awake()
    {
        entityDataCreator = transform.GetComponent<EntityDataCreator>();
    }

    /// <summary>
    /// 전투 시스템의 시작이 이루어짐
    /// </summary>
    private void Start()
    {
        entityDataList = entityDataCreator.CreateEntityDataWithID(playerCharacterID, enemyCharacterID);
        timelineSystem.OnCombatStart(entityDataList);

        List<EntityData> playerCharacterDataList = entityDataList.FindAll(element => element.Side == SIDE.PLAYER);
        List<EntityData> enemyCharacterDataList = entityDataList.FindAll(element => element.Side == SIDE.ENEMY);

        playerCharacterSpawner.CreateUnit(playerCharacterDataList);
        enemyCharacterSpawner.CreateUnit(enemyCharacterDataList);
    }
}
