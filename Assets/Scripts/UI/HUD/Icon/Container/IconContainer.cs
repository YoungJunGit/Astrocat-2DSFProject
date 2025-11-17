using AYellowpaper.SerializedCollections;
using DataEnum;
using System;
using UnityEngine;

public class IconContainer<T> : IconContainerBase
{
    [SerializedDictionary("Icon Type", "Sprite")]
    public SerializedDictionary<T, Sprite> IconDic;
}