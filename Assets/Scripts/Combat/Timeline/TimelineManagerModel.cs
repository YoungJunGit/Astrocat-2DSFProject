using NUnit.Framework;
using ObservableCollections;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class TimelineManagerModel
{
    private readonly TimelineModel _timelineModel;
    private readonly UnitManager _unitManager;

    public IReadOnlyObservableList<EntityBanner> BannersList => _timelineModel.BannerList;

    public TimelineManagerModel(UnitManager unitManager)
    {
        _timelineModel = new TimelineModel(0, 1);
        _unitManager = unitManager;
    }

    public void AddBanner(EntityBanner banner, BaseUnit unit)
    {
        banner.Init(unit.GetStat(), _timelineModel.BannerList.Count, _timelineModel.roundDepth);
        _timelineModel.BannerList.Add(banner);
    }

    public void RemoveBanner(EntityBanner banner)
    {

    }

    public void IncreaseRoundDepth()
    {
        _timelineModel.roundDepth++;
    }

    public void NextRound()
    {
        _timelineModel.curRound++;
    }
}
