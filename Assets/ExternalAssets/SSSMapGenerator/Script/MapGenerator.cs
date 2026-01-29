/*
 * SSSMapGenerator : Ver. 1.2.0
 * Written by Takashi Sowa @ loloop
*/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;//インプットシステムを使う為に必要｜Required to use the InputSystem
using UnityEngine.EventSystems;//EventSystemを使う為に必要｜Required to use the EventSystem

//名前空間｜Namespace
namespace S3MG{

	//マップ配置モード｜Map placement mode
	public enum MapPlacementMode{
		ScrollView,//ScrollViewのContent内に配置｜Place inside ScrollView Content
		Standalone//Canvas直下に配置｜Place under Canvas
	}

	//マップ生成クラス｜Map generator class
	public class MapGenerator : MonoBehaviour{

		//シングルトン用インスタンス｜Singleton instance
		public static MapGenerator instance;

		[Header("▼マップ配置モード｜Map Placement Mode")]
		[SerializeField] MapPlacementMode placementMode = MapPlacementMode.ScrollView;

		[Header("▼ScrollViewモード用：ContentのRectTransform｜For ScrollView Mode: Content RectTransform")]
		[SerializeField] RectTransform scrollViewContent;

		[Header("▼ノードの処理をスキップするかどうか：trueならノードの処理はスキップされ、ステージのみが移動｜Skip Node Processing : If true, node processing is skipped, and only the stage advances")]
		public bool skipNodeProcessing = false;

		[Header("▼実行と同時にマップ生成するかどうか｜Generate on Execution")]
		[SerializeField] bool playOnAwake = true;

		[Header("▼スタートノードを作るかどうか｜Make Start Node")]
		[SerializeField] bool makeStart = true;

		[Header("▼ノードをずらして配置するかどうか｜Offset Nodes in Placement")]
		[SerializeField] bool isOffset = true;

		[Header("▼パスを交差可能にするかどうか｜Allow Path Intersection")]
		[SerializeField] bool crossable = false;

		[Header("▼マップ生成にランダムseedを使うか固定seedを使うか：trueの場合は変数seedの値無効｜Whether to randomize the seed : If true, the value of the variable seed is invalid")]
		[SerializeField] bool randomizeSeed = true;
		[SerializeField] int seed = 0;

		[Header("▼マップ基本設定｜Map Basic Settings")]
		[SerializeField, Range(1,128)] int floorNum = 15;//フロア数｜Number of floors
		[SerializeField, Range(1,32)] int routeNum = 7;//ルート数｜Number of routes
		[SerializeField, Range(32,256)] float normalNodeSize = 80;//通常ノードの大きさ｜Normal node size
		[SerializeField, Range(32,256)] float startNodeSize = 160;//スタートノードの大きさ｜Start node size
		[SerializeField, Range(32,256)] float finalNodeSize = 160;//ファイナルノードの大きさ｜Final node size
		[SerializeField] float floorDistance = 80;//フロア間の距離｜Distance between floors
		[SerializeField] float routeDistance = 80;//ルート間の距離｜Distance between routes

		[Header("▼アクティブルート数：RouteNumより多い場合はRouteNumに制限｜Active Route Count : Limited to RouteNum if exceeded")]
		[SerializeField, Range(1,20)] int activeRouteNum = 4;
		Node[] activeRouteNodes;//アクティブルートnode格納配列｜Array for active route nodes

		[Header("▼マップを表示するCanvas｜Canvas to Display Map")]
		[SerializeField] GameObject mapCanvas;
		RectTransform mapParent;//マップを入れる親オブジェクト｜Parent object for the map

		[Header("▼ノード用Prefab：Nodeクラスを持つButtonPrefab｜Node Prefab: ButtonPrefab with Node class")]
		[SerializeField] Node nodePref;

		[Header("▼背景Prefab：9スライスが設定されたImageのPrefab、なくても可｜Background Prefab : Prefab is not required, Image Prefab with 9-slice scaling")]
		[SerializeField] GameObject backgroundPref;

		[Header("▼背景とマップの余白｜Padding Between Background and Map")]
		[SerializeField, Range(0,160)] float backgroundPadding = 80;

		[Header("▼コンポーネント参照｜Component References")]
		[SerializeField] MapNodePlacer nodePlacer;//ノード配置コンポーネント｜Node placer component
		[SerializeField] MapPathRenderer pathRenderer;//パス描画コンポーネント｜Path renderer component
		[SerializeField] MapDragController dragController;//マップドラッグ操作コンポーネント｜Map drag controller component
		[SerializeField] MapValidator validator;//バリデーションコンポーネント｜Validation component

