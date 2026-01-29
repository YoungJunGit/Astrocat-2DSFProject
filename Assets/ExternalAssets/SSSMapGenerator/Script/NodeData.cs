/*
 * SSSMapGenerator : Ver. 1.2.0
 * Written by Takashi Sowa @ loloop
*/

using UnityEngine;

//名前空間｜Namespace
namespace S3MG{

	/*------------------------------------------------------------
	ノードデータ用クラス：ノードデータを管理｜Node data class: Manages node data
	------------------------------------------------------------*/
	[CreateAssetMenu(menuName = "S3MG/createNodeData", fileName = "NodeData")]
	public class NodeData : ScriptableObject{

		//ノードタイプ｜Node type
		public enum Type{
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

		public Sprite sprite;//自分の画像｜Node image
		public Type type;//自分のノードタイプ｜Node type
		public string nodeName;//自分のノード名｜Node name

	}

}