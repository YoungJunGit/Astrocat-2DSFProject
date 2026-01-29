/*
 * SSSMapGenerator : Ver. 1.2.0
 * Written by Takashi Sowa @ loloop
*/

using UnityEngine;
using UnityEngine.UI;//ScrollRectを使う為に必要｜Required to use ScrollRect
using UnityEngine.InputSystem;//インプットシステムを使う為に必要｜Required to use the InputSystem

//名前空間｜Namespace
namespace S3MG{

	//マップドラッグ操作用クラス：マウスとゲームパッドでのマップ移動を担当｜Map drag controller class: Handles map movement with mouse and gamepad
	public class MapDragController : MonoBehaviour{

		[Header("▼マウスのドラッグ感度｜Mouse Drag Sensitivity")]
		[SerializeField, Range(1,4)] float mouseSensitivity = 1.0f;
		[Header("▼ゲームパッドの移動感度｜Gamepad Movement Sensitivity")]
		[SerializeField, Range(1,8)] float gamepadSensitivity = 4.0f;
		[Header("▼ドラッグ時の外側余白｜Outer Margin for Drag")]
		[SerializeField, Range(0,500)] float dragMargin = 200f;
		public float DragMargin => dragMargin;//外部参照用プロパティ｜Property for external access

		[Header("▼入力設定：未設定時はデフォルト入力を使用｜Input Settings: Uses default input if not set")]
		[SerializeField] InputActionReference dragButtonAction;//ドラッグボタン：未設定時はマウス左ボタン｜Drag button: Mouse left button if not set
		[SerializeField] InputActionReference gamepadMoveAction;//ゲームパッド移動：未設定時は左スティック｜Gamepad move: Left stick if not set

		Vector2 previousMousePos;//ドラッグ移動前の座標｜Position before drag
		Vector2 currentPosition;//現在の座標｜Current position
		bool isDragging = false;//ドラッグ中フラグ｜Dragging flag

		RectTransform mapParent;//マップを入れる親オブジェクト｜Parent object for the map
		float mapWidth;//マップ全体の幅｜Total map width
		float mapHeight;//マップ全体の高さ｜Total map height
		float backgroundPadding;//背景とマップの余白｜Padding between background and map
		float canvasWidth;//Canvasの幅｜Canvas width
		float canvasHeight;//Canvasの高さ｜Canvas height
		float mapTopY;//マップ上端のmapParent座標系でのY座標｜Map top Y in mapParent coordinate
		float mapBottomY;//マップ下端のmapParent座標系でのY座標｜Map bottom Y in mapParent coordinate
		Vector2 initialPosition;//初期座標（マップ生成時の位置）｜Initial position (position at map generation)
		bool isInitialized = false;//初期化済みフラグ｜Initialized flag
		float gamepadDeadzone = 0.3f;//ゲームパッドのデッドゾーン｜Gamepad deadzone

		//ScrollViewモード用｜For ScrollView mode
		bool isScrollViewMode = false;//ScrollViewモードフラグ｜ScrollView mode flag
		ScrollRect scrollRect;//ScrollRect参照｜ScrollRect reference

		//デフォルト入力アクション：InputActionReferenceが未設定の場合に使用｜Default input actions: Used when InputActionReference is not set
		InputAction defaultDragButton;
		InputAction defaultGamepadMove;

		//使用する入力アクションを取得｜Get input action to use
		InputAction GetDragButtonAction() => dragButtonAction != null ? dragButtonAction.action : defaultDragButton;
		InputAction GetGamepadMoveAction() => gamepadMoveAction != null ? gamepadMoveAction.action : defaultGamepadMove;

