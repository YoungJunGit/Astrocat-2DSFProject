using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseHUD : MonoBehaviour
{
    protected BaseUnit ControlledUnit;

    [Header("HP")]
    [SerializeField] protected TMP_Text hp_Text;
    [SerializeField] protected Slider hp_Slider;

    public abstract void Initialize(BaseUnit unit);
    public abstract void OnHPChanged();
    public abstract void OnDied();
}
