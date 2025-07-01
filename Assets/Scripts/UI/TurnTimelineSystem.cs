using DataEntity;
using DataEnum;
using System.Collections.Generic;
using System.Linq;
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

    private EntityBanner curBanner;

    private int roundDepth = 0;
    private int curRound = 1;

    [Header("테스트용")]
    public Button AddSpeedBtn;                                              // 테스트 후 삭제 예정
    public Button DieBtn;
    [Range(1, 3)]
    public int buffCharacterNumber;
    [Range(1, 10)]
    public int durationRound;
    public double addSpeedValue;
    [Range(1, 3)]
    public int dieCharacterNumber;
    public SIDE characterSide;

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

        AddSpeedBtn.onClick.AddListener(() => OnStartBuff(buffCharacterNumber, addSpeedValue));      // 임시
        DieBtn.onClick.AddListener(() => OnCharacterDie(dieCharacterNumber, characterSide));
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

        EntityInfoList.Sort((EntityBannerInfo a, EntityBannerInfo b) => a.CompareTo(b));

        CreateTimeline();
        Debug.Log("전투 시작\n 현재 라운드: " + curRound);
    }

    private void CreateTimeline()
    {
        while (TimelineList.Count < 7)
        {
            roundDepth++;
            foreach (EntityBannerInfo info in EntityInfoList)
            {
                int index = TimelineList.Count;
                GameObject gameObject = Instantiate(BannerPrefab);
                EntityBanner myBanner = gameObject.GetComponent<EntityBanner>();
                myBanner.InitBanner(info, index, roundDepth);
                myBanner.SetTransformByIndex(index);

                if (index >= 7)
                    gameObject.SetActive(false);

                TimelineList.Add(myBanner);
            }
        }

        curBanner = TimelineList[0];
        TimelineList.RemoveAt(0);
    }

    private void AddTimeline()
    {
        while (TimelineList.Count < 7)
        {
            roundDepth++;
            foreach (EntityBannerInfo info in EntityInfoList)
            {
                int index = TimelineList.Count;
                GameObject gameObject = Instantiate(BannerPrefab);
                EntityBanner myBanner = gameObject.GetComponent<EntityBanner>();
                myBanner.InitBanner(info, index, roundDepth);
                myBanner.SetTransformByIndex(index + 2);
                TimelineList.Add(myBanner);

                if (index >= 7)
                    gameObject.SetActive(false);
            }
        }
    }

    public void OnEndTurn()
    {
        curBanner.DestroyBanner();
        Pop();

        ArrangeBanner();
    }

    public void OnStartBuff(int number, double speedValue)
    {
        if (EntityInfoList.Count < number)
        {
            Debug.Log("버프할 캐릭터 설정 실패");
            return;
        }

        foreach(EntityBannerInfo info in EntityInfoList)
        {
            if(info.Side == SIDE.PLAYER && info.Priority == (number - 1))
            {
                info.Speed += speedValue;
            }
        }

        ArrangeBanner();
    }

    public void OnEndBuff()
    {

    }

    public void OnCharacterDie(int number, SIDE side)
    {
        number -= 1;
        EntityInfoList.Remove(EntityInfoList.Find(x => x.Side == side && x.Priority == number));

        List<EntityBanner> deleteBannerList = new List<EntityBanner>();
        foreach (EntityBanner banner in TimelineList)
        {
            if (banner.MyBannerInfo.Side == side && banner.MyBannerInfo.Priority == number)
            {
                deleteBannerList.Add(banner);
            }
        }

        int count = TimelineList.RemoveAll(x => x.MyBannerInfo.Side == side && x.MyBannerInfo.Priority == number);

        foreach (EntityBanner banner in deleteBannerList)
        {
            banner.DestroyBanner();
        }

        if(curBanner.MyBannerInfo.Side == side && curBanner.MyBannerInfo.Priority == number)
        {
            curBanner.DestroyBanner();
            Pop();
        }

        ArrangeBanner();
    }

    private void Pop()
    {
        curBanner = TimelineList[0];
        TimelineList.RemoveAt(0);
        curBanner.SetDestination(0);

        if (curBanner.Turn > curRound)
        {
            curRound++;
            Debug.Log("다음 라운드 시작!\n 현재 라운드: " + curRound);
        }
    }

    private void ArrangeBanner()
    {
        AddTimeline();
        TimelineList.Sort((EntityBanner a, EntityBanner b) => a.CompareTo(b));

        foreach (var banner in TimelineList.Select((value, index) => (value, index)))
        {
            banner.value.SetDestination(banner.index + 1);
        }
    }
}
