/*
 * SSSMapGenerator : Ver. 1.2.0
 * Written by Takashi Sowa @ loloop
*/

//▼イベントの実装方法｜How to implement events:
//各ノードタイプのメソッド内に処理を実装します｜Implement processing in each node type method.
//スクリプトから次のノードに進めるには MapGenerator.instance.ToNextNode() を呼び出します｜To advance to the next node from the script, call "MapGenerator.instance.ToNextNode()".
//スクリプトからマップを非アクティブにするには MapGenerator.instance.InactiveMap() を呼び出します｜To hide the map from scripts, call "MapGenerator.instance.InactiveMap()".
//スクリプトからマップをアクティブにするには MapGenerator.instance.ActiveMap() を呼び出します｜To show the map from scripts, call "MapGenerator.instance.ActiveMap()".
//スクリプトからマップを再構築するには MapGenerator.instance.ReGenerate() を呼び出します｜To regenerate the map from scripts, call "MapGenerator.instance.ReGenerate()".
//MapGeneratorの「playOnAwake」が「false」の時（ゲーム実行と同時にマップを生成しない時）、スクリプトからマップを生成するには MapGenerator.instance.Init() を呼び出します｜If MapGenerator's 'playOnAwake' is 'false' (when the map is not generated at the same time as the game), call "MapGenerator.instance.Init()".

using UnityEngine;

//名前空間｜Namespace
namespace S3MG{

	//ノードイベントハンドラー用クラス：ノードイベントの処理を担当｜Node event handler class: Handles node events
	public class NodeEventHandler : MonoBehaviour{

		/*------------------------------------------------------------
		有効化時にイベントを購読｜Subscribe to event when enabled
		------------------------------------------------------------*/
		void OnEnable(){
			MapGenerator.OnNodeEntered += HandleNodeEntered;
		}

		/*------------------------------------------------------------
		無効化時にイベント購読を解除｜Unsubscribe from event when disabled
		------------------------------------------------------------*/
		void OnDisable(){
			MapGenerator.OnNodeEntered -= HandleNodeEntered;
		}

		/*------------------------------------------------------------
		ノード進入時の処理：ノードタイプに応じて処理を分岐｜Handle node entered: Branch processing based on node type
		------------------------------------------------------------*/
		void HandleNodeEntered(NodeData.Type type, Node node){
			//ノードタイプに応じて処理を実行｜Execute processing based on node type
			switch(type){
				case NodeData.Type.Start:
					OnStartNode(node);
					break;
				case NodeData.Type.Camp:
					OnCampNode(node);
					break;
				case NodeData.Type.Shop:
					OnShopNode(node);
					break;
				case NodeData.Type.Event:
					OnEventNode(node);
					break;
				case NodeData.Type.Treasure:
					OnTreasureNode(node);
					break;
				case NodeData.Type.Trap:
					OnTrapNode(node);
					break;
				case NodeData.Type.Enemy:
					OnEnemyNode(node);
					break;
				case NodeData.Type.Middle:
					OnMiddleNode(node);
					break;
				case NodeData.Type.Final:
					OnFinalNode(node);
					break;
				case NodeData.Type.Random:
					OnRandomNode(node);
					break;
			}
		}

		/*------------------------------------------------------------
		各ノードタイプの処理：ここを編集してください｜Processing for each node type: Edit here
		------------------------------------------------------------*/
		void OnStartNode(Node node){
			Debug.Log($"スタートノードに進入しました｜Entered start node");
			//ゲーム開始処理を実装｜Implement game start processing
			//例：次のノードへ進む｜Ex: Advance to next node
			MapGenerator.instance.ToNextNode();
		}

		void OnCampNode(Node node){
			Debug.Log($"キャンプノードに進入しました｜Entered camp node");
			//キャンプ画面を開く処理を実装｜Implement camp screen opening
			//例：セーブを実行：頻繁に呼び出す場合はキャッシュ推奨｜Ex: Execute save: Cache recommended if called frequently
			//MapSaveManager saveManager = MapGenerator.instance.GetComponent<MapSaveManager>();
			//saveManager?.SaveMap();
		}

		void OnShopNode(Node node){
			Debug.Log($"ショップノードに進入しました｜Entered shop node");
			//ショップ画面を開く処理を実装｜Implement shop screen opening
			//例：UIを開いて、閉じたらToNextNode()を呼ぶ｜Ex: Open UI and call ToNextNode() when closed
		}

		void OnEventNode(Node node){
			Debug.Log($"イベントノードに進入しました｜Entered event node");
			//ランダムイベントを実行する処理を実装｜Implement random event execution
		}

		void OnTreasureNode(Node node){
			Debug.Log($"宝箱ノードに進入しました｜Entered treasure node");
			//宝箱を開ける処理を実装｜Implement treasure opening
		}

		void OnTrapNode(Node node){
			Debug.Log($"罠ノードに進入しました｜Entered trap node");
			//罠を発動する処理を実装｜Implement trap activation
		}

		void OnEnemyNode(Node node){
			Debug.Log($"敵ノードに進入しました｜Entered enemy node");
			//戦闘シーンに移行する処理を実装｜Implement battle scene transition
			//例：SceneManager.LoadScene("BattleScene");
		}

		void OnMiddleNode(Node node){
			Debug.Log($"中間ノードに進入しました｜Entered middle node");
			//中間地点の処理を実装｜Implement middle point processing
		}

		void OnFinalNode(Node node){
			Debug.Log($"ファイナルノードに進入しました｜Entered final node");
			//ボス戦シーンに移行する処理を実装｜Implement boss battle scene transition
			//例：ファイナルノードを複数登録している場合はNodeNameで分岐する事もできる｜Ex: If you register multiple final nodes, you can branch by NodeName
			switch(node.NodeName){
				case "Boss01":
					Debug.Log($"ボス01と戦います｜Fighting Boss01");
					break;
				case "Boss02":
					Debug.Log($"ボス02と戦います｜Fighting Boss02");
					break;
				default:
					Debug.Log($"ボス03と戦います｜Fighting Boss03");
					break;
			}
		}

		void OnRandomNode(Node node){
			Debug.Log($"ランダムノードに進入しました｜Entered random node");
			//ランダムな処理を実行｜Execute random processing
		}

	}

}
