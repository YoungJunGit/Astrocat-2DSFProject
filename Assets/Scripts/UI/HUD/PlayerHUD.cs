using DataEntity;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using DG.Tweening;

public class PlayerHUD : BaseHUD
{
    [Header("AP")]
    [SerializeField] private TMP_Text ap_Text;
    [SerializeField] private GameObject ap_Panel;
    [SerializeField] private Color ActivateColor;
    [SerializeField] private Color DeactivateColor;

    private Image[] ap_BoxList;

    [Space(10f)]
    [SerializeField] private Image statusBox;
    [SerializeField] private TMP_Text unitName;
    [SerializeField] private Color DieColor;

    [Header("HP Tween")]
    [SerializeField] private float hpTweenDuration = 0.5f;

    public override void Initialize(BaseUnit unit)
    {
        unitName.text = unit.GetStat().GetData().Name;
        ap_BoxList = ap_Panel.GetComponentsInChildren<Image>();

        unit.GetStat().OnHPChanged += OnHPChanged;
        unit.GetStat().OnAPChanged += OnAPChanged;
    }

    public override void OnHPChanged(float curHp, float maxHp)
    {
        float targetValue= curHp / maxHp;
        hp_Text.text = $"{curHp}/{maxHp}";

        hp_Slider.DOKill();

        hp_Slider.direction = Slider.Direction.RightToLeft;
        hp_Slider.DOValue(targetValue, hpTweenDuration);

        if (curHp <= 0)
        {
            statusBox.color = DieColor;
        }
        else
            statusBox.color = Color.white;
    }

    public void OnAPChanged(int curAp, int maxAp)
    {
        foreach (var box in ap_BoxList.Select((value, index) => (value, index)))
        {
            if (box.index < curAp)
            {
                box.value.color = ActivateColor;
            }
            else
            {
                box.value.color = DeactivateColor;
            }
        }

        ap_Text.text = $"{curAp}/{maxAp}";
    }
}
