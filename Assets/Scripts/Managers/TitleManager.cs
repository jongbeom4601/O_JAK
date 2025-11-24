using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleMenuHandler : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject mainMenuPanel;   // Play / Quit 메뉴만 있음

    [Header("버튼 선택 초기화")]
    public Button firstMainButton;     // 처음 선택될 버튼

    [Header("씬 이름")]
    public string playSceneName;       // Play 버튼 누르면 이동할 씬 이름

    void Start()
    {
        // 시작 상태
        mainMenuPanel.SetActive(true);

        Time.timeScale = 1f; // 혹시 멈춰 있으면 풀기

        if (firstMainButton)
            EventSystem.current.SetSelectedGameObject(firstMainButton.gameObject);
    }

    void Update()
    {
        // ESC로 종료
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnQuit();
        }
    }

    // ===== 버튼 함수 =====
    public void OnPlay()
    {
        SceneManager.LoadScene(playSceneName);
    }

    public void OnQuit()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
