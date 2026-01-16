using UnityEngine;

public class SignFlickerRandomize : MonoBehaviour
{
    //애니메이션 재생 속도 랜덤 기능
    [SerializeField] private Animator animator;
    [SerializeField] private Vector2 speedRange = new Vector2(0.85f, 1.15f);
    [SerializeField] private bool randomStart = true;

    private void Awake()
    {
        if (!animator) animator = GetComponent<Animator>();

        // 1) 속도 랜덤
        animator.speed = Random.Range(speedRange.x, speedRange.y);

        // 2) 시작 시점 랜덤 (0~1 = 클립 전체 길이의 비율)
        if (randomStart)
            animator.Play(0, 0, Random.value);
    }
}
