using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [Header("문 열림 효과")]
    public GameObject openEffect; // 문이 열릴 때 이펙트 (선택)

    public void Interact(GameObject interactor, Vector2 direction)
    {
        // 플레이어 컨트롤러에서 열쇠 상태 확인
        PlayerMovement player = interactor.GetComponent<PlayerMovement>();
        if (player != null && player.HasKey())
        {
            // 열쇠 사용
            player.UseKey();

            // 이펙트 출력 (선택)
            if (openEffect != null)
                Instantiate(openEffect, transform.position, Quaternion.identity);

            // 문 제거
            Destroy(gameObject);
            Debug.Log(" 문이 열렸습니다!");
        }
        else
        {
            Debug.Log(" 열쇠가 없습니다!");
        }
    }
}
