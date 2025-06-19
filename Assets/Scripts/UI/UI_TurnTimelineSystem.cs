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

public class UI_TurnTimelineSystem : MonoBehaviour
{
    public GameObject BannerPrefab;
    public GameObject ArrowObject;
    public CharacterData CharacterDataList;
    public MonsterData MonsterDataList;

    [SerializeField] // 테스트 후 삭제 예정
    private List<CharacterDataEntity> PlayerData;

    [SerializeField] // 테스트 후 삭제 예정
    private List<MonsterDataEntity> EnemyData;

    private List<UI_Banner> BannerList = new List<UI_Banner>();

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
    }

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
    }

    private int currentRound = 0;

    private void Awake()
    {
        PlayerData.Clear();
        EnemyData.Clear();

        for(int i = 0; i < 1; i++) // 임시
        {
            PlayerData.Add(CharacterDataList.data[i]);
            EnemyData.Add(MonsterDataList.data[i]);
        }
    }

    private void Start()
    {
        CreateTimeline();
        ArrowObject.SetActive(true);
    }

    private void CreateTimeline()
    {
        while(BannerList.Count < 7)
        {
            for (int i = 0; i < PlayerData.Count; i++)
            {
                if (BannerList.Count >= 7)
                    break;
                GameObject gameObject = Instantiate(BannerPrefab);
                EntityData entityInfo = CreateEntityData(PlayerData[i]);
                UI_Banner myBanner = gameObject.GetComponent<UI_Banner>();
                myBanner.InitBanner(entityInfo, i, currentRound);
                BannerList.Add(myBanner);
            }
            for (int i = 0; i < EnemyData.Count; i++)
            {
                if (BannerList.Count >= 7)
                    break;
                GameObject gameObject = Instantiate(BannerPrefab);
                EntityData entityInfo = CreateEntityData(EnemyData[i]);
                UI_Banner myBanner = gameObject.GetComponent<UI_Banner>();
                myBanner.InitBanner(entityInfo, i, currentRound);
                BannerList.Add(myBanner);
            }
            currentRound++;
        }

        SortBanner();
        InitLocateBanner();
    }

    private void SortBanner()
    {
        BannerList.Sort((UI_Banner a, UI_Banner b) => a.CompareTo(b));
    }

    private void CreateLastBanner()
    {
        GameObject gameObject = Instantiate(BannerPrefab);
        EntityData entityInfo = BannerList[0].EntityData;
        UI_Banner myBanner = gameObject.GetComponent<UI_Banner>();
        myBanner.InitBanner(entityInfo, 0, currentRound);           // 추후 캐릭터 배치 시스템 구현되면 변경되어야 함
        BannerList.Add(myBanner);
    }

    private void InitLocateBanner()
    {
        for (int i = 0; i < BannerList.Count; i++)
        {
            BannerList[i].transform.SetParent(GameObject.Find("Turn-Timeline").transform);
            BannerList[i].SetTransformByOrder(i);
        }
    }
}
