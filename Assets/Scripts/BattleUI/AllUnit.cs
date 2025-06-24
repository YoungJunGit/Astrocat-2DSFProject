using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AllUnit : MonoBehaviour
{
    public GameObject selectionBarPrefab;
    public GameObject handIconInstance;
    private Canvas canvas;
    private Color defaultColor;
    public bool targetselection;
    public string selectedActionType;

    private string[] unitNames = { "RifleMan", "Sniper", "Commissar" };
    private int currentUnitIndex = 0;

    void Start()
    {
        canvas = Object.FindFirstObjectByType<Canvas>();
        defaultColor = transform.Find("RifleMan").GetComponentInChildren<Renderer>().material.color;
        targetselection = false;
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
                Vector3 worldPos = unit.position + new Vector3(2.5f, 0.5f, 0);
                Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

                GameObject bar = Instantiate(selectionBarPrefab, canvas.transform);
                bar.name = "SelectionBar_" + unitName;

                RectTransform rect = bar.GetComponent<RectTransform>();
                rect.position = screenPos;

                // 콜백 등록
                SelectionScript selectionBox = bar.GetComponent<SelectionScript>();
                if (selectionBox != null)
                {
                    selectionBox.Initialize(OnActionSelected);
                }
            }
        }
    }

    // 버튼 클릭 시 SelectionBox로부터 받는 콜백
    void OnActionSelected(string actionType)
    {
        selectedActionType = actionType;
        targetselection = true;
        Debug.Log($"{actionType} 버튼이 눌렸습니다.");
    }

    public void NextTurn()
    {
        currentUnitIndex = (currentUnitIndex + 1) % unitNames.Length;
        targetselection = false;
        handIconInstance.SetActive(false);
        ShowCurrentUnitUI();
    }
}
