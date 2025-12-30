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
        LastAttacker = null;
        actionList = new Dictionary<string, Action>();
    }

    public Vector2 startPos;
    public Vector2 targetPos;
    public bool isFinishedAction;
    public BaseUnit LastAttacker;
    public Dictionary<string, Action> actionList;
}
