using UnityEngine;
using System.Collections.Generic;
using DataEntity;

[CreateAssetMenu(fileName = "DataHandler", menuName = "Data/DataHandler")]
public class DataHandler : ScriptableObject
{
    [SerializeField] private EntityDataScriptableObject entityData;
    [SerializeField] private SkillDataScriptableObject skillData;
    [SerializeField] private BuffDataScriptableObject buffData;
    [SerializeField] private ElementStatusDataScriptableObject elementStatusData;

    #region[ListData]
    public List<EntityData> EntityData => entityData.data;
    public List<SkillData> SkillData => skillData.data;
    public List<BuffData> BuffData => buffData.data;
    public List<ElementStatusData> ElementStatusDatas => elementStatusData.data;
    #endregion

    #region[SingleData]
    public EntityData FindEntityData(string id) => entityData.data.Find(e => e.code == id);
    public SkillData FindSkillData(string id) => skillData.data.Find(e => e.Skill_ID == id);
    public BuffData FindBuffData(string id) => buffData.data.Find(e => e.Buff_ID == id);
    public ElementStatusData FindElementStatusData(string id) => elementStatusData.data.Find(e => e.Element_Status_ID == id);
    #endregion
}