		Node startNode;//スタートnode格納用｜Start node
		Node finalNode;//ファイナルnode格納用｜Final node
		Node[,] map;//各エリアnode格納用二次元配列｜2D array for area nodes
		float mapWidth;//マップ全体の幅｜Total map width
		float mapHeight;//マップ全体の高さ｜Total map height

		float canvasWidth;//Canvasの幅｜Canvas width
		float canvasHeight;//Canvasの高さ｜Canvas height
		float OffsetMultiplier = 0.9f;//隣同士が近くなりすぎないようにするオフセット係数｜Offset multiplier to prevent nodes from being too close to each other
		public bool IsCompleted {get; private set;} = false;//マップの生成が完了したかどうか｜Whether map generation is completed

		//現在選択中のノード｜Currently selected node
		public Node nowNode {get;set;}

		//各ノードのイベント実行時や別操作の時に操作無効にしたいタイミングで外部から切り替え｜Switch from outside when you want to disable operations during node event execution or other operations
		public bool noMapOperation {get;set;} = false;

		//セーブ/ロード用ゲッター｜Getters for save/load
		public int GetSeed() => seed;
		public Node[,] GetMap() => map;
		public Node GetStartNode() => startNode;
		public Node GetFinalNode() => finalNode;
		public RectTransform GetMapParent() => mapParent;
		public ScrollRect GetScrollRect() => scrollViewContent != null ? scrollViewContent.GetComponentInParent<ScrollRect>() : null;

		//ノード進入イベント：外部からノードタイプに応じた処理を登録可能｜Node entered event: External scripts can register handlers for node types
		public static event System.Action<NodeData.Type, Node> OnNodeEntered;

		/*------------------------------------------------------------
		ノードイベント発火：Node.csから呼び出される｜Fire node event: Called from Node.cs
		------------------------------------------------------------*/
		public static void InvokeNodeEntered(NodeData.Type type, Node node){
			OnNodeEntered?.Invoke(type, node);
		}

		/*------------------------------------------------------------
		MonoBehaviourが作られた時に1度だけ実行｜Executed only once when MonoBehaviour is created
		------------------------------------------------------------*/
		void Awake(){
			//シングルトン化｜Singleton pattern
			if(instance == null){
				instance = this;
				//StandaloneモードのみDontDestroyOnLoadを適用｜Apply DontDestroyOnLoad only in Standalone mode
				if(placementMode == MapPlacementMode.Standalone){
					DontDestroyOnLoad(gameObject);
					DontDestroyOnLoad(mapCanvas);
				}

				//コンポーネント参照のnullチェックと自動取得｜Null check and auto-get component references
				if(validator == null) validator = GetComponent<MapValidator>();
				if(dragController == null) dragController = GetComponent<MapDragController>();
				if(pathRenderer == null) pathRenderer = GetComponent<MapPathRenderer>();
				if(nodePlacer == null) nodePlacer = GetComponent<MapNodePlacer>();
			}else{
				Destroy(gameObject);
				if(placementMode == MapPlacementMode.Standalone) Destroy(mapCanvas);
			}
		}

		/*------------------------------------------------------------
		Awakeの1フレーム後に実行｜Executed one frame after Awake
		------------------------------------------------------------*/
		void Start(){
			//実行と同時に生成するなら処理開始｜Start processing if generating on execution
			if(playOnAwake) Init();
		}

		/*------------------------------------------------------------
		初期化｜Initialize
		------------------------------------------------------------*/
		public void Init(){
			//生成前データチェックして問題があればログを表示して早期リターン｜Check data before generation, show log and return early if there is a problem
			if(validator.DataCheckBeforeGeneration(
				placementMode,
				mapCanvas,
				scrollViewContent,
				nodePref,
				makeStart,
				nodePlacer.GetStartNodeData(),
				nodePlacer.GetFinalNodeData(),
				nodePlacer.GetFixedNodeData(),
				nodePlacer.GetMapNodeData()
			)) return;

			//シードのランダム化が有効なら0からint.MaxValueの範囲内でランダムな整数を生成｜Generate random integer if seed randomization is enabled
			if(randomizeSeed) seed = Mathf.FloorToInt(Random.value * int.MaxValue);
			//変数seedを引数にしてRandom.InitStateで各random関数の元になる値を設定｜Set seed value for random functions
			Random.InitState(seed);

			//事前に必要なものを生成｜Pre-generate required objects
			PreGenerated();
			//マップを作成｜Create map
			CreateMap();
			//アクティブルートnodeを選択して配列を生成｜Select active route nodes and create array
			SelectFirstNode();
			//アクティブルートnode数分のルートをマップに反映｜Apply routes to map for each active route node
			for(int i = 0; i < activeRouteNodes.Length; i++){
				ConnectingNodes(activeRouteNodes[i]);
			}
			//背景プレファブがnullでなければ背景を生成｜Generate background if background prefab is not null
			if(backgroundPref != null) GenerateBackground();
			//未通過nodeを非表示｜Hide unconnected nodes
			HideEmpty();
			//各nodeを設定｜Set each node
			nodePlacer.SetNode(startNode, finalNode, map, floorNum, routeNum, makeStart);
			//開始する為のノード選択｜Select node to start
			ActiveNodeSelect();
			//生成完了フラグを立てる｜Set generation completed flag
			IsCompleted = true;
		}

