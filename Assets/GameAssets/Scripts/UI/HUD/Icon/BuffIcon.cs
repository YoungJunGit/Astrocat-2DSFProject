using DataEnum;
using UnityEngine;

public class BuffIcon : IconView<BUFF_TYPE>
{
    [SerializeField]
    private BuffIconContainer _iconContainer;
    protected override IconContainer<BUFF_TYPE> IconContainer => _iconContainer;
}
