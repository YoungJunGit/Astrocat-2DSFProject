using DataEnum;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class ElementIcon : BaseIcon<ELEMENT_TYPE>
{
    protected override ELEMENT_TYPE[] CountableIconList { get; } = new ELEMENT_TYPE[]
    {
        ELEMENT_TYPE.PHYSICAL,
        ELEMENT_TYPE.VOID,
        ELEMENT_TYPE.HOLY
    };
}
