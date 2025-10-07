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

    private Sprite[] sprites;
    [SerializeField] private Sprite[] prioritySprites;

    [Header("Banner Settings")]
    [SerializeField] private float moveDuration = 0.25f;
    [SerializeField] private BannerSetting bannerSetting;

    private UnitStat        _stat;
    public UnitStat Stat => _stat;

    private int _index;
    public int Index {
        get { return _index; }
        set
        {
            _index = value;
            _reactiveIndex.Value = value;
            if (_index <= bannerSetting.MaxBannerIndex - 1)
                gameObject.SetActive(true);
            else
                gameObject.SetActive(false);
        }
    }

    private ReactiveProperty<int> _reactiveIndex = new(0);
    public int ReactiveIndex => _reactiveIndex.Value;

    private int _round;
    public int Round => _round;

    public void Init(UnitStat stat, int index, int round)
    {
        _stat = stat;
        _index = index;
        _round = round;

        _reactiveIndex = new(index);
        _reactiveIndex.Do(idx => gameObject.name = $"Banner:{idx}")
                      .Subscribe(idx => ReactiveMove(idx))
                      .AddTo(this);

        sprites = AssetLoader.LoadImgAsset(_stat.GetData().Asset_File);
        bannerImg.sprite = sprites[0];
        priorityImg.sprite = _stat.GetData().Side == SIDE.PLAYER ? prioritySprites[_stat.Priority] : prioritySprites[_stat.Priority + 3];
        myAnimator.runtimeAnimatorController = AssetLoader.LoadAnimAsset(_stat.GetData().Asset_File);

        if (index == 0)
            myAnimator.SetTrigger("Skip");
    }

    public void ReactiveMove(int index)
    {
        Debug.Log(gameObject.name);
    }

    public void Move(Vector2 destination, bool isFirstBanner)
    {
        rectTransform.DOAnchorPos(destination, moveDuration);

        if (isFirstBanner)
        {
            myAnimator.SetTrigger("Move");
        }
    }

    public void SetAnchor(Vector2 anchorMax, Vector2 anchorMin)
    {
        priorityRectTransform.anchorMax = anchorMax;
        priorityRectTransform.anchorMin = anchorMin;
    }

    public void SetPostion(Vector2 pos)
    {
        rectTransform.anchoredPosition = pos;
    }

    public void SetScale(Vector2 scale)
    {
        rectTransform.localScale = scale;
    }

    public void SetSprite(int index)
    {
        bannerImg.sprite = sprites[index];
    }

    public void OnPop()
    {
        this.transform.DOKill();
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
        if (this.Round < other.Round) { return -1; }
        else if (this.Round > other.Round) { return 1; }
        else
        {
            return _stat.CompareTo(other._stat);
        }
    }
}
