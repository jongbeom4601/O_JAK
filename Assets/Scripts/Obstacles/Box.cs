using UnityEngine;
using System.Collections;

public class Box : MonoBehaviour, IInteractable {

    public int Priority => 50; // 2순위
    public float moveDuration = 0.1f;

    public void Interact(GameObject interactor, Vector2 direction) {
        Vector2 currentPos = transform.position;
        Vector2 targetPos = currentPos + direction;

        // 타겟 위치에 뭔가 있는지 확인
        Collider2D hit = Physics2D.OverlapCircle(targetPos, 0.1f);

        if (hit == null || !IsBlocked(hit))
        {
            // 밀 수 있으면 이동
            StartCoroutine(Move(targetPos));
        }
        else
        {
            Debug.Log("박스 밀기 실패: 뒤에 막힘");
        }
    }

    bool IsBlocked(Collider2D col)
    {
        if (col == null) return false;

        return col.CompareTag("Obstacle_Wall") ||
               col.CompareTag("Obstacle_Box") ||
               col.CompareTag("Obstacle_Breakable") ||
               col.CompareTag("Obstacle_JumpHole") ||
               col.CompareTag("Trigger_Door");
    }

    IEnumerator Move(Vector2 target)
    {
        Vector2 start = transform.position;
        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / moveDuration);

            // 감속 (EaseOutQuad)
            float easedT = 1f - (1f - t) * (1f - t);

            transform.position = Vector2.Lerp(start, target, easedT);
            yield return null;
        }

        transform.position = target;
    }
}
