using UnityEngine;

public enum AttachType
{
    SpriteRenderer,
    HitBox,
    InjectionBox,
    MeleeHitPos,
    UnitSelectArrowPos,
    StatusBoxPos,
    BuffBoxPos,
    BulletSpawnPos,
    DroneInjectionFiringPos,
}

public class UnitAttachment : MonoBehaviour
{
    public AttachType Type;
}