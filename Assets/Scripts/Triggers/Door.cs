using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    public int Priority => 100;

    [Header("�� �Ŵ���")]
    public DoorManager doorManager;

    public void Interact(GameObject interactor, Vector2 direction)
    {
        var input = interactor.GetComponent<PlayerInput>();
        var movement = interactor.GetComponent<PlayerMovement>();

        if (input != null && movement != null)
        {
            if (input.Config.hasKey)
            {
                input.Config.hasKey = false; // ���� ���
                Destroy(gameObject);
                movement.MoveTo(transform.position);

                if (doorManager != null)
                    doorManager.DoorOpened();
            }
            else
            {
                Debug.Log("���谡 �ʿ��մϴ�!");
            }
        }
    }
}
