using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    public int Priority => 100;

    [Header("문 매니저")]
    public DoorManager doorManager;

    public void Interact(GameObject interactor, Vector2 direction)
    {
        var input = interactor.GetComponent<PlayerInput>();
        var movement = interactor.GetComponent<PlayerMovement>();

        if (input != null && movement != null)
        {
            if (input.Config.hasKey)
            {
                input.Config.hasKey = false; // 열쇠 사용
                Destroy(gameObject);
                movement.MoveTo(transform.position);

                if (doorManager != null)
                    doorManager.DoorOpened();
            }
            else
            {
                Debug.Log("열쇠가 필요합니다!");
            }
        }
    }
}
