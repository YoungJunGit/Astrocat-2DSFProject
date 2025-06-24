using UnityEngine;
using UnityEngine.UI;
using System;

public class PrefabProgressBar : MonoBehaviour
{
    public Image LoadingBar;
    public Image LoadingBarYellow;
    public Image Center;
    public Text resultIndicator;

    public float duration = 1f;
    public float highlightMin = 0.15f;
    public float highlightMax = 0.55f;
    float highlightRange;
    float highlightStart;
    float highlightEnd;

    float remainingTime = 0f;
    float currentValue = 0f;
    bool isFailed = false;
    bool isPressed = false;
    float scale = 2.5f;

    private int index;
    private Action<int, string> resultCallback;

    public static int currentActiveIndex = 0;
    private static bool isWaitingForRelease = false;

    void Start()
    {
        highlightStart = duration * highlightMin;
        highlightEnd = duration * highlightMax;
        highlightRange = highlightMax - highlightMin;

        LoadingBarYellow.fillOrigin = 2;
        float startAngle = highlightMin * 360f;
        LoadingBarYellow.fillAmount = highlightRange;
        LoadingBarYellow.rectTransform.localRotation = Quaternion.Euler(0, 0, startAngle);
        LoadingBarYellow.gameObject.SetActive(true);
    }

    public void Initialize(int idx, Action<int, string> callback)
    {
        index = idx;
        resultCallback = callback;
    }

    void Update()
    {
        if (index != currentActiveIndex)
            return;

        if (currentValue < duration && !isFailed && !isPressed)
        {
            currentValue += Time.deltaTime;
            remainingTime = Mathf.Clamp(duration - currentValue, 0f, duration);
            LoadingBar.fillAmount = Mathf.Clamp01(remainingTime / duration);

            if (currentValue >= highlightStart && currentValue <= highlightEnd)
                UpdateYellowBar();

            else if (currentValue > duration * highlightMax)
                LoadingBarYellow.gameObject.SetActive(false);

            if (Input.GetKeyDown(KeyCode.A))
            {
                isPressed = true;
                HandlePress();
                isWaitingForRelease = true;
            }
        }
        else if (!isPressed && !isFailed && currentValue >= duration)
        {
            isFailed = true;
            HandleFail();
            isWaitingForRelease = true;
            currentActiveIndex++;
        }
        else if (isWaitingForRelease && Input.GetKeyUp(KeyCode.A))
        {
            currentActiveIndex++;
            isWaitingForRelease = false;
        }
    }

    private void HandlePress()
    {
        LoadingBarYellow.gameObject.SetActive(false);
        LoadingBar.fillAmount = 1f;
        SetAlpha(0.3f);

        LoadingBar.transform.localScale = new Vector2(scale, scale);
        Center.transform.localScale = new Vector2(scale, scale);
        resultIndicator.transform.localScale = new Vector2(scale / 2, scale / 2);

        if (currentValue >= 0.45f*duration && currentValue <= 0.55f*duration)
        {
            resultIndicator.text = "Excellent";
            resultCallback?.Invoke(index, "Excellent");
        }
        else
        {
            resultIndicator.text = "Success";
            resultCallback?.Invoke(index, "Success");
        }
    }

    private void HandleFail()
    {
        LoadingBar.fillAmount = 1f;
        SetAlpha(0.3f);

        LoadingBar.transform.localScale = new Vector2(scale, scale);
        Center.transform.localScale = new Vector2(scale, scale);
        resultIndicator.transform.localScale = new Vector2(scale / 2, scale / 2);

        resultIndicator.text = "Fail";
        resultCallback?.Invoke(index, "Fail");
    }

    private void SetAlpha(float alpha)
    {
        Color barColor = LoadingBar.color;
        barColor.a = alpha;
        LoadingBar.color = barColor;

        Color centerColor = Center.color;
        centerColor.a = alpha;
        Center.color = centerColor;
    }

    private void UpdateYellowBar()
    {
        // 전체 경과 비율 (duration 기준)
        float t = currentValue / duration;

        // 노란 바가 나타나는 시점 ~ 사라지는 시점 사이의 진행률 (0~1)
        float highlightProgress = (t - highlightMin) / highlightRange;
        highlightProgress = Mathf.Clamp01(highlightProgress);

        // 줄어드는 형태: 1 → 0으로 선형 감소
        LoadingBarYellow.fillAmount = Mathf.Lerp(highlightRange, 0f, highlightProgress);
    }
}