using System;
using UnityEngine;
using UnityEngine.UI;

public class TurnTimelineSystemTester : MonoBehaviour
{
    public Button AddSpeedBtn;
    public Button DieBtn;
    public SIDE selectCharacterSide;
    [Space(10f)]
    [Range(1, 3)]
    public int buffCharacterNumber;
    [Range(1, 10)]
    public int durationRound;
    public double addSpeedValue;
    [Space(10f)]
    [Range(1, 3)]
    public int dieCharacterNumber;

    private void Awake()
    {
        var timelineSystem = GameObject.FindAnyObjectByType<TurnTimelineSystem>();
        
        AddSpeedBtn.onClick.AddListener(() => timelineSystem.OnStartBuff(buffCharacterNumber, addSpeedValue));      // �ӽ�
        DieBtn.onClick.AddListener(() => timelineSystem.OnCharacterDie(dieCharacterNumber, selectCharacterSide));
    }
}
