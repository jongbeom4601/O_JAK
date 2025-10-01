using UnityEngine;

public class OneWayTeleporter : MonoBehaviour, IInteractable
{
    public int Priority => 100; // �ֿ켱

    [Header("�ڷ���Ʈ ������(���� ��ǥ)")]
    public Vector2 targetPoint;

    [Header("���� �ɼ�")]
    public GameObject teleportEffect;     // ����/������ ����Ʈ(����)
    public float delayBeforeTeleport = 0.2f; // �̵� �Ϸ� �� �߰� ���(����)
    public bool waitForMoveFinish = true; // MoveTo�� ���� ������ ��ٸ���

    public void Interact(GameObject interactor, Vector2 direction)
    {
        Vector2 padCenter = transform.position;

        PlayerMovement player = interactor.GetComponent<PlayerMovement>();
        if (player != null)
        {
            // ���� ����� �ڷ�ƾ����!
            StartCoroutine(TeleportSequence(player, padCenter));
        }
    }

    private System.Collections.IEnumerator TeleportSequence(PlayerMovement player, Vector2 padCenter)
    {
        player.LockInput();

        // 1) ���� �߾����� �̵� ����
        if ((Vector2)player.transform.position != padCenter)
            player.MoveTo(padCenter);

        // 2) (����) �̵��� ���� ������ ���
        if (waitForMoveFinish)
        {
            while (player.IsMoving)
                yield return null;
        }

        // 3) (����) �߰� ������ + ���� ����Ʈ
        if (teleportEffect != null)
            Instantiate(teleportEffect, padCenter, Quaternion.identity);

        if (delayBeforeTeleport > 0f)
            yield return new WaitForSeconds(delayBeforeTeleport);

        // 4) �������� �����̵�
        player.TeleportTo(targetPoint);

        // 5) (����) ���� ����Ʈ
        if (teleportEffect != null)
            Instantiate(teleportEffect, targetPoint, Quaternion.identity);

        player.UnlockInput();
    }
}
