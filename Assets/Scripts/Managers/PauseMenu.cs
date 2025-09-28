using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public Animator animator;
    public Animator bgAnimator;
    public string titleSceneName = "TitleScene";
    public GameObject optionPanel;   // 옵션 패널 (Inspector에서 연결)

    private bool isPaused = false;

    void Start()
    {
        if (optionPanel != null)
            optionPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (optionPanel.activeSelf)
            {
                // 옵션 열려 있을 때 ESC → 뒤로가기
                CloseOption();
                return;
            }

            if (isPaused) Resume();
            else Pause();
        }
    }

    public void Pause()
    {
        animator.SetTrigger("Open");
        bgAnimator.SetTrigger("FadeOut");
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void Resume()
    {
        animator.SetTrigger("Close");
        bgAnimator.SetTrigger("FadeIn");
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void OpenOption()
    {
        optionPanel.SetActive(true);   // 옵션 패널 열기
    }

    public void CloseOption()
    {
        optionPanel.SetActive(false);  // 옵션 패널 닫기
    }

    public void ReturnToTitle()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(titleSceneName);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