		/*------------------------------------------------------------
		MonoBehaviourが有効化された時に実行｜Executed when MonoBehaviour is enabled
		------------------------------------------------------------*/
		void OnEnable(){
			//デフォルト入力アクションを作成｜Create default input actions
			if(dragButtonAction == null){
				defaultDragButton = new InputAction("DragButton", InputActionType.Button, "<Mouse>/leftButton");
				defaultDragButton.Enable();
			}else{
				dragButtonAction.action.Enable();
			}

			if(gamepadMoveAction == null){
				defaultGamepadMove = new InputAction("GamepadMove", InputActionType.Value, "<Gamepad>/leftStick");
				defaultGamepadMove.Enable();
			}else{
				gamepadMoveAction.action.Enable();
			}
		}

		/*------------------------------------------------------------
		MonoBehaviourが無効化された時に実行｜Executed when MonoBehaviour is disabled
		------------------------------------------------------------*/
		void OnDisable(){
			//デフォルト入力アクションを無効化して破棄｜Disable and dispose default input actions
			defaultDragButton?.Disable();
			defaultDragButton?.Dispose();
			defaultDragButton = null;

			defaultGamepadMove?.Disable();
			defaultGamepadMove?.Dispose();
			defaultGamepadMove = null;

			//設定されたInputActionReferenceを無効化｜Disable set InputActionReferences
			dragButtonAction?.action.Disable();
			gamepadMoveAction?.action.Disable();
		}

		/*------------------------------------------------------------
		Inspectorで値が変更された時に実行：型チェック｜Executed when value is changed in Inspector: Type check
		------------------------------------------------------------*/
		#if UNITY_EDITOR
		void OnValidate(){
			//dragButtonActionの型チェック｜Type check for dragButtonAction
			if(dragButtonAction != null && dragButtonAction.action != null){
				if(dragButtonAction.action.type != InputActionType.Button){
					Debug.LogWarning($"dragButtonActionにはButton型のActionを設定してください｜Set Button type Action for dragButtonAction");
				}
			}
			//gamepadMoveActionの型チェック｜Type check for gamepadMoveAction
			if(gamepadMoveAction != null && gamepadMoveAction.action != null){
				if(gamepadMoveAction.action.type == InputActionType.Button){
					Debug.LogWarning($"gamepadMoveActionにはValue型（Vector2）のActionを設定してください｜Set Value type (Vector2) Action for gamepadMoveAction");
				}
			}
		}
		#endif

		/*------------------------------------------------------------
		初期化：MapGeneratorから呼び出される｜Initialize: Called from MapGenerator
		------------------------------------------------------------*/
		public void Initialize(RectTransform parent, float width, float height, float padding, float canvasW, float canvasH, float topY, float bottomY){
			mapParent = parent;
			mapWidth = width;
			mapHeight = height;
			backgroundPadding = padding;
			canvasWidth = canvasW;
			canvasHeight = canvasH;
			mapTopY = topY;
			mapBottomY = bottomY;
			initialPosition = mapParent.anchoredPosition;
			currentPosition = mapParent.anchoredPosition;
			isInitialized = true;
			isDragging = false;
		}

		/*------------------------------------------------------------
		リセット：ReGenerateから呼び出される｜Reset: Called from ReGenerate
		------------------------------------------------------------*/
		public void Reset(){
			mapParent = null;
			isInitialized = false;
			isScrollViewMode = false;
			scrollRect = null;
			isDragging = false;
			previousMousePos = Vector2.zero;
			currentPosition = Vector2.zero;
		}

		/*------------------------------------------------------------
		ScrollViewモード用：ScrollRect操作モードで初期化｜For ScrollView mode: Initialize in ScrollRect control mode
		------------------------------------------------------------*/
		public void InitializeForScrollView(ScrollRect targetScrollRect){
			isScrollViewMode = true;
			scrollRect = targetScrollRect;
			isInitialized = true;
		}

		/*------------------------------------------------------------
		マップ位置の同期用｜For syncing map position
		------------------------------------------------------------*/
		public void SyncPosition(Vector2 position){
			previousMousePos = currentPosition = position;
		}

