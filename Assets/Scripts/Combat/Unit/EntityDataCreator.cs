using System.Collections.Generic;
using UnityEngine;
using DataEntity;
using DataEnum;

[CreateAssetMenu(fileName = "EntityDataCreator", menuName = "EntityDataCreator", order = 4)]
public class EntityDataCreator : ScriptableObject
{
    public PlayerData playerData;
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
    private EntityData CreateEntityData(CharacterDataEntity characterData)
    {
        EntityData entityData = new EntityData();

        entityData.Side = SIDE.PLAYER;
        entityData.ID = characterData.Character_ID;
        entityData.Name = characterData.Char_Name;
        entityData.Default_HP = characterData.Char_Default_HP;
        entityData.Default_Attack = characterData.Char_Default_Attack;
        entityData.Default_Defense = characterData.Char_Default_Defense;
        entityData.Default_AP = characterData.Char_Default_AP;
        entityData.Default_Speed = characterData.Char_Default_Speed;
        entityData.Skill1_ID = characterData.Skill1_ID;
        entityData.Skill2_ID = characterData.Skill2_ID;
        entityData.Skill3_ID = characterData.Skill3_ID;
        entityData.Weak_Type = characterData.Weak_Type;
        entityData.Resist_Type = characterData.Resist_Type;
        entityData.Asset_File = characterData.Asset_File;

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
        entityData.Default_Defense = enemyData.Mob_Default_Defense;
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
