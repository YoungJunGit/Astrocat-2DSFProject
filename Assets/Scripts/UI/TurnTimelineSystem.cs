using DataEntity;
using DataEnum;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public enum SIDE
{
    NONE = 0,
    PLAYER,
    ENEMY
}
public class EntityData
{
    public SIDE Side = SIDE.NONE;
    public string ID;
    public string Name;
    public double Default_HP = 0.0f;
    public double Default_Attack = 0.0f;
    public int Default_AP = 0;
    public double Default_Speed = 0.0f;
    public string Skill1_ID;
    public string Skill2_ID;
    public string Skill3_ID;
    public ELEMENT_TYPE Weak_Type = ELEMENT_TYPE.NONE;
    public ELEMENT_TYPE Resist_Type = ELEMENT_TYPE.NONE;
}

public class TurnTimelineSystem : MonoBehaviour
{
    public GameObject BannerPrefab;
    public GameObject ArrowObject;
    public CharacterData CharacterDataList;
    public MonsterData MonsterDataList;

    [SerializeField] // 테스트 후 삭제 예정
    private List<CharacterDataEntity> PlayerData;

    [SerializeField] // 테스트 후 삭제 예정
    private List<MonsterDataEntity> EnemyData;

    private List<EntityBanner> TimelineList = new List<EntityBanner>();
    private List<EntityBannerInfo> EntityInfoList = new List<EntityBannerInfo>();

    private EntityData CreateEntityData(CharacterDataEntity playerData)
    {
        EntityData entityData = new EntityData();

        entityData.Side = SIDE.PLAYER;
        entityData.ID = playerData.Character_ID;
        entityData.Name = playerData.Char_Name;
        entityData.Default_HP = playerData.Char_Default_HP;
        entityData.Default_Attack = playerData.Char_Default_Attack;
        entityData.Default_AP = playerData.Char_Default_AP;
        entityData.Default_Speed = playerData.Char_Default_Speed;
        entityData.Skill1_ID = playerData.Skill1_ID;
        entityData.Skill2_ID = playerData.Skill2_ID;
        entityData.Skill3_ID = playerData.Skill3_ID;
        entityData.Weak_Type = playerData.Weak_Type;
        entityData.Resist_Type = playerData.Resist_Type;

        return entityData;
    }    // 테스트 후 삭제 예정

    private EntityData CreateEntityData(MonsterDataEntity enemyData)
    {
        EntityData entityData = new EntityData();

        entityData.Side = SIDE.ENEMY;
        entityData.ID = enemyData.Mob_ID;
        entityData.Name = enemyData.Mob_Name;
        entityData.Default_HP = enemyData.Mob_Default_HP;
        entityData.Default_Attack = enemyData.Mob_Default_Attack;
        entityData.Default_AP = enemyData.Mob_Default_AP;
        entityData.Default_Speed = enemyData.Mob_Default_Speed;
        entityData.Skill1_ID = enemyData.Skill1_ID;
        entityData.Skill2_ID = enemyData.Skill2_ID;
        entityData.Skill3_ID = enemyData.Skill3_ID;
        entityData.Weak_Type = enemyData.Weak_Type;
        entityData.Resist_Type = enemyData.Resist_Type;

        return entityData;
    }       // 테스트 후 삭제 예정

    private int round = 0;
    private int index = 0;

    private void Awake()
    {
        PlayerData.Clear();
        EnemyData.Clear();

        for (int i = 0; i < 2; i++) // 임시
        {
            PlayerData.Add(CharacterDataList.data[i]);
        }

        for (int i = 0; i < 1; i++) // 임시
        {
            EnemyData.Add(MonsterDataList.data[i]);
        }
    }

    private void Start()
    {
        ArrowObject.SetActive(true);
        InitTimelineSystem();
    }

    private void InitTimelineSystem()
    {
        for (int i = 0; i < PlayerData.Count; i++)
        {
            EntityData entityInfo = CreateEntityData(PlayerData[i]);
            EntityBannerInfo myBannerInfo = new EntityBannerInfo();
            myBannerInfo.InitBannerInfo(entityInfo, i);
            EntityInfoList.Add(myBannerInfo);
        }
        for (int i = 0; i < EnemyData.Count; i++)
        {
            EntityData entityInfo = CreateEntityData(EnemyData[i]);
            EntityBannerInfo myBannerInfo = new EntityBannerInfo();
            myBannerInfo.InitBannerInfo(entityInfo, i);
            EntityInfoList.Add(myBannerInfo);
        }

        SortBannerList();

        CreateTimeline();

        InitLocateBanner();
    }

    private void CreateTimeline()
    {
        while (round < 7)
        {
            foreach (EntityBannerInfo info in EntityInfoList)
            {
                GameObject gameObject = Instantiate(BannerPrefab);
                EntityBanner myBanner = gameObject.GetComponent<EntityBanner>();
                gameObject.name = "Banner:" + index;

                if (index >= 7)
                    gameObject.SetActive(false);

                myBanner.InitBanner(info, index);
                TimelineList.Add(myBanner);

                index++;
            }
            round++;
        }
    }

    public void AddTimeline()
    {
        foreach(EntityBannerInfo info in EntityInfoList)
        {
            GameObject gameObject = Instantiate(BannerPrefab);
            EntityBanner myBanner = gameObject.GetComponent<EntityBanner>();
            gameObject.name = "Banner:" + index;
            myBanner.InitBanner(info, index);
            TimelineList.Add(myBanner);

            index++;
        }
        round++;
        InitLocateBanner();
    }

    public void OnEndTurn()
    {
        foreach(EntityBannerInfo entityInfo in EntityInfoList)
        {
            entityInfo.stack--;
        }

        foreach(EntityBanner banner in TimelineList)
        {
            banner.SetDestination();
        }
    }

    private void SortBannerList()
    {
        EntityInfoList.Sort((EntityBannerInfo a, EntityBannerInfo b) => a.CompareTo(b));
    }

    private void CreateLastBanner()
    {
        
    }

    private void InitLocateBanner()
    {
        for (int i = 0; i < TimelineList.Count; i++)
        {
            TimelineList[i].transform.SetParent(GameObject.Find("Turn-Timeline").transform);
            TimelineList[i].SetTransformByOrder(i);
        }
    }
}
