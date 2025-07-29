using UnityEngine;

public class Key : MonoBehaviour, IInteractable
{
    public void Interact(GameObject interactor, Vector2 direction)
    {
        Vector2 currentPos = transform.position;

        PlayerMovement player = interactor.GetComponent<PlayerMovement>();
        if (player != null && !player.HasKey())
        {
            player.AcquireKey();
            player.MoveTo(currentPos);
            Destroy(gameObject); // ¿­¼è´Â È¹µæ ½Ã »ç¶óÁü
        }
        else
        {
            player.MoveTo(currentPos);
        }
    }
}
