using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using System.Reflection;
using UnityEngine.UI;
using DataEntity;
using DataEnum;

[CreateAssetMenu(fileName = "TimelineSystem", menuName = "GameScene/TimelineSystem", order = 3)]
public class TimelineSystem : ScriptableObject
{
    [SerializeField] public ScriptableListBaseUnit unitList = null;
    [SerializeField] private TimelineCanvas timelineCanvasPrefab;
    public TimelineCanvas timelineCanvas;
    public TimelineUI timelineUI;
    public EntityBanner previousTurnBanner;

    public int roundDepth;
    private int curRound;
    private int currentDie;
    private int foundIndex;
    private int maxBanner;

    public Action m_EndRound;
    private BannerActions actions;

    public void Init()
    {
        roundDepth = 0;
        curRound = 1;

        currentDie = 2;
        foundIndex = 0;
        maxBanner = 7;

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
        if (timelineUI.BannerList[0].stateIndex !=2 && timelineUI.BannerList[0].Round > curRound)
        {
            Debug.Log("Start Next Round!!!");
            m_EndRound?.Invoke();
            SortBanner();
            curRound++;
        }

        previousTurnBanner = timelineUI.GetCurrentTurnBanner();    
        timelineUI.GetCurrentTurnBanner().DestroyBanner();
        timelineUI.OnPop();
        OnTimelineChanged(unitList, currentDie, foundIndex);

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
        maxBanner--;
        SortBanner();
        OnTimelineChanged(this.unitList.GetUnits(), currentDie, foundIndex);
        foundIndex = 0;
    }

    public void OnCharacterAddBuff(Buff buff)
    {
        SortBanner();
        timelineUI.MoveBanners(foundIndex, maxBanner);
    }

    private void OnTimelineChanged(List<BaseUnit> unitList, int currentDie, int foundIndex)
    {
        if (currentDie == 2)
        {
            Debug.Log("Just turn selection");
            AddTimeline(unitList);
            timelineCanvas.SetParent(timelineUI.BannerList);
            timelineUI.MoveBanners(foundIndex, maxBanner);
        }
        else if (currentDie == 1) {
            Debug.Log($"maxbanner decreased: {maxBanner}");
            AddTimeline(unitList);
            timelineUI.MoveBanners(foundIndex, maxBanner);
            currentDie = 2;
        }
        else if (currentDie == 0) {
            AddTimeline(unitList);
            timelineUI.MoveBanners(foundIndex, maxBanner);
            currentDie = 2;
        }
    }

    /// <summary>
    /// Create Banners when timeline banners are lacking -> Called in HUDManager
    /// </summary>
    /// <param name="unitList"></param>
    public void AddTimeline(List<BaseUnit> unitList)
    {
        while (timelineUI.BannerList.Count < maxBanner && unitList != null && unitList.Count > 0)
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
}

public class BannerActions
{
    private readonly TimelineSystem owner;
    public BannerActions(TimelineSystem owner) => this.owner = owner;

    public void FaintingButton()
    {
        int nextBanner = owner.timelineUI.GetCurrentTurnBanner().Index - 1;
        EntityBanner faintingTarget = owner.timelineUI.BannerList[nextBanner];
        faintingTarget.stateIndex = 1;
        faintingTarget.FaintingEffect();
    }

    public void ExtraButton()
    {
        owner.roundDepth++;

        int index = owner.timelineUI.BannerList.Count;
        var unitStat = owner.timelineUI.GetCurrentTurnBanner().GetStat();

        EntityBanner banner = owner.timelineUI.effect.CreateExtraBanner(unitStat, index, owner.roundDepth);
        owner.timelineUI.BannerList.Add(banner);
        owner.timelineUI.effect.ReorderExtraTurn(index);
        owner.timelineCanvas.SetParent(owner.timelineUI.BannerList);
    }

    public int CheckBannerState()
    {
        switch (owner.timelineUI.GetCurrentTurnBanner().stateIndex)
        {
            case 1: return 1;
            case 2: return 2;
            default: return 0;
        }
    }

    public void UpdateAllUnitStacks()
    {
        var units = owner.unitList.GetUnits();

        foreach (BaseUnit unit in units)
        {
            UnitStat stat = unit.GetStat();
            var fields = typeof(UnitStat).GetFields(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
            );

            foreach (var field in fields)
            {
                if (field.FieldType == typeof(int) &&
                    field.Name.IndexOf("stack", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    int value = (int)field.GetValue(stat);

                    if (field.Name.Equals("forbiddenStack", StringComparison.OrdinalIgnoreCase))
                        field.SetValue(stat, value - 1);
                    else
                        field.SetValue(stat, value + 1);
                }
            }
        }
    }
}

