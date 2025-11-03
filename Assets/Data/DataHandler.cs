using UnityEngine;
using System.Collections.Generic;
using DataEntity;

[CreateAssetMenu(fileName = "DataHandler", menuName = "Data/DataHandler")]
public class DataHandler : ScriptableObject
{
    [SerializeField] private EntityDataScriptableObject characterData;

    public List<EntityData> CharacterData => characterData.data;
}
