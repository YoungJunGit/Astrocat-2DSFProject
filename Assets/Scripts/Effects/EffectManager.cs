using AYellowpaper.SerializedCollections;
using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;

public interface IEffectManager
{
    public void PlayEffect(string name, Transform parent);
    public UniTask<BaseEffect> PlayEffectAsync(string name, Transform parent);
    public void PlayHitEffect(Material material, string ID);
}

[CreateAssetMenu(fileName = "EffectManager", menuName = "Effect/EffectManager", order = 1)]
public class EffectManager : ScriptableObject , IEffectManager
{
    [SerializedDictionary("Effect Name", "Effect")]
    public SerializedDictionary<string, BaseEffect> effectDic;

    [SerializeField] private float hitEffectBlendAmount = 0.75f;
    [SerializeField] private float hitEffectBlendDuration = 0.25f;

    private const string HIT_BLEND_TWEEN_ID = "HIT_BLEND_";

    public void PlayEffect(string name, Transform tr)
    {
        BaseEffect effect = null;
        if (effectDic.TryGetValue(name, out var obj))
        {
            effect = Instantiate(obj, tr.position, tr.rotation);
            effect.PlayEffect().Forget();
        }
    }

    public async UniTask<BaseEffect> PlayEffectAsync(string name, Transform tr)
    {
        BaseEffect effect = null;
        if(effectDic.TryGetValue(name, out var obj))
        {
            effect = Instantiate(obj, tr.position, tr.rotation);
            return await effect.PlayEffect();
        }
        return null;
    }

    public void PlayHitEffect(Material material, string ID)
    {
        DOTween.Kill(HIT_BLEND_TWEEN_ID + ID);
        material.SetFloat("_HitEffectBlend", hitEffectBlendAmount);
        material.DOFloat(0.0f, "_HitEffectBlend", hitEffectBlendDuration)
            .SetEase(Ease.Linear)
            .SetId(HIT_BLEND_TWEEN_ID + ID);
    }
}