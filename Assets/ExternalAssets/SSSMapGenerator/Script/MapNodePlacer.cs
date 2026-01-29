/*
 * SSSMapGenerator : Ver. 1.2.0
 * Written by Takashi Sowa @ loloop
*/

using System.Collections.Generic;
using UnityEngine;

//名前空間｜Namespace
namespace S3MG{

	//ノード配置用クラス：ノードの配置ロジックを担当｜Node placer class: Handles node placement logic
	public class MapNodePlacer : MonoBehaviour{

		[Header("▼ノードにテキストを表示するかどうか｜Show Text on Nodes")]
		[SerializeField] bool showText = true;
		[Header("▼完全ランダム配置：固定ノードとノードの出現確率は維持｜Completely Random Placement: Fixed nodes and node chances are preserved")]
		[SerializeField] bool allRandom = false;
		[Header("▼スタートノードデータ｜Start Node Data")]
		[SerializeField] NodeData startNodeData;
		[Header("▼ファイナルノードデータ｜Final Node Data")]
		[SerializeField] NodeData[] finalNodeData;
		[Header("▼固定ノードデータ｜Fixed Node Data")]
		[SerializeField] FixedNodeData[] fixedNodeData;
		[Header("▼マップノードデータ｜Map Node Data")]
		[SerializeField] MapNodeData[] mapNodeData;

		public NodeData GetStartNodeData() => startNodeData;//スタートノードデータのゲッター｜Get start node data
		public NodeData[] GetFinalNodeData() => finalNodeData;//ファイナルノードデータのゲッター｜Get final node data
		public FixedNodeData[] GetFixedNodeData() => fixedNodeData;//固定ノードデータのゲッター｜Get fixed node data
		public MapNodeData[] GetMapNodeData() => mapNodeData;//マップノードデータのゲッター｜Get map node data
		public bool GetShowText() => showText;//テキスト表示フラグのゲッター｜Get show text flag
		public bool GetAllRandom() => allRandom;//完全ランダム配置フラグのゲッター｜Get all random flag

		int maxLoopIterations = 100;//最大ループ回数｜Maximum loop iterations

		/*------------------------------------------------------------
		各ノードを設定｜Set each node
		------------------------------------------------------------*/
		public void SetNode(Node startNode, Node finalNode, Node[,] map, int floorNum, int routeNum, bool makeStart){
			//スタートnode配置｜Place start node
			if(makeStart){
				startNode.SetNodeData(startNodeData.sprite, startNodeData.type, showText ? startNodeData.nodeName : "");
			}

			//ファイナルnode配置：複数ある中から1つ｜Place final node: one from multiple
			int finalNodeIndex = Random.Range(0, finalNodeData.Length);
			NodeData selectedFinalData = finalNodeData[finalNodeIndex];
			finalNode.SetNodeData(selectedFinalData.sprite, selectedFinalData.type, showText ? selectedFinalData.nodeName : "");

			//各マップnodeの出現確率を合計｜Calculate total chance for map nodes
			float totalChance = CalculateTotalChance();

			//マップnode配置｜Place map nodes
			if(allRandom){
				List<Node> unassignedNodes = GetUnassignedConnectedNodes(map, floorNum, routeNum);
				RandomlyAssignNodes(unassignedNodes, totalChance, true);
			}
			else{
				AssignNodesWithRules(map, floorNum, routeNum, totalChance);
			}

			//固定node配置｜Place fixed nodes
			AssignFixedNodes(map, floorNum, routeNum);
		}

		/*------------------------------------------------------------
		出現確率の合計を計算｜Calculate total chance
		------------------------------------------------------------*/
		float CalculateTotalChance(){
			float total = 0f;
			for(int i = 0; i < mapNodeData.Length; i++){
				total += mapNodeData[i].chance;
			}
			return total;
		}

		/*------------------------------------------------------------
		ルールを適用してノードを配置｜Assign nodes with rules
		------------------------------------------------------------*/
		void AssignNodesWithRules(Node[,] map, int floorNum, int routeNum, float totalChance){
			List<Node> unassignedNodes = GetUnassignedConnectedNodes(map, floorNum, routeNum);
			int loopCount = 0;

			while(unassignedNodes.Count > 0 && loopCount < maxLoopIterations){
				loopCount++;

				//ルールを適用してノードを配置｜Apply rules and assign nodes
				ApplyRulesAndAssignNode(unassignedNodes, totalChance);
				//分岐の重複ノードをリセット｜Reset duplicate branch nodes
				ResetDuplicateBranchNodes(map, floorNum, routeNum);
				//未設定ノードを再取得｜Re-get unassigned nodes
				unassignedNodes = GetUnassignedConnectedNodes(map, floorNum, routeNum);
			}

			//まだ未設定ノードがあればランダム配置｜Randomly assign if unassigned nodes remain
			if(unassignedNodes.Count > 0){
				RandomlyAssignNodes(unassignedNodes, totalChance);
				Debug.Log($"ループが{maxLoopIterations}を超えたのでランダムに配置しました｜Randomly assigned because loop exceeded {maxLoopIterations}");
			}
		}

