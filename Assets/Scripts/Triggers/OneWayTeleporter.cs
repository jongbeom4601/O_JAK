using UnityEngine;

public class OneWayTeleporter : MonoBehaviour, IInteractable
{
    [Header("텔레포트 목적지(절대 좌표)")]
    public Vector2 targetPoint;

    [Header("연출 옵션")]
    public GameObject teleportEffect;     // 발판/도착지 이펙트(선택)
    public float delayBeforeTeleport = 0.2f; // 이동 완료 후 추가 대기(선택)
    public bool waitForMoveFinish = true; // MoveTo가 끝날 때까지 기다릴지

    public void Interact(GameObject interactor, Vector2 direction)
    {
        Vector2 padCenter = transform.position;

        PlayerMovement player = interactor.GetComponent<PlayerMovement>();
        if (player != null)
        {
            // 순서 제어는 코루틴에서!
            StartCoroutine(TeleportSequence(player, padCenter));
        }
    }

    private System.Collections.IEnumerator TeleportSequence(PlayerMovement player, Vector2 padCenter)
    {
        player.LockInput();

        // 1) 발판 중앙으로 이동 시작
        if ((Vector2)player.transform.position != padCenter)
            player.MoveTo(padCenter);

        // 2) (선택) 이동이 끝날 때까지 대기
        if (waitForMoveFinish)
        {
            while (player.IsMoving)
                yield return null;
        }

        // 3) (선택) 추가 딜레이 + 발판 이펙트
        if (teleportEffect != null)
            Instantiate(teleportEffect, padCenter, Quaternion.identity);

        if (delayBeforeTeleport > 0f)
            yield return new WaitForSeconds(delayBeforeTeleport);

        // 4) 목적지로 순간이동
        player.TeleportTo(targetPoint);

        // 5) (선택) 도착 이펙트
        if (teleportEffect != null)
            Instantiate(teleportEffect, targetPoint, Quaternion.identity);

        player.UnlockInput();
    }
}
