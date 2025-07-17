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
        ControlledUnit = unit;
        unitName.text = unit.GetStat().GetData().Name;
        AP_BoxList = AP_Panel.GetComponentsInChildren<Image>();

        ControlledUnit.GetStat().m_HPChanged += OnHPChanged;
        ControlledUnit.GetStat().m_APChanged += OnAPChanged;
        ControlledUnit.GetStat().m_Die += OnDied;
    }

    public override void OnHPChanged()
    {
        hp_Slider.value = ControlledUnit.GetStat().Cur_HP / ControlledUnit.GetStat().Max_HP;
        hp_Text.text = ControlledUnit.GetStat().Cur_HP + " / " + ControlledUnit.GetStat().Max_HP;
    }

    public override void OnDied()
    {
        statusBox.color = DieColor;
    }

    public void OnAPChanged()
    {
        foreach (var box in AP_BoxList.Select((value, index) => (value, index)))
        {
            if (box.index < ControlledUnit.GetStat().Cur_AP)
            {
                box.value.color = ActivateColor;
            }
            else
            {
                box.value.color = DeactivateColor;
            }
        }

        apText.text = ControlledUnit.GetStat().Cur_AP + " / " + ControlledUnit.GetStat().Max_AP;
    }
}
