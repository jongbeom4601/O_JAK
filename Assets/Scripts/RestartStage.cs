using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartStage : MonoBehaviour
{
    [SerializeField] ScreenTransition transition;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            // 현재 활성화된 씬 이름 가져오기
            string currentScene = SceneManager.GetActiveScene().name;
            // 씬 다시 로드
            //SceneManager.LoadScene(currentScene);
            transition.PlayAndLoad(ScreenTransition.Style.SoftDim, currentScene);
        }
    }
}
