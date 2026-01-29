/*
 * SSSMapGenerator : Ver. 1.2.0
 * Written by Takashi Sowa @ loloop
*/

using System.IO;
using UnityEngine;
using UnityEngine.UI;

//名前空間｜Namespace
namespace S3MG{

	//マップセーブ管理クラス：マップのセーブ/ロードを担当｜Map save manager class: Handles map save/load
	public class MapSaveManager : MonoBehaviour{

		[Header("▼保存フォルダパス：Assets/以下のパス｜Save Folder Path: Path under Assets/")]
		[SerializeField] string saveFolder = "SSSMapGenerator/_MapSaveData";

		[Header("▼保存ファイル名：拡張子不要｜Save File Name: No extension needed")]
		[SerializeField] string saveFileName = "SaveData";

		/*------------------------------------------------------------
		保存フォルダのフルパスを取得｜Get full path of save folder
		------------------------------------------------------------*/
		string GetSaveFolderPath(){
			#if UNITY_EDITOR
			return Path.Combine(Application.dataPath, saveFolder);
			#else
			return Path.Combine(Application.persistentDataPath, saveFolder);
			#endif
		}

		/*------------------------------------------------------------
		保存ファイルのフルパスを取得｜Get full path of save file
		------------------------------------------------------------*/
		string GetSaveFilePath(){
			return Path.Combine(GetSaveFolderPath(), saveFileName + ".json");
		}

		/*------------------------------------------------------------
		マップをセーブ｜Save map
		------------------------------------------------------------*/
		public bool SaveMap(){
			if(MapGenerator.instance == null){
				Debug.LogWarning($"MapGeneratorが見つかりません｜MapGenerator not found");
				return false;
			}

			//セーブデータを作成｜Create save data
			MapSaveData saveData = CreateSaveData();
			if(saveData == null) return false;

			//JSONに変換｜Convert to JSON
			string json = JsonUtility.ToJson(saveData, true);

			//フォルダが存在しない場合は作成｜Create folder if not exists
			string folderPath = GetSaveFolderPath();
			if(!Directory.Exists(folderPath)){
				Directory.CreateDirectory(folderPath);
			}

			//ファイルに書き込み｜Write to file
			string filePath = GetSaveFilePath();
			File.WriteAllText(filePath, json);

			Debug.Log($"マップをセーブしました：<color=#00ccff>{filePath}</color>｜Map saved: <color=#00ccff>{filePath}</color>");
			return true;
		}

		/*------------------------------------------------------------
		セーブデータを作成｜Create save data
		------------------------------------------------------------*/
		MapSaveData CreateSaveData(){
			MapGenerator generator = MapGenerator.instance;
			MapSaveData saveData = new MapSaveData();

			//基本情報｜Basic info
			saveData.seed = generator.GetSeed();
			saveData.currentFloor = generator.nowNode != null ? generator.nowNode.floor : 0;

			//nowNodeの位置を保存：スタートノードは-1,-1、ファイナルノードは-2,-2｜Save nowNode position: Start node is -1,-1, Final node is -2,-2
			if(generator.nowNode == null){
				saveData.nowNodeFloor = -1;
				saveData.nowNodeRoute = -1;
			}
			else if(generator.nowNode == generator.GetStartNode()){
				saveData.nowNodeFloor = -1;
				saveData.nowNodeRoute = -1;
			}
			else if(generator.nowNode == generator.GetFinalNode()){
				saveData.nowNodeFloor = -2;
				saveData.nowNodeRoute = -2;
			}
			else{
				saveData.nowNodeFloor = generator.nowNode.floor;
				saveData.nowNodeRoute = generator.nowNode.route;
			}

			//訪問済みノードを正しい順序で収集｜Collect visited nodes in correct order
			//スタートノードを最初に追加｜Add start node first
			Node startNode = generator.GetStartNode();
			if(startNode != null && startNode.visited){
				saveData.visitedNodes.Add(new VisitedNodeData(-1, -1));//-1,-1はスタートノードを示す｜-1,-1 indicates start node
			}

			//通常ノードをフロア順に追加｜Add normal nodes in floor order
			Node[,] map = generator.GetMap();
			if(map != null){
				int floorNum = map.GetLength(0);
				int routeNum = map.GetLength(1);
				for(int f = 0; f < floorNum; f++){
					for(int r = 0; r < routeNum; r++){
						if(map[f, r] != null && map[f, r].visited){
							saveData.visitedNodes.Add(new VisitedNodeData(f, r));
						}
					}
				}
			}

			//ファイナルノードを最後に追加｜Add final node last
			Node finalNode = generator.GetFinalNode();
			if(finalNode != null && finalNode.visited){
				saveData.visitedNodes.Add(new VisitedNodeData(-2, -2));//-2,-2はファイナルノードを示す｜-2,-2 indicates final node
			}

			//スクロール/マップ位置を取得｜Get scroll/map position
			ScrollRect scrollRect = generator.GetScrollRect();
			if(scrollRect != null){
				saveData.scrollPositionX = scrollRect.horizontalNormalizedPosition;
				saveData.scrollPositionY = scrollRect.verticalNormalizedPosition;
			}

			RectTransform mapParent = generator.GetMapParent();
			if(mapParent != null){
				saveData.mapPositionX = mapParent.anchoredPosition.x;
				saveData.mapPositionY = mapParent.anchoredPosition.y;
			}

			return saveData;
		}

