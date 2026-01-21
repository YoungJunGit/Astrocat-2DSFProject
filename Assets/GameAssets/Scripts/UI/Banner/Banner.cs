using DataEnum;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using R3;

public interface IBanner
{
    int Index { get; set; }
    int Round { get; set; }
    UnitStat Stat { get; }
    void SetState(IBannerState newState);
    int CompareTo(IBanner other);
    bool CompareStat(UnitStat otherStat);
}

public abstract class Banner : MonoBehaviour, IBanner
{
    [Header("Component Settings")]
    [SerializeField] protected Animator       _myAnimator;
    [SerializeField] protected RectTransform  _rectTransform;
    [SerializeField] protected RectTransform  _priorityRectTransform;
    [SerializeField] protected Image          _bannerImg;
    [SerializeField] protected Image          _priorityImg;
    [SerializeField] protected Sprite[]       _prioritySprites;
    protected Sprite[]                        _sprites;

    [Header("Banner Settings")]
    [SerializeField] protected BannerSetting  _bannerSetting;

    protected BannerViewModel _bannerViewModel;
    public int Index
    {
        get { return _bannerViewModel.ReactiveIndex.CurrentValue; }
        set { _bannerViewModel.SetIndex(value); gameObject.name = $"Banner: {value}"; }
    }
    public int Round
    {
        get { return _bannerViewModel.ReactiveRound.CurrentValue; }
        set { _bannerViewModel.SetRound(value); }
    }
    public UnitStat Stat => _bannerViewModel.Stat;

    public int CompareTo(IBanner other)         => _bannerViewModel.CompareTo((other as Banner)._bannerViewModel);
    public bool CompareStat(UnitStat otherStat) => _bannerViewModel.CompareStat(otherStat);
    protected void Copy(Banner other)
    {
        _sprites         = other._sprites;
        _bannerImg       = other._bannerImg;
        _prioritySprites = other._prioritySprites;
        _myAnimator.runtimeAnimatorController = other._myAnimator.runtimeAnimatorController;
        _bannerViewModel = other._bannerViewModel;
    }

    protected void MovePosition(int index)
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

    protected void SetPosition(int index)
    {
        gameObject.SetActive(false);

        this.transform.DOKill();
        _rectTransform.anchoredPosition = _bannerSetting.FinalPos;
    }

    public void SetState(IBannerState newState)
    {
        newState.PlayState(this.transform);
    }
}
