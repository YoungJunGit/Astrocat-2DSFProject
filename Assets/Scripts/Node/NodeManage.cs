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

    public string node_id;                 // ��� ���� �ĺ� ID
    public ZoneType zone_type;            // �� ���� (start / middle / end)
    public NodeType type;                 // ��� ���� (battle_normal / battle_elite / ...)
    public int layer;                     // ���� (0���� ����)
    public string[] connect_to;           // ����Ǵ� ���� ��� ID ���
    public bool is_entry;                 // �÷��̾ ���� ������ ������� ���� (UI ǥ�ÿ�)

}
