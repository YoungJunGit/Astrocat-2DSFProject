using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MonUnitInteraction : MonoBehaviour
{
    public GameObject handIconInstance;
    public AllUnit allUnit;  // AllUnit.cs ���� �ʿ�

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

        // ���õ� �Ʊ� ������ ���ݷ� ��������
        attackerName = "stateBox_" + allUnit.selectingUnitName;

        StateUnit attackerData = allUnit.stateBoxs.Find(p => p.name == attackerName);

        if (attackerData != null)
        {
            attack = attackerData.basicDamage;
            Debug.Log($"[{attackerName}] �� �⺻ ���ݷ��� {attack}�Դϴ�.");
        }

        if (allUnit.selectedActionType == "BasicAttack") {
            monDead = matchedMonster.TakeDamage(attack);
            if (!monDead);
            else Debug.Log($"[{allUnit.selectingUnitName}]�� {gameObject.name}�� �׿����ϴ�.");
        }

        allUnit.NextTurn();
        sr.color = Color.white;
    }
}
