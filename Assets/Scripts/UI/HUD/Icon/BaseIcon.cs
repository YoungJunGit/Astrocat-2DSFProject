using NaughtyAttributes;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseIcon<T> : MonoBehaviour
{
    [SerializeField, Expandable] 
    IconContainerBase iconContainerBase;

    private IconContainer<T> iconContainer => iconContainerBase as IconContainer<T>;

    [SerializeField] protected TMP_Text count;
    [SerializeField] protected Image iconImg;

    private T iconType;
    public T IconType => iconType;

    public void Init(T type, int count)
    {
        iconType = type;
        
        if(iconContainer.IconDic.TryGetValue(type, out var sprite))
            iconImg.sprite = sprite;
    }

    public void UpdateIcon(int count)
    {
        this.count.text = IsCountable ? count.ToString() : "";
    }

    private void OnEnable()
    {
        RectTransform iconRect = GetComponent<RectTransform>();
        RectTransform textRect = count.GetComponent<RectTransform>();

        iconRect.sizeDelta = new Vector2(iconContainer.IconSize.x, iconContainer.IconSize.y);
        textRect.offsetMax = new Vector2(-iconContainer.Offset.OffsetMax.x, -iconContainer.Offset.OffsetMax.y);
        textRect.offsetMin = new Vector2(iconContainer.Offset.OffsetMin.x, iconContainer.Offset.OffsetMin.y);
    }

    private bool IsCountable => CountableIconList.Contains(iconType);
    protected abstract T[] CountableIconList { get; }
}
