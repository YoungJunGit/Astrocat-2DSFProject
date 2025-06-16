using UnityEngine;
using UnityEngine.UI;
using System;

public class PrefabProgressBar : MonoBehaviour
{
    public Image LoadingBar;
    public Image Center;
    public Text resultIndicator;

    public float duration = 1f;
    float remainingTime = 0f;
    float currentValue = 0f;
    bool isFailed = false;
    bool isPressed = false;
    float scale = 2.5f;

    private int index;
    private Action<int, string> resultCallback;

    public static int currentActiveIndex = 0;
    private static bool isWaitingForRelease = false;

    public void Initialize(int idx, Action<int, string> callback)
    {
        index = idx;
        resultCallback = callback;
    }
    /// <summary>
    /// currentValue += Time.deltaTime; -> 시간 흐름에 따라 증가
    /// remainingTime은 전체 시간에서 남은 시간 계산 -> ex) remainingTime = 1 - 0.3 = 0.7
    /// fillAmount는 남은 시간 비율로 설정, 남은 시간이 줄어들수록 fillAmount는 점점 0에 가까워짐 
    /// → 즉, Progress Bar가 점점 줄어드는 것처럼 보이는 것
    /// 
    /// 그냥 남는 시간만큼 fillamout가 되고, 굳이 duration으로 나누는 이유는 후에 초가 늘어날 수 있는 상황 대비
    /// currentValue가 0.3인 기준으로 보면, remainingTime = 1 - 0.3 = 0.7. 0.7/1은 0.7이니까 결국 fillamout는 0.7이 들어간다.
    /// </summary>
    void Update()
    {
        if (index != currentActiveIndex)
            return;

        if (currentValue < duration && !isFailed && !isPressed)
        {
            currentValue += Time.deltaTime;
            remainingTime = Mathf.Clamp(duration - currentValue, 0f, duration);
            LoadingBar.fillAmount = Mathf.Clamp01(remainingTime / duration);

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
        LoadingBar.fillAmount = 1f;
        SetAlpha(0.3f);

        LoadingBar.transform.localScale = new Vector2(scale, scale);
        Center.transform.localScale = new Vector2(scale, scale);
        resultIndicator.transform.localScale = new Vector2(scale / 2, scale / 2);

        if (currentValue >= 0.45f && currentValue <= 0.55f)
        {
            resultIndicator.text = "Perfect\n Success";
            resultCallback?.Invoke(index, "Perfect\n Success");
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
}
