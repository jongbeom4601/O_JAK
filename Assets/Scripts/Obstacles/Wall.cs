using UnityEngine;

public class Wall : MonoBehaviour, IInteractable
{
    public void Interact(GameObject interactor, Vector2 direction)
    {
        // 벽은 기본적으로 아무 일도 일어나지 않음
        Debug.Log("Wall: 상호작용 없음 (고정)");
    }
}
