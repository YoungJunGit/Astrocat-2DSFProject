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
    [SerializeField] private RectTransform _statusRectTransform;
    [SerializeField] private RectTransform _effectBoxRectTransform;
    [SerializeField] private Vector3 statusPosOffset;
    [SerializeField] private Vector3 effectBoxPosOffset;

    protected override RectTransform effectBoxRectTransform => _effectBoxRectTransform;
    private Transform _statusPos;
    private Transform _buffBoxPos;

    public override void Initialize(BaseUnit unit)
    {
        base.Initialize(unit);
    }

    public void AttachHUD(Transform statusPos, Transform buffBoxPos)
    {
        _statusPos = statusPos;
        _buffBoxPos = buffBoxPos;
        UpdatePublisher.SubscribeObserver(this);
    }
    
    public void ObserverUpdate(float dt)
    {
        gameObject.SetActive(true);
        _statusRectTransform.position = _statusPos.position + statusPosOffset;
        _effectBoxRectTransform.position = _buffBoxPos.position + effectBoxPosOffset;
    }

    private void OnDestroy()
    {
        UpdatePublisher.DiscribeObserver(this);
    }

    public override void OnFinishedDying(BaseUnit me)
    {
        UpdatePublisher.DiscribeObserver(this);
        gameObject.SetActive(false);
    }
}
