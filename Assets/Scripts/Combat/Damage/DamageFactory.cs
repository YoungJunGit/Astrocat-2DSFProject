using NaughtyAttributes;
using UnityEngine;
using Utils;

public interface IDamage
{
    float Value { get; }
    bool IsCritical { get; }
}
public record Damage(float Value, bool IsCritical) : IDamage
{
    public float Value { get; } = Value;
    public bool IsCritical { get; } = IsCritical;
}

[CreateAssetMenu(fileName = "DamageFactory", menuName = "GameScene/DamageFactory")]
public class DamageFactory : ScriptableObject
{
    public static IDamage CreateNormalDamage<T>(BaseUnit caster, BaseUnit target) where T : ICalculator, new()
    {
        ICalculator calculator = new T();

        IDamage result = calculator.Calculate(caster, target);

        return result;
    }
}
