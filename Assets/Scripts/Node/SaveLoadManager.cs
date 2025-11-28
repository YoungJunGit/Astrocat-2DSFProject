using System.IO;
using UnityEngine;
using S3MG;

public static class SaveLoadManager
{
    const string MAP_KEY = "SaveMapData";
    public static void SaveMap(NodeMapGenerator mapGenerator)
    {
        SaveMapData saveMap = new SaveMapData();

        foreach(var node in mapGenerator.Map)
        {
            SaveNodeData n = new SaveNodeData();
            n.idx = node.idx;
            n.floor = node.floor;
            n.route = node.route;

            n.nextNodeIds.AddRange(node.nextNodesIdx);
            n.prevNodeIds.AddRange(node.prevNodesIdx);

            n.nodeType = node.nodeType;
            n.nodeName = node.NodeText.text;

            n.isVisited = node.visited;
            n.isActive = node.isActive;

            n.position = new Vector3(node.xPos, node.yPos, node.zPos);

            saveMap.saveNodeDatas.Add(n);
        }

        ES3.Save(MAP_KEY, saveMap);
        Debug.Log("맵 데이터 저장 완료");
    }
}
