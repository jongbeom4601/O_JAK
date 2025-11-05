using UnityEngine;

public class ActivateOnClick : MonoBehaviour
{
    [SerializeField] private GameObject target; // 비활성화되어 있는 대상

    public void Activate()
    {
        if (target == null) return;
        target.SetActive(true); // 켜기
    }

    public void Toggle()
    {
        if (target == null) return;
        target.SetActive(!target.activeSelf); // 토글
    }
}
