using UnityEngine;

namespace DataHashAnim
{
    public class AnimCombat
    {
        readonly static public int IDLE = Animator.StringToHash("Idle");
        readonly static public int ATTACK = Animator.StringToHash("Attack");
        readonly static public int MOVE = Animator.StringToHash("Move");
        readonly static public int RETREAT = Animator.StringToHash("Retreat");
        readonly static public int HIT = Animator.StringToHash("Hit");
    }
}
