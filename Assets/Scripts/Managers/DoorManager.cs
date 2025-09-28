using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorManager : MonoBehaviour {

    [Header("문 개수")]
    public int totalDoors = 2;          // 열어야 하는 문 개수
    private int openedDoors = 0;        // 현재까지 열린 문 개수

    [Header("다음 씬 이름")]
    public string nextSceneName;        // 모두 열렸을 때 이동할 씬

    // Door에서 호출하는 함수
    // 문을 모두 열면 다음 씬으로 이동
    public void DoorOpened() {
        openedDoors++;

        if (openedDoors >= totalDoors) {
            if (!string.IsNullOrEmpty(nextSceneName))
                SceneManager.LoadScene(nextSceneName);
        }
    }
}
