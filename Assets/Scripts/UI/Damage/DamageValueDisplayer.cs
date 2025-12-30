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

        string stringValue = ((int)value).ToString();
        foreach (char c in stringValue)
        {
            if(!char.IsDigit(c))
                throw new System.Exception("DamageValue must include only digit!!!");

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

public class DamageValueDisplayer_BounceVer1 : DamageValueDisplayer
{
    public override void StartAnimation(DamageContainer container)
    {
        float posY = valueObject.GetComponent<RectTransform>().anchoredPosition.y;

        valueObject.GetComponent<RectTransform>()
            .DOAnchorPosY(posY + container.JumpValue, 0.5f)
            .SetEase(container.JumpEase);

        valueObject.GetComponent<CanvasGroup>()
            .DOFade(0f, 1f)
            .SetEase(container.FadeEase)
            .OnComplete(() => { Object.Destroy(valueObject); });
    }
}

public class DamageValueDisplayer_BounceVer2 : DamageValueDisplayer
{
    public override void StartAnimation(DamageContainer container)
    {
        int Max = valueList.Count - 1;
        for(int i = Max; i >= 0; i--)
        {
            float posY = valueList[Max - i].GetComponent<RectTransform>().anchoredPosition.y;

            valueList[Max - i].GetComponent<RectTransform>()
                .DOAnchorPosY(posY + container.JumpValue, 0.5f)
                .SetEase(container.JumpEase)
                .Goto((0.5f / valueList.Count / 5) * i, true);
        }

        valueObject.GetComponent<CanvasGroup>()
            .DOFade(0f, 1f)
            .SetEase(container.FadeEase)
            .OnComplete(() => { Object.Destroy(valueObject); });
    }
}