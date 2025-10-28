using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NodeMapGenerator_3D", menuName = "Map/3D Node Map Generator")]
public class NodeMapGenerator3D : MonoBehaviour
{
    [Header("Map Settings")]
    public int floorCount = 5;
    public int routeCount = 4;
    public float floorSpacing = 3f;
    public float routeSpacing = 2f;
    public float heightOffset = 0.5f;

    [Header("Prefabs")]
    public GameObject nodePrefab;

    [Header("Debug")]
    public bool showConnections = true;

    [HideInInspector] public List<List<Node3D>> nodes = new List<List<Node3D>>();
    [HideInInspector] public Node3D nowNode;

    void Start()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        ClearOldNodes();

        for (int floor = 0; floor < floorCount; floor++)
        {
            List<Node3D> floorNodes = new List<Node3D>();

            for (int route = 0; route < routeCount; route++)
            {
                Vector3 position = new Vector3(
                    route * routeSpacing,
                    floor * heightOffset,
                    floor * floorSpacing
                );

                GameObject nodeObj = Instantiate(nodePrefab, position, Quaternion.identity, transform);
                Node3D node = nodeObj.GetComponent<Node3D>();
                node.Init(this);

                node.floor = floor;
                node.route = route;

                // 간단한 타입 지정
                if (floor == 0)
                    node.SetNodeType("Start");
                else if (floor == floorCount - 1)
                    node.SetNodeType("Boss");
                else
                    node.SetNodeType("Normal");

                floorNodes.Add(node);
            }

            nodes.Add(floorNodes);
        }

        ConnectNodes();

        // 첫 노드 활성화
        foreach (Node3D start in nodes[0])
            start.HighlightAsConnected();
    }

    void ConnectNodes()
    {
        for (int floor = 0; floor < floorCount - 1; floor++)
        {
            List<Node3D> currentFloor = nodes[floor];
            List<Node3D> nextFloor = nodes[floor + 1];

            foreach (Node3D current in currentFloor)
            {
                int connections = Random.Range(1, 3);
                for (int i = 0; i < connections; i++)
                {
                    Node3D target = nextFloor[Random.Range(0, nextFloor.Count)];
                    if (!current.nextNodes.Contains(target))
                    {
                        current.nextNodes.Add(target);
                        target.prevNodes.Add(current);
                    }
                }
            }
        }
    }

    void ClearOldNodes()
    {
        foreach (Transform child in transform)
            DestroyImmediate(child.gameObject);
        nodes.Clear();
    }

    // 노드를 클릭했을 때 실행되는 핵심 로직
    public void OnNodeClicked(Node3D clickedNode)
    {
        // 1️⃣ 모든 노드 색상 초기화
        foreach (var floor in nodes)
        {
            foreach (var n in floor)
            {
                n.DisableNode();
            }
        }

        // 2️⃣ 현재 노드 표시
        clickedNode.SelectNode();
        nowNode = clickedNode;

        // 3️⃣ 연결된 노드만 활성화 표시
        foreach (Node3D next in clickedNode.nextNodes)
        {
            next.HighlightAsConnected();
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!showConnections || nodes == null) return;

        Gizmos.color = Color.cyan;
        foreach (var floorNodes in nodes)
        {
            foreach (var node in floorNodes)
            {
                if (node == null) continue;
                foreach (var next in node.nextNodes)
                {
                    if (next != null)
                        Gizmos.DrawLine(node.transform.position, next.transform.position);
                }
            }
        }
    }
#endif
}