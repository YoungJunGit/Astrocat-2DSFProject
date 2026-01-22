using AYellowpaper.SerializedCollections;
using UnityEngine;

public abstract class IconContainer<T> : ScriptableObject
{
    [SerializedDictionary("Icon Type", "Sprite")]
    public SerializedDictionary<T, Sprite> IconDic;
    public T[] CountableIconList;
}