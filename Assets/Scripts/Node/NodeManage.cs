using UnityEngine;

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

    public string node_id;                 // 노드 고유 식별 ID
    public ZoneType zone_type;            // 존 구분 (start / middle / end)
    public NodeType type;                 // 노드 종류 (battle_normal / battle_elite / ...)
    public int layer;                     // 층수 (0부터 시작)
    public string[] connect_to;           // 연결되는 다음 노드 ID 목록
    public bool is_entry;                 // 플레이어가 선택 가능한 노드인지 여부 (UI 표시용)

}
