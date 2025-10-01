using UnityEngine;
using UnityEngine.UIElements;

public class SpikeTrapTrigger : MonoBehaviour, IInteractable
{

    public int Priority => 100; // 최우선
    [Header("해제할 함정들의 태그 이름")]
    public string trapTag = "TrapA";  // 예: "TrapA", "TrapB"

    private bool triggered = false;

    public void Interact(GameObject interactor, Vector2 direction)
    {
        Vector2 currentPos = transform.position;

        PlayerMovement player = interactor.GetComponent<PlayerMovement>();
        if (player != null)
        {
            player.MoveTo(currentPos);

            if (triggered) return;

            GameObject[] trapObjects = GameObject.FindGameObjectsWithTag(trapTag);
            foreach (GameObject obj in trapObjects)
            {
                SpikeTrap trap = obj.GetComponent<SpikeTrap>();
                if (trap != null)
                {
                    trap.Deactivate();
                }
            }

            triggered = true;
        }
    }
}
