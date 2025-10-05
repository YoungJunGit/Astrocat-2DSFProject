using System;
using System.Collections.Generic;
using S3MG;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Node : MonoBehaviour, IUpdateObserver, IDisposable{

		[SerializeField] public int floor {get; set;}
		[SerializeField] public int route {get; set;}

		[SerializeField] public List<Node> prevNodes {get; set;} = new List<Node>();
		[SerializeField] public List<Node> nextNodes {get; set;} = new List<Node>();

		[SerializeField] public bool connected {get; set;} = false;
		[SerializeField] public bool visited {get; set;} = false;

		[SerializeField] public float xPos {get; set;}
		[SerializeField] public float yPos {get; set;}

		[SerializeField] public NodeData.Type? nodeType {get; set;} = null;

		[SerializeField] Image nodeImage;
		[SerializeField] Button nodeButton;
		[SerializeField] Image visitImage;
		[SerializeField] TextMeshProUGUI nodeText;
		[SerializeField] AudioSource AS;

		[SerializeField] float lerpDuration = 0.8f;
		[SerializeField] Color lerpColor = Color.gray;
		Color defaultColor = Color.white;
		Color currentColor;
		bool isForward = true;
		float lerpStartTime;

		[SerializeField] public bool onButton {get; set;} = false;
		float fillTime = 0.4f;
		float elapsedTime = 0f;

		NodeMapGenerator  _mapGenerator;

		/*------------------------------------------------------------
		Executed only once when MonoBehaviour is created, Will work if the GameObject is active even if the component is disabled
		------------------------------------------------------------*/
		public void Init(NodeMapGenerator mapGenerator){
			_mapGenerator = mapGenerator;
			currentColor = defaultColor;
			UpdatePublisher.SubscribeObserver(this);
		}

		/*------------------------------------------------------------
		Called once per frame, Executed when the GameObject and component are enabled
		------------------------------------------------------------*/
		public void ObserverUpdate(float dt)
		{
			if(!nodeButton.enabled || visited) return;

			float t = (Time.time - lerpStartTime) / lerpDuration;
			if(t > 1f){
				t = 0f;
				lerpStartTime = Time.time;
				isForward = !isForward;
				currentColor = isForward ? defaultColor : lerpColor;
			}
			nodeImage.color = Color.Lerp(currentColor, isForward ? lerpColor : defaultColor, t);

			if(onButton) fillUp();
		}

		/*------------------------------------------------------------
		Set image, type, and text on a node
		------------------------------------------------------------*/
		public void setNodeData(Sprite sprite, NodeData.Type? type, string text = ""){
			nodeImage.sprite = sprite;
			nodeType = type;
			nodeText.text = text;
		}

		/*------------------------------------------------------------
		Enable the Button component
		------------------------------------------------------------*/
		public void enableButton(){
			nodeButton.enabled = true;
		}

		/*------------------------------------------------------------
		Disable the Button component
		------------------------------------------------------------*/
		public void disableButton(){
			nodeButton.enabled = false;
		}

		/*------------------------------------------------------------
		Mark as passed
		------------------------------------------------------------*/
		public void passedNode(){
			disableButton();
			if(visited){
				nodeImage.color = defaultColor;
			}else{
				nodeImage.color = Color.gray;
				onButton = false;
				fillReset();
			}
		}

		/*------------------------------------------------------------
		Fill up
		------------------------------------------------------------*/
		void fillUp(){
			if(elapsedTime < fillTime){
				float amountValue = Mathf.Lerp(0f, 1.0f, elapsedTime / fillTime);
				visitImage.fillAmount = amountValue;
				elapsedTime += Time.deltaTime;
			}
			else{
				AS.Play();
				entryStart();
			}
		}

		/*------------------------------------------------------------
		Fill reset
		------------------------------------------------------------*/
		void fillReset(){
			visitImage.fillAmount = 0;
			elapsedTime = 0f;
		}

		/*------------------------------------------------------------
		Branch events based on node type
		------------------------------------------------------------*/
		void entryStart(){
			visited = true;

			if(nodeType != NodeData.Type.Start) _mapGenerator.paintPath(this);

			if(nodeType != NodeData.Type.Start && nodeType != NodeData.Type.Final){
				_mapGenerator.passedSameFloor(this);
			}
			else{
				passedNode();
			}

			_mapGenerator.nowNode = this;

			if(_mapGenerator.skipNodeProcessing){
				_mapGenerator.toNextNode();
			}
			else{
				switch(nodeType){
					case NodeData.Type.Start:
						handleStart();
						break;
					case NodeData.Type.Camp:
						handleCamp();
						break;
					case NodeData.Type.Shop:
						handleShop();
						break;
					case NodeData.Type.Event:
						handleEvent();
						break;
					case NodeData.Type.Treasure:
						handleTreasure();
						break;
					case NodeData.Type.Trap:
						handleTrap();
						break;
					case NodeData.Type.Enemy:
						handleEnemy();
						break;
					case NodeData.Type.Middle:
						handleMiddle();
						break;
					case NodeData.Type.Final:
						handleFinal();
						break;
					case NodeData.Type.Random:
						handleRandom();
						break;
					default:
						Debug.Log($"Nothing is set");
						break;
				}
			}

		}

		/*------------------------------------------------------------
		Processing each node
		------------------------------------------------------------*/
		void handleStart(){
			Debug.Log("Start the game");
		}

		void handleCamp(){
			Debug.Log("Open a camp");
		}

		void handleShop(){
			Debug.Log("Open a shop");
		}

		void handleEvent(){
			Debug.Log("Execute event");
		}

		void handleTreasure(){
			Debug.Log("Obtain treasure");
		}

		void handleTrap(){
			Debug.Log("Execute trap");
		}

		void handleEnemy(){
			Debug.Log("Fight a enemy");
		}

		void handleMiddle(){
			Debug.Log("Midpoint");
		}

		void handleFinal(){
			switch(nodeText.text){
				case "Boss01" :
					Debug.Log("Fight the Boss 01");
					break;
				case "Boss02" :
					Debug.Log("Fight the Boss 02");
					break;
				default :
					Debug.Log("Fight the Boss 03");
					break;
			}
		}

		void handleRandom(){
			Debug.Log("Execute random process");
		}

		public void Dispose()
		{
			UpdatePublisher.DiscribeObserver(this);
		}
	}
