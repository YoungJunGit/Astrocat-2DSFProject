using DataEnum;
using NaughtyAttributes;
using UnityEngine;

public class BuffIcon : BaseIcon<BUFF_TYPE>
{
    protected override BUFF_TYPE[] CountableIconList { get; } = new BUFF_TYPE[]
    {
        // TODO : Add Countable Buffs
    };
}
