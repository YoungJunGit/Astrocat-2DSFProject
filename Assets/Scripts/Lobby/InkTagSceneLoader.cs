using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using PixelCrushers.DialogueSystem;
using PixelCrushers.DialogueSystem.InkSupport;
using Ink.Runtime;

public class InkTagSceneLoader : MonoBehaviour
{
    [Serializable]
    public struct SceneMap
    {
        public string key;        // Ink에서 쓰는 키
        public string sceneName;  // 실제 Unity 씬 이름
    }

    public DialogueSystemInkIntegration inkIntegration;

    [Header("Ink JSON 이름(=TextAsset.name)")]
    public string inkJsonAssetName;

    [Header("Scene Mapping")]
    public SceneMap[] sceneMaps;

    private Dictionary<string, string> map;
    private bool isLoading = false;

    private void Awake()
    {
        map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var m in sceneMaps)
        {
            var key = (m.key ?? string.Empty).Trim();
            if (string.IsNullOrEmpty(key)) continue;

            if (!map.ContainsKey(key))
            {
                map.Add(key, m.sceneName);
                Debug.Log($"[InkTag] SceneMap 등록: {key} -> {m.sceneName}");
            }
        }
    }

    public void OnConversationLine(Subtitle subtitle)
    {
        Debug.Log($"[InkTag] subtitle.formattedText='{subtitle.formattedText}'");
        Debug.Log($"[InkTag] subtitle.sequence='{subtitle.sequence}'");

        if (isLoading)
        {
            Debug.Log("[InkTag] 이미 씬 로딩 중 → 무시");
            return;
        }

        if (inkIntegration == null)
        {
            Debug.LogWarning("[InkTag] inkIntegration이 null입니다.");
            return;
        }

        Story story = inkIntegration.GetStory(inkJsonAssetName);
        if (story == null)
        {
            Debug.LogError($"[InkTag] Story not found. inkJsonAssetName='{inkJsonAssetName}'");
            return;
        }

        var tags = story.currentTags;

        if (tags == null || tags.Count == 0)
        {
            Debug.Log("[InkTag] 현재 라인에 태그 없음");
            return;
        }

        Debug.Log($"[InkTag] 태그 감지됨 ({tags.Count}개): {string.Join(", ", tags)}");

        foreach (var raw in tags)
        {
            var tag = (raw ?? string.Empty).Trim();

            Debug.Log($"[InkTag] 태그 검사 중: '{tag}'");

            if (!tag.StartsWith("LOAD_SCENE:", StringComparison.OrdinalIgnoreCase))
            {
                Debug.Log("[InkTag] LOAD_SCENE 태그 아님 → 스킵");
                continue;
            }

            var sceneKey = tag.Substring("LOAD_SCENE:".Length).Trim();
            Debug.Log($"[InkTag] LOAD_SCENE 태그 감지! key = '{sceneKey}'");

            if (!map.TryGetValue(sceneKey, out var sceneName) || string.IsNullOrEmpty(sceneName))
            {
                Debug.LogError($"[InkTag] Scene key 매핑 실패: '{sceneKey}'");
                return;
            }

            Debug.Log($"[InkTag] 씬 로드 예약: '{sceneName}'");

            isLoading = true;
            StartCoroutine(LoadNextFrame(sceneName));
            return; // 한 번만 로드
        }
    }

    private IEnumerator LoadNextFrame(string sceneName)
    {
        Debug.Log("[InkTag] 대화 종료 요청");
        DialogueManager.StopConversation();

        yield return null;

        Debug.Log($"[InkTag] 씬 로드 실행: '{sceneName}'");
        SceneManager.LoadScene(sceneName);
    }
}