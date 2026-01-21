using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface IValueDisplayer
{
    void Display(float value, Bounds bounds, ValueContainer container);
}

public abstract class ValueDisplayer : IValueDisplayer
{
    protected DamageHealBuffValue value;
    protected List<GameObject> digitList = new();

    public void Display(float value, Bounds bounds, ValueContainer container)
    {
        Vector2 spawnBounds = new Vector2(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range((bounds.center.y + bounds.max.y) / 2, bounds.max.y)
            );
        this.value = Object.Instantiate(container.Value_Prefab, spawnBounds, Quaternion.identity);
        this.value.transform.localScale = container.ScaleValue;

        string stringValue = ((int)value).ToString();
        foreach (char c in stringValue)
        {
            if(!char.IsDigit(c))
                throw new System.Exception("DamageValue must include only digit!!!");

            int digit = c - '0';

            GameObject digit_object = Object.Instantiate(container.Digit_Prefab, this.value.transform);
            digitList.Add(digit_object);
            Image value_image = digit_object.GetComponent<Image>();
            value_image.sprite = container.Damage_Sprites[digit];
            value_image.SetNativeSize();
        }

        StartAnimation(container);
    }

    public abstract void StartAnimation(ValueContainer container);
}

public class DamageValueDisplayer_BounceVer1 : ValueDisplayer
{
    public override void StartAnimation(ValueContainer container)
    {
        float posY = value.rectTransform.anchoredPosition.y;

        value.rectTransform.DOAnchorPosY(posY + container.JumpValue, 0.5f)
            .SetEase(container.JumpEase);

        value.canvasGroup
            .DOFade(0f, 1f)
            .SetEase(container.FadeEase)
            .OnComplete(() => { Object.Destroy(value.gameObject); });
    }
}

public class DamageValueDisplayer_BounceVer2 : ValueDisplayer
{
    public override void StartAnimation(ValueContainer container)
    {
        int Max = digitList.Count - 1;
        for(int i = Max; i >= 0; i--)
        {
            var rt = digitList[Max - i].GetComponent<RectTransform>();
            float posY = rt.anchoredPosition.y;

            rt.DOAnchorPosY(posY + container.JumpValue, 0.5f)
                .SetEase(container.JumpEase)
                .Goto(0.5f / digitList.Count / 5 * i, true);
        }

        value.canvasGroup
            .DOFade(0f, 1f)
            .SetEase(container.FadeEase)
            .OnComplete(() => { Object.Destroy(value.gameObject); });
    }
}

public class HealValueDisplayer : ValueDisplayer
{
    public override void StartAnimation(ValueContainer container)
    {
        float posY = value.rectTransform.anchoredPosition.y;

        value.rectTransform.DOAnchorPosY(posY + 1f, 1f)
            .SetEase(Ease.Linear);

        value.canvasGroup
            .DOFade(0f, 1f)
            .SetEase(container.FadeEase)
            .OnComplete(() => { Object.Destroy(value.gameObject); });
    }
}

public class BuffValueDisplayer : ValueDisplayer
{
    public override void StartAnimation(ValueContainer container)
    {
        value.additive.SetActive(true);
        float posY = value.rectTransform.anchoredPosition.y;

        value.rectTransform.DOAnchorPosY(posY + 0.5f, 1f)
            .SetEase(Ease.Linear);

        value.canvasGroup
            .DOFade(0f, 1f)
            .SetEase(container.FadeEase)
            .OnComplete(() => { Object.Destroy(value.gameObject); });
    }
}