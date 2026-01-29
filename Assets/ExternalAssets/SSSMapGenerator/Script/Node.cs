/*
 * SSSMapGenerator : Ver. 1.2.0
 * Written by Takashi Sowa @ loloop
*/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;//シーンを使う為に必要｜Required to use SceneManager

//名前空間｜Namespace
namespace S3MG{

	//ノード用クラス：ノードの生成と管理を担当｜Node class: Handles node generation and management
	public class Node : MonoBehaviour{

		public int floor {get; set;}//フロア｜Floor
		public int route {get; set;}//ルート｜Route

		public List<Node> prevNodes {get; set;} = new List<Node>();//接続元｜Previous nodes
		public List<Node> nextNodes {get; set;} = new List<Node>();//接続先｜Next nodes

		public bool connected {get; set;} = false;//接続されているかどうか｜Whether connected
		public bool visited {get; set;} = false;//訪問済かどうか｜Whether visited

		public float xPos {get; set;}//RectTransformのx座標｜RectTransform x position
		public float yPos {get; set;}//RectTransformのy座標｜RectTransform y position

		public NodeData.Type? nodeType {get; set;} = null;//ノードタイプ：null許容値型使用｜Node type: using nullable type
		public string NodeName => nodeText.text;//ノード名：外部からアクセス用｜Node name: for external access

		[SerializeField] Image nodeImage;//自分のimageコンポーネント｜Image component
		[SerializeField] Button nodeButton;//自分のButtonコンポーネント｜Button component
		[SerializeField] Image visitImage;//滞在用imageコンポーネント｜Visit image component
		[SerializeField] TextMeshProUGUI nodeText;//TextMeshProコンポーネント｜TextMeshPro component
		[SerializeField] AudioSource audioSource;//オーディオソース｜AudioSource

		[SerializeField] float lerpDuration = 0.8f;//ラープ間隔｜Lerp duration
		[SerializeField] Color lerpColor = Color.gray;//ラープカラー｜Lerp color
		Color defaultColor = Color.white;//元のカラー｜Default color
		Color currentColor;//現在のカラー｜Current color
		bool isForward = true;//ラープの方向｜Lerp direction
		float lerpStartTime;//ラープ計算用｜For lerp calculation

		public bool onButton {get; set;} = false;//ノードが押されたかどうか｜Whether node is pressed
		float elapsedTime = 0f;//フィルアップ時間計算用｜For fillup time calculation

		const float FillDuration = 0.4f;//フィルアップにかける時間定数｜Fillup duration constant

		RectTransform cachedRectTransform;//RectTransformのキャッシュ用｜Cached RectTransform

		/*------------------------------------------------------------
		RectTransformのキャッシュを取得｜Get cached RectTransform
		------------------------------------------------------------*/
		public RectTransform CachedRectTransform{
			get{
				if(cachedRectTransform == null){
					cachedRectTransform = GetComponent<RectTransform>();
				}
				return cachedRectTransform;
			}
		}

		/*------------------------------------------------------------
		MonoBehaviourが作られた時に1度だけ実行｜Executed only once when MonoBehaviour is created
		------------------------------------------------------------*/
		void Awake(){
			//RectTransformをキャッシュ｜Cache RectTransform
			cachedRectTransform = GetComponent<RectTransform>();
			currentColor = defaultColor;
		}

		/*------------------------------------------------------------
		1フレームに1回呼び出される｜Called once per frame
		------------------------------------------------------------*/
		void Update(){
			//ボタンコンポーネントがfalseもしくは訪問済みなら早期リターン｜Return early if button is disabled or visited
			if(!nodeButton.enabled || visited) return;

			//現在の時間を取得してラープを計算｜Calculate lerp based on current time
			float t = (Time.time - lerpStartTime) / lerpDuration;
			if(t > 1f){
				//ラープの終了と始点と終点の入れ替え｜End lerp and swap start/end colors
				t = 0f;
				lerpStartTime = Time.time;
				isForward = !isForward;
				currentColor = isForward ? defaultColor : lerpColor;
			}
			//色のラープ｜Lerp color
			nodeImage.color = Color.Lerp(currentColor, isForward ? lerpColor : defaultColor, t);

			//ボタンが押されたらフィルアップ｜Fillup when button is pressed
			if(onButton) FillUp();
		}

