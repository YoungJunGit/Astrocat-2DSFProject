using Cysharp.Threading.Tasks;
using ObservableCollections;
using Obvious.Soap;
using R3;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TimelineManager : MonoBehaviour
{
    [SerializeField] private EntityBanner bannerPrefab;
    [SerializeField] private IntVariable MaxBannerIndex;

    private TimelineManagerModel _timelineManagerModel;
    private UnitManager _unitManager;

    public void Init(UnitManager unitManager)
    {
        _unitManager = unitManager;
        _timelineManagerModel = new TimelineManagerModel();
    }

    public void CreateTimeline()
    {
        while (_timelineManagerModel.BannerList.Count < MaxBannerIndex && _unitManager.GetAllUnits() != null && _unitManager.GetAllUnits().Count > 0)
        {
            CreateBanners();
        }
    }

    public void Prepare()
    {
        // Data Bindings
        _timelineManagerModel.BannerList.ObserveRemove()
                                        .Subscribe(_ => 
                                        {
                                            if (_timelineManagerModel.BannerList.Count < MaxBannerIndex)
                                            {
                                                CreateBanners();
                                                _timelineManagerModel.SortBanners();
                                            }
                                        })
                                        .AddTo(this);

        _timelineManagerModel.CurRound.Subscribe(_ => 
                                      {
                                          _timelineManagerModel.SortBanners();
                                      })
                                      .AddTo(this);
    }

    public BaseUnit Pop(List<BaseUnit> unitList)
    {
        return _timelineManagerModel.OnPop(unitList);
    }

    private void CreateBanners()
    {
        _timelineManagerModel.IncreaseRoundDepth();
        foreach (var unit in _unitManager.GetAllUnits())
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
            banner.DestroyBanner();
        }
    }
}
