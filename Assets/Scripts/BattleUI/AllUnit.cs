using UnityEngine;
using System.Collections;

public class AllUnit : MonoBehaviour
{
    private Color defaultColor;

    void Start()
    {
        // 첫 번째 유닛의 자식에서 초기 색상 한 번만 가져오기
        defaultColor = transform.Find("RifleMan").GetComponentInChildren<Renderer>().material.color;
        StartCoroutine(SetAllUnitsColor());
    }

    IEnumerator SetAllUnitsColor()
    {
        string[] unitNames = { "RifleMan", "Sniper", "Commissar", "Enemy1", "Enemy2", "Enemy3" };

        foreach (string currentUnitName in unitNames)
        {
            foreach (string unitName in unitNames)
            {
                Transform unit = transform.Find(unitName);
                Renderer renderer = unit.GetChild(0).GetComponent<Renderer>();
                if (renderer != null){
                    renderer.material.color = (unitName == currentUnitName) ? Color.black : defaultColor;
                }
            }

            yield return new WaitForSeconds(1f);
        }
    }
}
