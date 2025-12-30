using UnityEngine;
using DataEnum;
using Random = UnityEngine.Random;
using System.Collections.Generic;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class CCTester : MonoBehaviour
{
    enum CCSide
    {
        PlayerToEnemy,
        EnemyToPlayer
    }
    ICrowdControlManager crowdControlManager;
    IUnitManager unitManager;

    [SerializeField] Button addCrowdCtrlBtn;
    [SerializeField] Button rmvCrowdCtrlBtn;
    [SerializeField] Button debugCrowdCtrlsBtn;
    [SerializeField] ELEMENT_TYPE element_Type;

    [FoldoutGroup("Add"), SerializeField] CCSide targetSide;
    [FoldoutGroup("Add"), SerializeField] bool targetIsRandom = false;
    [FoldoutGroup("Add"), HideIf("targetIsRandom"), SerializeField] int enemyIndex;
    [FoldoutGroup("Add"), HideIf("targetIsRandom"), SerializeField] int playerIndex;

    [FoldoutGroup("Remove"), SerializeField] SIDE removeSide;
    [FoldoutGroup("Remove"), SerializeField] int removeIndex;

    [FoldoutGroup("Debug"), SerializeField] ELEMENT_TYPE[] elementsToPrint;
    [FoldoutGroup("Debug"), SerializeField] bool printAll;
    [FoldoutGroup("Debug"), HideIf("printAll"), SerializeField] SIDE debugSide;
    [FoldoutGroup("Debug"), HideIf("printAll"), SerializeField] int debugIndex;

    private void Awake()
    {
        ServiceLocator.For(this)
            .Get(out unitManager)
            .Get(out crowdControlManager);

        addCrowdCtrlBtn.onClick.AddListener(() => SetCCOnUnit());
        rmvCrowdCtrlBtn.onClick.AddListener(() => RemoveCCOnUnit());
        debugCrowdCtrlsBtn.onClick.AddListener(() => DebugCC());
    }

    private void SetCCOnUnit()
    {
        var enemyUnits = unitManager.GetEnemyUnits();
        var playerUnits = unitManager.GetPlayerUnits();

        enemyIndex = targetIsRandom ? Random.Range(0, enemyUnits.Count) : enemyIndex;
        playerIndex = targetIsRandom ? Random.Range(0, playerUnits.Count) : playerIndex;
        var target = enemyUnits[enemyIndex];
        var caster = playerUnits[playerIndex];

        if (targetSide == CCSide.PlayerToEnemy)
            crowdControlManager.AddCrowdControl(element_Type, target, caster);
        else
            crowdControlManager.AddCrowdControl(element_Type, caster, target);
    }

    private void RemoveCCOnUnit()
    {
        var units = unitManager.GetUnit(removeSide);
        var target = units[removeIndex];

        crowdControlManager.RemoveCrowdControl(element_Type, target);
    }

    private void DebugCC()
    {
        if(printAll)
        {
            var units = unitManager.GetAllUnits();
            foreach (var unit in units)
            {
                string str = ToString(elementsToPrint, unit);
                Debug.Log(str);
            }
        }
        else
        {
            var units = unitManager.GetUnit(debugSide);
            string str = ToString(elementsToPrint, units[debugIndex]);
            Debug.Log(str);
        }
    }

    private string ToString(ELEMENT_TYPE[] element_Type, BaseUnit unit)
    {
        string str1 = $"{unit.GetStat().CoreStat.Name}'s CC List\n";

        string str2 = "";
        foreach (var element in element_Type)
        {
            str2 += $"{element.ToString()} : ";
            IReadOnlyList<ELEMENT_STATUS_CATEGORY> ccList = unit.CCUnit.CurrentEffects[element];

            int index = 0;
            foreach (var cc in ccList)
            {
                str2 += $"{cc}";
                if(!(++index == ccList.Count))
                    str2 += ", ";
            }
            str2 += "\n";
        }

        return str1 + str2;
    }
}
