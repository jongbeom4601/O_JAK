using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public Animator animator;
    public Animator bgAnimator;
    public string titleSceneName = "TitleScene";
    public GameObject optionPanel;   // �ɼ� �г� (Inspector���� ����)

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
                // �ɼ� ���� ���� �� ESC �� �ڷΰ���
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
        optionPanel.SetActive(true);   // �ɼ� �г� ����
    }

    public void CloseOption()
    {
        optionPanel.SetActive(false);  // �ɼ� �г� �ݱ�
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
