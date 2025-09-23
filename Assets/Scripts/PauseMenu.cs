using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public Animator animator;
    public Animator bgAnimator;
    public string titleSceneName = "TitleScene";
    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) Resume();
            else Pause();
        }
    }

    public void Pause()
    {
        animator.SetTrigger("Open"); // SlideIn 실행
        bgAnimator.SetTrigger("FadeOut");
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void Resume()
    {
        animator.SetTrigger("Close"); // SlideOut 실행
        bgAnimator.SetTrigger("FadeIn");
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void ReturnToTitle()
    {
        SceneManager.LoadScene(titleSceneName);
    }

    public void QuitGame()
    {
        // 혹시나 정지 상태로 남지 않도록 복구
        Time.timeScale = 1f;

    #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }

}
