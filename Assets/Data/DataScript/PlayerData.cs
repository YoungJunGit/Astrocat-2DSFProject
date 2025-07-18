using UnityEngine;
using UnityEditor;
using DataEntity;

[CreateAssetMenu(fileName = "PlayerData", menuName = "SO/PlayerData", order = 1)]
public class PlayerData : BaseData<CharacterDataEntity>
{

}

[CustomEditor(typeof(PlayerData))]
public class CharacterDataEditor : BaseDataEditor<CharacterDataEntity>
{

}