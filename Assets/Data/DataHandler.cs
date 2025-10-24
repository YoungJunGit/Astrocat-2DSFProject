using UnityEngine;
using System.Collections.Generic;
using DataEntity;

[CreateAssetMenu(fileName = "DataHandler", menuName = "Data/DataHandler")]
public class DataHandler : ScriptableObject
{
    [SerializeField] private CharacterData characterData;

    public List<EntityData> CharacterData => characterData.data;
}