		/*------------------------------------------------------------
		ルールを適用してノードを配置（内部処理）｜Apply rules and assign nodes (internal)
		------------------------------------------------------------*/
		void ApplyRulesAndAssignNode(List<Node> unassignedNodes, float totalChance){
			for(int i = 0; i < unassignedNodes.Count; i++){
				MapNodeData selectedNodeData = SelectRandomNodeData(totalChance);
				if(selectedNodeData != null && CanPlaceNode(unassignedNodes[i], selectedNodeData)){
					NodeData nodeData = selectedNodeData.nodeData;
					unassignedNodes[i].SetNodeData(nodeData.sprite, nodeData.type, showText ? nodeData.nodeName : "");
				}
			}
		}

		/*------------------------------------------------------------
		未設定ノードリストを返す｜Return unassigned node list
		------------------------------------------------------------*/
		List<Node> GetUnassignedConnectedNodes(Node[,] map, int floorNum, int routeNum){
			List<Node> unassignedNodes = new List<Node>();
			for(int i = 0; i < floorNum; i++){
				for(int j = 0; j < routeNum; j++){
					if(map[i,j].connected && map[i,j].nodeType == null){
						unassignedNodes.Add(map[i,j]);
					}
				}
			}
			return unassignedNodes;
		}

		/*------------------------------------------------------------
		出現確率に応じてnodeを返す｜Return node based on chance
		------------------------------------------------------------*/
		MapNodeData SelectRandomNodeData(float totalChance){
			float randNum = Random.Range(0f, totalChance);
			float cumulativeChance = 0f;

			for(int j = 0; j < mapNodeData.Length; j++){
				cumulativeChance += mapNodeData[j].chance;
				if(randNum < cumulativeChance){
					return mapNodeData[j];
				}
			}
			return null;
		}

		/*------------------------------------------------------------
		配置可能かチェック｜Check if placement is possible
		------------------------------------------------------------*/
		bool CanPlaceNode(Node node, MapNodeData selectedNodeData){
			bool canPlace = false;

			//登場フロアの条件チェック｜Check floor appearance condition
			if(!selectedNodeData.reverseAppearedAfter){
				canPlace = node.floor >= selectedNodeData.appearedAfter - 1;
			}
			else{
				canPlace = node.floor < selectedNodeData.appearedAfter - 1;
			}

			//連続配置不可の場合、隣接ノードをチェック｜Check adjacent nodes if not serializable
			if(canPlace && !selectedNodeData.serializable){
				NodeData.Type targetType = selectedNodeData.nodeData.type;

				//接続先ノードに同じタイプがあるかチェック｜Check if next nodes have same type
				for(int i = 0; i < node.nextNodes.Count; i++){
					if(node.nextNodes[i].nodeType == targetType){
						return false;
					}
				}
				//接続元ノードに同じタイプがあるかチェック｜Check if previous nodes have same type
				for(int i = 0; i < node.prevNodes.Count; i++){
					if(node.prevNodes[i].nodeType == targetType){
						return false;
					}
				}
			}

			return canPlace;
		}

		/*------------------------------------------------------------
		重複している分岐ノードをリセット｜Reset duplicate branch nodes
		------------------------------------------------------------*/
		void ResetDuplicateBranchNodes(Node[,] map, int floorNum, int routeNum){
			for(int i = 0; i < floorNum; i++){
				for(int j = 0; j < routeNum; j++){
					Node node = map[i,j];
					if(node.nodeType == null) continue;

					int branchCount = node.nextNodes.Count;
					if(branchCount < 2) continue;

					//2分岐の場合｜If 2 branches
					if(branchCount == 2){
						if(AreSameTypeAndDifferentPosition(node.nextNodes[0], node.nextNodes[1])){
							node.nextNodes[0].SetNodeData(null, null);
						}
					}
					//3分岐の場合｜If 3 branches
					else if(branchCount == 3){
						Node first = node.nextNodes[0];
						Node second = node.nextNodes[1];
						Node third = node.nextNodes[2];

						//全て同じタイプの場合｜If all same type
						if(AreSameType(first, second) && AreSameType(second, third) && AreDifferentPositions(first, second, third)){
							first.SetNodeData(null, null);
							second.SetNodeData(null, null);
						}
						else{
							//部分的に同じタイプの場合｜If partially same type
							if(AreSameTypeAndDifferentPosition(first, second)) first.SetNodeData(null, null);
							if(AreSameTypeAndDifferentPosition(first, third)) first.SetNodeData(null, null);
							if(AreSameTypeAndDifferentPosition(second, third)) second.SetNodeData(null, null);
						}
					}
				}
			}
		}

