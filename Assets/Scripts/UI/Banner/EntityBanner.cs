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
    [Header("Component Settings")]
    [SerializeField] private Animator myAnimator;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private RectTransform priorityRectTransform;
    [SerializeField] private Image bannerImg;
    [SerializeField] private Image priorityImg;
    
    private Sprite[] sprites;
    [SerializeField] private Sprite[] prioritySprites;

    [Header("Banner Settings")]
    [SerializeField] private float bannerSpeed = 1.0f;
    [SerializeField] private IntVariable MaxShowBannerIndex;

    private UnitStat stat;
    public int stateIndex; // 0 == normal turn,  1 == faint, 2 == extra turn

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
    public int Round { get { return round; } }

    public CancellationTokenSource move;

    public void Init(UnitStat stat, int index, int round)
    {
        this.stat = stat;
        this.index = index;
        this.round = round;
        this.stateIndex = 0;

        sprites = AssetLoader.LoadImgAsset(this.stat.GetData().Asset_File);
        myAnimator.runtimeAnimatorController = AssetLoader.LoadAnimAsset(this.stat.GetData().Asset_File);

        priorityImg.sprite = this.stat.GetData().Side == SIDE.PLAYER ? prioritySprites[this.stat.Priority] : prioritySprites[this.stat.Priority + 3];
        bannerImg.sprite = sprites[0];

        if (index == 0)
            myAnimator.SetTrigger("Skip");
    }

    /// <summary>
    /// Move banner to destination smoothly -> Called in TimelineUI
    /// </summary>
    /// <param name="destination"></param>
    /// <param name="isFirstBanner"></param>
    /// <returns></returns>
    public async UniTaskVoid Move(Vector2 destination, bool isFirstBanner)
    {
        move = new CancellationTokenSource();
        Vector2 start = new Vector2(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y);

        if (isFirstBanner)
        {
            myAnimator.SetTrigger("Move");
        }

        float curTime = 0.0f;
        while (rectTransform.anchoredPosition != destination)
        {
            curTime += Time.deltaTime * bannerSpeed;
            rectTransform.anchoredPosition = Vector2.Lerp(start, destination, curTime);

            await UniTask.Yield(cancellationToken: move.Token);
        }
        move = null;
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

    public void DestroyBanner()
    {
        this.transform
            .DOScale(Vector3.zero, 0.4f)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
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

        Debug.Log("Fainting Effect applied (darken + fade)");
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

    private void OnDestroy()
    {
        move?.Cancel();
        move?.Dispose();
    }
}
