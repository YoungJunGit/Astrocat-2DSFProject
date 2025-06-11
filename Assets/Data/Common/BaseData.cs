using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 건들지 말 것
/// </summary>
/// <typeparam name="T"></typeparam>
public class BaseData<T> : ScriptableObject
{
    public TextAsset json;
    public bool decryptToggle = false;
    public List<T> data;
}
