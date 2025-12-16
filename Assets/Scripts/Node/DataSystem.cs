using System.Collections.Generic;
using UnityEngine;
using DataSystem;

namespace DataSystem
{
    public interface ISaveMapData
    {
        void SaveMapData(NodeMapGenerator mapGenerator);
    }

    public interface ILoadMapData
    {
        void LoadMapData(NodeMapGenerator mapGenerator);
    }
}

public class MapDataFactory : ISaveMapData, ILoadMapData
{
    private const string MAP_KEY = "SaveMapData";

    public void SaveMapData(NodeMapGenerator mapGenerator)
    {
        SaveMapData saveMapData = new SaveMapData();

        foreach (var node in mapGenerator.Map)
        {
            SaveNodeData n = new SaveNodeData();

            n.idx = node.idx;
            n.floor = node.floor;
            n.route = node.route;

            n.nextNodeIdx.AddRange(node.nextNodesIdx);
            n.prevNodeIdx.AddRange(node.prevNodesIdx);

            n.nodeType = node.nodeType;
            n.nodeName = node.NodeText.text;

            n.isVisited = node.visited;
            n.isActive = node.isActive;
            n.isConnected = node.connected;
            saveMapData.nowNodeIdx = mapGenerator.nowNodeIdx;
            n.position = node.transform.position;

            saveMapData.saveNodeDatas.Add(n);
        }

        ES3.Save(MAP_KEY, saveMapData);
        Debug.Log("맵 데이터 저장 완료");
    }

    public void LoadMapData(NodeMapGenerator mapGenerator)
    {

    }
}