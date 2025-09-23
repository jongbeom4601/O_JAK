using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    private bool isActive = true;

    public void Deactivate()
    {
        if (!isActive) return;

        isActive = false;
        Debug.Log("함정이 제거되었습니다!");
        Destroy(gameObject); // 완전 삭제
    }
}
