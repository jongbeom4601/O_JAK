using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingManager : MonoBehaviour
{
    // 인스펙터에서 지정할 씬 이름
    [SerializeField] private string sceneName;

    // 버튼에 OnClick 이벤트로 연결하면 됨
    public void LoadScene()
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogWarning("Scene name not set!");
        }
    }
}
