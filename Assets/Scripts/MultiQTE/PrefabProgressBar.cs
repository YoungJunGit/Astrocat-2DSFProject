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
    /// currentValue += Time.deltaTime; -> �ð� �帧�� ���� ����
    /// remainingTime�� ��ü �ð����� ���� �ð� ��� -> ex) remainingTime = 1 - 0.3 = 0.7
    /// fillAmount�� ���� �ð� ������ ����, ���� �ð��� �پ����� fillAmount�� ���� 0�� ������� 
    /// �� ��, Progress Bar�� ���� �پ��� ��ó�� ���̴� ��
    /// 
    /// �׳� ���� �ð���ŭ fillamout�� �ǰ�, ���� duration���� ������ ������ �Ŀ� �ʰ� �þ �� �ִ� ��Ȳ ���
    /// currentValue�� 0.3�� �������� ����, remainingTime = 1 - 0.3 = 0.7. 0.7/1�� 0.7�̴ϱ� �ᱹ fillamout�� 0.7�� ����.
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
