using UnityEngine;
using System;
using System.Collections.Generic;

public class UnitCombatInfo
{
    public UnitCombatInfo() 
    {
        startPos = Vector2.zero;
        targetPos = Vector2.zero;
        isFinishedAction = false;
        actionList = new Dictionary<string, Action>();
    }

    public Vector2 startPos;
    public Vector2 targetPos;
    public bool isFinishedAction;
    public Dictionary<string, Action> actionList;
}
