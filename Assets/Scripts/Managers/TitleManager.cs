using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleMenuHandler : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject pressAnyKeyText; // �����̴� �ؽ�Ʈ
    public GameObject mainMenuPanel;   // Play/Option/Quit
    public GameObject playSubPanel;    // Offline/Online
    public GameObject optionPanel;     // �ɼ� �޴�

    [Header("��ư ���� �ʱ�ȭ")]
    public Button firstMainButton;     // PressAnyKey �� ó�� ���õ� ��ư
    public Button firstPlayButton;     // Play ����޴����� �⺻ ���� ��ư
    public Button firstOptionButton;   // �ɼ� �޴����� �⺻ ���� ��ư

    [Header("�� �̸�")]
    public string offlineSceneName;
    public string onlineSceneName;

    private bool activated = false;

    void Start()
    {
        // ���� ����
        pressAnyKeyText.SetActive(true);
        mainMenuPanel.SetActive(false);
        playSubPanel.SetActive(false);
        optionPanel.SetActive(false);

        Time.timeScale = 1f; // Ȥ�� ���� ������ Ǯ��
    }

    void Update()
    {
        // ���� �޴� Ȱ��ȭ ��
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
            return; // ���⼭ ��
        }

        // === �޴� Ȱ��ȭ �� ESC ó�� ===
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (playSubPanel.activeSelf || optionPanel.activeSelf)
            {
                OnBackToMain();   // �ڷΰ���
            }
            else if (mainMenuPanel.activeSelf)
            {
                Debug.Log("MainMenu���� ESC �Է�");
                // ���⼭ Quit Ȯ��â�� ���ų� Application.Quit() ���� ����
            }
        }
    }


    // ===== �޴� ��ȯ �Լ� =====
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
