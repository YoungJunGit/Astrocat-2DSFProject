using UnityEngine;
using UnityEngine.UI;

public class EntityBanner : MonoBehaviour
{
    private RectTransform rectTransform;
    private Image BannerImg;
    private Color BannerColor;

    [SerializeField]
    private EntityBannerInfo myBannerInfo;

    public EntityBannerInfo MyBannerInfo { get { return myBannerInfo; } }

    [SerializeField]
    private float bannerSpeed = 100.0f;
    private Vector2 initialPos = Vector2.zero;
    private Vector2 start = Vector2.zero;
    private Vector2 destination = Vector2.zero;
    private float curTime = 0.0f;
    private int stackInterp = 0;
    private int bannerIndex = 0;
    private bool bInitialized = false;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        BannerImg = GetComponent<Image>();
    }

    private void Update()
    {
        if(!myBannerInfo.bDie && bInitialized)
        {
            curTime += Time.deltaTime * bannerSpeed;
            rectTransform.anchoredPosition = Vector2.Lerp(start, destination, curTime);
        }
    }

    public void SetDestination()
    {
        if(gameObject.activeSelf == false)
        {
            rectTransform.anchoredPosition = new Vector2(initialPos.x + myBannerInfo.stack * 55.0f, initialPos.y);
        }    
        start = new Vector2(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y);
        destination = new Vector2(initialPos.x + myBannerInfo.stack * 55.0f, initialPos.y);
        curTime = 0.0f;
    }

    public void InitBanner(EntityBannerInfo entityBattleInfo, int index)
    {
        myBannerInfo = entityBattleInfo;
        bannerIndex = index;

        switch (myBannerInfo.Side)
        {
            case SIDE.NONE:
                Debug.Log("소속이 정해지지 않았습니다!");
                break;
            case SIDE.PLAYER:
                BannerColor = Color.blue;
                break;
            case SIDE.ENEMY:
                BannerColor = Color.red;
                break;
        }

        BannerImg.color = BannerColor;
        transform.GetChild(0).GetComponent<Text>().text = myBannerInfo.EntityInfo.Name;
    }

    public void SetTransformByOrder(int order)
    {
        Vector2 pos = new Vector2(105.0f + 55.0f * order, -50.0f);
        if (order == 0)
        {
            pos.x = 50.0f;
            rectTransform.sizeDelta = new Vector2(60, 60);
        }

        rectTransform.anchoredPosition = pos;
        start = rectTransform.anchoredPosition;
        destination = rectTransform.anchoredPosition;
        initialPos = rectTransform.anchoredPosition;
        rectTransform.localScale = Vector3.one;
        bInitialized = true;
    }

    public bool Compare(EntityBanner b)
    {
        if(myBannerInfo.EntityInfo.ID == b.MyBannerInfo.EntityInfo.ID)
        {
            return true;
        }

        return false;
    }
}
