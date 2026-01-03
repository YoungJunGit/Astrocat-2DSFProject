using UnityEngine;

public static class AnimHash
{
    public static readonly int Idle = Animator.StringToHash("Base Layer.Idle");
    public static readonly int Attack = Animator.StringToHash("Base Layer.Attack");
    public static readonly int Hit = Animator.StringToHash("Base Layer.Hit");
    public static readonly int Death = Animator.StringToHash("Base Layer.Death");
    public static readonly int Move = Animator.StringToHash("Base Layer.Move");
    public static readonly int Retreat = Animator.StringToHash("Base Layer.Retreat");
}
