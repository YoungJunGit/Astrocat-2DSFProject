using DataEnum;
using UnityEngine;

public class ElementIcon : IconView<ELEMENT_TYPE>
{
    [SerializeField]
    private ElementIconContainer _iconContainer;
    protected override IconContainer<ELEMENT_TYPE> IconContainer => _iconContainer;
}
