using UnityEngine;
using UnityEngine.SceneManagement;

public class DevStageAndUI : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject panelT;  // T Ű�� ������ ������ �г�
    public GameObject panelY;  // Y Ű�� ������ ������ �г�

    void Start()
    {
        if (panelT != null) panelT.SetActive(false);
        if (panelY != null) panelY.SetActive(false);
    }

    void Update()
    {
        bool tActive = panelT != null && panelT.activeSelf;
        bool yActive = panelY != null && panelY.activeSelf;

        // panelT�� ���� ���� �� �� T�� üũ
        if (tActive)
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                panelT.SetActive(false);
                Debug.Log("T: panelT ����");
            }
            return; // �ٸ� �Է� ����
        }

        // panelY�� ���� ���� �� �� Y�� üũ
        if (yActive)
        {
            if (Input.GetKeyDown(KeyCode.Y))
            {
                panelY.SetActive(false);
                Debug.Log("Y: panelY ����");
            }
            return; // �ٸ� �Է� ����
        }

        // ----- ��� ���� (�г� �������� ��) -----
        if (Input.GetKeyDown(KeyCode.T) && panelT != null)
        {
            panelT.SetActive(true);
            Debug.Log("T: panelT ����");
            return;
        }

        if (Input.GetKeyDown(KeyCode.Y) && panelY != null)
        {
            panelY.SetActive(true);
            Debug.Log("Y: panelY ����");
            return;
        }

        // ----- �����ڿ� �� �̵� -----
        if (Input.GetKeyDown(KeyCode.P))
        {
            int currentIndex = SceneManager.GetActiveScene().buildIndex;
            int nextIndex = currentIndex + 1;

            if (nextIndex < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(nextIndex);
                Debug.Log($"������ Ű: P ������ {nextIndex}�� ������ �̵�");
            }
            else
            {
                Debug.LogWarning("���� ���� ���� ���ÿ� �����ϴ�!");
            }
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            int currentIndex = SceneManager.GetActiveScene().buildIndex;
            int prevIndex = currentIndex - 1;

            if (prevIndex >= 0)
            {
                SceneManager.LoadScene(prevIndex);
                Debug.Log($"������ Ű: B ������ {prevIndex}�� ������ �̵�");
            }
            else
            {
                Debug.LogWarning("���� ���� �����ϴ�!");
            }
        }
    }
}
