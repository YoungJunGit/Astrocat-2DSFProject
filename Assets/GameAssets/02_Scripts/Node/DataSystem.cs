using UnityEngine;
using DataSystem;
using System.Collections.Generic;
using S3MG;

namespace DataSystem
{
    public interface ISaveMapData
    {
        void SaveLastMapData(NodeMapGenerator mapGenerator);
        void SaveCompletedMapData();
    }

    public interface ILoadMapData
    {
        void LoadLastMapData(NodeMapGenerator mapGenerator);
    }
}

public class MapDataFactory : ISaveMapData, ILoadMapData
{
    private const string LAST_MAP_KEY = "SaveLastMapData";
    private const string CURRENT_MAP_KEY = "SaveCurrentMapData";

    public void SaveLastMapData(NodeMapGenerator mapGenerator)
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

        ES3.Save(LAST_MAP_KEY, saveMapData, Easy3MetaData.ProgressFile);
        Debug.Log("맵 데이터 저장 완료");
    }


    public void SaveCompletedMapData()
    {
        if(ES3.KeyExists(LAST_MAP_KEY, Easy3MetaData.ProgressFile))
        {
            SaveMapData SaveMapData = ES3.Load<SaveMapData>(LAST_MAP_KEY, Easy3MetaData.ProgressFile);
            ES3.Save(CURRENT_MAP_KEY, SaveMapData, Easy3MetaData.ProgressFile);
        }
    }

    public void LoadLastMapData(NodeMapGenerator mapGenerator)
    {
        if(IsKeyExist())
        {
            int floorNum = mapGenerator.floorNum;
            int routeNum = mapGenerator.routeNum;
            SaveMapData mapData = ES3.Load<SaveMapData>(CURRENT_MAP_KEY, Easy3MetaData.ProgressFile);
            List<SaveNodeData> nodes = mapData.saveNodeDatas;
            int index = 0;

            for (int i = 0;i < floorNum;i++)
            {
                for(int j = 0;j < routeNum;j++)
                {
                    Node node = Object.Instantiate(mapGenerator.nodePref, mapGenerator.mapParent.transform);
                    node.Init(mapGenerator);
                    mapGenerator.setNodeSize(node, mapGenerator.normalNodeSize);

                    node.idx = nodes[index].idx;
                    node.floor = nodes[index].floor;
                    node.route = nodes[index].route;

                    node.nextNodesIdx.AddRange(nodes[index].nextNodeIdx);
                    node.prevNodesIdx.AddRange(nodes[index].prevNodeIdx);

                    node.nodeType = nodes[index].nodeType;
                    node.NodeText.text = nodes[index].nodeName;

                    node.visited = nodes[index].isVisited;
                    node.isActive = nodes[index].isActive;
                    node.connected = nodes[index].isConnected;
                    mapGenerator.nowNodeIdx = mapData.nowNodeIdx;
                    node.transform.position = nodes[index].position;

                    mapGenerator.Map[i, j] = node;
                    mapGenerator.Map[i, j].gameObject.name = $"{i},{j}";

                    index++;
                }
            }
        }
    }

    public bool IsKeyExist()
    {
        if (ES3.KeyExists(CURRENT_MAP_KEY, Easy3MetaData.ProgressFile))
            return true;
        else
            return false;
    }
}