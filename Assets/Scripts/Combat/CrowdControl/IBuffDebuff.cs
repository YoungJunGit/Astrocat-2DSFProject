using UnityEngine;

public interface IBuffDebuff
{
    public void Apply();
}

public abstract class Buff : IBuffDebuff
{
    public abstract void Apply();
}

public abstract class Debuff : IBuffDebuff
{
    public abstract void Apply();
}