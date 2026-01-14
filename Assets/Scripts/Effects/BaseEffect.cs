using System;
using UnityEngine;
using Cysharp.Threading.Tasks;

public abstract class BaseEffect : MonoBehaviour
{
    public abstract UniTask PlayEffect();
}