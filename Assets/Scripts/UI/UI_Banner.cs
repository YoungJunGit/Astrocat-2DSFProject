using DataEnum;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class UI_Banner : MonoBehaviour
{
    [SerializeField]
    private SIDE side = SIDE.NONE;

    private Image BannerImg;
    private Color BannerColor;

    private EntityData entityInfo;

    public EntityData EntityData { get { return entityInfo; } }

    public double speed = 0.0f;
    public int priority = 0;
    public int turn = 0;

    public int CompareTo(UI_Banner obj)
    {
        if (this.turn < obj.turn) { return -1; }
        else if (this.turn > obj.turn) { return 1; }
        else
        {
            if (this.speed > obj.speed) { return -1; }
            else if(this.speed < obj.speed) { return 1; }
            else
            {
                if(this.side < obj.side) { return -1; }
                else if(this.side > obj.side ) { return 1; }
                else
                {
                    if (this.priority < obj.priority) { return -1; }
                    else return 1;
                }
            }
        }
    }

    private void Awake()
    {
        BannerImg = GetComponent<Image>();
    }

    private void Start()
    {
        BannerImg.color = BannerColor;
    }

    public void InitBanner(EntityData data, int priority, int turn)
    {
        this.entityInfo = data;
        this.side = entityInfo.Side;
        this.speed = entityInfo.Default_Speed;
        this.priority = priority;
        this.turn = turn;

        switch (side)
        {
            case SIDE.NONE:
                Debug.LogWarning("소속이 정해지지 않았습니다!");
                break;
            case SIDE.PLAYER:
                BannerColor = Color.blue;
                break;
            case SIDE.ENEMY:
                BannerColor = Color.red;
                break;
        }

        transform.GetChild(0).GetComponent<Text>().text = entityInfo.Name;
    }

    public void SetTransformByOrder(int order)
    {
        Vector2 pos = new Vector2(105.0f + 55.0f * order, -50.0f);
        if(order == 0)
        {
            pos.x = 50.0f;
            transform.GetComponent<RectTransform>().sizeDelta = new Vector2(60, 60);
        }

        transform.GetComponent<RectTransform>().anchoredPosition = pos;
        transform.GetComponent <RectTransform>().localScale = Vector3.one;
    }
}