		/*------------------------------------------------------------
		事前生成｜Pre-generate
		------------------------------------------------------------*/
		void PreGenerated(){
			//Standaloneモード：Canvas Scalerを画面サイズに合わせる｜Standalone mode: Adjust Canvas Scaler to screen size
			if(placementMode == MapPlacementMode.Standalone){
				CanvasScaler scaler = mapCanvas.GetComponent<CanvasScaler>();
				scaler.referenceResolution = new Vector2(Screen.width, Screen.height);
				Canvas.ForceUpdateCanvases();//レイアウトを強制更新してregenerate時にも即座に反映｜Force layout update for immediate reflection on regenerate
				canvasWidth = Screen.width;
				canvasHeight = Screen.height;

				//scrollViewContentが設定されていればScrollViewを非表示にする｜Hide ScrollView if scrollViewContent is set
				if(scrollViewContent != null){
					ScrollRect scrollRect = scrollViewContent.GetComponentInParent<ScrollRect>();
					if(scrollRect != null) scrollRect.gameObject.SetActive(false);
				}
			}
			//ScrollViewモード：ScrollViewが属するCanvasのCanvas Scalerも調整｜ScrollView mode: Also adjust Canvas Scaler of Canvas containing ScrollView
			else{
				//ScrollViewが属するCanvasを取得してCanvas Scalerを調整｜Get Canvas containing ScrollView and adjust Canvas Scaler
				Canvas scrollViewCanvas = scrollViewContent.GetComponentInParent<Canvas>();
				if(scrollViewCanvas != null){
					CanvasScaler scaler = scrollViewCanvas.GetComponent<CanvasScaler>();
					if(scaler != null){
						scaler.referenceResolution = new Vector2(Screen.width, Screen.height);
					}
				}
				Canvas.ForceUpdateCanvases();//ScrollViewのレイアウト更新｜Update ScrollView layout
				canvasWidth = scrollViewContent.rect.width;
				canvasHeight = scrollViewContent.rect.height;
			}

			//Mapを入れる親オブジェクトを作る｜Create parent object for the map
			GameObject parentObject = new GameObject("Parent");
			parentObject.layer = LayerMask.NameToLayer("UI");
			mapParent = parentObject.AddComponent<RectTransform>();

			//配置先を切り替え｜Switch placement target
			if(placementMode == MapPlacementMode.Standalone){
				mapParent.SetParent(mapCanvas.transform);
			}else{
				mapParent.SetParent(scrollViewContent);
			}

			mapParent.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0);
			mapParent.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
			mapParent.anchorMin = new Vector2(0.5f, 0.5f);
			mapParent.anchorMax = new Vector2(0.5f, 0.5f);
			mapParent.pivot = new Vector2(0.5f, 0.5f);
			//初期位置は(0,0)に設定：実際の中央揃えはCreateMap()でマップサイズ確定後に行う｜Initial position is (0,0): Actual centering is done in CreateMap() after map size is determined
			mapParent.anchoredPosition = Vector2.zero;
			mapParent.SetAsFirstSibling();

			//パスレンダラーを初期化｜Initialize path renderer
			pathRenderer.Initialize(mapParent);
		}