		/*------------------------------------------------------------
		2つのノードが同じタイプで位置が異なるかチェック｜Check if 2 nodes have same type and different position
		------------------------------------------------------------*/
		bool AreSameTypeAndDifferentPosition(Node a, Node b){
			return a.nodeType == b.nodeType && !Mathf.Approximately(a.xPos, b.xPos);
		}

		/*------------------------------------------------------------
		2つのノードが同じタイプかチェック｜Check if 2 nodes have same type
		------------------------------------------------------------*/
		bool AreSameType(Node a, Node b){
			return a.nodeType == b.nodeType;
		}

		/*------------------------------------------------------------
		3つのノードの位置が全て異なるかチェック｜Check if 3 nodes have different positions
		------------------------------------------------------------*/
		bool AreDifferentPositions(Node a, Node b, Node c){
			return !Mathf.Approximately(a.xPos, b.xPos) && !Mathf.Approximately(b.xPos, c.xPos);
		}

		/*------------------------------------------------------------
		固定ノードを配置｜Assign fixed nodes
		------------------------------------------------------------*/
		void AssignFixedNodes(Node[,] map, int floorNum, int routeNum){
			for(int i = 0; i < fixedNodeData.Length; i++){
				int targetFloor = fixedNodeData[i].appearedOn - 1;
				if(targetFloor >= floorNum) continue;

				NodeData nodeData = fixedNodeData[i].nodeData;
				for(int j = 0; j < routeNum; j++){
					if(map[targetFloor, j].connected){
						map[targetFloor, j].SetNodeData(nodeData.sprite, nodeData.type, showText ? nodeData.nodeName : "");
					}
				}
			}
		}

		/*------------------------------------------------------------
		ランダムにノードを配置｜Randomly assign nodes
		------------------------------------------------------------*/
		void RandomlyAssignNodes(List<Node> unassignedNodes, float totalChance, bool ignoreRules = false){
			for(int i = 0; i < unassignedNodes.Count; i++){
				//ルール無視モードの場合は即座にランダム配置｜If ignoring rules, place randomly immediately
				if(ignoreRules){
					MapNodeData randomNodeData = SelectRandomNodeData(totalChance);
					if(randomNodeData != null){
						NodeData nodeData = randomNodeData.nodeData;
						unassignedNodes[i].SetNodeData(nodeData.sprite, nodeData.type, showText ? nodeData.nodeName : "");
					}
					continue;
				}

				//配置可能なノードが見つかるまでリトライ｜Retry until a placeable node is found
				int retryCount = 0;
				int maxRetry = mapNodeData.Length * 3;//リトライ上限｜Max retry count
				bool placed = false;
				MapNodeData lastTriedNodeData = null;//最後に試行したノード｜Last tried node

				while(!placed && retryCount < maxRetry){
					lastTriedNodeData = SelectRandomNodeData(totalChance);
					if(lastTriedNodeData != null && CanPlaceNode(unassignedNodes[i], lastTriedNodeData)){
						NodeData nodeData = lastTriedNodeData.nodeData;
						unassignedNodes[i].SetNodeData(nodeData.sprite, nodeData.type, showText ? nodeData.nodeName : "");
						placed = true;
					}
					retryCount++;
				}

				//リトライ上限を超えた場合は配置可能な任意のノードを探す｜If max retry exceeded, find any placeable node
				if(!placed){
					for(int j = 0; j < mapNodeData.Length; j++){
						if(CanPlaceNode(unassignedNodes[i], mapNodeData[j])){
							NodeData nodeData = mapNodeData[j].nodeData;
							unassignedNodes[i].SetNodeData(nodeData.sprite, nodeData.type, showText ? nodeData.nodeName : "");
							placed = true;
							break;
						}
					}
				}

				//それでも配置できない場合はルール無視で強制配置｜Force place ignoring rules if still not placed
				if(!placed){
					NodeData fallbackData = (lastTriedNodeData != null) ? lastTriedNodeData.nodeData : mapNodeData[0].nodeData;
					unassignedNodes[i].SetNodeData(fallbackData.sprite, fallbackData.type, showText ? fallbackData.nodeName : "");
					Debug.Log($"フロア{unassignedNodes[i].floor}にルール無視で強制配置しました｜Forced placement ignoring rules for floor {unassignedNodes[i].floor}");
				}
			}
		}

	}

}
