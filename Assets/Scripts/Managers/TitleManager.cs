using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleMenuHandler : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject pressAnyKeyText; // 깜빡이는 텍스트
    public GameObject mainMenuPanel;   // Play/Option/Quit
    public GameObject playSubPanel;    // Offline/Online
    public GameObject optionPanel;     // 옵션 메뉴

    [Header("버튼 선택 초기화")]
    public Button firstMainButton;     // PressAnyKey 후 처음 선택될 버튼
    public Button firstPlayButton;     // Play 서브메뉴에서 기본 선택 버튼
    public Button firstOptionButton;   // 옵션 메뉴에서 기본 선택 버튼

    [Header("씬 이름")]
    public string offlineSceneName;
    public string onlineSceneName;

    private bool activated = false;

    void Start()
    {
        // 시작 상태
        pressAnyKeyText.SetActive(true);
        mainMenuPanel.SetActive(false);
        playSubPanel.SetActive(false);
        optionPanel.SetActive(false);

        Time.timeScale = 1f; // 혹시 멈춰 있으면 풀기
    }

    void Update()
    {
        // 아직 메뉴 활성화 전
        if (!activated)
        {
            if (Input.anyKeyDown && !Input.GetKeyDown(KeyCode.Escape))
            {
                activated = true;

                pressAnyKeyText.SetActive(false);
                mainMenuPanel.SetActive(true);

                if (firstMainButton)
                    EventSystem.current.SetSelectedGameObject(firstMainButton.gameObject);
            }
            return; // 여기서 끝
        }

        // === 메뉴 활성화 후 ESC 처리 ===
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (playSubPanel.activeSelf || optionPanel.activeSelf)
            {
                OnBackToMain();   // 뒤로가기
            }
            else if (mainMenuPanel.activeSelf)
            {
                Debug.Log("MainMenu에서 ESC 입력");
                // 여기서 Quit 확인창을 띄우거나 Application.Quit() 실행 가능
            }
        }
    }


    // ===== 메뉴 전환 함수 =====
    public void OnPlay()
    {
        mainMenuPanel.SetActive(false);
        playSubPanel.SetActive(true);

        if (firstPlayButton)
            EventSystem.current.SetSelectedGameObject(firstPlayButton.gameObject);
    }

    public void OnOption()
    {
        mainMenuPanel.SetActive(false);
        optionPanel.SetActive(true);

        if (firstOptionButton)
            EventSystem.current.SetSelectedGameObject(firstOptionButton.gameObject);
    }

    public void OnBackToMain()
    {
        playSubPanel.SetActive(false);
        optionPanel.SetActive(false);
        mainMenuPanel.SetActive(true);

        if (firstMainButton)
            EventSystem.current.SetSelectedGameObject(firstMainButton.gameObject);
    }

    public void OnPlayOffline()
    {
        SceneManager.LoadScene(offlineSceneName);
    }

    public void OnPlayOnline()
    {
        SceneManager.LoadScene(onlineSceneName);
    }

    public void OnQuit()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
