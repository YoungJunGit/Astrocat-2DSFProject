using UnityEngine;
using UnityEditor;
using DataEntity;

[CreateAssetMenu(fileName = "CharacterData", menuName = "SO/CharacterData", order = 1)]
public class CharacterData : BaseData<CharacterDataEntity>
{

}

[CustomEditor(typeof(CharacterData))]
public class CharacterDataEditor : BaseDataEditor<CharacterDataEntity>
{

}