		/*------------------------------------------------------------
		マップを作成｜Create map
		------------------------------------------------------------*/
		void CreateMap(){
			map = new Node[floorNum,routeNum];//フロア数とルート数を元に二次元配列作成｜Create 2D array based on floor and route count

			//スタートnodeを作るならスタートnodeを作成｜Create start node if makeStart is true
			if(makeStart){
				startNode = Instantiate(nodePref, mapParent);
				SetNodeSize(startNode, startNodeSize);
				startNode.CachedRectTransform.anchoredPosition = Vector2.zero;
				startNode.gameObject.name = "Start Node";
				startNode.connected = true;
			}

			//格子状にnodeを作成｜Create nodes in a grid
			for(int floorIndex = 0; floorIndex < floorNum; floorIndex++){
				for(int routeIndex = 0; routeIndex < routeNum; routeIndex++){
					//nodeを生成｜Generate node
					Node node = Instantiate(nodePref, mapParent);
					SetNodeSize(node, normalNodeSize);
					//nodeに自分のフロアとルートを保持｜Store floor and route in node
					node.floor = floorIndex;
					node.route = routeIndex;
					//nodeに自分の座標を保持｜Store position in node
					node.xPos = (routeDistance * routeIndex) + (normalNodeSize * routeIndex) - (routeDistance * (routeNum - 1) / 2 + (normalNodeSize * (routeNum - 1) / 2));
					//Y位置：スタートノードの上端からfloorDistanceの距離に配置｜Y position: place at floorDistance from start node top
					if(makeStart){
						//スタートノード上端(startNodeSize/2) + floorDistance + 通常ノード下側までの距離(normalNodeSize/2) + フロア間距離｜Start node top + floorDistance + distance to normal node bottom + floor spacing
						node.yPos = (startNodeSize / 2) + floorDistance + (normalNodeSize / 2) + (floorDistance + normalNodeSize) * floorIndex;
					}else{
						node.yPos = (floorDistance + normalNodeSize) * floorIndex;
					}
					//少しずらして配置するなら｜If offset is enabled
					if(isOffset){
						node.xPos += Random.Range(-routeDistance * OffsetMultiplier / 2, routeDistance * OffsetMultiplier / 2 + 1);
						node.yPos += Random.Range(-floorDistance * OffsetMultiplier / 2, floorDistance * OffsetMultiplier / 2 + 1);
					}
					//自分の座標を元に配置｜Place node based on position
					node.CachedRectTransform.anchoredPosition = new Vector2(node.xPos, node.yPos);
					//二次元配列にnodeを格納｜Store node in 2D array
					map[floorIndex,routeIndex] = node;
					//ゲームオブジェクト名を座標にする｜Set game object name to coordinates
					node.gameObject.name = $"{floorIndex},{routeIndex}";
				}
			}

			//ファイナルnodeを作成｜Create final node
			finalNode = Instantiate(nodePref, mapParent);
			SetNodeSize(finalNode, finalNodeSize);
			//ファイナルノードY位置：最終フロアの上端からfloorDistanceの距離に配置｜Final node Y position: place at floorDistance from last floor top
			float lastFloorY;
			if(makeStart){
				lastFloorY = (startNodeSize / 2) + floorDistance + (normalNodeSize / 2) + (floorDistance + normalNodeSize) * (floorNum - 1);
			}else{
				lastFloorY = (floorDistance + normalNodeSize) * (floorNum - 1);
			}
			//最終フロア上端(lastFloorY + normalNodeSize/2) + floorDistance + ファイナルノード下側(finalNodeSize/2)｜Last floor top + floorDistance + final node bottom
			float finalNodeY = lastFloorY + (normalNodeSize / 2) + floorDistance + (finalNodeSize / 2);
			finalNode.CachedRectTransform.anchoredPosition = new Vector2(0, finalNodeY);
			finalNode.gameObject.name = "Final Node";
			finalNode.connected = true;

			//マップ全体の幅と高さを保持｜Store total map width and height
			mapWidth = (routeNum * normalNodeSize) + (routeNum * routeDistance);

			//背景パディングを含むマップの上下端座標（mapParent座標系）｜Map top/bottom Y including background padding (in mapParent coordinate)
			float bottomNodeHalf = ((makeStart) ? startNodeSize : normalNodeSize) / 2;
			float mapBottomY = -bottomNodeHalf - backgroundPadding;
			float mapTopY = finalNodeY + (finalNodeSize / 2) + backgroundPadding;

			//マップの高さ（背景パディング含まず）｜Map height (without background padding)
			mapHeight = (mapTopY - backgroundPadding) - (mapBottomY + backgroundPadding);

			//マップ下端を画面下端に配置：Standaloneモードのみ｜Align map bottom to screen bottom: Standalone mode only
			if(placementMode == MapPlacementMode.Standalone){
				mapParent.anchoredPosition = new Vector2(0, -canvasHeight / 2 - mapBottomY);
			}

			//Standaloneモード：ドラッグコントローラーを初期化｜Standalone mode: Initialize drag controller
			if(placementMode == MapPlacementMode.Standalone){
				dragController.Initialize(mapParent, mapWidth, mapHeight, backgroundPadding, canvasWidth, canvasHeight, mapTopY, mapBottomY);
			}
			//ScrollViewモード：Contentサイズを調整、ScrollRect操作モードで初期化｜ScrollView mode: Adjust Content size, initialize in ScrollRect control mode
			else{
				AdjustScrollViewContent(mapTopY, mapBottomY);
			}
		}

