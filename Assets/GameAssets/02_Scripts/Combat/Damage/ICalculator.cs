public interface ICalculator<in TContext, out TResult> where TContext : CalculationContext
{
    TResult Calculate(TContext context);
}