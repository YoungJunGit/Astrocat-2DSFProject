using DataEntity;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class PlayerHUD : BaseHUD
{
    [Header("AP")]
    [SerializeField] private TMP_Text apText;
    [SerializeField] private GameObject AP_Panel;
    [SerializeField] private Color ActivateColor;
    [SerializeField] private Color DeactivateColor;

    private Image[] AP_BoxList;

    [Space(10f)]
    [SerializeField] private Image statusBox;
    [SerializeField] private TMP_Text unitName;
    [SerializeField] private Color DieColor;

    public override void Initialize(BaseUnit unit)
    {
        unitName.text = unit.GetStat().GetData().Name;
        AP_BoxList = AP_Panel.GetComponentsInChildren<Image>();

        unit.GetStat().OnHPChanged += OnHPChanged;
        unit.GetStat().OnAPChanged += OnAPChanged;
        unit.GetStat().OnDie += OnDied;
    }

    public override void OnHPChanged(float curHp, float maxHp)
    {
        hp_Slider.value = curHp / maxHp;
        hp_Text.text = $"{curHp}/{maxHp}";
    }

    public override void OnDied()
    {
        statusBox.color = DieColor;
    }

    public void OnAPChanged(int curAp, int maxAp)
    {
        foreach (var box in AP_BoxList.Select((value, index) => (value, index)))
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

        apText.text = $"{curAp}/{maxAp}";
    }
}
