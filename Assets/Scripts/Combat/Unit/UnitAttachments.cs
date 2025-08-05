using UnityEngine;
using NaughtyAttributes;

public class UnitAttachments : MonoBehaviour
{
    [SerializeField, ShowIf("IsUnit"), Required] 
    private Transform UnitSelectArrowPos;
    [SerializeField, ShowIf("IsUnit"), Required]
    private Transform HitBoxPos;
    [SerializeField, ShowIf("IsEnemy"), Required] 
    private Transform StatusPos;
    [SerializeField, ShowIf("IsPlayer"), Required] 
    private Transform ActionSelectorPos;
    [SerializeField, ShowIf("IsRange"), Required] 
    private Transform BulletSpawnPos;

    public Transform GetStatusPosition() => StatusPos;
    public Transform GetHitBoxPosition() => HitBoxPos;
    public Transform GetUnitSelectArrowPos() => UnitSelectArrowPos;
    public Transform GetActionSelectorPos() => ActionSelectorPos;
    public Transform GetBulletSpawnPos() => BulletSpawnPos;

    private bool IsUnit     => GetComponent<BaseUnit>() != null;
    private bool IsEnemy    => GetComponent<BaseUnit>() is EnemyUnit;
    private bool IsPlayer   => GetComponent<BaseUnit>() is PlayerUnit;
    private bool IsRange    => GetComponent<BaseUnit>() is IRange;
}
