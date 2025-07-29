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

    [HideInInspector] public string node_id;                 // ��� ���� �ĺ� ID
    [HideInInspector] public ZoneType zone_type;            // �� ���� (start / middle / end)
    [HideInInspector] public NodeType type;                 // ��� ���� (battle_normal / battle_elite / ...)
    [HideInInspector] public int layer;                     // ���� (0���� ����)
    [HideInInspector] public string[] connect_to;           // ����Ǵ� ���� ��� ID ���
    [HideInInspector] public bool is_entry;                 // �÷��̾ ���� ������ ������� ���� (UI ǥ�ÿ�)

    public void LoadMiddleBattleScene()
    {
        SceneManager.LoadScene(1);
    }

    public void LoadMiddleHealScene()
    {
        SceneManager.LoadScene(1);
    }
}
