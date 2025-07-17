using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

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

    private List<EntityBanner> TimelineList = new List<EntityBanner>();
    private List<EntityBannerInfo> EntityInfoList = new List<EntityBannerInfo>();

    private EntityBanner curBanner;
    private int roundDepth = 0;
    private int curRound = 1;

    delegate void EndRoundHandler();
    EndRoundHandler mEndRound;

    [Header("테스트용")]
    public SIDE selectCharacterSide;
    [Space(10f)]
    [Range(1, 3)]
    public int buffCharacterNumber;
    [Range(1, 10)]
    public int durationRound;
    public double addSpeedValue;
    [Space(10f)]
    [Range(1, 3)]
    public int dieCharacterNumber;

    public void Init(List<CharacterDataEntity> playerData, List<MonsterDataEntity> enemyData)
    {
        
    }

    public void OnCombatStart(List<EntityData> entityDataList)
    {
        ArrowObject.SetActive(true);
        InitTimelineSystem(entityDataList);
        CreateTimeline();
    }

    private void InitTimelineSystem(List<EntityData> entityDataList)
    {
        int playerStack = 0, enemyStack = 0;
        foreach (var entity in entityDataList.Select((value, index)=>(value, index)))
        {
            EntityBannerInfo myBannerInfo = new EntityBannerInfo();
            if (entity.value.Side == SIDE.PLAYER)
            {
                myBannerInfo.InitBannerInfo(entityDataList[entity.index], playerStack);
                playerStack++;
            }
            else if(entity.value.Side == SIDE.ENEMY)
            {
                myBannerInfo.InitBannerInfo(entityDataList[entity.index], enemyStack);
                enemyStack++;
            }
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
