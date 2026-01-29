/*
 * SSSMapGenerator : Ver. 1.2.0
 * Written by Takashi Sowa @ loloop
*/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//名前空間｜Namespace
namespace S3MG{

	//パス描画用クラス：パスの描画と色変更を担当｜Path renderer class: Handles path drawing and color changes
	public class MapPathRenderer : MonoBehaviour{

		//パス設定｜Path settings
		[Header("▼パスを描画するためのImageプレファブ｜Path Image Prefab")]
		[SerializeField] Image pathImagePref;
		[Header("▼パスの色｜Path Color")]
		[SerializeField] Color pathColor = Color.gray;
		[Header("▼通過後のパスの色｜Passed Path Color")]
		[SerializeField] Color passedPathColor = Color.white;
		[Header("▼パスの太さ｜Path Width")]
		[SerializeField, Range(0,16)] float pathWidth = 4f;
		[Header("▼ノードとのパディング｜Padding Between Nodes")]
		[SerializeField, Range(0,256)] float paddingBetweenNodes = 0;

		RectTransform mapParent;//マップを入れる親オブジェクト｜Parent object for the map
		Dictionary<(Node, Node), Image> pathDictionary = new Dictionary<(Node, Node), Image>();//ノードペアをキーにしたパスの辞書｜Dictionary with node pair as key
		List<RectTransform> pathList = new List<RectTransform>();//各パス格納用：外部からのアクセス用｜For storing paths: for external access

		/*------------------------------------------------------------
		初期化：MapGeneratorから呼び出される｜Initialize: Called from MapGenerator
		------------------------------------------------------------*/
		public void Initialize(RectTransform parent){
			mapParent = parent;

			//パス用のプレファブがnullならスクリプトから生成｜Create from script if path prefab is null
			if(pathImagePref == null){
				GameObject pathObject = new GameObject("Path");
				pathObject.AddComponent<RectTransform>();
				Image image = pathObject.AddComponent<Image>();
				image.raycastTarget = false;
				pathObject.layer = LayerMask.NameToLayer("UI");
				pathImagePref = image;
			}
		}

		/*------------------------------------------------------------
		リセット：ReGenerateから呼び出される｜Reset: Called from ReGenerate
		------------------------------------------------------------*/
		public void Reset(){
			mapParent = null;
			pathDictionary.Clear();
			pathList.Clear();
		}

		/*------------------------------------------------------------
		パスリストを取得｜Get path list
		------------------------------------------------------------*/
		public List<RectTransform> GetPathsRectTransform(){
			return pathList;
		}

		/*------------------------------------------------------------
		パスリストをクリア｜Clear path list
		------------------------------------------------------------*/
		public void ClearPaths(){
			pathDictionary.Clear();
			pathList.Clear();
		}

		/*------------------------------------------------------------
		パスを描画｜Draw path
		------------------------------------------------------------*/
		public void DrawPath(Node fromNode, Node toNode){
			Image path = Instantiate(pathImagePref, mapParent);
			path.color = pathColor;

			RectTransform fromRect = fromNode.CachedRectTransform;
			RectTransform toRect = toNode.CachedRectTransform;

			//長さを計算｜Calculate length
			float distance = Vector2.Distance(fromRect.anchoredPosition, toRect.anchoredPosition);
			float paddingTotal = paddingBetweenNodes * 2;
			distance -= (distance > paddingTotal) ? paddingTotal : distance;

			//パスのサイズ設定｜Set path size
			RectTransform pathRect = path.rectTransform;
			pathRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, pathWidth);
			pathRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, distance);
			pathRect.pivot = new Vector2(0.5f, 0.5f);
			pathRect.position = (fromRect.position + toRect.position) / 2;

			//角度を計算して設定｜Calculate and set angle
			float angle = Mathf.Atan2(toRect.anchoredPosition.y - fromRect.anchoredPosition.y, toRect.anchoredPosition.x - fromRect.anchoredPosition.x) * Mathf.Rad2Deg - 90;
			pathRect.rotation = Quaternion.Euler(0, 0, angle);

			path.transform.SetAsFirstSibling();

			//Dictionaryに登録：ノードペアをキーにする｜Register in Dictionary: use node pair as key
			pathDictionary[(fromNode, toNode)] = path;
			pathList.Add(pathRect);
		}

		/*------------------------------------------------------------
		通過パスを塗る｜Paint passed path
		------------------------------------------------------------*/
		public void PaintPath(Node nowNode, Node targetNode, bool makeStart){
			//スタートノードを作っていない時、第一フロアならパスがないので早期リターン｜Return early if no start node and first floor
			if(!makeStart && targetNode.floor == 0) return;

			//Dictionaryから直接取得｜Get directly from Dictionary
			if(pathDictionary.TryGetValue((nowNode, targetNode), out Image pathImage)){
				pathImage.color = passedPathColor;
			}
			//見つからない場合は逆方向も試す｜Try reverse direction if not found
			else if(pathDictionary.TryGetValue((targetNode, nowNode), out Image reversePathImage)){
				reversePathImage.color = passedPathColor;
			}
		}

	}

}
