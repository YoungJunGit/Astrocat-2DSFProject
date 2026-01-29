/*
 * SSSMapGenerator : Ver. 1.2.0
 * Written by Takashi Sowa @ loloop
*/

using System.Collections.Generic;

//名前空間｜Namespace
namespace S3MG{

	/*------------------------------------------------------------
	マップセーブデータ：シリアライズ可能クラス｜Map Save Data: Serializable class
	------------------------------------------------------------*/
	[System.Serializable]
	public class MapSaveData{
		public int seed;//マップ生成シード値｜Map generation seed
		public int currentFloor;//現在のフロア｜Current floor
		public int nowNodeFloor;//現在選択中のノードのフロア｜Current selected node floor
		public int nowNodeRoute;//現在選択中のノードのルート｜Current selected node route
		public List<VisitedNodeData> visitedNodes = new List<VisitedNodeData>();//訪問済みノードリスト｜Visited nodes list

		//スクロール/マップ位置｜Scroll/Map position
		public float scrollPositionX;//ScrollViewモード用｜For ScrollView mode
		public float scrollPositionY;
		public float mapPositionX;//Standaloneモード用｜For Standalone mode
		public float mapPositionY;
	}

	/*------------------------------------------------------------
	訪問済みノードデータ：シリアライズ可能クラス｜Visited Node Data: Serializable class
	------------------------------------------------------------*/
	[System.Serializable]
	public class VisitedNodeData{
		public int floor;
		public int route;

		public VisitedNodeData(int floor, int route){
			this.floor = floor;
			this.route = route;
		}
	}

}