		/*------------------------------------------------------------
		マップをロード｜Load map
		------------------------------------------------------------*/
		public bool LoadMap(){
			string filePath = GetSaveFilePath();

			//ファイルが存在しない場合｜If file does not exist
			if(!File.Exists(filePath)){
				Debug.LogWarning($"セーブファイルが見つかりません：<color=#ff0000>{filePath}</color>｜Save file not found: <color=#ff0000>{filePath}</color>");
				return false;
			}

			//ファイルを読み込み｜Read file
			string json = File.ReadAllText(filePath);

			//JSONをデシリアライズ｜Deserialize JSON
			MapSaveData saveData = JsonUtility.FromJson<MapSaveData>(json);
			if(saveData == null){
				Debug.LogWarning($"セーブデータの読み込みに失敗しました｜Failed to load save data");
				return false;
			}

			//マップを復元｜Restore map
			if(MapGenerator.instance == null){
				Debug.LogWarning($"MapGeneratorが見つかりません｜MapGenerator not found");
				return false;
			}

			MapGenerator.instance.LoadFromSaveData(saveData);

			Debug.Log($"マップをロードしました：<color=#00ff00>{filePath}</color>｜Map loaded: <color=#00ff00>{filePath}</color>");
			return true;
		}

		/*------------------------------------------------------------
		セーブファイルが存在するか確認｜Check if save file exists
		------------------------------------------------------------*/
		public bool HasSaveFile(){
			return File.Exists(GetSaveFilePath());
		}

		/*------------------------------------------------------------
		セーブファイルを削除｜Delete save file
		------------------------------------------------------------*/
		public bool DeleteSave(){
			string filePath = GetSaveFilePath();
			if(!File.Exists(filePath)){
				Debug.LogWarning($"セーブファイルが見つかりません：<color=#ff0000>{filePath}</color>｜Save file not found: <color=#ff0000>{filePath}</color>");
				return false;
			}

			File.Delete(filePath);
			Debug.Log($"セーブファイルを削除しました：<color=#ffcc00>{filePath}</color>｜Save file deleted: <color=#ffcc00>{filePath}</color>");
			return true;
		}

		/*------------------------------------------------------------
		ButtonのonClick用：セーブ｜For Button onClick: Save
		------------------------------------------------------------*/
		public void OnClickSave(){
			SaveMap();
		}

		/*------------------------------------------------------------
		ButtonのonClick用：ロード｜For Button onClick: Load
		------------------------------------------------------------*/
		public void OnClickLoad(){
			LoadMap();
		}

		/*------------------------------------------------------------
		ButtonのonClick用：セーブファイル削除｜For Button onClick: Delete save
		------------------------------------------------------------*/
		public void OnClickDeleteSave(){
			DeleteSave();
		}

	}

}
