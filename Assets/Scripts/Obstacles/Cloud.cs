//using UnityEngine;

//public class Cloud : MonoBehaviour, IInteractable
//{
//    public int Priority => 10; // 4����(������)
//    private bool isUsed = false;
//    private bool isSteppedOn = false;

//    void Start()
//    {
//        Collider2D hit = Physics2D.OverlapCircle(transform.position, 0.1f);
//        if (hit != null && hit.CompareTag("Player"))
//        {
//            isSteppedOn = true;
//        }
//    }


//    public void Interact(GameObject interactor, Vector2 direction)
//    {
//        if (isUsed)
//        {
//            Debug.Log("�� ������ �̹� ��������ϴ�.");
//            return;
//        }

//        var player = interactor.GetComponent<PlayerMovement>();
//        if (player != null)
//        {
//            // �÷��̾ ���� ��ġ�� �̵�
//            isSteppedOn = true; // ���� ���� ���
//        }
//    }

//    private void OnTriggerExit2D(Collider2D other)
//    {
//        if (isSteppedOn && other.CompareTag("Player") && !isUsed)
//        {
//            isUsed = true;
//            GetComponent<SpriteRenderer>().color = Color.black; // ������ ó��
//            gameObject.tag = "Obstacle_Wall";                   // ���� �� ���
//            Debug.Log("������ �˰� ���ϰ� ���� �Ǿ� �ٽ� �� ����");
//        }
//    }
//}

using UnityEngine;

public class Cloud : MonoBehaviour, IOnEnter, IOnStay, IOnExit {
    private bool active;
    private bool locked;
    [SerializeField]
    SpriteRenderer sr;
    Color lockedColor = new(0f, 0f, 0f, 1f);

    public void OnEnter(GameObject interactor, Vector2 dir) {
        if (locked) return;
        active = true;
    }

    public void OnStay(GameObject interactor) {
        if (locked) return;
        active = true;
    }

    public void OnExit(GameObject interactor) {
        if (locked) return;
        locked = true;

        gameObject.tag = "Obstacle_Wall";
        gameObject.layer = LayerMask.NameToLayer("preMoveMask");

        if (sr) sr.color = lockedColor;
    }
}
