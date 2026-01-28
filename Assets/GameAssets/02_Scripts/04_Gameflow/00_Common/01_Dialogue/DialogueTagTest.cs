using System;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;

public class DialogueTagTest : MonoBehaviour
{
    [Serializable]
    public class UIMap
    {
        public string key;
        public GameObject target;
    }

    public List<UIMap> mappings = new();
    public bool exclusiveShow = false;

    Dictionary<string, GameObject> _map;

    void Awake()
    {
        _map = new Dictionary<string, GameObject>(StringComparer.OrdinalIgnoreCase);
        foreach (var m in mappings)
        {
            if (m == null || string.IsNullOrWhiteSpace(m.key) || m.target == null) continue;
            _map[m.key.Trim()] = m.target;
        }

        Debug.Log($"[DialogueTagTest] UIMap 초기화 완료. 등록된 키 수: {_map.Count}");
    }

    // story.currentTags 를 그대로 넣어주면 되는 진입점
    public void HandleInkTags(IList<string> tags)
    {
        if (tags == null)
        {
            Debug.LogWarning("[DialogueTagTest] HandleInkTags 호출됨, tags == null");
            return;
        }

        if (tags.Count == 0)
        {
            Debug.Log("[DialogueTagTest] HandleInkTags 호출됨, 태그 없음");
            return;
        }

        Debug.Log($"[DialogueTagTest] HandleInkTags 호출됨, 태그 {tags.Count}개 감지");

        foreach (var raw in tags)
        {
            Debug.Log($"[DialogueTagTest] 원본 태그 수신: '{raw}'");

            if (TryParse(raw, "SHOW_UI", out var key))
            {
                Debug.Log($"[DialogueTagTest] SHOW_UI 태그 인식 성공 → key = '{key}'");

                if (exclusiveShow)
                {
                    Debug.Log("[DialogueTagTest] exclusiveShow 활성 → 기존 UI 전부 숨김");
                    HideAll();
                }

                SetActive(key, true);
            }
            else if (TryParse(raw, "HIDE_UI", out key))
            {
                Debug.Log($"[DialogueTagTest] HIDE_UI 태그 인식 성공 → key = '{key}'");

                if (key.Equals("ALL", StringComparison.OrdinalIgnoreCase))
                {
                    Debug.Log("[DialogueTagTest] HIDE_UI: ALL → 전체 UI 숨김");
                    HideAll();
                }
                else
                {
                    SetActive(key, false);
                }
            }
            else
            {
                Debug.Log($"[DialogueTagTest] 태그 파싱 실패 또는 무시됨: '{raw}'");
            }
        }
    }

    bool TryParse(string raw, string cmd, out string value)
    {
        value = null;
        if (string.IsNullOrWhiteSpace(raw)) return false;

        // Ink tag 예: "SHOW_UI: tutorial_a"
        var s = raw.Trim();

        if (!s.StartsWith(cmd, StringComparison.OrdinalIgnoreCase))
            return false;

        var idx = s.IndexOf(':');
        if (idx < 0)
        {
            Debug.LogWarning($"[DialogueTest] '{cmd}' 태그에 ':' 없음 → '{raw}'");
            return false;
        }

        value = s[(idx + 1)..].Trim();
        return !string.IsNullOrEmpty(value);
    }

    void SetActive(string key, bool active)
    {
        if (_map.TryGetValue(key, out var go) && go != null)
        {
            go.SetActive(active);
            Debug.Log($"[DialogueTest] UI '{key}' SetActive({active})");
        }
        else
        {
            Debug.LogWarning($"[DialogueTest] UI key 매핑 실패: '{key}'");
        }
    }

    void HideAll()
    {
        Debug.Log("[DialogueTest] HideAll 호출");
        foreach (var kv in _map)
        {
            if (kv.Value != null)
                kv.Value.SetActive(false);
        }
    }
}
