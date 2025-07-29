using UnityEngine;
using System.Collections;

public class JumpHole : MonoBehaviour, IInteractable
{
    public float moveDuration = 0.1f;

    public void Interact(GameObject interactor, Vector2 direction)
    {
        Vector2 currentPos = transform.position;
        Vector2 targetPos = currentPos + direction;

        // 다음 칸 확인
        Collider2D hit = Physics2D.OverlapCircle(targetPos, 0.1f);

        if (hit == null || !IsBlocked(hit))
        {
            PlayerMovement player = interactor.GetComponent<PlayerMovement>();
            if (player != null)
            {
                player.MoveTo(targetPos);
                Debug.Log("JumpHole: 이동 성공");

                // 이동 완료 후 재상호작용 시도
                player.StartCoroutine(DelayedRecheck(player, direction));
            }
        }
        else
        {
            Debug.Log("JumpHole: 이동 실패 - 막힘");
        }
    }

    bool IsBlocked(Collider2D col)
    {
        if (col == null) return false;

        return col.CompareTag("Obstacle_Wall") ||
               col.CompareTag("Obstacle_Box") ||
               col.CompareTag("Obstacle_Breakable");
    }

    IEnumerator DelayedRecheck(PlayerMovement player, Vector2 direction)
    {
        // isMoving이 false가 될 때까지 기다림
        while (player.IsMoving)
            yield return null;

        // 현재 위치에서 다시 상호작용 시도
        player.TryInteractOnly(direction);
    }
}
