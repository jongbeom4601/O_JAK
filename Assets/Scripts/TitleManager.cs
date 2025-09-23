using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PressAnyKeyHandler_Text : MonoBehaviour
{
    public GameObject pressAnyKeyText; // 깜빡이는 Text 오브젝트
    public GameObject menuGroup;       // Play/Option/Quit 묶음(처음 비활성화)
    public Button firstButton;         // 처음 선택될 버튼(Play)
    public string StageSceneName;

    bool activated = false;

    void Start()
    {
        if (menuGroup) menuGroup.SetActive(false);
        if (pressAnyKeyText) pressAnyKeyText.SetActive(true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) return; // esc는 예외
        if (activated) return;

        if (Input.anyKeyDown)
        {
            activated = true;

            if (pressAnyKeyText) pressAnyKeyText.SetActive(false);
            if (menuGroup) menuGroup.SetActive(true);

            if (firstButton)
                EventSystem.current.SetSelectedGameObject(firstButton.gameObject);

            Time.timeScale = 1f;
            SceneManager.LoadScene(StageSceneName);
        }
    }
}
