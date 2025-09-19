using Cysharp.Threading.Tasks;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.PlayerLoop;
using DG.Tweening;

public class EnemyHUD : BaseHUD, IUpdateObserver
{
    [Space(10f)]
    [SerializeField] private Vector3 posOffset;
    private RectTransform rectTransform;
    private Transform statusPos;
    private UnitStat stat;

    [HideInInspector] public Vector3 spawnPos;

    [Header("HP Tween")]
    [SerializeField] private float hpTweenDuration = 0.5f; 

    public override void Initialize(BaseUnit unit)
    {
        rectTransform = GetComponent<RectTransform>();
        stat = unit.GetStat();
        stat.OnHPChanged += OnHPChanged;
    }

    public void AttachHUD(Transform statusPos)
    {
        this.statusPos = statusPos;
        UpdatePublisher.SubscribeObserver(this);
    }

    public override void OnHPChanged(float curHp, float maxHp)
    {
        float targetValue = curHp / maxHp;
        hp_Text.text = $"{curHp}/{maxHp}";

        hp_Slider.DOKill();

        hp_Slider.direction = Slider.Direction.RightToLeft;
        hp_Slider.DOValue(targetValue, hpTweenDuration);

        if (curHp <= 0)
        {
            UpdatePublisher.DiscribeObserver(this);
            gameObject.SetActive(false);

        }
    }

    public void ObserverUpdate(float dt)
    {
        gameObject.SetActive(true);
        rectTransform.position = Camera.main.WorldToScreenPoint(statusPos.position + posOffset);
    }

    private void OnDestroy()
    {
        UpdatePublisher.DiscribeObserver(this);
    }
}
