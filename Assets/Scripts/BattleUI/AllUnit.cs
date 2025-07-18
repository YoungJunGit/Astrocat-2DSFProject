using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DataEntity;

public class AllUnit : MonoBehaviour
{
    public GameObject selectionBarPrefab, stateBoxPrefab, MonStateBoxPrefab, handIconInstance;
    public Transform startPos;
    public PlayerData playerDataList;
    public MonsterData MonsterDataList;

    [HideInInspector] public bool targetselection;
    [HideInInspector] public string selectedActionType, selectingUnitName;
    [HideInInspector] public List<StateUnit> stateBoxs = new List<StateUnit>();
    [HideInInspector] public List<MonsterState> MonStateBoxs = new List<MonsterState>();

    [SerializeField] private List<CharacterDataEntity> PlayerData;
    [SerializeField] private List<MonsterDataEntity> MonsterData;

    private string[] unitNames = { "RifleMan", "Sniper", "Commissar" };
    private string[] enemyNames = { "A", "B", "C" };
    private string[] sortedUnits = { "RifleMan", "A", "Sniper", "B", "Commissar", "C" };

    private Canvas canvas;
    private Color defaultColor, handIconColor;
    private int currentUnitIndex = 0, currentEnemyIndex = 0;
    private SpriteRenderer sr;

    void Update()
    {
        if (targetselection && Input.GetKeyDown(KeyCode.A)) MoveHandIcon(1);
        else if (targetselection && Input.GetKeyDown(KeyCode.D)) MoveHandIcon(-1);
        else if (targetselection && Input.GetKeyDown(KeyCode.F)) StartCoroutine(SelectCurrentEnemy());
    }

    /// <summary>
    /// ���� �ʱ�ȭ: ������ �ε� �� ���� UI ����
    /// </summary>
    void Awake()
    {
        canvas = Object.FindFirstObjectByType<Canvas>();
        defaultColor = transform.Find("RifleMan").GetComponentInChildren<Renderer>().material.color;
        targetselection = false;

        sr = handIconInstance.GetComponent<SpriteRenderer>();
        handIconColor = sr.color;

        InitData();
        CreateUnitUI();
        CreateMonsterUI();
        ShowCurrentUnitUI();
    }

    /// <summary>
    /// ĳ���� �� ���� ������ �ʱ�ȭ
    /// </summary>
    void InitData()
    {
        PlayerData.Clear();
        MonsterData.Clear();

        for (int i = 0; i < 3; i++)
        {
            PlayerData.Add(playerDataList.data[i]);
            MonsterData.Add(MonsterDataList.data[i]);
        }
    }

    /// <summary>
    /// �÷��̾� ���� ���� UI ����
    /// </summary>
    void CreateUnitUI()
    {
        for (int i = 0; i < unitNames.Length; i++)
        {
            StateUnit state = Instantiate(stateBoxPrefab, canvas.transform).GetComponentInChildren<StateUnit>();
            state.name = $"stateBox_{unitNames[i]}";
            state.GetComponent<RectTransform>().position = Camera.main.WorldToScreenPoint(startPos.position + new Vector3(0, -1.1f * i, 0));
            state.Initialize(PlayerData[i]);
            stateBoxs.Add(state);
        }
    }

    /// <summary>
    /// ���� ���� UI ����
    /// </summary>
    void CreateMonsterUI()
    {
        for (int i = 0; i < enemyNames.Length; i++)
        {
            MonsterState monsterState = Instantiate(MonStateBoxPrefab, canvas.transform).GetComponentInChildren<MonsterState>();
            monsterState.name = $"MonStateBox_{enemyNames[i]}";
            monsterState.Initialize(MonsterData[i]);
            Transform enemy = transform.Find(enemyNames[i]);
            monsterState.GetComponent<RectTransform>().position = Camera.main.WorldToScreenPoint(enemy.position + new Vector3(0, 1.5f, 0));
            MonStateBoxs.Add(monsterState);
        }
    }

