using UnityEngine;
using UnityEngine.SceneManagement;

public class DevStageAndUI : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject panelT;  // T 키로 열리고 닫히는 패널
    public GameObject panelY;  // Y 키로 열리고 닫히는 패널

    void Start()
    {
        if (panelT != null) panelT.SetActive(false);
        if (panelY != null) panelY.SetActive(false);
    }

    void Update()
    {
        bool tActive = panelT != null && panelT.activeSelf;
        bool yActive = panelY != null && panelY.activeSelf;

        // panelT가 켜져 있을 때 → T만 체크
        if (tActive)
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                panelT.SetActive(false);
                Debug.Log("T: panelT 닫음");
            }
            return; // 다른 입력 무시
        }

        // panelY가 켜져 있을 때 → Y만 체크
        if (yActive)
        {
            if (Input.GetKeyDown(KeyCode.Y))
            {
                panelY.SetActive(false);
                Debug.Log("Y: panelY 닫음");
            }
            return; // 다른 입력 무시
        }

        // ----- 평소 상태 (패널 꺼져있을 때) -----
        if (Input.GetKeyDown(KeyCode.T) && panelT != null)
        {
            panelT.SetActive(true);
            Debug.Log("T: panelT 열림");
            return;
        }

        if (Input.GetKeyDown(KeyCode.Y) && panelY != null)
        {
            panelY.SetActive(true);
            Debug.Log("Y: panelY 열림");
            return;
        }

        // ----- 개발자용 씬 이동 -----
        if (Input.GetKeyDown(KeyCode.P))
        {
            int currentIndex = SceneManager.GetActiveScene().buildIndex;
            int nextIndex = currentIndex + 1;

            if (nextIndex < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(nextIndex);
                Debug.Log($"개발자 키: P 눌러서 {nextIndex}번 씬으로 이동");
            }
            else
            {
                Debug.LogWarning("다음 씬이 빌드 세팅에 없습니다!");
            }
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            int currentIndex = SceneManager.GetActiveScene().buildIndex;
            int prevIndex = currentIndex - 1;

            if (prevIndex >= 0)
            {
                SceneManager.LoadScene(prevIndex);
                Debug.Log($"개발자 키: B 눌러서 {prevIndex}번 씬으로 이동");
            }
            else
            {
                Debug.LogWarning("이전 씬이 없습니다!");
            }
        }
    }
}
