using UnityEngine;
using System.Linq;

public interface ICombatEffectManager
{
    public IEffectable AddCombatEffect(string ID, EffectInfo info, EffectContext context);
    public IEffectable AddCrowdControlEffect(string ID, EffectInfo info, EffectContext context);
}

public record EffectInfo(string Name, float Value = 0.0f, int Duration = 0);
public record EffectContext(BaseUnit Target, BaseUnit Caster);

[CreateAssetMenu(fileName = "CombatEffectManager", menuName = "SO/Combat/Manager/CombatEffectManager", order = 1)]
public class CombatEffectManager : ScriptableObject, ICombatEffectManager
{
    private ICombatTextManager _textManager;

    public void Init()
    {
        ServiceLocator.For(this).Get(out _textManager);
    }

    public IEffectable AddCombatEffect(string ID, EffectInfo info, EffectContext context)
    {
        var exist = context.Target.CEUnit.NormalEffectList.ToList().Find(e => e.EffectName == info.Name);
        if (exist != null && exist.Effect.Trait == EffectTrait.Duration)
        {
            exist.Effect.ResetTimerInternal();
            return exist.Effect;
        }

        IEffectable combatEffect = CreateCombatEffect(ID);
        if (combatEffect == null) return null;

        context.Target.CEUnit.Add(new EffectContainer(info.Name, combatEffect));
        _textManager.OnBuff(context.Target.Attachments.GetHitBox().bounds, info.Value);
        combatEffect?.Apply(info, context);

        return combatEffect;
    }

    public IEffectable AddCrowdControlEffect(string ID, EffectInfo info, EffectContext context)
    {
        IEffectable combatEffect = CreateCombatEffect(ID);
        if (combatEffect == null) return null;

        context.Target.CEUnit.Add(new EffectContainer(info.Name, combatEffect));
        combatEffect?.Apply(info, context);

        return combatEffect;
    }

    private IEffectable CreateCombatEffect(string ID)
    {
        var factory = EffectFactoryData.GetEffectFactory(ID);
        if (factory == null) return null;
        return factory.Invoke();
    }
}