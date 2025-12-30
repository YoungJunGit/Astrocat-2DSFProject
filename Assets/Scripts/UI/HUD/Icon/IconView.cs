using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class IconView<T> : MonoBehaviour
{
    [SerializeField] protected TMP_Text count;
    [SerializeField] protected Image iconImg;

    private IconViewModel<T> _iconViewModel;
    public T IconType => _iconViewModel.IconType;

    public void Init(T type, int count)
    {
        _iconViewModel = new IconViewModel<T>(type, count);

        if (IconContainer.IconDic.TryGetValue(type, out var sprite))
            iconImg.sprite = sprite;

        UpdateIcon(count);
    }

    public void UpdateIcon(int count)
    {
        this.count.text = IsCountable ? count.ToString() : "";
    }

    private bool IsCountable => IconContainer.CountableIconList.Contains(_iconViewModel.IconType);
    protected abstract IconContainer<T> IconContainer { get; }
}