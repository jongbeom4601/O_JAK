using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorManager : MonoBehaviour {

    [Header("�� ����")]
    public int totalDoors = 2;          // ����� �ϴ� �� ����
    private int openedDoors = 0;        // ������� ���� �� ����

    [Header("���� �� �̸�")]
    public string nextSceneName;        // ��� ������ �� �̵��� ��

    // Door���� ȣ���ϴ� �Լ�
    // ���� ��� ���� ���� ������ �̵�
    public void DoorOpened() {
        openedDoors++;

        if (openedDoors >= totalDoors) {
            if (!string.IsNullOrEmpty(nextSceneName))
                SceneManager.LoadScene(nextSceneName);
        }
    }
}