		/*------------------------------------------------------------
		ノードに画像とタイプとテキストを設定｜Set image, type and text to node
		------------------------------------------------------------*/
		public void SetNodeData(Sprite sprite, NodeData.Type? type, string text = ""){
			nodeImage.sprite = sprite;
			nodeType = type;
			nodeText.text = text;
		}

		/*------------------------------------------------------------
		Buttonコンポーネントを有効化｜Enable Button component
		------------------------------------------------------------*/
		public void EnableButton(){
			nodeButton.enabled = true;
		}

		/*------------------------------------------------------------
		Buttonコンポーネントを無効化｜Disable Button component
		------------------------------------------------------------*/
		public void DisableButton(){
			nodeButton.enabled = false;
		}

		/*------------------------------------------------------------
		通過済にする｜Mark as passed
		------------------------------------------------------------*/
		public void PassedNode(){
			DisableButton();
			//訪問済なら白、未訪問ならグレー｜White if visited, gray if not
			if(visited){
				nodeImage.color = defaultColor;
			}else{
				nodeImage.color = Color.gray;
				//フィルアップ中に別のノードを押している可能性があるのでフラグを戻してフィルもリセット｜Reset flag and fill as another node may have been pressed during fillup
				onButton = false;
				FillReset();
			}
		}

		/*------------------------------------------------------------
		フィルアップ｜Fillup
		------------------------------------------------------------*/
		void FillUp(){
			//経過時間に達してなければ｜If not reached elapsed time
			if(elapsedTime < FillDuration){
				//フィルをアップし続ける｜Continue filling up
				float amountValue = Mathf.Lerp(0f, 1.0f, elapsedTime / FillDuration);
				visitImage.fillAmount = amountValue;
				elapsedTime += Time.deltaTime;
			}
			//経過時間に達したら｜When elapsed time is reached
			else{
				//音を鳴らしてノードのイベント実行｜Play sound and execute node event
				audioSource.Play();
				EntryStart();
			}
		}

		/*------------------------------------------------------------
		フィルリセット｜Reset fill
		------------------------------------------------------------*/
		void FillReset(){
			//フィルの途中経過を元に戻す｜Reset fill progress
			visitImage.fillAmount = 0;
			elapsedTime = 0f;
		}

		/*------------------------------------------------------------
		滞在イメージを即座に満たす：ロード時用｜Fill visit image immediately: For load
		------------------------------------------------------------*/
		public void FillComplete(){
			visitImage.fillAmount = 1f;
		}

		/*------------------------------------------------------------
		ノードタイプに応じて各イベントに分岐｜Branch to each event based on node type
		------------------------------------------------------------*/
		void EntryStart(){
			//訪問済フラグを立てる｜Set visited flag
			visited = true;

			//通過パスに色をつける｜Color the passed path
			if(nodeType != NodeData.Type.Start) MapGenerator.instance.PaintPath(this);

			//スタートノードでもファイナルノードでもなければ同一フロアの他のノードを通過済にする｜Mark other nodes on same floor as passed if not start or final
			if(nodeType != NodeData.Type.Start && nodeType != NodeData.Type.Final){
				MapGenerator.instance.PassedSameFloor(this);
			}
			//スタートノードかファイナルノードなら通過済にする｜Mark as passed if start or final node
			else{
				PassedNode();
			}

			//自分自身を現在選択されているノードとして渡す｜Pass self as currently selected node
			MapGenerator.instance.nowNode = this;

			//ノード処理スキップが有効なら次のノードへ進めるだけ｜Only advance to next node if skip is enabled
			if(MapGenerator.instance.skipNodeProcessing){
				MapGenerator.instance.ToNextNode();
			}
			//ノード処理スキップが無効ならイベント発火｜Fire event if skip is disabled
			else{
				//イベント発火：NodeEventHandlerで処理を受け取る｜Fire event: NodeEventHandler receives this
				if(nodeType.HasValue){
					MapGenerator.InvokeNodeEntered(nodeType.Value, this);
				}
			}

		}
	}

}