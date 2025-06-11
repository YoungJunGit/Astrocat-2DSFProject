using UnityEngine;
using UnityEditor;
using DataEntity;

[CreateAssetMenu(fileName = "SkillData", menuName = "SO/SkillData", order = 3)]
public class SkillData : BaseData<SkillDataEntity>
{

}

[CustomEditor(typeof(SkillData))]
public class SkillDataEditor : BaseDataEditor<SkillDataEntity>
{

}