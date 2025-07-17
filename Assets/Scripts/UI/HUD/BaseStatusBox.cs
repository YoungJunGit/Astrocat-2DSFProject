using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BaseStatusBox : MonoBehaviour
{
    protected BaseUnit ControlledUnit;

    [Header("HP°ü·Ã")]
    public TMP_Text hpText;
    public Slider hpSlider;

    public virtual void Initialize(BaseUnit unit)
    {
        hpSlider.onValueChanged.AddListener(OnHpChanged);

        ControlledUnit = unit;
    }

    public virtual void OnDied()
    {

    }

    public virtual void OnAPChanged()
    {

    }

    private void OnHpChanged(float value)
    {
        if (value <= 0f)
            OnDied();
    }
}
