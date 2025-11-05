using UnityEngine;
using UnityEngine.SceneManagement;

public class DevStageAndUI : MonoBehaviour
{

    void Update()
    {

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
