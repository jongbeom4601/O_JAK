using UnityEngine;
using UnityEngine.EventSystems;

public class UIBlocker : MonoBehaviour {
    void Start() {
        // 씬 시작 시 UI 선택 해제
        EventSystem.current.SetSelectedGameObject(null);
    }

    void Update() {
        // 플레이 중 계속 UI에 포커스가 가면 풀어줌
        if (EventSystem.current.currentSelectedGameObject != null)
            EventSystem.current.SetSelectedGameObject(null);
    }
}
