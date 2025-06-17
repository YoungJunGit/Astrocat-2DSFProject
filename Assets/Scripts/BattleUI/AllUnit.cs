using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AllUnit : MonoBehaviour
{
    public GameObject selectionBarPrefab;
    private Canvas canvas;
    private Color defaultColor;
    private string[] unitNames = { "RifleMan", "Sniper", "Commissar" };
    private int currentUnitIndex = 0;

    void Start()
    {
        canvas = Object.FindFirstObjectByType<Canvas>();
        defaultColor = transform.Find("RifleMan").GetComponentInChildren<Renderer>().material.color;
        ShowCurrentUnitUI();
    }

    void ShowCurrentUnitUI()
    {
        foreach (Transform child in canvas.transform)
        {
            if (child.name.StartsWith("SelectionBar_"))
                Destroy(child.gameObject);
        }

        for (int i = 0; i < unitNames.Length; i++)
        {
            string unitName = unitNames[i];
            Transform unit = transform.Find(unitName);
            Renderer renderer = unit.GetChild(0).GetComponent<Renderer>();

            if (renderer != null)
                renderer.material.color = (i == currentUnitIndex) ? Color.black : defaultColor;

            if (i == currentUnitIndex)
            {
                Vector3 worldPos = unit.position + new Vector3(2f, 0, 0);
                Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

                GameObject bar = Instantiate(selectionBarPrefab, canvas.transform);
                bar.name = "SelectionBar_" + unitName;

                RectTransform rect = bar.GetComponent<RectTransform>();
                rect.position = screenPos;

                // �ݹ� ���
                SelectionScript selectionBox = bar.GetComponent<SelectionScript>();
                if (selectionBox != null)
                {
                    selectionBox.Initialize(OnActionSelected);
                }
            }
        }
    }

    // ��ư Ŭ�� �� SelectionBox�κ��� �޴� �ݹ�
    void OnActionSelected(string actionType)
    {
        Debug.Log($"{actionType} ��ư�� ���Ƚ��ϴ�.");
        NextTurn();
    }

    void NextTurn()
    {
        currentUnitIndex = (currentUnitIndex + 1) % unitNames.Length;
        ShowCurrentUnitUI();
    }
}
