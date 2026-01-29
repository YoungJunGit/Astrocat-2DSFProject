/*
 * SSSMapGenerator : Ver. 1.2.0
 * Written by Takashi Sowa @ loloop
*/

using UnityEngine;

//名前空間｜Namespace
namespace S3MG{

	//バリデーション用クラス：生成前のデータチェックを担当｜Validation class: Handles data check before generation
	public class MapValidator : MonoBehaviour{

		/*------------------------------------------------------------
		生成前データチェック：問題があればtrueを返す｜Data check before generation: Returns true if there is a problem
		------------------------------------------------------------*/
		public bool DataCheckBeforeGeneration(
			MapPlacementMode placementMode,
			GameObject mapCanvas,
			RectTransform scrollViewContent,
			Node nodePref,
			bool makeStart,
			NodeData startNodeData,
			NodeData[] finalNodeData,
			FixedNodeData[] fixedNodeData,
			MapNodeData[] mapNodeData
		){
			bool hasError = false;

			//Standaloneモード用チェック｜Standalone mode check
			if(placementMode == MapPlacementMode.Standalone){
				if(mapCanvas == null){
					Debug.Log($"マップを表示するCanvasを登録してください｜Register the Canvas to display the map");
					hasError = true;
				}
				else if(mapCanvas.transform.parent != null){
					Debug.Log($"マップを表示するCanvasはRootに配置してください｜Place the Canvas to display the map at Root");
					hasError = true;
				}
			}
			//ScrollViewモード用チェック｜ScrollView mode check
			else{
				if(scrollViewContent == null){
					Debug.Log($"ScrollViewモードではscrollViewContentを設定してください｜Set scrollViewContent for ScrollView mode");
					hasError = true;
				}
			}

			//プレファブのチェック｜Check prefab
			if(nodePref == null){
				Debug.Log($"ノード用のプレファブを登録してください｜Register the Node Prefab");
				hasError = true;
			}

			//スタート用ノードデータのチェック｜Check start node data
			if(makeStart && startNodeData == null){
				Debug.Log($"スタート用のノードデータを登録してください｜Register the Start Node Data");
				hasError = true;
			}

			//マップ用ノードデータのチェック｜Check map node data
			if(finalNodeData.Length == 0 || mapNodeData.Length == 0){
				Debug.Log($"fixedNodeData以外の全てのマップ用ノードデータを登録してください｜Register all the map node data except for fixedNodeData");
				hasError = true;
			}

			//配列内のnullチェック｜Check for null in arrays
			if(HasNullElement(finalNodeData)){
				Debug.Log($"finalNodeDataのいずれかがnullになっています｜One or more elements in finalNodeData are null");
				hasError = true;
			}

			//マップ用ノードデータのチェック｜Check map node data
			if(HasNullNodeData(mapNodeData)){
				Debug.Log($"mapNodeDataのいずれかがnullになっています｜One or more elements in mapNodeData are null");
				hasError = true;
			}

			//固定用ノードデータのチェック｜Check fixed node data
			if(HasNullNodeData(fixedNodeData)){
				Debug.Log($"fixedNodeDataのいずれかがnullになっています｜One or more elements in fixedNodeData are null");
				hasError = true;
			}

			//シリアライズ可能なノードの存在チェック｜Check for serializable node existence
			if(!HasSerializableNode(mapNodeData)){
				Debug.Log($"mapNodeDataには連続可能なノードを最低1つ登録してください｜Register at least one serializable node in mapNodeData");
				hasError = true;
			}

			//第一フロアから始められるノードの存在チェック｜Check for first floor startable node existence
			if(!HasFirstFloorNode(mapNodeData)){
				Debug.Log($"mapNodeDataには第一フロアから始められるノードを最低1つ登録してください｜Register at least one node that can start from the first floor in mapNodeData");
				hasError = true;
			}

			//出現確率が全て0ではないかチェック｜Check if all chances are not 0
			if(!HasAnyChance(mapNodeData)){
				Debug.Log($"mapNodeDataには出現確率が0より大きいノードを最低1つ登録してください｜Register at least one node with chance greater than 0 in mapNodeData");
				hasError = true;
			}

			return hasError;
		}

		/*------------------------------------------------------------
		配列内にnull要素があるかチェック｜Check if array has null element
		------------------------------------------------------------*/
		bool HasNullElement(NodeData[] array){
			for(int i = 0; i < array.Length; i++){
				if(array[i] == null) return true;
			}
			return false;
		}

		/*------------------------------------------------------------
		MapNodeData配列内にnullのnodeDataがあるかチェック｜Check if MapNodeData array has null nodeData
		------------------------------------------------------------*/
		bool HasNullNodeData(MapNodeData[] array){
			for(int i = 0; i < array.Length; i++){
				if(array[i].nodeData == null) return true;
			}
			return false;
		}

		/*------------------------------------------------------------
		FixedNodeData配列内にnullのnodeDataがあるかチェック｜Check if FixedNodeData array has null nodeData
		------------------------------------------------------------*/
		bool HasNullNodeData(FixedNodeData[] array){
			for(int i = 0; i < array.Length; i++){
				if(array[i].nodeData == null) return true;
			}
			return false;
		}

		/*------------------------------------------------------------
		シリアライズ可能なノードが存在するかチェック｜Check if serializable node exists
		------------------------------------------------------------*/
		bool HasSerializableNode(MapNodeData[] array){
			for(int i = 0; i < array.Length; i++){
				if(array[i].serializable) return true;
			}
			return false;
		}

		/*------------------------------------------------------------
		第一フロアから始められるノードが存在するかチェック｜Check if first floor startable node exists
		------------------------------------------------------------*/
		bool HasFirstFloorNode(MapNodeData[] array){
			for(int i = 0; i < array.Length; i++){
				if(!array[i].reverseAppearedAfter && array[i].appearedAfter == 1){
					return true;
				}
			}
			return false;
		}

		/*------------------------------------------------------------
		出現確率が0より大きいノードが存在するかチェック｜Check if any node has chance greater than 0
		------------------------------------------------------------*/
		bool HasAnyChance(MapNodeData[] array){
			for(int i = 0; i < array.Length; i++){
				if(array[i].chance > 0f) return true;
			}
			return false;
		}

	}

}
