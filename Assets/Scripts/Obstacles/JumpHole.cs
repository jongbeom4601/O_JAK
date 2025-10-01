using UnityEngine;

public class JumpHole : MonoBehaviour, IInteractable
{
    public int Priority => 100;   // �ֿ켱
    public float moveDuration = 0.1f;

    public void Interact(GameObject interactor, Vector2 direction)
    {
        var player = interactor.GetComponent<PlayerMovement>();
        if (player == null) return;

        // ���� ��ǥ ��ġ = �� ��ġ + dir * 2
        Vector2 targetPos = (Vector2)interactor.transform.position + direction * 2f;

        // ���� ������ ���� �ִ� ��� ������Ʈ Ȯ��
        Collider2D[] hits = Physics2D.OverlapCircleAll(targetPos, 0.1f);

        // �ϳ��� ������ ������Ʈ�� ������ ���� �Ұ�
        foreach (var hit in hits)
        {
            if (IsBlocked(hit))
            {
                Debug.Log("JumpHole: ���� ���� - ����");
                return;
            }
        }

        // ������ �� ������ ���� �̵�
        player.MoveTo(targetPos, direction);
        Debug.Log("JumpHole: ���� ����");

        // �̵� �Ϸ� �� ���ȣ�ۿ� �õ�
    }

    private bool IsBlocked(Collider2D col)
    {
        if (col == null) return false;

        // ������ ������Ʈ �±׸� üũ
        return col.CompareTag("Obstacle_Wall") ||
               col.CompareTag("Obstacle_Box") ||
               col.CompareTag("Obstacle_Breakable") ||
               col.CompareTag("Obstacle_JumpHole") ||
               col.CompareTag("Trigger_Door");
    }
}