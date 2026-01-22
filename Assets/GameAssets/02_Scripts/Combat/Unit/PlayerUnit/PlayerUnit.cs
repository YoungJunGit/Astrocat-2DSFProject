using Cysharp.Threading.Tasks;
using System;

public class PlayerUnit : BaseUnit
{
    protected override async UniTask OnFinshedDeathAnim()
    {
        gameObject.SetActive(false);
        m_FinishedDying.Invoke(this);
    }

    protected override void PlayDeathSound()
    {
        _soundService.PlayEffectSound("Die");
    }
}
