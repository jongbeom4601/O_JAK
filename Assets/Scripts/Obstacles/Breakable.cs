using UnityEngine;

public class Breakable : MonoBehaviour, IInteractable
{
    public int Priority => 50; // 2����

    public void Interact(GameObject interactor, Vector2 direction)
    {
        Debug.Log("BreakableWall: �ı���!");
        Destroy(gameObject);
    }
}
