//using UnityEngine;

//public class Cloud : MonoBehaviour, IInteractable
//{
//    public int Priority => 10; // 4순위(최하위)
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
//            Debug.Log("이 구름은 이미 사라졌습니다.");
//            return;
//        }

//        var player = interactor.GetComponent<PlayerMovement>();
//        if (player != null)
//        {
//            // 플레이어를 구름 위치로 이동
//            isSteppedOn = true; // 밟음 상태 기록
//        }
//    }

//    private void OnTriggerExit2D(Collider2D other)
//    {
//        if (isSteppedOn && other.CompareTag("Player") && !isUsed)
//        {
//            isUsed = true;
//            GetComponent<SpriteRenderer>().color = Color.black; // 검은색 처리
//            gameObject.tag = "Obstacle_Wall";                   // 이제 벽 취급
//            Debug.Log("구름이 검게 변하고 벽이 되어 다시 못 밟음");
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
