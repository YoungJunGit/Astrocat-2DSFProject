using DataEnum;
using ObservableCollections;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using R3;
using static CrowdControlManager;
using Unity.VisualScripting;

public class CrowdControlUnit : IUpdatable
{
    // 현재 캐릭터한테 적용된 상태이상이 무엇인지 저장하기 위한 딕셔너리
    private readonly Dictionary<ELEMENT_TYPE, List<ELEMENT_STATUS_CATEGORY>> _currentEffects = new()
        {
            {ELEMENT_TYPE.PHYSICAL, new List<ELEMENT_STATUS_CATEGORY>() },
            {ELEMENT_TYPE.FIRE, new List<ELEMENT_STATUS_CATEGORY>() },
            {ELEMENT_TYPE.RADIATION, new List<ELEMENT_STATUS_CATEGORY>() },
            {ELEMENT_TYPE.GRAVITY, new List<ELEMENT_STATUS_CATEGORY>() },
            {ELEMENT_TYPE.VOID, new List<ELEMENT_STATUS_CATEGORY>() },
            {ELEMENT_TYPE.HOLY, new List<ELEMENT_STATUS_CATEGORY>() },
            {ELEMENT_TYPE.ETC, new List<ELEMENT_STATUS_CATEGORY>() }
        };
    public IReadOnlyDictionary<ELEMENT_TYPE, IReadOnlyList<ELEMENT_STATUS_CATEGORY>> CurrentEffects =>
        _currentEffects.ToDictionary(kv => kv.Key, kv => (IReadOnlyList<ELEMENT_STATUS_CATEGORY>)kv.Value);

    // 캐릭터한테 적용된 상태이상의 중첩 또는 지속 턴 수를 저장하기 위한 딕셔너리
    private readonly Dictionary<ELEMENT_TYPE, ReactiveProperty<int>> _effectsCountDic = new()
    {
        { ELEMENT_TYPE.PHYSICAL, new ReactiveProperty<int>(0) },
        { ELEMENT_TYPE.FIRE, new ReactiveProperty<int>(0) },
        { ELEMENT_TYPE.RADIATION, new ReactiveProperty<int>(0) },
        { ELEMENT_TYPE.GRAVITY, new ReactiveProperty<int>(0) },
        { ELEMENT_TYPE.VOID, new ReactiveProperty<int>(0) },
        { ELEMENT_TYPE.HOLY, new ReactiveProperty<int>(0) },
        { ELEMENT_TYPE.ETC, new ReactiveProperty<int>(0) }
    };
    public IReadOnlyDictionary<ELEMENT_TYPE, ReadOnlyReactiveProperty<int>> EffectsCountDic =>
        _effectsCountDic.ToDictionary(kv => kv.Key, kv => kv.Value.ToReadOnlyReactiveProperty());

    private readonly Dictionary<ELEMENT_TYPE, List<ICrowdControl>> _stackableCC = new();
    private readonly Dictionary<ELEMENT_TYPE, ICrowdControl> _nonStackableCC = new();

    public ICrowdControl GetNonStackCC(ELEMENT_TYPE type) => _nonStackableCC.TryGetValue(type, out var cc) ? cc : null;
    public ELEMENT_TYPE Previous_Element_Type { get; set; } = ELEMENT_TYPE.NONE;

    public void Add(ELEMENT_TYPE elementType)
    {
        if (elementType != ELEMENT_TYPE.ETC)
            Previous_Element_Type = elementType;

        _effectsCountDic[elementType].Value++;
        UpdateCurrentEffects(elementType);
    }

    public void Remove(ELEMENT_TYPE elementType)
    {
        _currentEffects[elementType].Clear();
        _effectsCountDic[elementType].Value = 0;
        UpdateCurrentEffects(elementType);

        if(!CheckAnyEffects())
            Previous_Element_Type = ELEMENT_TYPE.NONE;
    }

    // 중첩이 가능한 상태이상 저장용
    public void AddStackCC(ELEMENT_TYPE type, ICrowdControl cc)
    {
        _stackableCC.TryAdd(type, new List<ICrowdControl>());
        _stackableCC[type].Add(cc);
    }

    // 중첩이 되지 않는(지속 턴이 존재하는) 상태이상 저장용
    public void AddNonStackCC(ELEMENT_TYPE type, ICrowdControl cc)
    {
        _nonStackableCC[type] = cc;

        if(cc is AttributeControl ac)
        {
            ac.Effect.OnDispose += () => { _nonStackableCC.Remove(type); };
        }
    }

    public void RemoveStackCC(ELEMENT_TYPE type)
    {
        if (_stackableCC.TryGetValue(type, out var list))
        {
            foreach (var cc in list)
                cc.Dispose();
            list.Clear();
        }
        _stackableCC.Remove(type);
    }

    public void RemoveNonStackCC(ELEMENT_TYPE type)
    {
        if (_nonStackableCC.TryGetValue(type, out var cc))
        {
            cc.Dispose();
        }
        _nonStackableCC.Remove(type);
    }

    // Not Using
    public void OnRoundUpdate() { }

    public void OnTurnUpdate()
    {
        Reduce(ELEMENT_TYPE.PHYSICAL);
        Reduce(ELEMENT_TYPE.VOID);
        Reduce(ELEMENT_TYPE.HOLY);
        Reduce(ELEMENT_TYPE.ETC);
    }

    private void Reduce(ELEMENT_TYPE elementType)
    {
        if (_effectsCountDic[elementType].Value <= 0)
            return;

        _effectsCountDic[elementType].Value--;

        var result = UpdateCurrentEffects(elementType);
        if (result == 0)
        {
            _nonStackableCC.Remove(elementType);

            if (!CheckAnyEffects())
                Previous_Element_Type = ELEMENT_TYPE.NONE;
        }
        else if (result == -1)
        {
            Debug.Log("Unexpected Error Ocurred!!!");
        }
    }

    // -1 : Unexpected Error, 0 : Failed, 1 : Success
    private int UpdateCurrentEffects(ELEMENT_TYPE elementType)
    {
        if (!_currentEffects.TryGetValue(elementType, out var list) || !_effectsCountDic.TryGetValue(elementType, out var countRp))
            return -1;

        list.Clear();

        int count = Mathf.Max(0, countRp.Value);
        if (count == 0)
            return 0;

        // ETC는 단일/중첩 개념 없이 "count 1개당 Chaos 1개"
        if (elementType == ELEMENT_TYPE.ETC)
        {
            for (int i = 0; i < count; i++)
                list.Add(ELEMENT_STATUS_CATEGORY.CHAOS);

            return 1;
        }

        // 일반 속성: 룰테이블에서 Basic / Enhanced 가져오기
        if (!ElementStatusRuleTable.TryGetRule(elementType, out var basic, out var enhanced))
            return -1;

        // 2개당 Enhanced(중첩) 1개
        int stacked = count / 2;
        for (int i = 0; i < stacked; i++)
            list.Add(enhanced);

        // 나머지 1개면 Basic(단일) 1개
        if ((count % 2) == 1)
            list.Add(basic);

        return 1;
    }

    private bool CheckAnyEffects() => CurrentEffects.Any(kv => kv.Value.Count > 0);
}