using UnityEngine;
using UnityEditor;
using DataEntity;

[CreateAssetMenu(fileName = "StringData", menuName = "SO/StringData", order = 3)]
public class StringData : BaseData<StringDataEntity>
{

}

[CustomEditor(typeof(StringData))]
public class StringDataEditor : BaseDataEditor<StringDataEntity>
{

}