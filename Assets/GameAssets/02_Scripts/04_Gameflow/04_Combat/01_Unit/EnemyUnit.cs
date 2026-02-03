using DG.Tweening;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class EnemyUnit : BaseUnit
{
    protected override async UniTask OnFinshedDeathAnim()
    {
        await Attachments.Get<SpriteRenderer>(AttachType.SpriteRenderer).material.DOFloat(0.0f, "_Alpha", 0.5f).OnComplete(() =>
        {
            gameObject.SetActive(false);
            m_FinishedDying.Invoke(this);
        }).ToUniTask();
    }
}
