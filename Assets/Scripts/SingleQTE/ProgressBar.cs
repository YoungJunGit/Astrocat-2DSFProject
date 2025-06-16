using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    public GameObject StartButton;
    public Text ProgressIndicator;
    public Image LoadingBar;
    public Image Center;
    public Text resultIndicator;
    public float duration = 1f;

    float remainingTime = 0f;
    float currentValue = 0f;
    float scale = 2.5f;
    bool isFailed = false;
    bool isPressed = false;
    bool isStarted = false;

    void Start()
    {
        ProgressIndicator.gameObject.SetActive(false);
        LoadingBar.gameObject.SetActive(false);
        Center.gameObject.SetActive(false);
        resultIndicator.gameObject.SetActive(false);
    }

    /// <summary>
    /// Mathf.Clamp: 숫자 값을 특정 범위 안으로 제한(clamp)
    /// Mathf.Clamp(x, min, max), x: 클램핑할 대상 값, min: 최소값, max: 최대값
    /// 결과: x가 min보다 작으면 min, max보다 크면 max, 그 외는 x
    /// duration: 총 제한 시간 (예: 1초), currentValue: 지금까지 경과한 시간(Time.deltaTime으로 누적)
    /// duration - currentValue: 남은 시간
    /// Clamp(..., 0f, duration) : 남은 시간이 0보다 작거나 duration보다 큰 경우를 방지
    /// remainingTime / duration : 총 시간 중에 얼마나 남았는가"를 비율로 계산한 것
    ///                            이 값을 fillAmount에 넣으면 시간이 줄어들수록 바가 비워지는 효과
    /// </summary>
    void Update()
    {
        if (!isStarted)
            return;

        if (currentValue < duration && !isFailed && !isPressed)
        {
            currentValue += Time.deltaTime;
            remainingTime = Mathf.Clamp(duration - currentValue, 0f, duration);

            ProgressIndicator.text = currentValue.ToString("F2") + "s";
            LoadingBar.fillAmount = Mathf.Clamp01(remainingTime / duration);

            if (Input.GetKeyDown(KeyCode.A))
            {
                isPressed = true;
                LoadingBar.fillAmount = 1f;
                SetAlphaScale(0.3f);

                if (currentValue >= 0.45f && currentValue <= 0.55f)
                {
                    resultIndicator.text = "Perfect Success";
                }
                else
                {
                    resultIndicator.text = "Success";
                }
            }
        }
        else if (!isPressed && !isFailed)
        {
            isFailed = true;
            ProgressIndicator.text = "Done";
            LoadingBar.fillAmount = 1f;
            resultIndicator.text = "Fail";
        }
    }

    /// <summary>
    /// 버튼이 눌리기 전까지는 전부 setactive(false)
    /// 버튼이 눌리면 모두 초기화
    /// </summary>
    public void OnStartButton()
    {
        ProgressIndicator.gameObject.SetActive(true);
        LoadingBar.gameObject.SetActive(true);
        Center.gameObject.SetActive(true);
        resultIndicator.gameObject.SetActive(true);

        currentValue = 0f;
        isFailed = false;
        isPressed = false;
        isStarted = true;

        ProgressIndicator.text = "0.00s";
        LoadingBar.fillAmount = 1f;
        resultIndicator.text = "A";

        ObjectReset();
    }

    private void SetAlphaScale(float alpha)
    {
        Color barColor = LoadingBar.color;
        barColor.a = alpha;
        LoadingBar.color = barColor;

        Color centerColor = Center.color;
        centerColor.a = alpha;
        Center.color = centerColor;

        LoadingBar.transform.localScale = new Vector2(scale, scale);
        Center.transform.localScale = new Vector2(scale, scale);
        resultIndicator.transform.localScale = new Vector2(scale / 2, scale / 2);
    }

    private void ObjectReset() {
        Color barColor = LoadingBar.color;
        barColor.a = 1f;
        LoadingBar.color = barColor;

        Color centerColor = Center.color;
        centerColor.a = 1f;
        Center.color = centerColor;

        LoadingBar.transform.localScale = new Vector2(3f, 3f);
        Center.transform.localScale = new Vector2(2.7f, 2.7f);
        resultIndicator.transform.localScale = new Vector2(1f, 1f);
    }
}
