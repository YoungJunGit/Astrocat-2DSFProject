using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class EnemyHUD : BaseHUD, IUpdateObserver
{
    [Space(10f)]
    [SerializeField] private Vector3 statusPosOffset;
    [SerializeField] private Vector3 effectBoxPosOffset;
    private RectTransform _statusRectTransform;
    private Transform _statusPos;
    private Transform _buffBoxPos;

    [HideInInspector] public Vector3 spawnPos;

    protected override void UpdateIconBoxSize<T>(List<T> iconList)
    {

    }

    public override void Initialize(BaseUnit unit)
    {
        base.Initialize(unit);

        _statusRectTransform = GetComponent<RectTransform>();
        unit.GetStat().OnHPChanged += OnHPChanged;
    }

    public void AttachHUD(Transform statusPos, Transform buffBoxPos)
    {
        _statusPos = statusPos;
        _buffBoxPos = buffBoxPos;
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
        _statusRectTransform.position = Camera.main.WorldToScreenPoint(_statusPos.position + statusPosOffset);
        _effectBoxRectTransform.position = Camera.main.WorldToScreenPoint(_buffBoxPos.position + effectBoxPosOffset);
    }

    private void OnDestroy()
    {
        UpdatePublisher.DiscribeObserver(this);
    }
}
