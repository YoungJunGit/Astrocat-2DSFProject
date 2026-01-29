/*
 * SSSMapGenerator : Ver. 1.1.0
 * マップノードデータのカスタムPropertyDrawer｜Custom PropertyDrawer for MapNodeData
 * 新規要素追加時にデフォルト値を設定｜Set default values when adding new elements
*/

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

//名前空間｜Namespace
namespace S3MG{

	[CustomPropertyDrawer(typeof(MapNodeData))]
	public class MapNodeDataDrawer : PropertyDrawer{
		/*------------------------------------------------------------
		初期化処理：初めて描画される時にデフォルト値を設定｜Initialize: Set default values when first drawn
		------------------------------------------------------------*/
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label){
			//appearedAfterが0なら未初期化とみなす｜If appearedAfter is 0, consider it uninitialized
			SerializedProperty appearedAfterProp = property.FindPropertyRelative("appearedAfter");
			SerializedProperty chanceProp = property.FindPropertyRelative("chance");
			SerializedProperty serializableProp = property.FindPropertyRelative("serializable");

			if(appearedAfterProp.intValue == 0){
				appearedAfterProp.intValue = 1;
				chanceProp.floatValue = 1f;
				serializableProp.boolValue = true;
				property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
			}

			//デフォルトの描画｜Default drawing
			EditorGUI.PropertyField(position, property, label, true);
		}

		/*------------------------------------------------------------
		プロパティの高さを返す｜Return property height
		------------------------------------------------------------*/
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label){
			return EditorGUI.GetPropertyHeight(property, label, true);
		}
	}

	[CustomPropertyDrawer(typeof(FixedNodeData))]
	public class FixedNodeDataDrawer : PropertyDrawer{
		/*------------------------------------------------------------
		初期化処理：初めて描画される時にデフォルト値を設定｜Initialize: Set default values when first drawn
		------------------------------------------------------------*/
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label){
			//appearedOnが0なら未初期化とみなす｜If appearedOn is 0, consider it uninitialized
			SerializedProperty appearedOnProp = property.FindPropertyRelative("appearedOn");

			if(appearedOnProp.intValue == 0){
				appearedOnProp.intValue = 8;
				property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
			}

			//デフォルトの描画｜Default drawing
			EditorGUI.PropertyField(position, property, label, true);
		}

		/*------------------------------------------------------------
		プロパティの高さを返す｜Return property height
		------------------------------------------------------------*/
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label){
			return EditorGUI.GetPropertyHeight(property, label, true);
		}
	}

}
#endif
