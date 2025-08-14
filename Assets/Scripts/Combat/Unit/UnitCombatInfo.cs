using UnityEngine;
using System;
using System.Collections.Generic;

public class UnitCombatInfo : MonoBehaviour
{
    [HideInInspector] public Vector2 startPos;
    [HideInInspector] public Vector2 targetPos;
    [HideInInspector] public bool isFinishedAction;
    [HideInInspector] public Dictionary<string, Action> actionList = new Dictionary<string, Action>();

    /// <summary>
    /// AnimationEvent - Used for move duration, never change!!!
    /// </summary>
    private void StartMovePosition() { }

    /// <summary>
    /// Animation Event  - Used for move duration, never change!!!
    /// </summary>
    private void EndMovePosition() { }
}
