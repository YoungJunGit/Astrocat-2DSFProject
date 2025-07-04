using DataEntity;
using DataEnum;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
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
    public string Asset_File;
}
public class Buff
{
    public string Buff_Name;
    public int Buff_Duration = 0;
    public double HP_Value = 0.0f;
    public double Attack_Value = 0.0f;
    public int AP_Value = 0;
    public double Speed_Value = 0.0f;

    public Buff(string buff_Name, int buff_Duration, double hp_Value, double attack_Value, int ap_Value, double speed_Value)
    {
        Buff_Name = buff_Name;
        Buff_Duration = buff_Duration;
        HP_Value = hp_Value;
        Attack_Value = attack_Value;
        AP_Value = ap_Value;
        Speed_Value = speed_Value;
    }
}

public class TurnTimelineSystem : MonoBehaviour
{
    public GameObject BannerPrefab;
    public GameObject ArrowObject;
    public CharacterData CharacterDataList;
    public MonsterData MonsterDataList;
    private List<CharacterDataEntity> PlayerData = new List<CharacterDataEntity>();
    private List<MonsterDataEntity> EnemyData = new List<MonsterDataEntity>();
    private List<EntityBanner> TimelineList = new List<EntityBanner>();
    private List<EntityBannerInfo> EntityInfoList = new List<EntityBannerInfo>();

    private EntityBanner curBanner;
    private int roundDepth = 0;
    private int curRound = 1;

    delegate void EndRoundHandler();
    EndRoundHandler mEndRound;

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
        entityData.Asset_File = playerData.Asset_File;

        return entityData;
    }    // �׽�Ʈ �� ���� ����
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
        entityData.Asset_File = enemyData.Asset_File;

        return entityData;
    }       // �׽�Ʈ �� ���� ����

    [Header("�׽�Ʈ��")]
    public Button AddSpeedBtn;
    public Button DieBtn;
    public SIDE selectCharacterSide;
    [Range(1, 3)]
    public int buffCharacterNumber;
    [Range(1, 10)]
    public int durationRound;
    public double addSpeedValue;
    [Range(1, 3)]
    public int dieCharacterNumber;

    private void Awake()
    {
        PlayerData.Clear();
        EnemyData.Clear();

        for (int i = 0; i < 3; i++) // �ӽ�
        {
            PlayerData.Add(CharacterDataList.data[i]);
        }

        for (int i = 0; i < 3; i++) // �ӽ�
        {
            EnemyData.Add(MonsterDataList.data[i]);
        }

        AddSpeedBtn.onClick.AddListener(() => OnStartBuff(buffCharacterNumber, addSpeedValue));      // �ӽ�
        DieBtn.onClick.AddListener(() => OnCharacterDie(dieCharacterNumber, selectCharacterSide));
    }

    private void Start()
    {
        ArrowObject.SetActive(true);
        InitTimelineSystem();
        CreateTimeline();
        Debug.Log("���� ����\n ���� ����: " + curRound);
    }

    private void InitTimelineSystem()
    {
        for (int i = 0; i < PlayerData.Count; i++)
        {
            EntityData entityInfo = CreateEntityData(PlayerData[i]);
            EntityBannerInfo myBannerInfo = new EntityBannerInfo();
            myBannerInfo.InitBannerInfo(entityInfo, i);
            mEndRound += new EndRoundHandler(myBannerInfo.OnEndRound);
            EntityInfoList.Add(myBannerInfo);
        }
        for (int i = 0; i < EnemyData.Count; i++)
        {
            EntityData entityInfo = CreateEntityData(EnemyData[i]);
            EntityBannerInfo myBannerInfo = new EntityBannerInfo();
            myBannerInfo.InitBannerInfo(entityInfo, i);
            mEndRound += new EndRoundHandler(myBannerInfo.OnEndRound);
            EntityInfoList.Add(myBannerInfo);
        }

        EntityInfoList.Sort((EntityBannerInfo a, EntityBannerInfo b) => a.CompareTo(b));
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
            Debug.Log("������ ĳ���� ���� ����");
            return;
        }

        int duration = TimelineList.Exists(element => element.MyBannerInfo.Side == selectCharacterSide && 
                                                      element.MyBannerInfo.Priority == (number - 1) &&
                                                      element.Turn == curRound) ? durationRound : durationRound + 1;

        Buff buff = new Buff("Speed Buff", duration, 0, 0, 0, addSpeedValue);

        foreach(EntityBannerInfo info in EntityInfoList)
        {
            if(info.Side == selectCharacterSide && info.Priority == (number - 1))
            {
                info.AddBuff(buff);
            }
        }

        ArrangeBanner();
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

        TimelineList.RemoveAll(x => x.MyBannerInfo.Side == side && x.MyBannerInfo.Priority == number);
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
            mEndRound();
            Debug.Log("���� ���� ����!\n ���� ����: " + curRound);
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
}
