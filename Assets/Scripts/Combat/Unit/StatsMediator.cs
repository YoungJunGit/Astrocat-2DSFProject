using System.Collections.Generic;
using UnityEngine;
using ObservableCollections;
using DataEnum;

public interface IQuery { }
public class Query<T, TValue> : IQuery
{
    public readonly T BuffType;
    public TValue Value;

    public Query(T BuffType, TValue Value)
    {
        this.BuffType = BuffType;
        this.Value = Value;
    }
}

public class StatsMediator
{
    private readonly List<StatModifier> modifiers = new List<StatModifier>();

    public event System.EventHandler<IQuery> Queries;
    public void PerformQuery<T, TValue>(object sender, Query<T, TValue> query) => Queries?.Invoke(sender, query);

    public void AddModifier(StatModifier modifier)
    {
        modifiers.Add(modifier);
        Queries += modifier.Handle;

        modifier.OnDispose += _ =>
        {
            modifiers.Remove(modifier);
            Queries -= modifier.Handle;
        };
    }
}