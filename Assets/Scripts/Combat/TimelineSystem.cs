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
    [SerializeField] private ScriptableDictionaryUnit_HUD unit_HUD_Dic = null;
    [SerializeField] private TimelineCanvas timelineCanvasPrefab;
    private TimelineCanvas timelineCanvas;
    private TimelineUI timelineUI;

    private int roundDepth;
    private int curRound;

    public Action m_EndRound;

    public void Init()
    {
        roundDepth = 0;
        curRound = 1;

        timelineCanvas = Instantiate(timelineCanvasPrefab);
        timelineUI = timelineCanvas.GetComponentInChildren<TimelineUI>();
    }

    public void CreateBanners()
    {
        AddTimeline(unit_HUD_Dic.GetUnits());
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
    public BaseUnit Pop(List<BaseUnit> unitList)
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

        OnTimelineChanged(unitList);

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

        OnTimelineChanged(unit_HUD_Dic.GetUnits());
    }

    public void OnCharacterAddBuff(Buff buff)
    {
        SortBanner();
        timelineUI.MoveBanners();
    }

    private void OnTimelineChanged(List<BaseUnit> unitList)
    {
        AddTimeline(unitList);
        timelineCanvas.SetParent(timelineUI.BannerList);
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
}
