using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Runtime.CompilerServices;
using System.Collections;
using Unity.VisualScripting;

public class EntityBanner : MonoBehaviour
{
    private RectTransform rectTransform;
    private RectTransform priorityRectTransform;
    private Animator myAnimator;
    private Image BannerImg;
    private Image PriorityImg;

    [SerializeField]
    private Sprite[] mySprites;

    [SerializeField]
    private Sprite[] prioritySprites;

    [SerializeField]
    private EntityBannerInfo myBannerInfo;

    public EntityBannerInfo MyBannerInfo { get { return myBannerInfo; } }

    [SerializeField]
    private float bannerSpeed = 1.0f;

    [SerializeField]
    private Vector2 initialPos = Vector2.zero;

    private int bannerIndex = 0;
    public int BannerIndex {  get { return bannerIndex; } }

    private string TmpAssetPath = "05_UI_UX/Banner/";

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        priorityRectTransform = transform.GetChild(0).GetComponent<RectTransform>();
        BannerImg = GetComponent<Image>();
        PriorityImg = transform.GetChild(0).GetComponent<Image>();
        myAnimator = GetComponent<Animator>();
    }

    private IEnumerator MoveBanner()
    {
        Vector2 start = new Vector2(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y);
        Vector2 destination = new Vector2((initialPos.x * 2) + 80.0f * bannerIndex, initialPos.y);

        if (bannerIndex == 0)
        {
            destination = new Vector2(initialPos.x, initialPos.y);
            myAnimator.SetTrigger("PenguinBerserker");
        }

        float curTime = 0.0f;
        while (rectTransform.anchoredPosition != destination)
        {
            curTime += Time.deltaTime * bannerSpeed;
            rectTransform.anchoredPosition = Vector2.Lerp(start, destination, curTime);
            yield return null;
        }
    }

    public void SetDestination(int stack)
    {
        bannerIndex += stack;

        gameObject.name = "Banner:" + bannerIndex;

        if (bannerIndex < 7)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }

        if(gameObject.activeSelf)
            StartCoroutine("MoveBanner");
        else
            rectTransform.anchoredPosition = new Vector2((initialPos.x * 2) + 80.0f * bannerIndex, initialPos.y);
    }

    public void InitBanner(EntityBannerInfo entityBattleInfo, int index)
    {
        myBannerInfo = entityBattleInfo;
        bannerIndex = index;

        if (bannerIndex == 0)
            myAnimator.SetTrigger("Skip");

        if (myBannerInfo.Side == SIDE.PLAYER)
            PriorityImg.sprite = prioritySprites[myBannerInfo.Priority];
        else
            PriorityImg.sprite = prioritySprites[myBannerInfo.Priority + 3];

        LoadAsset();
        BannerImg.sprite = mySprites[0];

        transform.SetParent(GameObject.Find("Turn-Timeline").transform);
        SetTransformByIndex();
    }

    private void LoadAsset()
    {
        mySprites = Resources.LoadAll<Sprite>(TmpAssetPath + "player_Commissar");

        if (mySprites.Length <= 0)
            Debug.Log("배너 이미지의 경로 설정 오류!");
    }

    public void SetTransformByIndex()
    {
        Vector2 pos = new Vector2((initialPos.x * 2) + 80.0f * bannerIndex, initialPos.y);
        if (bannerIndex == 0)
        {
            pos.x = initialPos.x;
            priorityRectTransform.anchorMax = new Vector2(0.31f, 0.29f);
            priorityRectTransform.anchorMin = new Vector2(0.077f, 0.079f);
            BannerImg.sprite = mySprites[4];
        }

        rectTransform.anchoredPosition = pos;
        rectTransform.localScale = Vector3.one;
    }

    public bool Compare(EntityBanner b)
    {
        if (myBannerInfo.EntityInfo.ID == b.MyBannerInfo.EntityInfo.ID)
        {
            return true;
        }

        return false;
    }
}
