using System.Collections.Generic;
using UnityEngine;
using ObservableCollections;
using DataEnum;

public interface IQuery { }
public class Query<T> : IQuery
{
    public readonly BUFF_TYPE BuffType;
    public T Value;

    public Query(BUFF_TYPE BuffType, T Value)
    {
        this.BuffType = BuffType;
        this.Value = Value;
    }
}

public class StatsMediator
{
    private readonly List<StatModifier> modifiers = new List<StatModifier>();

    public event System.EventHandler<IQuery> Queries;
    public void PerformQuery<T>(object sender, Query<T> query) => Queries?.Invoke(sender, query);

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