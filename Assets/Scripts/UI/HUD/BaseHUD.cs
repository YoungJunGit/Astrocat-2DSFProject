using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using ObservableCollections;
using R3;
using System.Collections.Generic;
using DataEnum;
using DG.Tweening;
using System;

public abstract class BaseHUD : MonoBehaviour
{
    [Header("RectTransforms")]
    [SerializeField] protected RectTransform _effectBoxRectTransform;

    [Header("Icons")]
    [SerializeField] private GameObject CCIconPrefab;
    [SerializeField] private GameObject BuffIconPrefab;

    [Header("HP"), Space(10f)]
    [SerializeField] private TMP_Text hp_Text;
    [SerializeField] private Image hp_Image;
    [SerializeField] private float hpTweenDuration = 0.5f;

    private List<ElementIcon> _ccIconList = new();
    private List<BuffIcon> _buffIconList = new();

    public virtual void Initialize(BaseUnit unit)
    {
        var stream = unit.CCUnit.EffectsCountDic.Select(kv => kv.Value.Select(_ => new { Element = kv.Key, Count = kv.Value })).Merge();

        stream.Subscribe(value =>
            {
                var icon = _ccIconList.Find(e => e.IconType == value.Element);

                // 삭제
                if (value.Count.CurrentValue <= 0)
                {
                    if (icon != null)
                    {
                        _ccIconList.Remove(icon);
                        Destroy(icon.gameObject);
                    }
                    return;
                }

                // 생성
                if (icon == null)
                {
                    icon = CreateIcon(_ccIconList, _effectBoxRectTransform);
                    icon.Init(value.Element, value.Count.CurrentValue);
                    icon.enabled = true;
                    return;
                }

                // 갱신 (이게 핵심)
                icon.UpdateIcon(value.Count.CurrentValue);
            }
        );
    }

    public virtual void OnHPChanged(float curHp, float maxHp)
    {
        float targetValue = curHp / maxHp;
        hp_Text.text = $"{curHp}/{maxHp}";

        hp_Image.DOKill();
        hp_Image.DOFillAmount(targetValue, hpTweenDuration);
    }

    private T CreateIcon<T>(List<T> iconList, Transform parent)
    {
        var objectToInstantiate = typeof(T) == typeof(ELEMENT_TYPE) ? CCIconPrefab : BuffIconPrefab;
        var icon = Instantiate(objectToInstantiate, parent, false).GetComponent<T>();
        iconList.Add(icon);

        return icon;
    }
}
