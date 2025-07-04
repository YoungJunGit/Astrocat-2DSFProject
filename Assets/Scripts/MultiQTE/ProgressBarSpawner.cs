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
        PrefabProgressBar.currentActiveIndex = 0; // �� ���� �߰�

        foreach (GameObject instance in instances)
        {
            Destroy(instance);
        }
        instances.Clear();
        resultMap.Clear();

        StartCoroutine(SpawnProgressBars());
    }

    /// <summary>
    /// ������ �پ� �ִ� PrefabProgressBar��ũ��Ʈ�� �ݹ��������� �ۿ�
    /// �ݹ�: �� ������ �� �Լ� �θ��� ����, ���� �� ��Ȯ�� ���ϸ�, 
    /// �ݹ��� �ٸ� �Լ�(�޼���)�� ���ڷ� �����ؼ�,� ���� ������ �� �� �Լ��� ���߿� ȣ��Ǵ� ����
    /// private void LogResult(int index, string result) -> 1. ProgressBarSpawner.cs�� �ݹ� �Լ� LogResult()�� ����
    /// 
    /// bar.Initialize(i, LogResult); -> 2. �� �ݹ��� �� �����տ� ������
    /// ��, �� �����տ��� ����/���и� �Ǵ����� �� LogResult()�� �Ҹ���. �ش� �ε����� ����� ���� �Ѱ��ִ� ���
    /// private Action<int, string> resultCallback; public void Initialize(int idx, Action<int, string> callback)
    /// 3. PrefabProgressBar.cs �ȿ��� �ݹ��� ����� 
    /// Action<int, string>: �� ���� ���� �ѱ� �� �ִ� �Լ� (�ε���, ���)�̰� resultCallback�� �����صΰ�
    /// ���߿� ������ �����Ǹ� �ݹ� ȣ��
    /// 4. ���߿� ������ �����Ǹ� �ݹ� ȣ��
    /// esultCallback?.Invoke(index, "Perfect Success"); -> Invoke(...)�� �ݹ� �Լ��� �����մϴ�. �� ���� �� Spawner�� LogResult() �� ����˴ϴ�.
    /// 
    /// Spawner.cs
    /// ���� LogResult(index, result)
    /// ���� SpawnProgressBars()
    ///     ���� bar.Initialize(i, LogResult)  �� �ݹ� ����
    ///     
    /// PrefabProgressBar.cs
    /// ���� resultCallback = LogResult
    ///     ���� ����ڰ� A ���� or �ð� �ʰ�
    ///         ���� resultCallback.Invoke(index, ���) �� �ݹ� ����!
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
        Debug.Log($"[�����̴� ����] ȣ���! ��: {value}, ȣ�� ����: {Time.time}");    

        foreach (GameObject pb in instances)
        {
            var bar = pb.GetComponentInChildren<PrefabProgressBar>();
            bar.UpdateHighlightRange();
        }
    }

    private void LogResult(int index, string result)
    {
        Debug.Log("[Index" + index +"]"+ "���:" + result);

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
            Debug.Log("���� ����: �Ϻ� ����");
        }
        else if (failCount >= 2)
        {
            Debug.Log("���� ����: ����");
        }
        else
        {
            Debug.Log("���� ����: �Ϲ� ����");
        }
    }
}
