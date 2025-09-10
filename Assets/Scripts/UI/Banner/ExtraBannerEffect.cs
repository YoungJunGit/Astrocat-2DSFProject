using DG.Tweening;
using Obvious.Soap;
using System.Collections.Generic;
using UnityEngine;

public class ExtraBannerEffect
{
    private readonly TimelineUI timelineUI;
    private readonly EntityBanner bannerPrefab;
    private readonly BannerLocationSetting location;
    private readonly IntVariable maxShowBannerIndex;

    public ExtraBannerEffect(TimelineUI timelineUI, EntityBanner bannerPrefab, BannerLocationSetting location, IntVariable maxShowBannerIndex)
    {
        this.timelineUI = timelineUI;
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

        maxShowBannerIndex.Add(1);
        banner.stateIndex = 2;
        return banner;
    }

    public void ReorderExtraTurn(int extraIndex)
    {
        var bannerList = timelineUI.BannerList;
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

    public void Apply(List<EntityBanner> bannerList, int foundIndex)
    {
        if (bannerList == null || bannerList.Count == 0) return;

        Vector2 dest;

        if (foundIndex == 0)
        {
            foreach (EntityBanner banner in bannerList)
            {
                dest = new Vector2(
                    (location.InitialPos.x * 2) + location.Distance * Mathf.Clamp(banner.Index, 1, maxShowBannerIndex),
                    location.InitialPos.y
                );

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
        else
        {
            for (int i = foundIndex; i < bannerList.Count; i++)
            {
                var banner = bannerList[i];
                dest = new Vector2(
                    (location.InitialPos.x * 2) + location.Distance * Mathf.Clamp(banner.Index, 1, Mathf.Max(1, maxShowBannerIndex - 1)),
                    location.InitialPos.y
                );

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
    }
}