using UnityEngine;
using System.Collections;

public class GeonwooMovement : MonoBehaviour
{
    public float gridSize = 1f;
    public float moveDuration = 0.1f;

    private bool isMoving = false;

    private int moveCount = 0; // 움직인 횟수
    private int inputCount = 0; // 입력한 횟수 (상호작용도 포함)


    void Update()
    {
        if (isMoving) return;

        Vector2 input = Vector2.zero;
        if (Input.GetKeyDown(KeyCode.W)) input = Vector2.up;
        else if (Input.GetKeyDown(KeyCode.S)) input = Vector2.down;
        else if (Input.GetKeyDown(KeyCode.A)) input = Vector2.left;
        else if (Input.GetKeyDown(KeyCode.D)) input = Vector2.right;

        if (input != Vector2.zero)
        {

            inputCount++;
            Debug.Log("방향키 입력 횟수: " + inputCount);

            Vector2 playerPos = transform.position;
            Vector2 targetPos = playerPos + input;
            Collider2D hit = Physics2D.OverlapCircle(targetPos, 0.1f);

            // 0. 앞에 구멍이 있고, 구멍 다음 칸이 비어있으면 → 점프 이동
            if (hit != null && hit.CompareTag("Obstacle_JumpHole"))
            {
                Vector2 gapNextPos = targetPos + input;
                Collider2D nextHit = Physics2D.OverlapCircle(gapNextPos, 0.1f);

                if (nextHit == null)
                {
                    // 구멍 + 다음 칸 비어 있으면 → 점프 이동
                    StartCoroutine(Move(transform, gapNextPos));
                    return;
                }
                else
                {
                    // 구멍 다음 칸에 뭔가 있음 → 점프 실패 → 이동 없음
                    return;
                }
            }

            // 1. 앞에 Breakable 태그 오브젝트가 있으면 파괴하고 끝
            if (hit != null && hit.CompareTag("Obstacle_Breakable"))
            {
                Destroy(hit.gameObject);
                Debug.Log("파괴 가능한 벽이 파괴됨!");
                return;
            }

            // 2. Box가 있다면 → 뒤가 비었으면 박스 밀기
            if (hit != null && hit.CompareTag("Obstacle_Box"))
            {
                Vector2 boxTargetPos = targetPos + input;
                Collider2D behindHit = Physics2D.OverlapCircle(boxTargetPos, 0.1f);

                // 박스 뒤에 아무것도 없거나, 벽/박스/구멍이 아니면 밀기 가능
                if (behindHit == null ||
                    (!behindHit.CompareTag("Obstacle_Box") &&
                     !behindHit.CompareTag("Obstacle_Wall") &&
                     !behindHit.CompareTag("Obstacle_Breakable") &&
                     !behindHit.CompareTag("Obstacle_JumpHole"))) // ← 여기 추가
                {
                    StartCoroutine(Move(hit.transform, boxTargetPos));
                }

                return;
            }

            // 3. 이동 가능한 경우 (빈 공간)
            if (hit == null || (!hit.CompareTag("Obstacle_Wall") && !hit.CompareTag("Obstacle_Box") && !hit.CompareTag("Obstacle_Breakable")))
            {
                StartCoroutine(Move(transform, targetPos));
            }
        }
    }

    IEnumerator Move(Transform obj, Vector2 target)
    {
        isMoving = true;

        Vector2 start = obj.position;
        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / moveDuration);
            obj.position = Vector2.Lerp(start, target, t);
            yield return null;
        }

        obj.position = target;
        isMoving = false;

        //  플레이어가 이동한 경우에만 이동 횟수 증가
        if (obj == transform)
        {
            moveCount++;
            Debug.Log("이동 횟수: " + moveCount);
        }
    }
}
