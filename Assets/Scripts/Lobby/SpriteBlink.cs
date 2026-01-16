using UnityEngine;

public class SpriteBlink : MonoBehaviour
{
    public Color colorA = Color.yellow;
    public Color colorB = Color.green;

    public float blinkSpeed = 5f;
    public float colorSpeed = 1f;

    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // 알파 빠르게
        float alpha = (Mathf.Sin(Time.time * blinkSpeed) + 1) * 0.5f;

        // 색 천천히
        float t = (Mathf.Sin(Time.time * colorSpeed) + 1) * 0.5f;
        Color c = Color.Lerp(colorA, colorB, t);
        c.a = alpha;

        sr.color = c;
    }
}
