using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EntityBanner : MonoBehaviour
{
    private RectTransform rectTransform;
    private RectTransform priorityRectTransform;
    private Animator myAnimator;
    private Image BannerImg;
    private Image PriorityImg;
    private Sprite[] mySprites;

    [SerializeField]
    private Sprite[] prioritySprites;
    [SerializeField]
    private float bannerSpeed = 1.0f;
    [SerializeField]
    private Vector2 initialPos = Vector2.zero;

    private EntityBannerInfo myBannerInfo;
    public EntityBannerInfo MyBannerInfo { get { return myBannerInfo; } }

    private int turn = 0;
    public int Turn { get { return turn; } }

    private Coroutine MoveCoroutine;

    public int CompareTo(EntityBanner other)
    {
        if (this.turn < other.turn) { return -1; }
        else if (this.turn > other.turn)  { return 1; }
        else
        {
            if (this.myBannerInfo.Speed > other.myBannerInfo.Speed) { return -1; }
            else if (this.myBannerInfo.Speed < other.myBannerInfo.Speed) { return 1; }
            else
            {
                if (this.myBannerInfo.Side < other.myBannerInfo.Side) { return -1; }
                else if (this.myBannerInfo.Side > other.myBannerInfo.Side) { return 1; }
                else
                {
                    if (this.myBannerInfo.Priority < other.myBannerInfo.Priority) { return -1; }
                    else return 1;
                }
            }
        }
    }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        priorityRectTransform = transform.GetChild(0).GetComponent<RectTransform>();
        BannerImg = GetComponent<Image>();
        PriorityImg = transform.GetChild(0).GetComponent<Image>();
        myAnimator = GetComponent<Animator>();
    }

    private IEnumerator MoveBanner(int index)
    {
        Vector2 start = new Vector2(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y);
        Vector2 destination = new Vector2((initialPos.x * 2) + 80.0f * index, initialPos.y);

        if (index == 0)
        {
            destination = new Vector2(initialPos.x, initialPos.y);
            myAnimator.SetTrigger("Move");
        }

        float curTime = 0.0f;
        while (rectTransform.anchoredPosition != destination)
        {
            curTime += Time.deltaTime * bannerSpeed;
            rectTransform.anchoredPosition = Vector2.Lerp(start, destination, curTime);
            yield return null;
        }
    }

    public void SetDestination(int index)
    {
        gameObject.name = "Banner:" + index;

        if (index < 7)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }

        if (MoveCoroutine != null)
            StopCoroutine(MoveCoroutine);

        if (gameObject.activeSelf)
            MoveCoroutine = StartCoroutine(MoveBanner(index));
        else
            rectTransform.anchoredPosition = new Vector2((initialPos.x * 2) + 80.0f * 7, initialPos.y);
    }

    public void InitBanner(EntityBannerInfo entityBannerInfo, int index, int round)
    {
        gameObject.name = $"Banner: {index}";
        myBannerInfo = entityBannerInfo;
        turn = round;

        mySprites = AssetLoader.LoadImgAsset(myBannerInfo.EntityInfo.Asset_File);
        myAnimator.runtimeAnimatorController = AssetLoader.LoadAnimAsset(myBannerInfo.EntityInfo.Asset_File);

        if (index == 0)
            myAnimator.SetTrigger("Skip");

        if (myBannerInfo.Side == SIDE.PLAYER)
            PriorityImg.sprite = prioritySprites[myBannerInfo.Priority];
        else
            PriorityImg.sprite = prioritySprites[myBannerInfo.Priority + 3];

        BannerImg.sprite = mySprites[0];

        transform.SetParent(GameObject.Find("Turn-Timeline").transform);
    }

    public void SetTransformByIndex(int index)
    {
        int clampedIndex = Mathf.Clamp(index, 0, 7);
        Vector2 pos = new Vector2((initialPos.x * 2) + 80.0f * index, initialPos.y);
        if (index == 0)
        {
            pos.x = initialPos.x;
            priorityRectTransform.anchorMax = new Vector2(0.31f, 0.29f);
            priorityRectTransform.anchorMin = new Vector2(0.077f, 0.079f);
            BannerImg.sprite = mySprites[4];
        }

        rectTransform.anchoredPosition = pos;
        rectTransform.localScale = Vector3.one;
    }

    public void DestroyBanner()
    {
        Destroy(gameObject);
    }
}
