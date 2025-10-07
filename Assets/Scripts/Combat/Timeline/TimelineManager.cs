using System.Collections.Generic;
using UnityEngine;

public class TimelineManager : MonoBehaviour
{
    [SerializeField] private EntityBanner bannerPrefab;
    [SerializeField] private BannerSetting bannerSetting;

    TimelineManagerModel _timelineManagerModel;

    public void Init()
    {
        UnitManager unitManager;
        ServiceLocator.For(this)
                      .Get(out unitManager);
        _timelineManagerModel = new TimelineManagerModel(unitManager);
    }

    public void CreateBanners(List<BaseUnit> units)
    {
        while (_timelineManagerModel.BannersList.Count < bannerSetting.MaxBannerIndex && units != null && units.Count > 0)
        {
            _timelineManagerModel.IncreaseRoundDepth();
            foreach (var unit in units)
            {
                EntityBanner banner = Instantiate(bannerPrefab).GetComponent<EntityBanner>();
                _timelineManagerModel.AddBanner(banner, unit);
            }
        }
    }
}
