using UnityEngine;
using System.Collections;
using Unity.VisualScripting.FullSerializer;

public enum Motion { None, Walk, Jump }

public class PlayerMovement : MonoBehaviour
{
    private PlayerConfig config;
    private float moveDuration = 0.1f;

    [Header("애니메이션")]
    [SerializeField] private Animator animator;

    private bool isMoving = false;
    private bool inputLocked = false;

    public bool IsMoving => isMoving;
    public bool IsInputLocked => inputLocked;



    public void MoveTo(Vector2 targetPos, Motion motion = Motion.Walk, Vector2 dir = default) {
        if (!isMoving) {
            // 방향이 안 넘어왔으면 자동 계산
            if (dir == default)
                dir = (targetPos - (Vector2)transform.position).normalized;

            PlayMotion(motion);
            StartCoroutine(Move(transform, targetPos));
        }
    }

    private IEnumerator Move(Transform obj, Vector2 target) {
        isMoving = true;
        Vector2 start = obj.position;
        float elapsed = 0f;
        // animator.SetTrigger("Walk");
        while (elapsed < moveDuration) {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / moveDuration);
            float easedT = 1f - (1f - t) * (1f - t);
            obj.position = Vector2.Lerp(start, target, easedT);
            yield return null;
        }
        obj.position = target;
        isMoving = false;
    }

    void PlayMotion(Motion motion)
    {
        if (!animator) return;

        // 중복 방지: 관련 트리거 초기화
        animator.ResetTrigger("Walk");
        animator.ResetTrigger("Jump");

        switch (motion)
        {
            case Motion.Walk:
                animator.SetTrigger("Walk");
                break;
            case Motion.Jump:
                animator.SetTrigger("Jump");
                break;
            default:
                break;
        }
    }




    // JumpHole 전용 메서드... 최적화 시 수정해야 함
    IEnumerator Jump(Transform obj, Vector2 target)
    {
        isMoving = true;

        Vector2 start = obj.position;
        float elapsed = 0f;

        float JumpDuration = moveDuration *2; // 점프 시간만 따로 설정

        while (elapsed < moveDuration)
        {

            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / moveDuration);

            // 감속 (EaseOutQuad)
            float easedT = 1f - (1f - t) * (1f - t);

            obj.position = Vector2.Lerp(start, target, easedT);
            yield return null;
        }

        obj.position = target;
        isMoving = false;

    }

    public void JumpTo(Vector2 targetPos)
    {
        if (!isMoving)
        {
            StartCoroutine(Jump(transform, targetPos));
        }
    }


    // JumpHole에서 사용
    public void TryInteractOnly(Vector2 dir) {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 0.1f);
        foreach (var hit in hits) {
            if (hit.gameObject == gameObject) continue;
            IInteractable interactable = hit.GetComponent<IInteractable>();
            if (interactable != null) {
                interactable.Interact(gameObject, dir);
                break; // 첫 번째만 처리
            }
        }
    }

    public void TeleportTo(Vector2 targetPos)
    {
        transform.position = targetPos;
    }

    public void LockInput() => inputLocked = true;   //  잠금 메서드
    public void UnlockInput() => inputLocked = false;  //  해제 메서드
}



