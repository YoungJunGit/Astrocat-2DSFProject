using Cysharp.Threading.Tasks;
using S3MG;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Node3D : MonoBehaviour
{
    [Header("Node Info")]
    public int floor;
    public int route;
    public string nodeType = "Normal";

    [Header("Connections")]
    public List<Node3D> prevNodes = new List<Node3D>();
    public List<Node3D> nextNodes = new List<Node3D>();

    [Header("Visual Settings")]
    public Color defaultColor = Color.white;
    public Color connectedColor = Color.cyan;
    public Color selectedColor = Color.yellow;
    public Color inactiveColor = Color.gray;

    private Renderer nodeRenderer;
    private NodeMapGenerator3D generator;

    private void Awake()
    {
        nodeRenderer = GetComponent<Renderer>();
        nodeRenderer.material.color = defaultColor;
    }

    public void Init(NodeMapGenerator3D mapGenerator)
    {
        generator = mapGenerator;
    }

    public void SetNodeType(string type)
    {
        nodeType = type;

        if (nodeRenderer != null)
        {
            switch (type)
            {
                case "Start": nodeRenderer.material.color = Color.green; break;
                case "Boss": nodeRenderer.material.color = Color.red; break;
                default: nodeRenderer.material.color = defaultColor; break;
            }
        }
    }

    public void SelectNode()
    {
        nodeRenderer.material.color = selectedColor;
    }

    public void DeselectNode()
    {
        nodeRenderer.material.color = defaultColor;
    }

    public void HighlightAsConnected()
    {
        nodeRenderer.material.color = connectedColor;
    }

    public void DisableNode()
    {
        nodeRenderer.material.color = inactiveColor;
    }

    private void OnMouseDown()
    {
        if (generator != null)
        {
            generator.OnNodeClicked(this);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (nextNodes != null && nextNodes.Count > 0)
        {
            Gizmos.color = Color.cyan;
            foreach (var next in nextNodes)
            {
                if (next != null)
                    Gizmos.DrawLine(transform.position, next.transform.position);
            }
        }
    }
#endif
}