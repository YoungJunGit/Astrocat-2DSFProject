using System;
using UnityEngine;

public class PlayerSaveData
{
    public float HP;
    public float SP;
    public PlayerSaveData(float HP, float SP)
    {
        this.HP = HP;
        this.SP = SP;
    }
}

[CreateAssetMenu(fileName = "PlayerPartyData", menuName = "SO/Combat/SaveData/PlayerPartyData", order = 1)]
public class PlayerPartyData : ScriptableObject
{
    // 추후 SerializeFiled에서 제외
    [SerializeField]
    private string[] selectedPlayerUnitID = new string[3];
    public string[] SelectedPlayerUnitID => selectedPlayerUnitID;
    private const string PLAYER_DATA_KEY = "Player_Data_Key_";

    public void EnsureInitialized()
    {
        foreach (var ID in selectedPlayerUnitID)
        {
            if (ID == String.Empty || ES3.KeyExists(PLAYER_DATA_KEY + ID))
                continue;

            ES3.Save(PLAYER_DATA_KEY + ID, new PlayerSaveData(1.0f, 1.0f), Easy3MetaData.PlayerFile);
        }
    }

    public void SavePlayerData(BaseUnit unit)
    {
        string ID = unit.GetStat().CoreStat.ID;
        float HP = unit.GetStat().HP / unit.GetStat().ModifierStat.MaxHP;
        float SP = (float)unit.GetStat().SP / unit.GetStat().ModifierStat.MaxSP;
        
        ES3.Save(PLAYER_DATA_KEY + ID, new PlayerSaveData(HP, SP), Easy3MetaData.PlayerFile);
    }

    public PlayerSaveData LoadPlayerData(string ID)
    {
        return ES3.Load<PlayerSaveData>(PLAYER_DATA_KEY + ID, Easy3MetaData.PlayerFile);
    }
}