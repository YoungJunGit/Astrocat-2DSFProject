using DataEnum;
using UnityEngine;

public interface IEffectable
{
    public void Apply();
}

public interface IEachTurn
{
    public void OnEachTurn();
}

public abstract class AttributeEffect : IEffectable
{
    public abstract BUFF_TYPE Buff_Type { get; }
    public abstract void Apply();
}

public abstract class ControlEffect : IEffectable
{
    public abstract ELEMENT_TYPE Element_Type { get; }
    public abstract void Apply();
}