    /// <summary>
    /// ���� ���õ� ������ ���� ǥ���ϰ� ���� UI ǥ��
    /// </summary>
    void ShowCurrentUnitUI()
    {
        foreach (Transform child in canvas.transform)
            if (child.name.StartsWith("SelectionBar_")) Destroy(child.gameObject);

        for (int i = 0; i < unitNames.Length; i++)
        {
            Transform unit = transform.Find(unitNames[i]);
            unit.GetChild(0).GetComponent<Renderer>().material.color = (i == currentUnitIndex) ? Color.black : defaultColor;

            if (i == currentUnitIndex)
            {
                GameObject bar = Instantiate(selectionBarPrefab, canvas.transform);
                bar.name = "SelectionBar_" + unitNames[i];
                bar.GetComponent<RectTransform>().position = Camera.main.WorldToScreenPoint(unit.position + new Vector3(2.5f, 0.5f, 0));
                bar.GetComponent<SelectionScript>().Initialize(OnActionSelected);
            }
        }
    }

    /// <summary>
    /// ���õ� �ൿ ��ư�� ������ �� ȣ���
    /// </summary>
    /// <param name="actionType">���õ� �׼� Ÿ��</param>
    void OnActionSelected(string actionType)
    {
        selectedActionType = actionType;
        targetselection = true;
        selectingUnitName = unitNames[currentUnitIndex];
        Debug.Log($"{actionType} ��ư�� ���Ƚ��ϴ�.");

        Transform enemy = transform.Find("A");
        Vector3 worldPos = enemy.position + new Vector3(1.5f, 0, 0);
        handIconInstance.transform.position = worldPos;
        handIconInstance.SetActive(true);
    }

    /// <summary>
    /// Ű �Է¿� ���� handIcon�� �¿�� �̵�
    /// </summary>
    /// <param name="direction">+1 �Ǵ� -1</param>
    void MoveHandIcon(int direction)
    {
        if (enemyNames.Length == 0) return;

        currentEnemyIndex = (currentEnemyIndex + direction + enemyNames.Length) % enemyNames.Length;
        string enemyName = enemyNames[currentEnemyIndex];
        Transform enemy = transform.Find(enemyName);

        Vector3 worldPos = enemy.position + new Vector3(1.5f, 0, 0);
        handIconInstance.transform.position = worldPos;
        handIconInstance.SetActive(true);
        Debug.Log($"HandIcon moved to enemy [{enemyName}]");
    }

    /// <summary>
    /// ���� ���õ� ������ ���� ����
    /// </summary>
    IEnumerator SelectCurrentEnemy()
    {
        if (enemyNames.Length == 0) yield break;

        string enemyName = enemyNames[currentEnemyIndex];
        sr.color = Color.black;
        yield return new WaitForSeconds(0.5f);
        sr.color = handIconColor;
        currentEnemyIndex = 0;
        ExecuteAttack(enemyName);
    }

    /// <summary>
    /// ���� ���� ����
    /// </summary>
    /// <param name="enemyName">���� ��� ���� �̸�</param>
    void ExecuteAttack(string enemyName)
    {
        var target = MonStateBoxs.Find(m => m.name == $"MonStateBox_{enemyName}");
        var attacker = stateBoxs.Find(p => p.name == $"stateBox_{selectingUnitName}");

        if (target == null || attacker == null) return;

        if (selectedActionType == "BasicAttack")
        {
            bool dead = target.TakeDamage(attacker.basicDamage);
            Debug.Log(dead ? $"[{selectingUnitName}]�� {enemyName}�� �׿����ϴ�."
                           : $"[{selectingUnitName}]�� {enemyName}�� �����߽��ϴ�. ���� HP: {target.currentHP}");
        }

        sr.color = Color.white;
        NextTurn();
    }

    /// <summary>
    /// ���� �������� �� �ѱ�
    /// </summary>
    public void NextTurn()
    {
        currentUnitIndex = (currentUnitIndex + 1) % unitNames.Length;
        targetselection = false;
        handIconInstance.SetActive(false);
        ShowCurrentUnitUI();
    }
}
