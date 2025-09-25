using Cysharp.Threading.Tasks;
using DataEntity;
using DataEnum;
using Obvious.Soap;
using System.Collections;
using System.ComponentModel;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class EntityBanner : MonoBehaviour
{
    public enum BannerState
    {
        NORMAL,
        FAINT,
        EXTRA
    }

    [Header("Component Settings")]
    [SerializeField] private Animator myAnimator;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private RectTransform priorityRectTransform;
    [SerializeField] private Image bannerImg;
    [SerializeField] private Image priorityImg;

    private Sprite[] sprites;
    [SerializeField] private Sprite[] prioritySprites;

    [Header("Banner Move Settings")]
    [SerializeField] private float moveDuration = 0.25f;
    [SerializeField] private IntVariable MaxShowBannerIndex;

    private UnitStat stat;
    private BannerState state;

    private int index;
    public int Index {
        get { return index; }
        set
        {
            index = value;
            if (index <= MaxShowBannerIndex.Value - 1)
                gameObject.SetActive(true);
            else
                gameObject.SetActive(false);

            // For Debugging
            gameObject.name = $"Banner:{index}";
        }
    }

    private int round;
    public int Round => round;

    public void Init(UnitStat stat, int index, int round)
    {
        this.stat = stat;
        this.index = index;
        this.round = round;

        sprites = AssetLoader.LoadImgAsset(this.stat.GetData().Asset_File);
        myAnimator.runtimeAnimatorController = AssetLoader.LoadAnimAsset(this.stat.GetData().Asset_File);
        bannerImg.sprite = sprites[0];
        priorityImg.sprite = this.stat.GetData().Side == SIDE.PLAYER ? prioritySprites[this.stat.Priority] : prioritySprites[this.stat.Priority + 3];

        if (index == 0)
            myAnimator.SetTrigger("Skip");
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

    /// <summary>
    /// EntityBanner : For Debugging
    /// </summary>
    /// <param name="index"> Index For Banner Name </param>
    public void SetName(string name)
    {
        gameObject.name = name;
    }

    public void SetState(BannerState state)
    {
        this.state = state;
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
        if (this.round < other.round) { return -1; }
        else if (this.round > other.round) { return 1; }
        else
        {
            return stat.CompareTo(other.stat);
        }
    }

    public UnitStat GetStat() { return stat; }
    public BannerState GetState() { return state; }
}
