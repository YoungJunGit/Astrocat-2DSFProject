using Cysharp.Threading.Tasks;
using System;

public class PlayerUnit : BaseUnit
{
    public override void OnFinshedDeath(Action done)
    {
        gameObject.SetActive(false);
        m_FinishedDying.Invoke(this);
        done();
    }

    public override void PlayDeathSound()
    {
        _soundService.PlayEffectSound("Die");
        _soundService.PlayEffectSound("Hover", 2f);
    }
}
