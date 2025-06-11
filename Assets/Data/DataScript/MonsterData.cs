using UnityEngine;
using UnityEditor;
using DataEntity;

[CreateAssetMenu(fileName = "MonsterData", menuName = "SO/MonsterData", order = 2)]
public class MonsterData : BaseData<MonsterDataEntity>
{

}

[CustomEditor(typeof(MonsterData))]
public class MonsterDataEditor : BaseDataEditor<MonsterDataEntity>
{

}