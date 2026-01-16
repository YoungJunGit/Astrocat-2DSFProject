public sealed record DamageResult(float DamageValue, bool IsCritical);

public class DamageFactory
{
    public static DamageResult CreateDamage<T>(DamageContext context) where T : IDamageCalculator, new()
    {
        IDamageCalculator calculator = new T();

        return calculator.Calculate(context); ;
    }
}