using NUnit.Framework;
using ObservableCollections;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using R3;

public class TimelineManagerModel
{
    private readonly TimelineModel _timelineModel;

    public IReadOnlyObservableList<IBanner> BannerList => _timelineModel.BannerList;
    public ReadOnlyReactiveProperty<int> CurRound      => _timelineModel.curRound.ToReadOnlyReactiveProperty();
    public IBanner GetBanner(UnitStat stat)            => BannerList.ToList().Find(b => b.CompareStat(stat));

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

        IBanner bannerToDelete = _timelineModel.CurrentTurnBanner;
        _timelineModel.CurrentTurnBanner = _timelineModel.BannerList[0];
        _timelineModel.BannerList.RemoveAt(0);
        _timelineModel.CurrentTurnBanner.SetState(new BannerCurrent());

        if(bannerToDelete != null)
            bannerToDelete.SetState(new BannerDestroy());

        SortBanners();

        return unitList.Find(unit => _timelineModel.CurrentTurnBanner.CompareStat(unit.GetStat()));
    }

    public void AddBanner(NormalBanner banner, BaseUnit unit)
    {
        banner.Init(unit.GetStat(), _timelineModel.BannerList.Count, _timelineModel.roundDepth);
        _timelineModel.BannerList.Add(banner);
    }

    public void RemoveBanner(IBanner banner)
    {
        _timelineModel.BannerList.Remove(banner);
        banner.SetState(new BannerDestroy());
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

    class BannerComparer : IComparer<IBanner>
    {
        public int Compare(IBanner a, IBanner b)
        {
            return a.CompareTo(b);
        }
    }
}
