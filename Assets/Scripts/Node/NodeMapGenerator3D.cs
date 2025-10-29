using S3MG;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CreateNodeMapGenerator3D", fileName = "NodeMapGenerator3D")]
public class NodeMapGenerator3D : ScriptableObject
{
    [Header("Map Settings")]
    public GameObject nodePrefab;
    public int floorCount = 5;
    public int routeCount = 3;
    public float xSpacing = 4f;
    public float zSpacing = 3f;
    public float yRandomOffset = 0.2f;

    [Header("Node Visual Settings")]
    public Sprite defaultSprite;
    public Sprite startSprite;
    public Sprite finalSprite;

    [HideInInspector] public List<Node3D> allNodes = new List<Node3D>();
    [HideInInspector] public Node3D nowNode;
    [HideInInspector] public bool skipNodeProcessing = false;

    private Transform parentTransform;

    /// <summary>
    /// ScriptableObject는 MonoBehaviour가 아니므로 직접 실행할 때 부모 Transform을 받아야 함
    /// </summary>
    public void GenerateMap(Transform parent)
    {
        parentTransform = parent;
        allNodes.Clear();

        for (int f = 0; f < floorCount; f++)
        {
            for (int r = 0; r < routeCount; r++)
            {
                Vector3 pos = new Vector3(r * xSpacing, Random.Range(-yRandomOffset, yRandomOffset), f * zSpacing);
                GameObject nodeObj = Instantiate(nodePrefab, pos, Quaternion.identity, parentTransform);
                Node3D node = nodeObj.GetComponent<Node3D>();

                node.floor = f;
                node.route = r;
                node.Init(this);
                allNodes.Add(node);

                // 노드 타입 및 스프라이트 설정
                if (f == 0 && r == routeCount / 2)
                {
                    node.SetNodeData(startSprite ? startSprite : defaultSprite, NodeData.Type.Start, "Start");
                }
                else if (f == floorCount - 1)
                {
                    node.SetNodeData(finalSprite ? finalSprite : defaultSprite, NodeData.Type.Final, "Final");
                }
                else
                {
                    node.SetNodeData(defaultSprite, NodeData.Type.Enemy, $"Node_{f}_{r}");
                }
            }
        }

        ConnectNodes();
    }

    private void ConnectNodes()
    {
        for (int i = 0; i < allNodes.Count; i++)
        {
            Node3D node = allNodes[i];
            if (node.floor >= floorCount - 1) continue;

            foreach (Node3D next in allNodes)
            {
                if (next.floor == node.floor + 1 && Mathf.Abs(next.route - node.route) <= 1)
                {
                    node.nextNodes.Add(next);
                    next.prevNodes.Add(node);
                }
            }
        }
    }

    public void PaintPath(Node3D node)
    {
        foreach (Node3D next in node.nextNodes)
        {
            Debug.DrawLine(node.transform.position, next.transform.position, Color.yellow, 3f);
        }
    }

    public void ToNextNode()
    {
        Debug.Log("Proceeding to next node");
    }

    public void PassedSameFloor(Node3D node)
    {
        foreach (Node3D n in allNodes)
        {
            if (n.floor == node.floor && !n.visited)
            {
                var sr = n.GetComponent<SpriteRenderer>();
                if (sr) sr.color = Color.gray;
            }
        }
    }
}