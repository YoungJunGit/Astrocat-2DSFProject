using UnityEngine;
using ObservableCollections;
using System.Collections.Generic;
using R3;
using System.Linq;

public class CodeTester : MonoBehaviour
{
    public ObservableList<int> list = new ObservableList<int>();

    void Start()
    {
        list.ObserveRemove()
            .Subscribe(x => Debug.Log($"Removed: {x.Value}"))
            .AddTo(this);

        for (int i = 0; i < 10; i++)
        {
            list.Add(i);
        }
        var itemsToRemove = list.Where(item => item % 2 == 0).ToList();
        foreach(var item in itemsToRemove)
        {
            list.Remove(item);
        }
    }
}
