using System.Threading;
using TMPro;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseHUD : MonoBehaviour
{
    [Header("HP")]
    [SerializeField] protected TMP_Text hp_Text;
    [SerializeField] protected Slider hp_Slider;

    public abstract void Initialize(BaseUnit unit);
    public abstract void OnHPChanged(float curHp, float maxHp);
    public abstract void OnDied(UnitStat stat);
}
