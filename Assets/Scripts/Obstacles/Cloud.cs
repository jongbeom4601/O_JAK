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
            Debug.Log("이 구름은 이미 사라졌습니다.");
            return;
        }

        var player = interactor.GetComponent<PlayerMovement>();
        if (player != null)
        {
            // 플레이어를 구름 위치로 이동
            player.MoveTo((Vector2)transform.position);
            isSteppedOn = true; // 밟음 상태 기록
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (isSteppedOn && other.CompareTag("Player") && !isUsed)
        {
            isUsed = true;
            GetComponent<SpriteRenderer>().color = Color.black; // 검은색 처리
            gameObject.tag = "Obstacle_Wall";                   // 이제 벽 취급
            Debug.Log("구름이 검게 변하고 벽이 되어 다시 못 밟음");
        }
    }
}
