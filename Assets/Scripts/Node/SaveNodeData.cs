using System.Collections.Generic;
using S3MG;
using UnityEngine;

public class SaveNodeData
{
    public int idx;
    public int floor;
    public int route;

    public List<int> nextNodeIdx = new List<int>();
    public List<int> prevNodeIdx = new List<int>();

    public NodeType nodeType;
    public string nodeName;

    public bool isVisited;
    public bool isActive;

    public Vector3 position = new Vector3(0,0,0);
}
