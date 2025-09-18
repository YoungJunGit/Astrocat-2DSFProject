using Cysharp.Threading.Tasks;
using System;
using DataEnum;
using DataHashAnim;
using UnityEngine;

public interface IUnitAction
{
    public UniTask Execute(IUnitActionContext context);
}
