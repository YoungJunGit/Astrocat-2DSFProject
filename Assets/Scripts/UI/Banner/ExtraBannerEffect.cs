using DG.Tweening;
using Obvious.Soap;
using System.Collections.Generic;
using UnityEngine;

public class ExtraBannerEffect
{
    private readonly Banner bannerPrefab;
    private readonly BannerSetting location;
    private readonly IntVariable maxShowBannerIndex;

    public ExtraBannerEffect(Banner bannerPrefab, BannerSetting location, IntVariable maxShowBannerIndex)
    {
        this.bannerPrefab = bannerPrefab;
        this.location = location;
        this.maxShowBannerIndex = maxShowBannerIndex;
    }

    public Banner CreateExtraBanner(UnitStat unit, int index, int round)
    {
        Banner banner = Object.Instantiate(bannerPrefab, new Vector2(location.InitialPos.x, (location.InitialPos.y * 2.3f)), Quaternion.identity).GetComponent<Banner>();

        banner.transform.localScale = Vector3.zero;
        banner.transform.DOScale(Vector3.one, 0.4f)
            .SetEase(Ease.Linear);
        banner.gameObject.name = $"Banner:{index}";

        //banner.SetState(BannerState.EXTRA);
        return banner;
    }

    public void ReorderExtraTurn(List<Banner> bannerList, int extraIndex)
    {
        Banner extraBanner = bannerList[extraIndex];

        List<Banner> newList = new List<Banner>();

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