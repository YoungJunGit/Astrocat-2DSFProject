using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class PrefabProgressBar : MonoBehaviour
{
    public Image LoadingBar;
    public Image LoadingBarYellow;
    public Image Center;
    public Text resultIndicator;

    public float duration = 1f;

    public static float highlightRange = 0.1f;  // static으로 변경

    float remainingTime = 0f;
    float currentValue = 0f;
    float yellowStartTime;
    bool isFailed = false;
    bool isPressed = false;
    float scale = 2.5f;

    private int index;
    private Action<int, string> resultCallback;

    public static int currentActiveIndex = 0;
    private static bool isWaitingForRelease = false;


    void Start()
    {
        LoadingBarYellow.fillOrigin = 2;
        LoadingBarYellow.fillAmount = highlightRange;
        yellowStartTime = duration * (1f - highlightRange);
        currentValue = 0;
    }

    public void UpdateHighlightRange()
    {
        if (isFailed) return;

        yellowStartTime = duration * (1f - highlightRange);
        LoadingBarYellow.fillAmount = highlightRange;
        Debug.Log($"[UpdateHighlightRange] index:{index}, fill:{highlightRange}, called from: {new System.Diagnostics.StackTrace()}");
    }


    public void Initialize(int idx, Action<int, string> callback)
    {
        index = idx;
        resultCallback = callback;
        LoadingBarYellow.fillAmount = highlightRange;
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

            if (currentValue >= yellowStartTime && !isFailed)
                UpdateYellowBar();

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

        if (currentValue >= yellowStartTime)
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
        if (isFailed) return;

        float elapsed = currentValue - yellowStartTime;
        float progress = Mathf.Clamp01(elapsed / (highlightRange*duration));   // 0 ~ 1

        LoadingBarYellow.fillAmount = Mathf.Lerp(highlightRange, 0f, progress);
    }

}