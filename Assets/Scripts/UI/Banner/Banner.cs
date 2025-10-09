using DataEnum;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using R3;

public interface IBanner
{
    
    int CompareTo(IBanner other);
}

public abstract class Banner : MonoBehaviour, IBanner
{
    [Header("Component Settings")]
    [SerializeField] private Animator       _myAnimator;
    [SerializeField] private RectTransform  _rectTransform;
    [SerializeField] private RectTransform  _priorityRectTransform;
    [SerializeField] private Image          _bannerImg;
    [SerializeField] private Image          _priorityImg;
    [SerializeField] private Sprite[]       _prioritySprites;
    private Sprite[]                        _sprites;

    [Header("Banner Settings")]
    [SerializeField] private BannerSetting  _bannerSetting;

    private BannerViewModel _bannerViewModel;
    public int Index
    {
        get { return _bannerViewModel.ReactiveIndex.CurrentValue; }
        set { _bannerViewModel.SetIndex(value); }
    }
    public int Round
    {
        get { return _bannerViewModel.ReactiveRound.CurrentValue; }
        set { _bannerViewModel.SetRound(value); }
    }

    public void Init(UnitStat stat, int index, int round)
    {
        _sprites                = AssetLoader.LoadImgAsset(stat.GetData().Asset_File);
        _bannerImg.sprite       = _sprites[0];
        _priorityImg.sprite     = stat.GetData().Side == SIDE.PLAYER ? _prioritySprites[stat.Priority] : _prioritySprites[stat.Priority + 3];
        _myAnimator.runtimeAnimatorController = AssetLoader.LoadAnimAsset(stat.GetData().Asset_File);

        _bannerViewModel = new BannerViewModel(stat, index, round);

        _bannerViewModel.ReactiveIndex.Where(idx => idx < _bannerSetting.MaxBannerIndex)
                                      .Subscribe(idx => Move(idx))
                                      .AddTo(this);
        _bannerViewModel.ReactiveIndex.Where(idx => idx >= _bannerSetting.MaxBannerIndex)
                                      .Subscribe(idx => Set(idx))
                                      .AddTo(this);
    }

    private void Move(int index)
    {
        gameObject.SetActive(true);
        if (index == 0)
        {
            _rectTransform.DOAnchorPos(_bannerSetting.InitialPos, _bannerSetting.MoveDuration);
            _myAnimator.SetTrigger("Move");
        }
        else
        {
            _rectTransform.DOAnchorPos(_bannerSetting.CurrentPos(index), _bannerSetting.MoveDuration);
        }
    }

    private void Set(int index)
    {
        gameObject.SetActive(false);

        this.transform.DOKill();
        _rectTransform.anchoredPosition = _bannerSetting.FinalPos;
    }

    public int CompareTo(IBanner other)
    {
        return _bannerViewModel.CompareTo((other as Banner)._bannerViewModel);
    }
}
