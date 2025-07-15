using DataEntity;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class PlayerStatusBox : BaseStatusBox
{
    [Header("AP°ü·Ã")]
    public TMP_Text apText;
    public GameObject AP_Panel;
    private Image[] AP_BoxList;
    public Color ActivateColor;
    public Color DeactivateColor;

    [Space(10f)]
    public Image statusBox;
    public TMP_Text unitName;
    public Color DieColor;

    public override void Initialize(BaseUnit unit)
    {
        base.Initialize(unit);
        unitName.text = ControlledUnit.MyData.Name;

        AP_BoxList = AP_Panel.GetComponentsInChildren<Image>();
    }

    public override void OnDied()
    {
        statusBox.color = DieColor;
    }

    public override void OnAPChanged()
    {
        foreach (var box in AP_BoxList.Select((value, index) => (value, index)))
        {
            if (box.index < ControlledUnit.CurAP)
            {
                box.value.color = ActivateColor;
            }
            else
            {
                box.value.color = DeactivateColor;
            }
        }

        apText.text = ControlledUnit.CurAP + " / " + ControlledUnit.MaxAP;
    }
}
