using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MonUnitInteraction : MonoBehaviour
{
    public GameObject handIconInstance;
    public AllUnit allUnit;  // AllUnit.cs 참조 필요

    int attack;
    int monHp;
    bool monDead;
    string attackerName;
    string targetName;


    void OnMouseEnter()
    {
        if (!CompareTag("Enemy")) return;
        if (!allUnit.targetselection) return;

        Vector3 worldPos = transform.position + new Vector3(1.5f, 0, 0);
        handIconInstance.transform.position = worldPos;
        handIconInstance.SetActive(true);
    }

    void OnMouseDown()
    {
        if (!CompareTag("Enemy")) return;
        if (!allUnit.targetselection) return;

        SpriteRenderer sr = handIconInstance.GetComponent<SpriteRenderer>();
        sr.color = Color.black;

        StartCoroutine(DelayAction(sr));
    }

    void OnMouseExit()
    {
        handIconInstance.SetActive(false);
    }

    IEnumerator DelayAction(SpriteRenderer sr)
    {
        yield return new WaitForSeconds(0.5f);

        targetName = "MonStateBox_" + gameObject.name;

        MonsterState matchedMonster = allUnit.MonStateBoxs.Find(m => m.name == targetName);

        if (matchedMonster != null)
        {
            monHp = (int)matchedMonster.currentHP;
        }

        // 선택된 아군 유닛의 공격력 가져오기
        attackerName = "stateBox_" + allUnit.selectingUnitName;

        StateUnit attackerData = allUnit.stateBoxs.Find(p => p.name == attackerName);

        if (attackerData != null)
        {
            attack = attackerData.basicDamage;
            Debug.Log($"[{attackerName}] 의 기본 공격력은 {attack}입니다.");
        }

        if (allUnit.selectedActionType == "BasicAttack") {
            monDead = matchedMonster.TakeDamage(attack);
            if (!monDead);
            else Debug.Log($"[{allUnit.selectingUnitName}]가 {gameObject.name}을 죽였습니다.");
        }

        allUnit.NextTurn();
        sr.color = Color.white;
    }
}
