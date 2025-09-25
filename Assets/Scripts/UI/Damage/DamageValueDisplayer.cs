using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface IDamageValueDisplayer
{
    void Display(float value, Bounds bounds, DamageContainer container);
}
public abstract class DamageValueDisplayer : IDamageValueDisplayer
{
    protected GameObject valueObject;
    protected List<GameObject> valueList = new();

    public void Display(float value, Bounds bounds, DamageContainer container)
    {
        Vector2 spawnBounds = new Vector2(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range((bounds.center.y + bounds.max.y) / 2, bounds.max.y)
            );
        valueObject = Object.Instantiate(container.Value_Prefab, spawnBounds, Quaternion.identity);

        string stringValue = value.ToString();
        foreach (char c in stringValue)
        {
            int digit = c - '0';

            GameObject digit_object = Object.Instantiate(container.Digit_Prefab, valueObject.transform);
            valueList.Add(digit_object);
            Image value_image = digit_object.GetComponent<Image>();
            value_image.sprite = container.Damage_Sprites[digit];
            value_image.SetNativeSize();
        }

        StartAnimation(container);
    }

    public abstract void StartAnimation(DamageContainer container);
}

public class DamageValueDisplayer_Bounce : DamageValueDisplayer
{
    public override void StartAnimation(DamageContainer container)
    {
        float posY = valueObject.GetComponent<RectTransform>().anchoredPosition.y;
        valueObject.GetComponent<RectTransform>().DOAnchorPosY(posY + container.JumpValue, 0.5f).SetEase(container.JumpEase);
        valueObject.GetComponent<CanvasGroup>().DOFade(0f, 1f).SetEase(container.FadeEase).OnComplete(() => { Object.Destroy(valueObject); });
    }
}