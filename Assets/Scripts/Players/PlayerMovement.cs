using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [Header("ĳ���� �ִϸ��̼�")]
    public Animator characterAnim; // ĳ���� �ִϸ�����

    private PlayerConfig config;
    private float gridSize = 1f;
    private float moveDuration = 0.1f;

    private bool isMoving = false;
    private bool inputLocked = false;

    public bool IsMoving => isMoving;
    public bool IsInputLocked => inputLocked;

    public void MoveTo(Vector2 targetPos, Vector2 dir = default) {
        if (!isMoving) {
            // ������ �� �Ѿ������ �ڵ� ���
            if (dir == default)
                dir = (targetPos - (Vector2)transform.position).normalized;
            StartCoroutine(Move(transform, targetPos));
        }
    }

    private IEnumerator Move(Transform obj, Vector2 target) {
        isMoving = true;
        Vector2 start = obj.position;
        float elapsed = 0f;
        characterAnim.SetTrigger("Move");
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

    // JumpHole ���� �޼���... ����ȭ �� �����ؾ� ��
    IEnumerator Jump(Transform obj, Vector2 target)
    {
        isMoving = true;

        Vector2 start = obj.position;
        float elapsed = 0f;

        float JumpDuration = moveDuration *2; // ���� �ð��� ���� ����

        while (elapsed < moveDuration)
        {

            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / moveDuration);

            // ���� (EaseOutQuad)
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


    // JumpHole���� ���
    public void TryInteractOnly(Vector2 dir) {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 0.1f);
        foreach (var hit in hits) {
            if (hit.gameObject == gameObject) continue;
            IInteractable interactable = hit.GetComponent<IInteractable>();
            if (interactable != null) {
                interactable.Interact(gameObject, dir);
                break; // ù ��°�� ó��
            }
        }
    }

    public void TeleportTo(Vector2 targetPos)
    {
        transform.position = targetPos;
    }

    public void LockInput() => inputLocked = true;   //  ��� �޼���
    public void UnlockInput() => inputLocked = false;  //  ���� �޼���
}



