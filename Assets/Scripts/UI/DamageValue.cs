using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using DG.Tweening;

public class DamageValue : MonoBehaviour
{
    [SerializeField] private GameObject value_prefab;
    [SerializeField] private Sprite[] normal_damage_sprites;
    [SerializeField] private Sprite[] critical_damage_sprites;

    [SerializeField] private bool isCritical; // tmp
    [SerializeField] private double value; // tmp
    [SerializeField] private AnimationCurve jumpEase;
    [SerializeField] private AnimationCurve fadeEase;
    [SerializeField] private float jumpValue;

    public void SetValue(double value, bool isCritical)
    {
        List<GameObject> valueList = new List<GameObject>();
        
        string stringValue = value.ToString();
        foreach(char c in stringValue)
        {
            int digit = c - '0';

            GameObject value_object = Instantiate(value_prefab, this.transform);
            valueList.Add(value_object);
            Image value_image = value_object.GetComponent<Image>();
            value_image.sprite = isCritical ? critical_damage_sprites[digit] : normal_damage_sprites[digit];
            value_image.SetNativeSize();
        }

        foreach(GameObject value_object in valueList)
        {
            value_object.transform.DOLocalMoveY(jumpValue, 0.5f).SetEase(jumpEase);
            value_object.GetComponent<Image>().DOFade(0f, 1f).SetEase(fadeEase);
        }
    }
}
