using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerParty", menuName = "PlayerParty", order = 3)]
public class PlayerParty : ScriptableObject
{
    [SerializeField]
    private List<string> playerCharacterID = new List<string>();

    public List<string> GetPlayerParty()
    {
        return playerCharacterID;
    }
}
