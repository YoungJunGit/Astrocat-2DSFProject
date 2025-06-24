using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UnitInteraction : MonoBehaviour
{
    public GameObject handIconInstance;
    public AllUnit allUnit;  // AllUnit.cs 참조 필요

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

        Debug.Log($"[{gameObject.name}] 유닛이 [{allUnit.selectedActionType}] 액션을 받았습니다.");
        allUnit.NextTurn();
        sr.color = Color.white;
    }
}
