using System.IO;
using UnityEngine;
using S3MG;

public static class SaveLoadManager
{
#if UNITY_EDITOR
    private static string savePath => Path.Combine(Application.persistentDataPath, "map_save.json");
#else
    private static string savePath => Path.Combine(Application.dataPath, "map_save.json");
#endif

    public static void SaveMap(NodeMapGenerator mapGenerator)
    {
        SaveNodeData data = new SaveNodeData();

        foreach(var node in mapGenerator.Map)
        {
            SaveNodeData n = new SaveNodeData();
            n.floor = node.floor;
            n.route = node.route;
            n.nodeType = node.nodeType.Value;

            n.isVisited = node.visited;
            n.isActive = node.isActive;

            n.xPos = node.xPos;
            n.yPos = node.yPos;
            n.zPos = node.zPos;
        }
    }
}
