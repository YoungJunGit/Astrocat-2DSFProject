using DG.Tweening;
using Obvious.Soap;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TimelineUI : MonoBehaviour
{
    [SerializeField] private BannerSetting bannerSetting;
    [SerializeField] private IntVariable MaxShowBannerIndex;

    public ExtraBannerEffect effect;

    private void Awake()
    {
        effect = new ExtraBannerEffect(bannerSetting.BannerPrefab, bannerSetting, MaxShowBannerIndex);
    }

    /// <summary>
    /// Change BannerList Collection
    /// </summary>
    public void OnPop(EntityBanner banner)
    {
        banner.transform.DOKill();
        banner.transform
            .DOScale(Vector3.one * 1.2f, 0.4f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.Linear);
    }

    public void MoveBanners(EntityBanner banner, List<EntityBanner> bannerList, int foundIndex)
    {
        banner.Move(bannerSetting.InitialPos, true);

        effect.Apply(bannerList, foundIndex);
    }

    public EntityBanner CreateBanner(BaseUnit unit, int index, int round)
    {
        EntityBanner banner = Instantiate(bannerSetting.BannerPrefab, new Vector2((bannerSetting.InitialPos.x * 2) + bannerSetting.Distance * MaxShowBannerIndex, bannerSetting.InitialPos.y), Quaternion.identity).GetComponent<EntityBanner>();
        banner.Init(unit.GetStat(), index, round);
        return banner;
    }
}