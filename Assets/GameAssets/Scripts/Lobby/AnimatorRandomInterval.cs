using UnityEngine;
using System.Collections;
public class AnimatorRandomInterval : MonoBehaviour
{
    // 이 오브젝트에 붙어 있는 Animator 참조
    // Inspector에서 직접 넣어도 되고, 비어 있으면 Awake에서 자동으로 가져옴
    [SerializeField] private Animator animator;

    // 애니메이션 재생이 끝난 후 대기할 랜덤 시간 범위 (초 단위)
    // 매번 Play 사이클마다 이 범위 안에서 다시 랜덤으로 뽑힘
    [SerializeField] private Vector2 waitRange = new Vector2(3f, 5f);

    private void Awake()
    {
        // Inspector에 Animator를 안 넣었을 경우
        // 같은 GameObject에 붙어 있는 Animator를 자동으로 찾음
        if (!animator)
            animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        // 오브젝트가 활성화될 때 첫 재생 시작
        StartCoroutine(Play());
    }

    private IEnumerator Play()
    {
        // 1 애니메이션을 처음 프레임부터 재생
        // (Animator의 첫 번째 레이어, 첫 번째 상태)
        animator.Play(0, 0, 0f);

        // 2 Animator 상태 정보가 갱신되도록 한 프레임 대기
        // (이걸 안 하면 length 값이 정확하지 않을 수 있음)
        yield return null;

        // 3 현재 재생 중인 애니메이션의 길이(초)를 가져와서
        // 애니메이션이 끝날 때까지 대기
        float length = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(length);

        // 4 애니메이션이 끝난 후,
        // 지정한 범위 안에서 매번 새로 랜덤 대기 시간 계산
        yield return new WaitForSeconds(Random.Range(waitRange.x, waitRange.y));

        // 5 다시 자기 자신을 호출해서
        // "재생 → 랜덤 대기 → 재생" 구조를 반복 (재귀 Coroutine)
        StartCoroutine(Play());
    }
}
