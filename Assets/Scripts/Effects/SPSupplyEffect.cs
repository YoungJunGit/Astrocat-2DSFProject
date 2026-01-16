using Cysharp.Threading.Tasks;
using UnityEngine;

public class SPSupplyEffect : BaseEffect
{
    public override async UniTask<BaseEffect> PlayEffect()
    {
        return this;
    }
}