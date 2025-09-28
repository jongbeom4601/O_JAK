using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingManager : MonoBehaviour
{
    // �ν����Ϳ��� ������ �� �̸�
    [SerializeField] private string sceneName;

    // ��ư�� OnClick �̺�Ʈ�� �����ϸ� ��
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
