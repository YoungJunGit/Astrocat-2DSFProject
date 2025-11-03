using DataEntity;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 건들지 말 것
/// </summary>
/// <typeparam name="T"></typeparam>
public class BaseDataEditor<T> : Editor
{
    private BaseDataScriptableObject<T> _target;

    private void OnEnable()
    {
        _target = (BaseDataScriptableObject<T>)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (_target != null)
        {
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("TextDataLoad") == true)
            {
                Debug.Log("Load");
                Load(_target.Json.text);
            }

            EditorGUILayout.EndHorizontal();
        }
    }

    private void Load(string data)
    {
        if (string.IsNullOrEmpty(data))
            return;

        string decryptedData = data;

        if (_target.DecryptToggle)
            decryptedData = Security.AESDecrypt256(data);

        _target.data.Clear();

        JArray jArray = JArray.Parse(decryptedData);

        foreach (var json in jArray)
        {
            T protocolData = json.ToObject<T>();
            _target.data.Add(protocolData);
        }
    }
}

[CustomEditor(typeof(EntityDataScriptableObject))]
public class EntityDataEditor : BaseDataEditor<EntityData>
{

}

[CustomEditor(typeof(SkillDataScriptableObject))]
public class SkillDataEditor : BaseDataEditor<SkillData>
{

}

[CustomEditor(typeof(BuffDataScriptableObject))]
public class BuffDataEditor : BaseDataEditor<BuffData>
{

}

[CustomEditor(typeof(ElementStatusDataScriptableObject))]
public class ElementStatusDataEditor : BaseDataEditor<ElementStatusData>
{

}

[CustomEditor(typeof(StringDataScriptableObject))]
public class StringDataEditor : BaseDataEditor<StringData>
{

}