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
        var addStream = unit.crowdControlUnit.EffectDictionary.Select(kv => kv.Value.ObserveAdd().Select(_ => new { Element = kv.Key, List = kv.Value})).Merge();
        var removeStream = unit.crowdControlUnit.EffectDictionary.Select(kv => kv.Value.ObserveRemove().Select(_ => new { Element = kv.Key, List = kv.Value})).Merge();

        addStream.Subscribe(value =>
            {
                var icon = _ccIconList.Find(e => e.IconType == value.Element);
                // Update Exist Icon
                if (icon != null)
                {
                    
                }
                // Create Icon
                else
                {
                    icon = CreateIcon(_ccIconList, _crowdControlRectTransform);
                    icon.Init(value.Element, value.List.Count);
                    icon.enabled = true;
                }
            }
        );

        removeStream.Subscribe(value =>
            {
                if(value.List.Count == 0)
                {
                    var icon = _ccIconList.Find(e => e.IconType == value.Element);
                    if (icon != null)
                    {
                        _ccIconList.Remove(icon);
                        Destroy(icon.gameObject);
                    }
                }
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