		/*------------------------------------------------------------
		1フレームに1回呼び出される｜Called once per frame
		------------------------------------------------------------*/
		void Update(){
			if(!isInitialized) return;
			if(MapGenerator.instance != null && MapGenerator.instance.noMapOperation) return;

			//ScrollViewモードの場合はゲームパッド入力のみ処理｜For ScrollView mode, only process gamepad input
			if(isScrollViewMode){
				HandleGamepadInput();
				return;
			}

			//Standaloneモードの場合はmapParentが必要｜For Standalone mode, mapParent is required
			if(mapParent == null) return;

			HandleMouseInput();
			HandleGamepadInput();
		}

		/*------------------------------------------------------------
		マウス入力処理｜Handle mouse input
		------------------------------------------------------------*/
		void HandleMouseInput(){
			InputAction buttonAction = GetDragButtonAction();
			if(buttonAction == null) return;
			if(Mouse.current == null) return;

			//ドラッグ開始｜Start dragging
			if(buttonAction.WasPressedThisFrame()){
				previousMousePos = Mouse.current.position.ReadValue();
				isDragging = true;
			}
			//ドラッグ中｜During dragging
			else if(buttonAction.IsPressed() && isDragging){
				Vector2 mousePos = Mouse.current.position.ReadValue();
				Vector2 delta = mousePos - previousMousePos;
				currentPosition.x += delta.x * mouseSensitivity;
				currentPosition.y += delta.y * mouseSensitivity;
				ClampPosition();
				mapParent.anchoredPosition = currentPosition;
				previousMousePos = mousePos;
			}
			//ドラッグ終了｜End dragging
			else if(buttonAction.WasReleasedThisFrame()){
				isDragging = false;
			}
		}

		/*------------------------------------------------------------
		ゲームパッド入力処理｜Handle gamepad input
		------------------------------------------------------------*/
		void HandleGamepadInput(){
			//ゲームパッドが接続されていない場合は処理しない｜Skip if no gamepad is connected
			if(Gamepad.current == null) return;

			//gamepadMoveActionが設定されている場合はそれを使用、なければ左スティックを直接読み取り｜Use gamepadMoveAction if set, otherwise read left stick directly
			Vector2 stickValue;
			InputAction moveAction = GetGamepadMoveAction();
			if(gamepadMoveAction != null && moveAction != null){
				//設定されたアクションを使用｜Use the configured action
				stickValue = moveAction.ReadValue<Vector2>();
			}else{
				//デフォルト：左スティックを直接読み取り（マウスの影響を受けない）｜Default: Read left stick directly (not affected by mouse)
				stickValue = Gamepad.current.leftStick.ReadValue();
			}

			//ScrollViewモードの場合はScrollRect操作｜Use ScrollRect control for ScrollView mode
			if(isScrollViewMode){
				HandleScrollViewGamepadInput(stickValue);
				return;
			}

			//Standaloneモードの場合はmapParent移動｜Move mapParent for Standalone mode
			if(stickValue.y > gamepadDeadzone) MoveVertical(true);
			if(stickValue.y < -gamepadDeadzone) MoveVertical(false);
			if(stickValue.x < -gamepadDeadzone) MoveHorizontal(true);
			if(stickValue.x > gamepadDeadzone) MoveHorizontal(false);
		}

