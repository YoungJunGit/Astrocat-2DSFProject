using DataEntity;
using DataEnum;
using System.Collections.Generic;
using UnityEngine;

public enum SIDE
{
    NONE = 0,
    PLAYER,
    ENEMY
}
public class EntityData
{
    public SIDE Side = SIDE.NONE;
    public string ID;
    public string Name;
    public double Default_HP = 0.0f;
    public double Default_Attack = 0.0f;
    public int Default_AP = 0;
    public double Default_Speed = 0.0f;
    public string Skill1_ID;
    public string Skill2_ID;
    public string Skill3_ID;
    public ELEMENT_TYPE Weak_Type = ELEMENT_TYPE.NONE;
    public ELEMENT_TYPE Resist_Type = ELEMENT_TYPE.NONE;
    public string Asset_File;
}

public class EntityDataCreator : MonoBehaviour
{
    public CharacterData playerData;
    public MonsterData enemyData;

    public List<EntityData> CreateEntityDataWithID(List<string> playerCharacterID, List<string> enemyCharacterID)
    {
        List<EntityData> entityData = new List<EntityData>();
        foreach (string id in playerCharacterID)
        {
            CharacterDataEntity entity = playerData.data.Find(element => element.Character_ID == id);
            if (entity != null)
            {
                entityData.Add(CreateEntityData(entity));
            }
        }

        foreach (string id in enemyCharacterID)
        {
            MonsterDataEntity entity = enemyData.data.Find(element => element.Mob_ID == id);
            if (entity != null)
            {
                entityData.Add(CreateEntityData(entity));
            }
        }

        return entityData;
    }
    private EntityData CreateEntityData(CharacterDataEntity playerData)
    {
        EntityData entityData = new EntityData();

        entityData.Side = SIDE.PLAYER;
        entityData.ID = playerData.Character_ID;
        entityData.Name = playerData.Char_Name;
        entityData.Default_HP = playerData.Char_Default_HP;
        entityData.Default_Attack = playerData.Char_Default_Attack;
        entityData.Default_AP = playerData.Char_Default_AP;
        entityData.Default_Speed = playerData.Char_Default_Speed;
        entityData.Skill1_ID = playerData.Skill1_ID;
        entityData.Skill2_ID = playerData.Skill2_ID;
        entityData.Skill3_ID = playerData.Skill3_ID;
        entityData.Weak_Type = playerData.Weak_Type;
        entityData.Resist_Type = playerData.Resist_Type;
        entityData.Asset_File = playerData.Asset_File;

        return entityData;
    }
    private EntityData CreateEntityData(MonsterDataEntity enemyData)
    {
        EntityData entityData = new EntityData();

        entityData.Side = SIDE.ENEMY;
        entityData.ID = enemyData.Mob_ID;
        entityData.Name = enemyData.Mob_Name;
        entityData.Default_HP = enemyData.Mob_Default_HP;
        entityData.Default_Attack = enemyData.Mob_Default_Attack;
        entityData.Default_AP = enemyData.Mob_Default_AP;
        entityData.Default_Speed = enemyData.Mob_Default_Speed;
        entityData.Skill1_ID = enemyData.Skill1_ID;
        entityData.Skill2_ID = enemyData.Skill2_ID;
        entityData.Skill3_ID = enemyData.Skill3_ID;
        entityData.Weak_Type = enemyData.Weak_Type;
        entityData.Resist_Type = enemyData.Resist_Type;
        entityData.Asset_File = enemyData.Asset_File;

        return entityData;
    }
}
