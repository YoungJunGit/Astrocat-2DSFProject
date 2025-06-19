using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 건들지 말 것
/// </summary>
/// <typeparam name="T"></typeparam>
public class BaseData<T> : ScriptableObject
{
    [SerializeField]
    private TextAsset json;

    [SerializeField]
    private bool decryptToggle;

    public TextAsset Json
    {
        get { return json; }
    }

    public bool DecryptToggle
    {
        get {  return decryptToggle; }
    }

    public List<T> data;
}
