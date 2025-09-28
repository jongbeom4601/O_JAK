using UnityEngine;

public class Cloud : MonoBehaviour, IInteractable
{
    private bool isUsed = false;
    private bool isSteppedOn = false;

    void Start()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, 0.1f);
        if (hit != null && hit.CompareTag("Player"))
        {
            isSteppedOn = true;
        }
    }


    public void Interact(GameObject interactor, Vector2 direction)
    {
        if (isUsed)
        {
            Debug.Log("�� ������ �̹� ��������ϴ�.");
            return;
        }

        var player = interactor.GetComponent<PlayerMovement>();
        if (player != null)
        {
            // �÷��̾ ���� ��ġ�� �̵�
            player.MoveTo((Vector2)transform.position);
            isSteppedOn = true; // ���� ���� ���
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (isSteppedOn && other.CompareTag("Player") && !isUsed)
        {
            isUsed = true;
            GetComponent<SpriteRenderer>().color = Color.black; // ������ ó��
            gameObject.tag = "Obstacle_Wall";                   // ���� �� ���
            Debug.Log("������ �˰� ���ϰ� ���� �Ǿ� �ٽ� �� ����");
        }
    }
}
