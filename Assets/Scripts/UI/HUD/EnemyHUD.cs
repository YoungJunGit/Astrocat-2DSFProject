using Cysharp.Threading.Tasks;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class EnemyHUD : BaseHUD, IUpdateObserver
{
    [Space(10f)]
    [SerializeField] private Vector3 posOffset;
    private RectTransform rectTransform;
    private Transform statusPos;

    [HideInInspector] public Vector3 spawnPos;

    public override void Initialize(BaseUnit unit)
    {
        rectTransform = GetComponent<RectTransform>();
        unit.GetStat().OnHPChanged += OnHPChanged;
    }

    public void AttachHUD(Transform statusPos)
    {
        this.statusPos = statusPos;
        UpdatePublisher.SubscribeObserver(this);
    }

    public override void OnHPChanged(float curHp, float maxHp)
    {
        hp_Slider.value = curHp / maxHp;
        hp_Text.text = $"{curHp}/{maxHp}";

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
}
