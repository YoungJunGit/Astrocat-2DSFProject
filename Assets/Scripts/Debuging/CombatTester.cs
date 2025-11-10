using System;
using UnityEngine;
using UnityEngine.UI;
using DataEnum;

public class CombatTester : MonoBehaviour
{
    private CombatManager combatManager;

    private void Awake()
    {
        
    }

    public void OnDieButton()
    {
        
    }
    public void OnFaintingButton()
    {
        combatManager.OnFainting();
    }

    public void OnExtraButton()
    {
        combatManager.OnExtraTurn();
    }
}
