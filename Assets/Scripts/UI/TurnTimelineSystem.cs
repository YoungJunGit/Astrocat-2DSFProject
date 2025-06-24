using DataEntity;
using DataEnum;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

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

    [SerializeField]
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

    public static int MaxShowRound = 2;
    private int round = 0;

    [Header("테스트용")]
    public Button AddSpeedBtn;                                              // 테스트 후 삭제 예정
    [Range(1, 3)]
    public int buffCharacterNumber;
    public double addSpeedValue;

    private void Awake()
    {
        PlayerData.Clear();
        EnemyData.Clear();

        for (int i = 0; i < 3; i++) // 임시
        {
            PlayerData.Add(CharacterDataList.data[i]);
        }

        for (int i = 0; i < 3; i++) // 임시
        {
            EnemyData.Add(MonsterDataList.data[i]);
        }

        AddSpeedBtn.onClick.AddListener(() => OnBuff(buffCharacterNumber, addSpeedValue));      // 임시
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
    }

    private void CreateTimeline()
    {
        while (round < TurnTimelineSystem.MaxShowRound)
        {
            foreach (EntityBannerInfo info in EntityInfoList)
            {
                int index = TimelineList.Count;
                GameObject gameObject = Instantiate(BannerPrefab);
                EntityBanner myBanner = gameObject.GetComponent<EntityBanner>();
                gameObject.name = "Banner:" + index;
                myBanner.InitBanner(info, index);

                if (index >= 7)
                    gameObject.SetActive(false);

                TimelineList.Add(myBanner);
            }
            round++;
        }
    }

    public void AddTimeline()
    {
        foreach (EntityBannerInfo info in EntityInfoList)
        {
            int index = TimelineList.Count;
            GameObject gameObject = Instantiate(BannerPrefab);
            EntityBanner myBanner = gameObject.GetComponent<EntityBanner>();
            gameObject.name = "Banner:" + index;
            myBanner.InitBanner(info, index);
            TimelineList.Add(myBanner);

            if(index >= 7)
                gameObject.SetActive(false);
        }
        round++;
    }

    public void OnEndTurn()
    {
        if (TimelineList.Count % EntityInfoList.Count == 1)
            AddTimeline();

        int destroyTargetindex = TimelineList.FindIndex(x => x.BannerIndex == 0);

        Destroy(TimelineList[destroyTargetindex].gameObject);
        TimelineList.RemoveAt(destroyTargetindex);

        foreach (EntityBanner banner in TimelineList)
        {
            banner.SetDestination(-1);
        }
    }

    public void OnBuff(int number, double speedValue)
    {
        if (EntityInfoList.Count < number)
        {
            Debug.Log("버프할 캐릭터 설정 실패");
            return;
        }

        List<EntityBannerInfo> tmpList = new List<EntityBannerInfo>();

        foreach (EntityBannerInfo info in EntityInfoList)
            tmpList.Add(info);

        foreach(EntityBannerInfo info in EntityInfoList)
        {
            if(info.Side == SIDE.PLAYER && info.Priority == (number - 1))
            {
                info.Speed += speedValue;
            }
        }

        SortBannerList();

        foreach(EntityBannerInfo info in EntityInfoList)
        {
            int sortListIndex = EntityInfoList.IndexOf(info);
            int unsortListIndex = tmpList.IndexOf(info);
            int depth = sortListIndex - unsortListIndex;
            foreach (EntityBanner banner in TimelineList)
            {
                if (banner.MyBannerInfo == info && banner.BannerIndex > 0)
                {
                    if (banner.BannerIndex + depth > 0)
                        banner.SetDestination(depth);
                    else
                        banner.SetDestination(depth + 1);
                }
            }
        }
    }

    private void SortBannerList()
    {
        EntityInfoList.Sort((EntityBannerInfo a, EntityBannerInfo b) => a.CompareTo(b));
    }
}
