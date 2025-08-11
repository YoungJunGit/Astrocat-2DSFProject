using System;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimationEventHandler", menuName = "Core/AnimationEventHandler", order = 1)]
public class AnimationEventHandler : ScriptableObject
{
    public event Action attack;
    public event Action move;

    public void AttackEvent()
    {
        attack?.Invoke();
        attack = null;
    }

    public void MoveEvent()
    {
        move?.Invoke();
        move = null;
    }    
}
