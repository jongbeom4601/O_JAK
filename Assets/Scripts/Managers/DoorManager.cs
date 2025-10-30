using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class DoorManager : MonoBehaviour {

    [Header("문 개수")]
    public int totalDoors = 2;          // 열어야 하는 문 개수
    private int openedDoors = 0;        // 현재까지 열린 문 개수

    [Header("다음 씬 이름")]
    [SerializeField] ScreenTransition transition;
    public string nextSceneName;        // 모두 열렸을 때 이동할 씬

    [Header("전환 지연(초)")]
    [SerializeField] private float delaySeconds = 0.6f; // 씬 전환 전 대기 시간
    private bool transitionStarted = false;             // 중복 전환 방지

    // Door에서 호출하는 함수
    // 문을 모두 열면 다음 씬으로 이동
    public void DoorOpened() {
        openedDoors++;

        if (!transitionStarted && openedDoors >= totalDoors && !string.IsNullOrEmpty(nextSceneName)) {
            transitionStarted = true;
            StartCoroutine(LoadNextSceneWithDelay());
        }
    }

    private IEnumerator LoadNextSceneWithDelay()
    {
        yield return new WaitForSeconds(delaySeconds);
        transition.PlayAndLoad(ScreenTransition.Style.SoftDim, nextSceneName);
    }
}
