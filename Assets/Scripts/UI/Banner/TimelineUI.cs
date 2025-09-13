using DG.Tweening;
using Obvious.Soap;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.FilePathAttribute;

public class TimelineUI : MonoBehaviour
{
    private List<EntityBanner> bannerList = new List<EntityBanner>();
    public List<EntityBanner> BannerList { get { return bannerList; } }
    private EntityBanner currentTurnBanner;

    [SerializeField] private GameObject Arrow;
    [SerializeField] private EntityBanner BannerPrefab;
    [SerializeField] private BannerLocationSetting _locationSetting;
    [SerializeField] private IntVariable MaxShowBannerIndex;

    public ExtraBannerEffect effect;

    private void Awake()
    {
        effect = new ExtraBannerEffect(BannerPrefab, _locationSetting, MaxShowBannerIndex);
    }

    /// <summary>
    /// Change BannerList Collection
    /// </summary>
    public void OnPop()
    {
        currentTurnBanner = bannerList[0];
        bannerList.RemoveAt(0);

        currentTurnBanner.transform.DOKill();
        currentTurnBanner.transform
            .DOScale(Vector3.one * 1.2f, 0.4f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.Linear);
    }

    /// <summary>
    /// This called when received message -> TimelineSystem : m_EndTurn
    /// </summary>
    public void MoveBanners(int foundIndex)
    {
        currentTurnBanner.move?.Cancel();
        currentTurnBanner.Move(_locationSetting.InitialPos, true).Forget();

        effect.Apply(bannerList, foundIndex);
    }

    public EntityBanner CreateBanner(BaseUnit unit, int index, int round)
    {
        EntityBanner banner = Instantiate(BannerPrefab, new Vector2((_locationSetting.InitialPos.x * 2) + _locationSetting.Distance * MaxShowBannerIndex, _locationSetting.InitialPos.y), Quaternion.identity).GetComponent<EntityBanner>();
        banner.Init(unit.GetStat(), index, round);
        return banner;
    }

    public EntityBanner GetCurrentTurnBanner() { return currentTurnBanner; }
}