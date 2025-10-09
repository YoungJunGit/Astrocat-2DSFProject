using Cysharp.Threading.Tasks;
using DataEntity;
using DataEnum;
using DG.Tweening;
using Obvious.Soap;
using R3;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class EntityBanner : MonoBehaviour
{
    [Header("Component Settings")]
    [SerializeField] private Animator myAnimator;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private RectTransform priorityRectTransform;
    [SerializeField] private Image bannerImg;
    [SerializeField] private Image priorityImg;
    [SerializeField] private Sprite[] prioritySprites;
    private Sprite[] sprites;

    [Header("Banner Settings")]
    [SerializeField] private BannerSetting bannerSetting;

    private UnitStat        _stat;
    private ReactiveProperty<int> _reactiveIndex;
    private int _round;

    public UnitStat Stat => _stat;
    public int Index {
        get { return _reactiveIndex.Value; }
        set
        {
            gameObject.name = $"Banner:{value}";
            _reactiveIndex.Value = value;
        }
    }
    public int Round => _round;

    public void Init(UnitStat stat, int index, int round)
    {
        _stat = stat;
        _round = round;
        _reactiveIndex = new(index);

        sprites = AssetLoader.LoadImgAsset(_stat.GetData().Asset_File);
        bannerImg.sprite = sprites[0];
        priorityImg.sprite = _stat.GetData().Side == SIDE.PLAYER ? prioritySprites[_stat.Priority] : prioritySprites[_stat.Priority + 3];
        myAnimator.runtimeAnimatorController = AssetLoader.LoadAnimAsset(_stat.GetData().Asset_File);

        _reactiveIndex.Where(idx => idx < bannerSetting.MaxBannerIndex)
                      .Subscribe(idx => Move(idx))
                      .AddTo(this);
        _reactiveIndex.Where(idx => idx >= bannerSetting.MaxBannerIndex)
                      .Subscribe(idx => Set(idx))
                      .AddTo(this);
    }

    private void Move(int index)
    {
        gameObject.SetActive(true);
        if (index == 0)
        {
            rectTransform.DOAnchorPos(bannerSetting.InitialPos, bannerSetting.MoveDuration);
            myAnimator.SetTrigger("Move");
        }
        else
        {
            rectTransform.DOAnchorPos(bannerSetting.CurrentPos(index), bannerSetting.MoveDuration);
        }
    }

    private void Set(int index)
    {
        gameObject.SetActive(false);

        this.transform.DOKill();
        rectTransform.anchoredPosition = bannerSetting.FinalPos;
    }

    public void OnPop()
    {
        this.transform
            .DOScale(Vector3.one * 1.2f, 0.4f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.Linear);
    }

    public void DestroyBanner()
    {
        this.transform
            .DOScale(Vector3.zero, 0.4f)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                this.transform.DOKill();
                Destroy(gameObject);
            });
    }

    public void FaintingEffect()
    {
        var c = bannerImg.color;
        var darkColor = new Color(c.r * 0.3f, c.g * 0.3f, c.b * 0.3f, c.a);

        DOTween.Sequence()
            .Join(bannerImg.DOColor(darkColor, 0.4f).SetEase(Ease.OutQuad))
            .Join(bannerImg.DOFade(0.7f, 0.4f).SetEase(Ease.OutQuad))
            .Play();
    }

    public int CompareTo(EntityBanner other)
    {
        if (this._round < other._round) { return -1; }
        else if (this._round > other._round) { return 1; }
        else
        {
            return _stat.CompareTo(other._stat);
        }
    }
}
