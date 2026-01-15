public class HealFactory
{
    public static float CreateHeal<T>(HealContext context) where T : IHealCalulator, new()
    {
        IHealCalulator calculator = new T();

        return calculator.Calculate(context); ;
    }
}