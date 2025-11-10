using UnityEngine;

public interface IEffectable
{
    public void Apply();
}

public abstract class AttributeEffect : IEffectable
{
    public abstract void Apply();
}

public abstract class ControlEffect : IEffectable
{
    public abstract void Apply();
}