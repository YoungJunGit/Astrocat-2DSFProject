using Cysharp.Threading.Tasks;
using ObservableCollections;
using Obvious.Soap;
using R3;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TimelineManager : MonoBehaviour
{
    [SerializeField] private EntityBanner bannerPrefab;
    [SerializeField] private IntVariable MaxBannerIndex;

    private TimelineManagerModel _timelineManagerModel;

    public void Init()
    {
        _timelineManagerModel = new TimelineManagerModel();
    }

    public void CreateTimeline(List<BaseUnit> units)
    {
        while (_timelineManagerModel.BannerList.Count < MaxBannerIndex && units != null && units.Count > 0)
        {
            CreateBanners(units);
        }
    }

    public void Prepare(List<BaseUnit> units)
    {
        // Data Bindings
        _timelineManagerModel.BannerList.ObserveRemove()
                                        .Subscribe(_ => 
                                        {
                                            if (_timelineManagerModel.BannerList.Count < MaxBannerIndex)
                                            {
                                                CreateBanners(units);
                                                _timelineManagerModel.SortBanners();
                                            }
                                        })
                                        .AddTo(this);
    }

    public BaseUnit Pop(List<BaseUnit> unitList)
    {
        return _timelineManagerModel.OnPop(unitList);
    }

    private void CreateBanners(List<BaseUnit> units)
    {
        _timelineManagerModel.IncreaseRoundDepth();
        foreach (var unit in units)
        {
            EntityBanner banner = Instantiate(bannerPrefab).GetComponent<EntityBanner>();
            banner.transform.SetParent(transform, false);
            _timelineManagerModel.AddBanner(banner, unit);
        }
    }

    public void DeleteBanners(BaseUnit unit)
    {
        var bannersToRemove = _timelineManagerModel.BannerList.Where(banner => banner.Stat == unit.GetStat()).ToList();
        foreach (var banner in bannersToRemove)
        {
            _timelineManagerModel.RemoveBanner(banner);
        }
    }
}
