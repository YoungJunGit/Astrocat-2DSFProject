using UnityEngine;
using NaughtyAttributes;

public class UnitAttachments : MonoBehaviour
{
    [SerializeField, ShowIf("IsUnit"), Required] 
    private Transform UnitSelectArrowPos;
    [SerializeField, ShowIf("IsUnit"), Required]
    private Collider2D HitBox;
    [SerializeField, ShowIf("IsEnemy"), Required] 
    private Transform StatusPos;
    [SerializeField, ShowIf("IsPlayer"), Required] 
    private Transform ActionSelectorPos;
    [SerializeField, ShowIf("IsRange"), Required] 
    private Transform BulletSpawnPos;

    public Transform GetStatusPosition() => StatusPos;
    public Collider2D GetHitBox() => HitBox;
    public Transform GetUnitSelectArrowPos() => UnitSelectArrowPos;
    public Transform GetActionSelectorPos() => ActionSelectorPos;
    public Transform GetBulletSpawnPos() => BulletSpawnPos;

    private bool IsUnit     => GetComponent<BaseUnit>() != null;
    private bool IsEnemy    => IsUnit && GetComponent<BaseUnit>() is EnemyUnit;
    private bool IsPlayer   => IsUnit && GetComponent<BaseUnit>() is PlayerUnit;
    private bool IsRange    => IsUnit && GetComponent<BaseUnit>().GetUnitType() == DataEnum.UNIT_TYPE.RANGE;
}
