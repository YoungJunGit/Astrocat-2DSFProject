using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

public interface IValueDisplayInvoker
{
    void Invoke(IValueDisplayer displayer, float value, Bounds bounds, ValueContainer container);
}

public class ValueDisplayInvoker : IValueDisplayInvoker
{
    public void Invoke(IValueDisplayer displayer, float value, Bounds bounds, ValueContainer container)
    {
        displayer.Display(value, bounds, container);
    }
}
