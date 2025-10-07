using DG.Tweening;
using NUnit.Framework.Internal;
using Obvious.Soap;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.FilePathAttribute;

public class TimelineUI : MonoBehaviour
{
    [SerializeField] private EntityBanner  bannerPrefab;
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
        foreach (var banner in bannerList.Select((value, index) => (value, index)))
        {
            Vector2 pos = new Vector2((bannerSetting.InitialPos.x * 2) + bannerSetting.Distance * banner.index, bannerSetting.InitialPos.y);
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

    public List<EntityBanner> CreateBanners(List<BaseUnit> unitList, int index, int round)
    {
        List<EntityBanner> createdUnits = new();
        foreach (var unit in unitList.Select((value, index) => (value, index)))
        {
            EntityBanner banner = Instantiate(bannerPrefab).GetComponent<EntityBanner>();
            banner.Init(unit.value.GetStat(), index + unit.index, round);
            createdUnits.Add(banner);
        }

        return createdUnits;
    }

    public void DeleteBanners(List<EntityBanner> bannerList)
    {
        foreach(EntityBanner banner in bannerList)
        {
            banner.DestroyBanner();
        }
    }
}