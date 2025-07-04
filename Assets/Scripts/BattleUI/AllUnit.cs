using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DataEntity;

public class AllUnit : MonoBehaviour
{
    public GameObject selectionBarPrefab, stateBoxPrefab, MonStateBoxPrefab, handIconInstance;
    public Transform startPos;
    public CharacterData CharacterDataList;
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
    /// 게임 초기화: 데이터 로드 및 유닛 UI 생성
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
    /// 캐릭터 및 몬스터 데이터 초기화
    /// </summary>
    void InitData()
    {
        PlayerData.Clear();
        MonsterData.Clear();

        for (int i = 0; i < 3; i++)
        {
            PlayerData.Add(CharacterDataList.data[i]);
            MonsterData.Add(MonsterDataList.data[i]);
        }
    }

    /// <summary>
    /// 플레이어 유닛 상태 UI 생성
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
    /// 몬스터 상태 UI 생성
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
    /// 현재 선택된 유닛을 강조 표시하고 선택 UI 표시
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
    /// 선택된 행동 버튼이 눌렸을 때 호출됨
    /// </summary>
    /// <param name="actionType">선택된 액션 타입</param>
    void OnActionSelected(string actionType)
    {
        selectedActionType = actionType;
        targetselection = true;
        selectingUnitName = unitNames[currentUnitIndex];
        Debug.Log($"{actionType} 버튼이 눌렸습니다.");

        Transform enemy = transform.Find("A");
        Vector3 worldPos = enemy.position + new Vector3(1.5f, 0, 0);
        handIconInstance.transform.position = worldPos;
        handIconInstance.SetActive(true);
    }

    /// <summary>
    /// 키 입력에 따라 handIcon을 좌우로 이동
    /// </summary>
    /// <param name="direction">+1 또는 -1</param>
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
    /// 현재 선택된 적에게 공격 실행
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
    /// 공격 실행 로직
    /// </summary>
    /// <param name="enemyName">공격 대상 적의 이름</param>
    void ExecuteAttack(string enemyName)
    {
        var target = MonStateBoxs.Find(m => m.name == $"MonStateBox_{enemyName}");
        var attacker = stateBoxs.Find(p => p.name == $"stateBox_{selectingUnitName}");

        if (target == null || attacker == null) return;

        if (selectedActionType == "BasicAttack")
        {
            bool dead = target.TakeDamage(attacker.basicDamage);
            Debug.Log(dead ? $"[{selectingUnitName}]가 {enemyName}을 죽였습니다."
                           : $"[{selectingUnitName}]가 {enemyName}을 공격했습니다. 남은 HP: {target.currentHP}");
        }

        sr.color = Color.white;
        NextTurn();
    }

    /// <summary>
    /// 다음 유닛으로 턴 넘김
    /// </summary>
    public void NextTurn()
    {
        currentUnitIndex = (currentUnitIndex + 1) % unitNames.Length;
        targetselection = false;
        handIconInstance.SetActive(false);
        ShowCurrentUnitUI();
    }
}
