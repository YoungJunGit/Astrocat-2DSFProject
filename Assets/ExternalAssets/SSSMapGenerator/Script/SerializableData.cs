/*
 * SSSMapGenerator : Ver. 1.2.0
 * Written by Takashi Sowa @ loloop
*/

using UnityEngine;

//名前空間｜Namespace
namespace S3MG{

	/*------------------------------------------------------------
	マップノードデータ：シリアライズ可能クラス｜Map Node Data: Serializable class
	------------------------------------------------------------*/
	[System.Serializable]
	public class MapNodeData{
		public NodeData nodeData;//ノードデータ｜Node data
		public bool serializable;//連続で配置できるか｜Whether can be placed consecutively
		[Range(0,1)] public float chance;//出現確率｜Appearance chance
		[Range(1,100)] public int appearedAfter;//どのフロア以降から登場するか｜From which floor onwards to appear
		public bool reverseAppearedAfter;//appearedAfterを反転するか｜Whether to reverse appearedAfter

		//初期化：ランタイムで新規作成する時のデフォルト値設定｜Initialize: Set default values when creating at runtime
		public void init(){
			serializable = true;
			chance = 1;
			appearedAfter = 1;
			reverseAppearedAfter = false;
		}
	}

	/*------------------------------------------------------------
	固定ノードデータ：シリアライズ可能クラス｜Fixed Node Data: Serializable class
	------------------------------------------------------------*/
	[System.Serializable]
	public class FixedNodeData{
		public NodeData nodeData;//ノードデータ｜Node data
		[Range(1,100)] public int appearedOn;//どのフロアに登場するか｜Which floor to appear on

		//初期化：ランタイムで新規作成する時のデフォルト値設定｜Initialize: Set default values when creating at runtime
		public void init(){
			appearedOn = 1;
		}
	}

}
