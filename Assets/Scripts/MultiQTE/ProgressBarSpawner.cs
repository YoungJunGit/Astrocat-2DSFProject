using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ProgressBarSpawner : MonoBehaviour
{
    public GameObject progressBarPrefab;
    public int count = 5;
    Vector2 startPos = new Vector2(-700, 0);
    public GameObject StartButton;
    public Slider highlightSlider;

    private List<GameObject> instances = new List<GameObject>();
    private Dictionary<int, string> resultMap = new Dictionary<int, string>();

    void Start()
    {
        highlightSlider.onValueChanged.AddListener(OnHighlightRangeChanged);
        highlightSlider.value = PrefabProgressBar.highlightRange;
    }

    public void OnStartButton()
    {
        PrefabProgressBar.currentActiveIndex = 0; // ← 여기 추가

        foreach (GameObject instance in instances)
        {
            Destroy(instance);
        }
        instances.Clear();
        resultMap.Clear();

        StartCoroutine(SpawnProgressBars());
    }

    /// <summary>
    /// 프랩에 붙어 있는 PrefabProgressBar스크립트랑 콜백형식으로 작용
    /// 콜백: 다 끝나면 이 함수 부르는 구조, 조금 더 정확히 말하면, 
    /// 콜백은 다른 함수(메서드)를 인자로 전달해서,어떤 일이 끝났을 때 그 함수가 나중에 호출되는 구조
    /// private void LogResult(int index, string result) -> 1. ProgressBarSpawner.cs가 콜백 함수 LogResult()를 정의
    /// 
    /// bar.Initialize(i, LogResult); -> 2. 그 콜백을 각 프리팹에 전달함
    /// 즉, 각 프리팹에서 성공/실패를 판단했을 때 LogResult()를 불르고. 해당 인덱스와 결과를 같이 넘겨주는 방식
    /// private Action<int, string> resultCallback; public void Initialize(int idx, Action<int, string> callback)
    /// 3. PrefabProgressBar.cs 안에서 콜백이 저장됨 
    /// Action<int, string>: 두 개의 값을 넘길 수 있는 함수 (인덱스, 결과)이걸 resultCallback에 저장해두고
    /// 나중에 조건이 만족되면 콜백 호출
    /// 4. 나중에 조건이 만족되면 콜백 호출
    /// esultCallback?.Invoke(index, "Perfect Success"); -> Invoke(...)는 콜백 함수를 실행합니다. 이 순간 → Spawner의 LogResult() 가 실행됩니다.
    /// 
    /// Spawner.cs
    /// ├─ LogResult(index, result)
    /// └─ SpawnProgressBars()
    ///     └─ bar.Initialize(i, LogResult)  ← 콜백 전달
    ///     
    /// PrefabProgressBar.cs
    /// └─ resultCallback = LogResult
    ///     └─ 사용자가 A 누름 or 시간 초과
    ///         └─ resultCallback.Invoke(index, 결과) ← 콜백 실행!
    /// </summary>
    /// <returns></returns>
    private IEnumerator SpawnProgressBars()
    {
        for (int i = 0; i < count; i++)
        {
            GameObject pb = Instantiate(progressBarPrefab, transform);
            pb.name = $"ProgressBar_{i}";

            RectTransform rt = pb.GetComponent<RectTransform>();
            rt.anchoredPosition = startPos + new Vector2(350 * i, 0);

            PrefabProgressBar bar = pb.GetComponentInChildren<PrefabProgressBar>();
            bar.Initialize(i, LogResult);

            instances.Add(pb);
            yield return new WaitForSeconds(0.5f);
        }
    }
    public void OnHighlightRangeChanged(float value)
    {
        PrefabProgressBar.highlightRange = value;
        Debug.Log($"[슬라이더 변경] 호출됨! 값: {value}, 호출 시점: {Time.time}");    

        foreach (GameObject pb in instances)
        {
            var bar = pb.GetComponentInChildren<PrefabProgressBar>();
            bar.UpdateHighlightRange();
        }
    }

    private void LogResult(int index, string result)
    {
        Debug.Log("[Index" + index +"]"+ "결과:" + result);

        resultMap[index] = result;

        if (resultMap.Count == count)
        {
            EvaluateFinalResult();
        }
    }

    private void EvaluateFinalResult()
    {
        int perfectSuccessCount = 0;
        int failCount = 0;

        foreach (var res in resultMap.Values)
        {
            if (res == "Perfect Success") perfectSuccessCount++;
            if (res == "Fail") failCount++;
        }

        if (perfectSuccessCount == count)
        {
            Debug.Log("최종 판정: 완벽 성공");
        }
        else if (failCount >= 2)
        {
            Debug.Log("최종 판정: 실패");
        }
        else
        {
            Debug.Log("최종 판정: 일반 성공");
        }
    }
}
