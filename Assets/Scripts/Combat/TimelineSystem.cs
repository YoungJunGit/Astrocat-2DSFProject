using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;
using DataEntity;
using DataEnum;

[CreateAssetMenu(fileName = "TimelineSystem", menuName = "GameScene/TimelineSystem", order = 3)]
public class TimelineSystem : ScriptableObject
{
    private TimelineUI timelineUI;

    private int roundDepth;
    private int curRound;

    public Action m_TimelineChanged;            // Funcs In this message (Sequence) HUDManager : OnEndTurn -> TimelineUI : OnEndTurn
    public Action m_EndRound;

    public void Init(TimelineUI timelineUI)
    {
        roundDepth = 0;
        curRound = 1;
        this.timelineUI = timelineUI;
        m_TimelineChanged += this.timelineUI.MoveBanners;
    }

    /// <summary>
    /// For Preparing Combat -> Called in CombatManager
    /// </summary>
    /// <param name="unitList"></param>
    /// <returns> Give Unit to CombatManager before combat start </returns>
    public BaseUnit PrepareCombat(List<BaseUnit> unitList)
    {
        // Attaching Actions
        foreach (BaseUnit unit in unitList)
        {
            m_EndRound += unit.OnEndRound;
            unit.GetStat().OnDie += OnCharacterDie;
            unit.m_AddBuff += OnCharacterAddBuff;
        }

        // Inititalize and Sort BannerList for combat
        timelineUI.OnPop();
        SortBanner();

        return unitList.Find(unit => unit.GetStat() == timelineUI.GetCurrentTurnBanner().GetStat());
    }

    /// <summary>
    /// This called when received message -> CombatManager : m_EndTurn
    /// </summary>
    /// <param name="unitList"></param>
    /// <returns> Give Unit to CombatManager for next turn </returns>
    public BaseUnit Pop(List<BaseUnit> unitList = null)
    {
        if (timelineUI.BannerList[0].Round > curRound)
        {
            Debug.Log("다음 라운드 시작!");
            m_EndRound?.Invoke();
            SortBanner();
            curRound++;
        }

        timelineUI.GetCurrentTurnBanner().DestroyBanner();
        timelineUI.OnPop();

        m_TimelineChanged?.Invoke();

        return unitList?.Find(unit => unit.GetStat() == timelineUI.GetCurrentTurnBanner().GetStat());
    }

    public void OnCharacterDie(UnitStat stat)
    {
        List<EntityBanner> deleteBannerList = timelineUI.BannerList.FindAll(banner => banner.GetStat() == stat);
        timelineUI.BannerList.RemoveAll(banner => banner.GetStat() == stat);
        foreach (EntityBanner banner in deleteBannerList)
        {
            banner.DestroyBanner();
        }

        //if (timelineUI.GetCurrentTurnBanner().GetStat() == stat)
        //{
        //    Pop();
        //}

        m_TimelineChanged?.Invoke();
    }

    public void OnCharacterAddBuff(Buff buff)
    {
        SortBanner();
        timelineUI.MoveBanners();
    }

    /// <summary>
    /// Create Banners when timeline banners are lacking -> Called in HUDManager
    /// </summary>
    /// <param name="unitList"></param>
    public void AddTimeline(List<BaseUnit> unitList)
    {
        while (timelineUI.BannerList.Count < 7)
        {
            roundDepth++;
            foreach (BaseUnit unit in unitList)
            {
                int index = timelineUI.BannerList.Count;
                EntityBanner banner = timelineUI.CreateBanner(unit, index, roundDepth);
                timelineUI.BannerList.Add(banner);
            }
        }
        SortBanner();
    }

    private void SortBanner()
    {
        // For Debugging
        if(timelineUI.GetCurrentTurnBanner() != null)
            timelineUI.GetCurrentTurnBanner().SetName("CurrentBanner");

        timelineUI.BannerList.Sort((EntityBanner a, EntityBanner b) => a.CompareTo(b));
        foreach (var banner in timelineUI.BannerList.Select((value, index) => (value, index)))
        {
            banner.value.Index = banner.index + 1;
        }
    }

    public List<EntityBanner> GetBannerList()
    {
        return timelineUI.BannerList;
    }

    //public void OnStartBuff(int number, int durationRound, double speedValue, SIDE side)
    //{
    //    if (EntityInfoList.Count < number)
    //    {
    //        return;
    //    }

    //    int duration = TimelineList.Exists(element => element.MyBannerInfo.Side == side && 
    //                                                  element.MyBannerInfo.Priority == (number - 1) &&
    //                                                  element.Turn == curRound) ? durationRound : durationRound + 1;

    //    Buff buff = new Buff("Speed Buff", duration, 0, 0, 0, speedValue);

    //    foreach(EntityBannerInfo info in EntityInfoList)
    //    {
    //        if(info.Side == side && info.Priority == (number - 1))
    //        {
    //            info.OnAddBuff(buff);
    //        }
    //    }

    //    ArrangeBanner();
    //}

    //private void AddTimeline()
    //{
    //    while (TimelineList.Count < 7)
    //    {
    //        roundDepth++;
    //        foreach (EntityBannerInfo info in EntityInfoList)
    //        {
    //            int index = TimelineList.Count;
    //            GameObject gameObject = Instantiate(BannerPrefab);
    //            EntityBanner myBanner = gameObject.GetComponent<EntityBanner>();
    //            myBanner.InitBanner(info, index, roundDepth);
    //            myBanner.SetTransformByIndex(index + 2);
    //            TimelineList.Add(myBanner);

    //            if (index >= 7)
    //                gameObject.SetActive(false);
    //        }
    //    }
    //}
}