		/*------------------------------------------------------------
		ノードのサイズを設定する｜Set node size
		------------------------------------------------------------*/
		void SetNodeSize(Node target, float size){
			RectTransform rectTransform = target.CachedRectTransform;
			rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
			rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);
		}

		/*------------------------------------------------------------
		アクティブルートnodeをランダムで選んで配列に格納｜Randomly select active route nodes and store in array
		------------------------------------------------------------*/
		void SelectFirstNode(){
			//ルート数分で初期化された一時リストを作成｜Create temporary list initialized with route count
			List<Node> firstFloorNodes = new List<Node>(routeNum);
			//一時リストに第一フロアの全nodeを格納｜Store all first floor nodes in temporary list
			for(int i = 0; i < routeNum; i++){
				firstFloorNodes.Add(map[0,i]);
			}

			//アクティブルート数をルート数の範囲内にクランプ｜Clamp active route count within route count range
			activeRouteNum = Mathf.Clamp(activeRouteNum, 1, routeNum);

			//一時リストから不要な要素を削除｜Remove unnecessary elements from temporary list
			int removeCount = routeNum - activeRouteNum;
			for(int i = 0; i < removeCount; i++){
				firstFloorNodes.RemoveAt(Random.Range(0, firstFloorNodes.Count));
			}

			//残った要素を配列に格納｜Store remaining elements in array
			activeRouteNodes = firstFloorNodes.ToArray();
		}

		/*------------------------------------------------------------
		ノードを接続｜Connect nodes
		------------------------------------------------------------*/
		void ConnectingNodes(Node node){
			if(!node.connected) node.connected = true;
			//最終フロアでなければ処理｜Process if not final floor
			if(node.floor < floorNum - 1){
				int connectDirection = Random.Range(-1,2);//-1が左上、0が真上、1が右上｜-1: upper left, 0: directly above, 1: upper right
				if(node.route == 0 && connectDirection == -1) connectDirection++;
				if(node.route == routeNum - 1 && connectDirection == 1) connectDirection--;

				//交差不可の場合｜If crossing is not allowed
				if(!crossable){
					//自分のrouteが左端ではない、かつ接続先が左上の時｜When not leftmost and connecting to upper left
					if(node.route != 0 && connectDirection == -1){
						Node leftNeighbor = map[node.floor, node.route - 1];
						//自分の左隣りのnodeの接続先に自分の真上のnodeがあったら｜If left neighbor's next nodes contain the node directly above
						if(leftNeighbor.nextNodes.Contains(map[node.floor + 1, node.route])){
							connectDirection++;
						}
					}
					//自分のrouteが右端ではない、かつ接続先が右上の時｜When not rightmost and connecting to upper right
					if(node.route != routeNum - 1 && connectDirection == 1){
						Node rightNeighbor = map[node.floor, node.route + 1];
						//自分の右隣りのnodeの接続先に自分の真上のnodeがあったら｜If right neighbor's next nodes contain the node directly above
						if(rightNeighbor.nextNodes.Contains(map[node.floor + 1, node.route])){
							connectDirection--;
						}
					}
				}

				//スタートノードを作る設定で第一フロアの時｜When making start node and on first floor
				if(makeStart && node.floor == 0){
					startNode.nextNodes.Add(node);
					node.prevNodes.Add(startNode);
					pathRenderer.DrawPath(startNode, node);
				}

				//接続先のnodeを取得｜Get next node
				Node nextNode = map[node.floor + 1, node.route + connectDirection];

				//自分の接続先nodeリストにすでに同じnodeがなければパス描画｜Draw path if not already in next nodes list
				if(!node.nextNodes.Contains(nextNode)){
					pathRenderer.DrawPath(node, nextNode);
				}

				node.nextNodes.Add(nextNode);
				nextNode.prevNodes.Add(node);

				ConnectingNodes(nextNode);//再帰呼び出し｜Recursive call
			}
			//最終フロアなら｜If final floor
			else{
				//自分の接続先nodeリストにfinalNodeがなければパス描画｜Draw path if finalNode not in next nodes list
				if(!node.nextNodes.Contains(finalNode)){
					pathRenderer.DrawPath(node, finalNode);
				}

				node.nextNodes.Add(finalNode);
				finalNode.prevNodes.Add(node);

				//最終フロアが唯一のフロアで、かつスタートノードを作る設定なら｜If only one floor and making start node
				if(floorNum == 1 && makeStart){
					startNode.nextNodes.Add(node);
					node.prevNodes.Add(startNode);
					pathRenderer.DrawPath(startNode, node);
				}
			}
		}

		/*------------------------------------------------------------
		背景を生成｜Generate background
		------------------------------------------------------------*/
		void GenerateBackground(){
			GameObject bg = Instantiate(backgroundPref, mapParent);
			RectTransform rectTransform = bg.GetComponent<RectTransform>();
			rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, mapWidth + (backgroundPadding * 2));
			rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, mapHeight + (backgroundPadding * 2));
			Vector2 pos = rectTransform.anchoredPosition;
			rectTransform.anchoredPosition = new Vector2(pos.x, pos.y - ((makeStart) ? startNodeSize / 2 : normalNodeSize / 2) - backgroundPadding);
			bg.transform.SetAsFirstSibling();
		}

		/*------------------------------------------------------------
		未通過ノードを非表示｜Hide unconnected nodes
		------------------------------------------------------------*/
		void HideEmpty(){
			for(int i = 0; i < floorNum; i++){
				for(int j = 0; j < routeNum; j++){
					if(!map[i,j].connected) map[i,j].gameObject.SetActive(false);
				}
			}
		}

		/*------------------------------------------------------------
		開始する為のノード選択｜Select node to start
		------------------------------------------------------------*/
		void ActiveNodeSelect(){
			//ファイナルノードと全ノードを押せないようButtonコンポーネント無効化｜Disable button component for final node and all nodes
			finalNode.DisableButton();
			for(int i = 0; i < floorNum; i++){
				for(int j = 0; j < routeNum; j++){
					if(map[i,j].connected && map[i,j].nodeType != null) map[i,j].DisableButton();
				}
			}

			//スタートノードを作っているなら｜If making start node
			if(makeStart){
				if(Gamepad.current != null) EventSystem.current.SetSelectedGameObject(startNode.gameObject);
			}
			//スタートノードを作っていないなら｜If not making start node
			else{
				for(int i = 0; i < activeRouteNodes.Length; i++){
					activeRouteNodes[i].EnableButton();
				}
				if(Gamepad.current != null) EventSystem.current.SetSelectedGameObject(activeRouteNodes[0].gameObject);
			}
		}

		/*------------------------------------------------------------
		通過パスを塗る｜Paint passed path
		------------------------------------------------------------*/
		public void PaintPath(Node node){
			pathRenderer.PaintPath(nowNode, node, makeStart);
		}

		/*------------------------------------------------------------
		引数と同一フロアのノードを押せないように通過済にする｜Mark nodes on the same floor as passed
		------------------------------------------------------------*/
		public void PassedSameFloor(Node node){
			for(int j = 0; j < routeNum; j++){
				if(map[node.floor, j].connected){
					map[node.floor, j].PassedNode();
				}
			}
		}

		/*------------------------------------------------------------
		次のノードに進む：外部クラスからの操作用｜Move to next node: For external class operation
		------------------------------------------------------------*/
		public void ToNextNode(){
			if(nowNode == finalNode) return;
			for(int i = 0; i < nowNode.nextNodes.Count; i++){
				nowNode.nextNodes[i].EnableButton();
			}
			if(Gamepad.current != null) EventSystem.current.SetSelectedGameObject(nowNode.nextNodes[0].gameObject);
		}

		/*------------------------------------------------------------
		マップアクティブ化：外部クラスからの操作用｜Activate map: For external class operation
		------------------------------------------------------------*/
		public void ActiveMap(){
			noMapOperation = false;
			mapCanvas.SetActive(true);
		}

		/*------------------------------------------------------------
		マップ非アクティブ化：外部クラスからの操作用｜Deactivate map: For external class operation
		------------------------------------------------------------*/
		public void InactiveMap(){
			noMapOperation = true;
			mapCanvas.SetActive(false);
		}

		/*------------------------------------------------------------
		再構築：外部クラスからの操作用｜Regenerate: For external class operation
		------------------------------------------------------------*/
		public void ReGenerate(){
			//既存マップの破棄｜Destroy existing map
			if(mapParent != null){
				Destroy(mapParent.gameObject);
			}
			//各コンポーネントをリセット｜Reset each component
			pathRenderer.Reset();
			dragController.Reset();
			//全て初期化｜Initialize all
			mapParent = null;
			activeRouteNodes = null;
			startNode = null;
			finalNode = null;
			map = null;
			nowNode = null;
			IsCompleted = false;
			noMapOperation = false;

			//1フレーム待ってから初期化を実行：Destroyは現在のフレーム終了時に実行されるため｜Wait 1 frame before initialization: Destroy is executed at the end of the current frame
			StartCoroutine(DelayedInit());
		}

		/*------------------------------------------------------------
		遅延初期化：ReGenerateから呼び出される｜Delayed initialization: Called from ReGenerate
		------------------------------------------------------------*/
		System.Collections.IEnumerator DelayedInit(){
			yield return null;//1フレーム待機｜Wait 1 frame
			Init();
		}

		/*------------------------------------------------------------
		ScrollViewのContentサイズを調整｜Adjust ScrollView Content size
		------------------------------------------------------------*/
		void AdjustScrollViewContent(float mapTopY, float mapBottomY){
			float totalHeight = mapTopY - mapBottomY;
			float totalWidth = mapWidth + backgroundPadding * 2;

			//ContentのRectTransformサイズを設定：外側マージン分を追加｜Set Content RectTransform size: Add outer margin
			float outerMargin = dragController.DragMargin;//外側マージン｜Outer margin
			float contentWidth = totalWidth + outerMargin * 2;
			float contentHeight = totalHeight + outerMargin * 2;

			//Viewportサイズを取得してContentサイズを保証：マップが小さくても中央に配置｜Get viewport size and ensure Content size: Center map even if small
			ScrollRect scrollRect = scrollViewContent.GetComponentInParent<ScrollRect>();
			if(scrollRect != null){
				RectTransform viewport = scrollRect.viewport != null ? scrollRect.viewport : scrollRect.GetComponent<RectTransform>();
				float viewportWidth = viewport.rect.width;
				float viewportHeight = viewport.rect.height;

				//ContentサイズがViewportより小さい場合はViewportサイズに合わせる｜Match viewport size if Content is smaller
				contentWidth = Mathf.Max(contentWidth, viewportWidth);
				contentHeight = Mathf.Max(contentHeight, viewportHeight);
			}

			scrollViewContent.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, contentWidth);
			scrollViewContent.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, contentHeight);

			//mapParentのアンカーとピボットを中央に配置｜Position mapParent anchors and pivot to center
			mapParent.anchorMin = new Vector2(0.5f, 0.5f);//中央｜Center
			mapParent.anchorMax = new Vector2(0.5f, 0.5f);
			mapParent.pivot = new Vector2(0.5f, 0.5f);

			//mapParentの位置を設定：Content内でマップを下寄せ｜Set mapParent position: Align map to bottom within Content
			mapParent.anchoredPosition = new Vector2(0, -totalHeight / 2 + backgroundPadding + startNodeSize / 2);

			//ScrollViewのスクロール位置を設定：左右中央、下端はマージンを除いたマップ下端｜Set ScrollView scroll position: center horizontally, bottom excluding margin
			if(scrollRect != null){
				Canvas.ForceUpdateCanvases();//レイアウト更新を強制｜Force layout update

				//左右中央に設定｜Set to horizontal center
				scrollRect.horizontalNormalizedPosition = 0.5f;

				//下端はマージン分だけ上にオフセット：マップ自体の下端が見える位置｜Offset bottom by margin: position where map bottom (not including margin) is visible
				float actualViewportHeight = scrollRect.viewport != null ? scrollRect.viewport.rect.height : scrollRect.GetComponent<RectTransform>().rect.height;
				float actualContentHeight = scrollViewContent.rect.height;
				if(actualContentHeight > actualViewportHeight){
					//外側マージン分のオフセットを正規化座標で計算｜Calculate outer margin offset in normalized coordinates
					float scrollableRange = actualContentHeight - actualViewportHeight;
					float offsetFromBottom = outerMargin / scrollableRange;
					scrollRect.verticalNormalizedPosition = Mathf.Clamp01(offsetFromBottom);
				}else{
					scrollRect.verticalNormalizedPosition = 0f;
				}
			}

			//ScrollbarのNavigationを無効化：ゲームパッドでの誤選択を防止｜Disable Scrollbar navigation: Prevent accidental selection with gamepad
			DisableScrollbarNavigation(scrollRect);

			//ドラッグコントローラーをScrollViewモードで初期化：左スティックでScrollRect操作｜Initialize drag controller for ScrollView mode: Left stick controls ScrollRect
			dragController.InitializeForScrollView(scrollRect);
		}

		/*------------------------------------------------------------
		ScrollbarのNavigationを無効化｜Disable Scrollbar Navigation
		------------------------------------------------------------*/
		void DisableScrollbarNavigation(ScrollRect scrollRect){
			if(scrollRect == null) return;

			//水平スクロールバー｜Horizontal scrollbar
			if(scrollRect.horizontalScrollbar != null){
				Navigation nav = scrollRect.horizontalScrollbar.navigation;
				nav.mode = Navigation.Mode.None;
				scrollRect.horizontalScrollbar.navigation = nav;
			}

			//垂直スクロールバー｜Vertical scrollbar
			if(scrollRect.verticalScrollbar != null){
				Navigation nav = scrollRect.verticalScrollbar.navigation;
				nav.mode = Navigation.Mode.None;
				scrollRect.verticalScrollbar.navigation = nav;
			}
		}

		/*------------------------------------------------------------
		セーブデータからマップを復元｜Load map from save data
		------------------------------------------------------------*/
		public void LoadFromSaveData(MapSaveData saveData){
			//シードを設定してマップを再生成｜Set seed and regenerate map
			randomizeSeed = false;
			seed = saveData.seed;

			//既存マップの破棄｜Destroy existing map
			if(mapParent != null){
				Destroy(mapParent.gameObject);
			}
			//各コンポーネントをリセット｜Reset each component
			pathRenderer.Reset();
			dragController.Reset();
			//全て初期化｜Initialize all
			mapParent = null;
			activeRouteNodes = null;
			startNode = null;
			finalNode = null;
			map = null;
			nowNode = null;
			IsCompleted = false;
			noMapOperation = false;

			//1フレーム待ってからロード処理を実行｜Wait 1 frame before load processing
			StartCoroutine(DelayedLoadFromSaveData(saveData));
		}

		/*------------------------------------------------------------
		遅延ロード処理：LoadFromSaveDataから呼び出される｜Delayed load processing: Called from LoadFromSaveData
		------------------------------------------------------------*/
		System.Collections.IEnumerator DelayedLoadFromSaveData(MapSaveData saveData){
			yield return null;//1フレーム待機｜Wait 1 frame

			//マップを生成｜Generate map
			Init();

			//訪問済みノードを復元｜Restore visited nodes
			Node previousNode = null;//パス描画用の前ノード｜Previous node for path drawing
			foreach(VisitedNodeData visited in saveData.visitedNodes){
				//-1,-1はスタートノード｜-1,-1 is start node
				if(visited.floor == -1 && visited.route == -1){
					if(startNode != null){
						startNode.visited = true;
						startNode.PassedNode();
						startNode.FillComplete();
						previousNode = startNode;
					}
				}
				//-2,-2はファイナルノード｜-2,-2 is final node
				else if(visited.floor == -2 && visited.route == -2){
					if(finalNode != null){
						finalNode.visited = true;
						finalNode.PassedNode();
						finalNode.FillComplete();

						//通過パスを塗る｜Paint passed path
						if(previousNode != null){
							pathRenderer.PaintPath(previousNode, finalNode, makeStart);
						}

						previousNode = finalNode;
					}
				}
				else if(map != null && visited.floor >= 0 && visited.floor < map.GetLength(0) && visited.route >= 0 && visited.route < map.GetLength(1)){
					Node node = map[visited.floor, visited.route];
					if(node != null){
						node.visited = true;
						node.PassedNode();
						node.FillComplete();

						//通過パスを塗る｜Paint passed path
						if(previousNode != null){
							pathRenderer.PaintPath(previousNode, node, makeStart);
						}

						//同一フロアの他のノードを通過済みに｜Mark other nodes on same floor as passed
						for(int r = 0; r < routeNum; r++){
							if(map[visited.floor, r] != null && map[visited.floor, r].connected && !map[visited.floor, r].visited){
								map[visited.floor, r].PassedNode();
							}
						}

						previousNode = node;
					}
				}
			}

			//現在選択中のノードを復元｜Restore current selected node
			if(saveData.nowNodeFloor >= 0 && saveData.nowNodeRoute >= 0){
				if(map != null && saveData.nowNodeFloor < map.GetLength(0) && saveData.nowNodeRoute < map.GetLength(1)){
					nowNode = map[saveData.nowNodeFloor, saveData.nowNodeRoute];
				}
			}
			else if(saveData.nowNodeFloor == -1 && saveData.nowNodeRoute == -1){
				nowNode = startNode;
			}
			else if(saveData.nowNodeFloor == -2 && saveData.nowNodeRoute == -2){
				nowNode = finalNode;
			}

			//スクロール/マップ位置を復元｜Restore scroll/map position
			yield return null;//レイアウト更新を待つ｜Wait for layout update

			if(placementMode == MapPlacementMode.ScrollView){
				ScrollRect scrollRect = GetScrollRect();
				if(scrollRect != null){
					scrollRect.horizontalNormalizedPosition = saveData.scrollPositionX;
					scrollRect.verticalNormalizedPosition = saveData.scrollPositionY;
				}
			}
			else{
				if(mapParent != null){
					mapParent.anchoredPosition = new Vector2(saveData.mapPositionX, saveData.mapPositionY);
					dragController.SyncPosition(mapParent.anchoredPosition);
				}
			}

			//次のノードに進める状態にする or 初期状態を復元｜Prepare to advance to next node or restore initial state
			if(saveData.visitedNodes.Count == 0){
				//訪問済みノードがない場合は初期状態（スタートノード選択可能）｜If no visited nodes, restore initial state (start node selectable)
				ActiveNodeSelect();
			}
			else if(nowNode != null){
				//訪問済みノードがある場合は次のノードに進める状態に｜If visited nodes exist, prepare to advance to next node
				ToNextNode();
			}
		}

		/*------------------------------------------------------------
		Inspectorで値が変更された時に実行：設定チェック｜Executed when value is changed in Inspector: Settings check
		------------------------------------------------------------*/
		#if UNITY_EDITOR
		void OnValidate(){
			if(placementMode == MapPlacementMode.ScrollView && scrollViewContent == null){
				Debug.LogWarning($"ScrollViewモードではscrollViewContentを設定してください｜Set scrollViewContent for ScrollView mode");
			}
		}
		#endif

	}

}