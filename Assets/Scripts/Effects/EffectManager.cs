using AYellowpaper.SerializedCollections;
using Cysharp.Threading.Tasks;
using UnityEngine;

public interface IEffectManager
{
    public void PlayEffect(string name, Transform parent);
    public UniTask PlayEffectAsync(string name, Transform parent);
}

[CreateAssetMenu(fileName = "EffectManager", menuName = "Effect/EffectManager", order = 1)]
public class EffectManager : ScriptableObject , IEffectManager
{
    [SerializedDictionary("Effect Name", "Effect")]
    public SerializedDictionary<string, BaseEffect> effectDic;

    public void PlayEffect(string name, Transform parent)
    {
        BaseEffect effect = null;
        if (effectDic.TryGetValue(name, out var obj))
        {
            effect = Instantiate(obj, parent, false);
            effect.PlayEffect().Forget();
        }
    }

    public async UniTask PlayEffectAsync(string name, Transform parent)
    {
        BaseEffect effect = null;
        if(effectDic.TryGetValue(name, out var obj))
        {
            effect = Instantiate(obj, parent, false);
            await effect.PlayEffect();
        }
    }
}