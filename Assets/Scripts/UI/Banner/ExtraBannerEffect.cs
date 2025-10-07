using DG.Tweening;
using Obvious.Soap;
using System.Collections.Generic;
using UnityEngine;
using static EntityBanner;

public class ExtraBannerEffect
{
    private readonly EntityBanner bannerPrefab;
    private readonly BannerSetting location;
    private readonly IntVariable maxShowBannerIndex;

    public ExtraBannerEffect(EntityBanner bannerPrefab, BannerSetting location, IntVariable maxShowBannerIndex)
    {
        this.bannerPrefab = bannerPrefab;
        this.location = location;
        this.maxShowBannerIndex = maxShowBannerIndex;
    }

    public EntityBanner CreateExtraBanner(UnitStat unit, int index, int round)
    {
        EntityBanner banner = Object.Instantiate(bannerPrefab, new Vector2(location.InitialPos.x, (location.InitialPos.y * 2.3f)), Quaternion.identity).GetComponent<EntityBanner>();
        banner.Init(unit, index, round);

        banner.transform.localScale = Vector3.zero;
        banner.transform.DOScale(Vector3.one, 0.4f)
            .SetEase(Ease.Linear);
        banner.gameObject.name = $"Banner:{index}";

        //banner.SetState(BannerState.EXTRA);
        return banner;
    }

    public void ReorderExtraTurn(List<EntityBanner> bannerList, int extraIndex)
    {
        EntityBanner extraBanner = bannerList[extraIndex];

        List<EntityBanner> newList = new List<EntityBanner>();

        newList.Add(extraBanner);

        for (int i = 0; i < bannerList.Count; i++)
        {
            if (i == extraIndex) continue;
            newList.Add(bannerList[i]);
        }

        bannerList.Clear();
        bannerList.AddRange(newList);

        for (int i = 0; i < bannerList.Count; i++)
        {
            bannerList[i].Index = i + 1;
        }
    }
}