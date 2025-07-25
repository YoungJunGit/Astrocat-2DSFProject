using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TimelineCanvas : MonoBehaviour
{
    [SerializeField] private Transform _timelinePanel;
    [SerializeField] private BannerLocationSetting _locationSetting;

    /// <summary>
    /// Banner : Integrated -> Set Parent + Set RectTransform
    /// </summary>
    /// <param name="bannerList"></param>
    public void SetBanners(List<EntityBanner> bannerList)
    {
        SetParent(bannerList);
        SetRectTransform(bannerList);
    }

    /// <summary>
    /// // Banner : Set Parent
    /// </summary>
    /// <param name="bannerList"></param>
    public void SetParent(List<EntityBanner> bannerList)
    {
        foreach (EntityBanner banner in bannerList)
        {
            banner.transform.SetParent(_timelinePanel, false);
        }
    }

    /// <summary>
    /// // Banner : Set RectTransform
    /// </summary>
    /// <param name="bannerList"></param>
    public void SetRectTransform(List<EntityBanner> bannerList)
    {
        Vector2 pos;
        foreach (var banner in bannerList.Select((value, index) => (value, index)))
        {
            pos = new Vector2((_locationSetting.InitialPos.x * 2) + _locationSetting.Distance * banner.index, _locationSetting.InitialPos.y);
            if (banner.index == 0)
            {
                pos.x = _locationSetting.InitialPos.x;
                banner.value.SetAnchor(_locationSetting.Anchor.max, _locationSetting.Anchor.min);
                banner.value.SetSprite(4);
            }
            banner.value.SetPostion(pos);
            banner.value.SetScale(Vector2.one);
        }
    }
}