		/*------------------------------------------------------------
		ScrollViewモード用ゲームパッド入力処理｜Handle gamepad input for ScrollView mode
		------------------------------------------------------------*/
		void HandleScrollViewGamepadInput(Vector2 stickValue){
			if(scrollRect == null) return;

			//デッドゾーン以下なら無視｜Ignore if below deadzone
			if(Mathf.Abs(stickValue.x) < gamepadDeadzone && Mathf.Abs(stickValue.y) < gamepadDeadzone) return;

			//スクロール感度を調整：Standaloneと同程度の速度になるよう調整｜Adjust scroll sensitivity: Match Standalone speed
			float scrollSpeed = gamepadSensitivity * 0.001f;

			//水平スクロール：入力右でマップ右へ｜Horizontal scroll: Input right to scroll map right
			if(Mathf.Abs(stickValue.x) > gamepadDeadzone){
				float newHorizontal = scrollRect.horizontalNormalizedPosition + stickValue.x * scrollSpeed;
				scrollRect.horizontalNormalizedPosition = Mathf.Clamp01(newHorizontal);
			}

			//垂直スクロール：入力上でマップ上へ｜Vertical scroll: Input up to scroll map up
			if(Mathf.Abs(stickValue.y) > gamepadDeadzone){
				float newVertical = scrollRect.verticalNormalizedPosition + stickValue.y * scrollSpeed;
				scrollRect.verticalNormalizedPosition = Mathf.Clamp01(newVertical);
			}
		}

		/*------------------------------------------------------------
		水平移動｜Move horizontally
		------------------------------------------------------------*/
		void MoveHorizontal(bool isLeft){
			if(isLeft){
				currentPosition.x += gamepadSensitivity;
			}else{
				currentPosition.x -= gamepadSensitivity;
			}
			ClampHorizontalPosition();
			mapParent.anchoredPosition = currentPosition;
		}

		/*------------------------------------------------------------
		垂直移動｜Move vertically
		------------------------------------------------------------*/
		void MoveVertical(bool isUp){
			if(isUp){
				currentPosition.y -= gamepadSensitivity;
			}else{
				currentPosition.y += gamepadSensitivity;
			}
			ClampVerticalPosition();
			mapParent.anchoredPosition = currentPosition;
		}

		/*------------------------------------------------------------
		座標を範囲内にクランプ｜Clamp position within range
		------------------------------------------------------------*/
		void ClampPosition(){
			ClampHorizontalPosition();
			ClampVerticalPosition();
		}

		/*------------------------------------------------------------
		水平方向の座標をクランプ｜Clamp horizontal position
		------------------------------------------------------------*/
		void ClampHorizontalPosition(){
			float totalMapWidth = mapWidth + backgroundPadding * 2;

			//マップ＋余白が画面幅より小さい場合は移動しない｜Don't move if map + margin is smaller than screen width
			if(totalMapWidth + dragMargin * 2 <= canvasWidth){
				currentPosition.x = initialPosition.x;
				return;
			}

			//マップの移動可能範囲を計算：マップ端＋余白が画面端に来る位置｜Calculate movable range: map edge + margin reaches screen edge
			float halfExcess = (totalMapWidth + dragMargin * 2 - canvasWidth) / 2;

			float maxX = initialPosition.x + halfExcess;
			float minX = initialPosition.x - halfExcess;

			currentPosition.x = Mathf.Clamp(currentPosition.x, minX, maxX);
		}

		/*------------------------------------------------------------
		垂直方向の座標をクランプ｜Clamp vertical position
		------------------------------------------------------------*/
		void ClampVerticalPosition(){
			float totalMapHeight = mapHeight + backgroundPadding * 2;

			//マップ＋余白が画面高さより小さい場合は移動しない｜Don't move if map + margin is smaller than screen height
			if(totalMapHeight + dragMargin * 2 <= canvasHeight){
				currentPosition.y = initialPosition.y;
				return;
			}

			//絶対座標ベースでクランプ：マップ端がdragMargin分だけ画面内側に入った位置で制限｜Absolute clamp: limit when map edge enters screen by dragMargin
			float maxY = -canvasHeight / 2 + dragMargin - mapBottomY;//マップ下端の外側がdragMargin分見える位置
			float minY = canvasHeight / 2 - dragMargin - mapTopY;//マップ上端の外側がdragMargin分見える位置

			currentPosition.y = Mathf.Clamp(currentPosition.y, minY, maxY);
		}

	}

}
