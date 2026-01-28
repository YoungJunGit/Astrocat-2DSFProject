using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "BulletSetting", menuName = "SO/Combat/Projectile/BulletSetting", order = 1)]
public class BulletSetting : ScriptableObject
{
    [SerializeField, Tag]
    private string targetTag;
    [SerializeField]
    private int speed;

    public string TargetTag => targetTag;
    public int Speed => speed;
}
