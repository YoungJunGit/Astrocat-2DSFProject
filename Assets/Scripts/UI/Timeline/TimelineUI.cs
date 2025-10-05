using DG.Tweening;
using NUnit.Framework.Internal;
using Obvious.Soap;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.FilePathAttribute;

public class TimelineUI : MonoBehaviour
{
    [SerializeField] private BannerSetting bannerSetting;

    public void SetParent(List<EntityBanner> bannerList)
    {
        foreach (EntityBanner banner in bannerList)
        {
            banner.transform.SetParent(transform, false);
        }
    }

    public void SetRectTransform(List<EntityBanner> bannerList)
    {
        Vector2 pos;
        foreach (var banner in bannerList.Select((value, index) => (value, index)))
        {
            pos = new Vector2((bannerSetting.InitialPos.x * 2) + bannerSetting.Distance * banner.index, bannerSetting.InitialPos.y);
            if (banner.index == 0)
            {
                pos.x = bannerSetting.InitialPos.x;
                banner.value.SetAnchor(bannerSetting.Anchor.max, bannerSetting.Anchor.min);
                banner.value.SetSprite(4);
            }
            banner.value.SetPostion(pos);
            banner.value.SetScale(Vector2.one);
        }
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

    public void MoveBanners(EntityBanner banner, List<EntityBanner> bannerList)
    {
        banner.Move(bannerSetting.InitialPos, true);

        foreach (EntityBanner entityBanner in bannerList)
        {
            Vector2 dest = new Vector2(
                (bannerSetting.InitialPos.x * 2) + bannerSetting.Distance * Mathf.Clamp(entityBanner.Index, 1, bannerSetting.MaxBannerIndex),
                bannerSetting.InitialPos.y
            );

            if (entityBanner.gameObject.activeSelf)
            {
                entityBanner.Move(dest, false);
            }
            else
            {
                entityBanner.SetPostion(dest);
            }
        }
    }

    public EntityBanner CreateBanner(BaseUnit unit, int index, int round)
    {
        EntityBanner banner = Instantiate(bannerSetting.BannerPrefab, new Vector2((bannerSetting.InitialPos.x * 2) + bannerSetting.Distance * bannerSetting.MaxBannerIndex, bannerSetting.InitialPos.y), Quaternion.identity).GetComponent<EntityBanner>();
        banner.Init(unit.GetStat(), index, round);
        return banner;
    }
}