using Obvious.Soap;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TimelineUI : MonoBehaviour
{
    private List<EntityBanner> bannerList = new List<EntityBanner>();
    public List<EntityBanner> BannerList { get { return bannerList; } }
    private EntityBanner currentTurnBanner;

    [SerializeField] private GameObject Arrow;
    [SerializeField] private EntityBanner BannerPrefab;
    [SerializeField] private BannerLocationSetting _locationSetting;
    [SerializeField] private IntVariable MaxShowBannerIndex;

    /// <summary>
    /// Change BannerList Collection
    /// </summary>
    public void OnPop()
    {
        currentTurnBanner = bannerList[0];
        bannerList.RemoveAt(0);
    }

    /// <summary>
    /// This called when received message -> TimelineSystem : m_EndTurn
    /// </summary>
    public void MoveBanners()
    {
        currentTurnBanner.move?.Cancel();
        currentTurnBanner.Move(_locationSetting.InitialPos, true).Forget();

        Vector2 dest;
        foreach (EntityBanner banner in bannerList)
        {
            dest = new Vector2((_locationSetting.InitialPos.x * 2) + _locationSetting.Distance * Mathf.Clamp(banner.Index, 1, MaxShowBannerIndex), _locationSetting.InitialPos.y);

            banner.move?.Cancel();
            if (banner.gameObject.activeSelf)
            {
                banner.Move(dest, false).Forget();
            }
            else
            {
                banner.SetPostion(dest);
            }
        }
    }

    public EntityBanner CreateBanner(BaseUnit unit, int index, int round)
    {
        EntityBanner banner = Instantiate(BannerPrefab, new Vector2((_locationSetting.InitialPos.x * 2) + _locationSetting.Distance * MaxShowBannerIndex, _locationSetting.InitialPos.y), Quaternion.identity).GetComponent<EntityBanner>();
        banner.Init(unit.GetStat(), index, round);

        return banner;
    }

    public EntityBanner GetCurrentTurnBanner() { return currentTurnBanner; }
}
