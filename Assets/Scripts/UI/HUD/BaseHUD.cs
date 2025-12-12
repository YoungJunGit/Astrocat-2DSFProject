using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using ObservableCollections;
using R3;
using System.Collections.Generic;
using DataEnum;

public abstract class BaseHUD : MonoBehaviour
{
    [Header("RectTransforms")]
    [SerializeField] protected RectTransform _buffBoxRectTransform;
    [SerializeField] protected RectTransform _crowdControlRectTransform;

    [Header("Icons")]
    [SerializeField] protected GameObject statusIconPrefab;

    [Header("HP"), Space(10f)]
    [SerializeField] protected TMP_Text hp_Text;
    [SerializeField] protected Slider hp_Slider;
    [SerializeField] protected float hpTweenDuration = 0.5f;

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
                    icon = CreateIcon(_ccIconList, _crowdControlRectTransform);
                    icon.Init(value.Element, value.Count.CurrentValue);
                    icon.enabled = true;
                    return;
                }

                // 갱신 (이게 핵심)
                icon.UpdateIcon(value.Count.CurrentValue);
            }
        );
    }

    private T CreateIcon<T>(List<T> iconList, Transform parent)
    {
        var icon = Instantiate(statusIconPrefab, parent, false).GetComponent<T>();
        iconList.Add(icon);
        UpdateIconBoxSize(iconList);

        return icon;
    }
    protected abstract void UpdateIconBoxSize<T>(List<T> iconList);
    public abstract void OnHPChanged(float curHp, float maxHp);
}
