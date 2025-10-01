using UnityEngine;
using UnityEngine.UIElements;

public class SpikeTrapTrigger : MonoBehaviour, IInteractable
{

    public int Priority => 100; // �ֿ켱
    [Header("������ �������� �±� �̸�")]
    public string trapTag = "TrapA";  // ��: "TrapA", "TrapB"

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
