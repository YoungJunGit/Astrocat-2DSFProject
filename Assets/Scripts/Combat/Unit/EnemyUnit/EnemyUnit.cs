using DG.Tweening;
using Cysharp.Threading.Tasks;
using System;

public class EnemyUnit : BaseUnit
{
    public override void OnFinshedDeath(Action done)
    {
        Attachments.GetSpriteRenderer().DOFade(0, 0.5f).OnComplete(() =>
        {
            gameObject.SetActive(false);
            m_FinishedDying.Invoke(this);
            done();
        });
    }

    public override void PlayDeathSound()
    {
        _soundService.PlayEffectSound("Die");
    }
}
