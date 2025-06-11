using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// �ǵ��� �� ��
/// </summary>
/// <typeparam name="T"></typeparam>
public class BaseData<T> : ScriptableObject
{
    public TextAsset json;
    public bool decryptToggle = false;
    public List<T> data;
}
