using UnityEngine;

public class Door : MonoBehaviour, IInteractable {

    [Header("문 매니저")]
    public DoorManager doorManager;     // DoorManager 연결

    public void Interact(GameObject interactor, Vector2 direction) {
        Vector2 currentPos = transform.position;
        PlayerMovement player = interactor.GetComponent<PlayerMovement>();

        // 플레이어 컨트롤러에서 열쇠 상태 확인
        if (player != null && player.HasKey()) {
            player.UseKey();        // 열쇠 사용
            Destroy(gameObject);    // 문 제거
            player.MoveTo(currentPos); // 플레이어 이동

            // DoorManager에 값 전달
            if (doorManager != null)
                doorManager.DoorOpened();
        }
    }
}
