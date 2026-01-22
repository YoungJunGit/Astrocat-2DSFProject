using DataEnum;
using UnityEngine;
using R3;

public class NormalBanner : Banner
{
    public void Init(UnitStat stat, int index, int round)
    {
        _sprites = AssetLoader.LoadImgAsset(stat.CoreStat.AssetFileName);
        _bannerImg.sprite = _sprites[0];
        _priorityImg.sprite = stat.CoreStat.Side == SIDE.PLAYER ? _prioritySprites[stat.Priority] : _prioritySprites[stat.Priority + 3];
        _myAnimator.runtimeAnimatorController = AssetLoader.LoadAnimAsset(stat.CoreStat.AssetFileName);

        _bannerViewModel = new BannerViewModel(stat, index, round);

        _bannerViewModel.ReactiveIndex.Where(idx => idx < _bannerSetting.MaxBannerIndex)
                                      .Subscribe(idx => MovePosition(idx))
                                      .AddTo(this);
        _bannerViewModel.ReactiveIndex.Where(idx => idx >= _bannerSetting.MaxBannerIndex)
                                      .Subscribe(idx => SetPosition(idx))
                                      .AddTo(this);
    }
}
