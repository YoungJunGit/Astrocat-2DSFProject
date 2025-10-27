using UnityEngine;

public interface IEffectable
{
    public void ApplyEffect();
    public void Dispose();
}

public interface IEveryTurnEffect
{
    public void ApplyEveryTurn();
}

public abstract class UnitEffect : IEffectable
{
    public abstract int TurnCount { get; }
    public abstract void ApplyEffect();
    public abstract void Dispose();
}