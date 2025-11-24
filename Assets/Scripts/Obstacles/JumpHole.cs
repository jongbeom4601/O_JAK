using UnityEngine;

public class JumpHole : MonoBehaviour, IInteractable
{
    public int Priority => 100;   // 최우선
    public float moveDuration = 0.1f;

    [Header("점프 성공 효과음")]
    public AudioClip jumpSound;
    public AudioSource audioSource;

    public void Interact(GameObject interactor, Vector2 direction)
    {
        var player = interactor.GetComponent<PlayerMovement>();
        if (player == null) return;

        // 점프 목표 위치 = 현 위치 + dir * 2
        Vector2 targetPos = (Vector2)interactor.transform.position + direction * 2f;

        // 도착 지점에 겹쳐 있는 모든 오브젝트 확인
        Collider2D[] hits = Physics2D.OverlapCircleAll(targetPos, 0.1f);

        // 하나라도 막히는 오브젝트가 있으면 점프 불가
        foreach (var hit in hits)
        {
            if (IsBlocked(hit))
            {
                Debug.Log("JumpHole: 점프 실패 - 막힘");
                return;
            }
        }

        // 막히는 게 없으면 점프 이동
        player.MoveTo(targetPos, Motion.Jump, direction);
        Debug.Log("JumpHole: 점프 성공");

        //  점프 성공 시 효과음 재생
        PlayJumpSound();
    }

    private void PlayJumpSound()
    {
        if (jumpSound == null) return;

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0f; // 2D 사운드
        }

        audioSource.PlayOneShot(jumpSound);
    }

    private bool IsBlocked(Collider2D col)
    {
        if (col == null) return false;

        // 막히는 오브젝트 태그만 체크
        return col.CompareTag("Obstacle_Wall") ||
               col.CompareTag("Obstacle_Box") ||
               col.CompareTag("Obstacle_Breakable") ||
               col.CompareTag("Obstacle_JumpHole") ||
               col.CompareTag("Trigger_Door");
    }
}
