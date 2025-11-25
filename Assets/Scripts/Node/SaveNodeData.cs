using System.Collections.Generic;
using S3MG;

public class SaveNodeData
{
    public int idx;
    public int floor;
    public int route;

    public List<int> nextNodeIds;
    public List<int> prevNodeIds;

    public NodeData.Type nodeType;
    public string nodeName;

    public bool isVisited;
    public bool isActive;

    public float xPos;
    public float yPos;
    public float zPos;
}
