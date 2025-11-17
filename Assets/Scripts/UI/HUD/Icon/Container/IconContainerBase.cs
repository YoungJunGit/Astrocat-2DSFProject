using System;
using UnityEngine;

public class IconContainerBase : ScriptableObject
{
    [Serializable]
    public class CustomOffset
    {
        public Vector2 OffsetMax;
        public Vector2 OffsetMin;
    }

    public Vector2 IconSize;
    public CustomOffset Offset;
}
