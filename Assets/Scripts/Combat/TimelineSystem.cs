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

[CreateAssetMenu(fileName = "TimelineSystem", menuName = "GameScene/Timeline/TimelineSystem", order = 3)]
public class TimelineSystem : ScriptableObject
{
    [SerializeField] public ScriptableListBaseUnit unitList = null;
    private List<EntityBanner>  bannerList = new();
    private EntityBanner        currentTurnBanner;

    [SerializeField] private IntVariable    MaxShowBannerIndex;
    [SerializeField] private TimelineCanvas timelineCanvasPrefab;
    [HideInInspector] public TimelineCanvas timelineCanvas;
    [HideInInspector] public TimelineUI     timelineUI;

    public int  roundDepth;
    private int curRound;
    private int foundIndex;

    public Action           m_EndRound;
    private BannerActions   actions;

    public List<EntityBanner> BannerList    => bannerList;
    public EntityBanner CurrentTurnBanner   => currentTurnBanner;
    public BannerActions GetActions()       => actions;

    public void Init()
    {
        bannerList.Clear();
        currentTurnBanner = null;
        roundDepth = 0;
        curRound = 1;

        foundIndex = 0;

        timelineCanvas = Instantiate(timelineCanvasPrefab);
        timelineUI = timelineCanvas.GetComponentInChildren<TimelineUI>();
        actions = new BannerActions(this);
    }

    public void CreateBanners()
    {
        AddTimeline(unitList.GetUnits());
        timelineCanvas.SetBanners(bannerList);
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
        currentTurnBanner = bannerList[0];
        bannerList.RemoveAt(0);
        timelineUI.OnPop(currentTurnBanner);
        SortBanner();

        return unitList.Find(unit => unit.GetStat() == currentTurnBanner.GetStat());
    }

    /// <summary>
    /// This called when received message -> CombatManager : m_EndTurn
    /// </summary>
    /// <param name="unitList"></param>
    /// <returns> Give Unit to CombatManager for next turn </returns>
    public BaseUnit Pop(List<BaseUnit> unitList)
    {
        if (bannerList[0].GetState() != BannerState.EXTRA && bannerList[0].Round > curRound)
        {
            Debug.Log("Start Next Round!!!");
            m_EndRound?.Invoke();
            SortBanner();
            curRound++;
        }

        currentTurnBanner.DestroyBanner();
        currentTurnBanner = bannerList[0];
        bannerList.RemoveAt(0);
        timelineUI.OnPop(currentTurnBanner);
        OnTimelineChanged(unitList, foundIndex);

        //actions.UpdateAllUnitStacks();

        return unitList?.Find(unit => unit.GetStat() == currentTurnBanner.GetStat());
    }

    public void OnCharacterDie(BaseUnit unit)
    {
        List<EntityBanner> deleteBannerList = bannerList.FindAll(banner => banner.GetStat() == unit.GetStat());
        bannerList.RemoveAll(banner => banner.GetStat() == unit.GetStat());

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
        timelineUI.MoveBanners(currentTurnBanner, bannerList, foundIndex);
    }

    private void OnTimelineChanged(List<BaseUnit> unitList, int foundIndex)
    {
        AddTimeline(unitList);
        timelineCanvas.SetParent(bannerList);
        timelineUI.MoveBanners(currentTurnBanner, bannerList, foundIndex);
    }

    public void AddTimeline(List<BaseUnit> unitList)
    {
        while (bannerList.Count < MaxShowBannerIndex && unitList != null && unitList.Count > 0)
        {
            roundDepth++;
            foreach (BaseUnit unit in unitList)
            {
                int index = bannerList.Count;
                EntityBanner banner = timelineUI.CreateBanner(unit, index, roundDepth);
                bannerList.Add(banner);
            }
        }
        SortBanner();
    }

    private void SortBanner()
    {
        // For Debugging
        if (currentTurnBanner != null)
            currentTurnBanner.SetName("CurrentBanner");

        bannerList.Sort((EntityBanner a, EntityBanner b) => a.CompareTo(b));
        foreach (var banner in bannerList.Select((value, index) => (value, index)))
        {
            banner.value.Index = banner.index + 1;
        }
    }
}
