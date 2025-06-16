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
    /// Mathf.Clamp: ���� ���� Ư�� ���� ������ ����(clamp)
    /// Mathf.Clamp(x, min, max), x: Ŭ������ ��� ��, min: �ּҰ�, max: �ִ밪
    /// ���: x�� min���� ������ min, max���� ũ�� max, �� �ܴ� x
    /// duration: �� ���� �ð� (��: 1��), currentValue: ���ݱ��� ����� �ð�(Time.deltaTime���� ����)
    /// duration - currentValue: ���� �ð�
    /// Clamp(..., 0f, duration) : ���� �ð��� 0���� �۰ų� duration���� ū ��츦 ����
    /// remainingTime / duration : �� �ð� �߿� �󸶳� ���Ҵ°�"�� ������ ����� ��
    ///                            �� ���� fillAmount�� ������ �ð��� �پ����� �ٰ� ������� ȿ��
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
    /// ��ư�� ������ �������� ���� setactive(false)
    /// ��ư�� ������ ��� �ʱ�ȭ
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
