/*
 * SSSMapGenerator : Ver. 1.0.2
 * Written by Takashi Sowa @ loloop
*/
using UnityEngine;

namespace S3MG{

    public enum NodeType
    {
        Empty,
        Start,
        Camp,
        Shop,
        Event,
        Treasure,
        Trap,
        Enemy,
        Middle,
        Final,
        Random,
    }

    [CreateAssetMenu(menuName = "S3MG/createNodeData", fileName = "NodeData")]
	public class NodeData : ScriptableObject{

		public Sprite sprite;
		public NodeType type;
		public string nodeName;
	}
}