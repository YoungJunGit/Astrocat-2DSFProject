using DataEntity;
using DataEnum;
using Obvious.Soap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using static EntityBanner;

[CreateAssetMenu(fileName = "TimelineSystem", menuName = "GameScene/TimelineSystem", order = 3)]
public class TimelineSystem : ScriptableObject
{
    [SerializeField] public ScriptableListBaseUnit unitList = null;
    [SerializeField] private IntVariable MaxShowBannerIndex;
    [SerializeField] private TimelineCanvas timelineCanvasPrefab;
    [HideInInspector] public TimelineCanvas timelineCanvas;
    [HideInInspector] public TimelineUI timelineUI;

    public int roundDepth;
    private int curRound;
    private int foundIndex;

    public Action m_EndRound;
    private BannerActions actions;

    public void Init()
    {
        roundDepth = 0;
        curRound = 1;

        foundIndex = 0;

        timelineCanvas = Instantiate(timelineCanvasPrefab);
        timelineUI = timelineCanvas.GetComponentInChildren<TimelineUI>();
        actions = new BannerActions(this);
    }

    public void CreateBanners()
    {
        AddTimeline(this.unitList.GetUnits());
        timelineCanvas.SetBanners(timelineUI.BannerList);
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
            unit.m_FinishedDying += OnCharacterDie;
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
    public BaseUnit Pop(List<BaseUnit> unitList)
    {
        if (timelineUI.BannerList[0].GetState() != BannerState.EXTRA && timelineUI.BannerList[0].Round > curRound)
        {
            Debug.Log("Start Next Round!!!");
            m_EndRound?.Invoke();
            SortBanner();
            curRound++;
        }

        timelineUI.GetCurrentTurnBanner().DestroyBanner();
        timelineUI.OnPop();
        OnTimelineChanged(unitList, foundIndex);

        actions.UpdateAllUnitStacks();

        return unitList?.Find(unit => unit.GetStat() == timelineUI.GetCurrentTurnBanner().GetStat());
    }

    public void OnCharacterDie(BaseUnit unit)
    {
        List<EntityBanner> deleteBannerList = timelineUI.BannerList.FindAll(banner => banner.GetStat() == unit.GetStat());
        timelineUI.BannerList.RemoveAll(banner => banner.GetStat() == unit.GetStat());

        foreach (EntityBanner banner in deleteBannerList)
        {
            banner.DestroyBanner();
        }
        SortBanner();
        OnTimelineChanged(this.unitList.GetUnits(), foundIndex);
        foundIndex = 0;
    }

    public void OnCharacterAddBuff(Buff buff)
    {
        SortBanner();
        timelineUI.MoveBanners(foundIndex);
    }

    private void OnTimelineChanged(List<BaseUnit> unitList, int foundIndex)
    {
        AddTimeline(unitList);
        timelineCanvas.SetParent(timelineUI.BannerList);
        timelineUI.MoveBanners(foundIndex);
    }

    /// <summary>
    /// Create Banners when timeline banners are lacking -> Called in HUDManager
    /// </summary>
    /// <param name="unitList"></param>
    public void AddTimeline(List<BaseUnit> unitList)
    {
        while (timelineUI.BannerList.Count < MaxShowBannerIndex && unitList != null && unitList.Count > 0)
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
        if (timelineUI.GetCurrentTurnBanner() != null)
            timelineUI.GetCurrentTurnBanner().SetName("CurrentBanner");

        timelineUI.BannerList.Sort((EntityBanner a, EntityBanner b) => a.CompareTo(b));
        foreach (var banner in timelineUI.BannerList.Select((value, index) => (value, index)))
        {
            banner.value.Index = banner.index + 1;
        }
    }

    public BannerActions GetActions() => actions;
}
