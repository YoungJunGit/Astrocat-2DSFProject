using UnityEngine;
using UnityEngine.SceneManagement;

public class NodeManage : MonoBehaviour
{
    public enum ZoneType
    {
        Start,
        Middle,
        End
    }

    public enum NodeType
    {
        Battle_Normal,
        Battle_Elite,
        Rest,
        Boss,
        Scenario
    }

    [HideInInspector] public string node_id;                 // 노드 고유 식별 ID
    [HideInInspector] public ZoneType zone_type;            // 존 구분 (start / middle / end)
    [HideInInspector] public NodeType type;                 // 노드 종류 (battle_normal / battle_elite / ...)
    [HideInInspector] public int layer;                     // 층수 (0부터 시작)
    [HideInInspector] public string[] connect_to;           // 연결되는 다음 노드 ID 목록
    [HideInInspector] public bool is_entry;                 // 플레이어가 선택 가능한 노드인지 여부 (UI 표시용)

    public void LoadMiddleBattleScene()
    {
        SceneManager.LoadScene(1);
    }

    public void LoadMiddleHealScene()
    {
        SceneManager.LoadScene(1);
    }
}
