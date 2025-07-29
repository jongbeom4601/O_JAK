using UnityEngine;

public class SplitScreenSetup : MonoBehaviour
{
    public Camera player1Camera; // 견우
    public Camera player2Camera; // 직녀

    void Start()
    {
        // 왼쪽 절반: Player1
        player1Camera.rect = new Rect(0f, 0f, 0.5f, 1f);

        // 오른쪽 절반: Player2
        player2Camera.rect = new Rect(0.5f, 0f, 0.5f, 1f);
    }
}
