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
    [SerializeField] private IntVariable    MaxShowBannerIndex;
    [SerializeField] private TimelineUI     _timelineUIPrefab;
    private TimelineUI _timelineUI;

    private List<EntityBanner> bannerList = new();
    private EntityBanner currentTurnBanner;
    private int _roundDepth;
    private int _curRound;

    public void Init()
    {
        bannerList.Clear();
        currentTurnBanner = null;
        _roundDepth = 0;
        _curRound = 1;

        _timelineUI = Instantiate(_timelineUIPrefab);
    }

    public void CreateBanners(List<BaseUnit> unitList)
    {
        AddTimeline(unitList);
        _timelineUI.SetParent(bannerList);
        _timelineUI.SetRectTransform(bannerList);
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
            unit.m_FinishedDying += OnCharacterDie;
            unit.m_AddBuff += OnCharacterAddBuff;
        }

        // Inititalize and Sort BannerList for combat
        currentTurnBanner = bannerList[0];
        bannerList.RemoveAt(0);
        currentTurnBanner.OnPop();
        SortBanner();

        return unitList.Find(unit => unit.GetStat() == currentTurnBanner.Stat);
    }

    /// <summary>
    /// This called when received message -> CombatManager : m_EndTurn
    /// </summary>
    /// <param name="unitList"></param>
    /// <returns> Give Unit to CombatManager for next turn </returns>
    public BaseUnit Pop(List<BaseUnit> unitList)
    {
        if (bannerList[0].Round > _curRound)
        {
            Debug.Log("Start Next Round!!!");
            SortBanner();
            _curRound++;
        }

        currentTurnBanner.DestroyBanner();
        currentTurnBanner = bannerList[0];
        bannerList.RemoveAt(0);
        currentTurnBanner.OnPop();
        OnTimelineChanged(unitList);

        //actions.UpdateAllUnitStacks();

        return unitList?.Find(unit => unit.GetStat() == currentTurnBanner.Stat);
    }

    public void OnCharacterDie(BaseUnit unit)
    {
        List<EntityBanner> deleteBannerList = bannerList.FindAll(banner => banner.Stat == unit.GetStat());
        bannerList.RemoveAll(banner => banner.Stat == unit.GetStat());

        _timelineUI.DeleteBanners(deleteBannerList);
        SortBanner();

        UnitManager unitManager;
        ServiceLocator.For(this).Get(out unitManager);
        OnTimelineChanged(unitManager.GetAllUnits());
    }

    public void OnCharacterAddBuff(Buff buff)
    {
        SortBanner();
        _timelineUI.MoveBanners(currentTurnBanner, bannerList);
    }

    private void OnTimelineChanged(List<BaseUnit> unitList)
    {
        AddTimeline(unitList);
        _timelineUI.SetParent(bannerList);
        _timelineUI.MoveBanners(currentTurnBanner, bannerList);
    }

    public void AddTimeline(List<BaseUnit> unitList)
    {
        while (bannerList.Count < MaxShowBannerIndex && unitList != null && unitList.Count > 0)
        {
            _roundDepth++;
            List<EntityBanner> createdList = _timelineUI.CreateBanners(unitList, bannerList.Count, _roundDepth); 
            bannerList.AddRange(createdList);
        }
        SortBanner();
    }

    private void SortBanner()
    {
        if (currentTurnBanner != null)
            currentTurnBanner.Index = 0;

        bannerList.Sort((EntityBanner a, EntityBanner b) => a.CompareTo(b));
        foreach (var banner in bannerList.Select((value, index) => (value, index)))
        {
            banner.value.Index = banner.index + 1;
        }
    }
}
