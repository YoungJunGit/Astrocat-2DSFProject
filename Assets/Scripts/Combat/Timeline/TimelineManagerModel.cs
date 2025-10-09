using NUnit.Framework;
using ObservableCollections;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using R3;

public class TimelineManagerModel
{
    private readonly TimelineModel _timelineModel;

    public IReadOnlyObservableList<EntityBanner> BannerList => _timelineModel.BannerList;
    public ReadOnlyReactiveProperty<int> CurRound           => _timelineModel.curRound.ToReadOnlyReactiveProperty();

    public TimelineManagerModel()
    {
        _timelineModel = new TimelineModel(0, 1);
    }

    public BaseUnit OnPop(List<BaseUnit> unitList)
    {
        if (_timelineModel.BannerList[0].Round > _timelineModel.curRound.Value)
        {
            _timelineModel.curRound.Value++;
            SortBanners();
        }

        EntityBanner bannerToDelete = _timelineModel.CurrentTurnBanner;
        _timelineModel.CurrentTurnBanner = _timelineModel.BannerList[0];
        _timelineModel.BannerList.RemoveAt(0);
        _timelineModel.CurrentTurnBanner.OnPop();

        if(bannerToDelete != null)
            bannerToDelete.DestroyBanner();

        SortBanners();

        return unitList.Find(unit => unit.GetStat() == _timelineModel.CurrentTurnBanner.Stat);
    }

    public void AddBanner(EntityBanner banner, BaseUnit unit)
    {
        banner.Init(unit.GetStat(), _timelineModel.BannerList.Count, _timelineModel.roundDepth);
        _timelineModel.BannerList.Add(banner);
    }

    public void RemoveBanner(EntityBanner banner)
    {
        _timelineModel.BannerList.Remove(banner);
        banner.DestroyBanner();
    }

    public void IncreaseRoundDepth()
    {
        _timelineModel.roundDepth++;
    }

    public void SortBanners()
    {
        // Sort Banners
        _timelineModel.BannerList.Sort(new BannerComparer());

        // Update Index
        if (_timelineModel.CurrentTurnBanner != null)
            _timelineModel.CurrentTurnBanner.Index = 0;
        foreach (var banner in _timelineModel.BannerList.Select((value, index) => (value, index)))
            banner.value.Index = banner.index + 1;
    }

    class BannerComparer : IComparer<EntityBanner>
    {
        public int Compare(EntityBanner a, EntityBanner b)
        {
            return a.CompareTo(b);
        }
    }
}
