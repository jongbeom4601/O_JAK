using UnityEngine;

public class Breakable : MonoBehaviour, IInteractable
{
    public int Priority => 50; // 2¼øÀ§

    public void Interact(GameObject interactor, Vector2 direction)
    {
        Debug.Log("BreakableWall: ÆÄ±«µÊ!");
        Destroy(gameObject);
    }
}
