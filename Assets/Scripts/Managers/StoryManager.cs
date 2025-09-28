using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

[System.Serializable]

public class DialogueLine {
    public string speakerName;          // ȭ�� �̸�
    [TextArea(3, 5)]
    public string text;                 // ���
}

public class StoryManager : MonoBehaviour {
    [Header("UI ����")]
    public GameObject dialoguePanel;    // ��ȭâ �г�
    public TMP_Text nameText;           // ȭ�� �̸� ǥ�ÿ� TMP �ؽ�Ʈ
    public TMP_Text dialogueText;       // ��� ǥ�ÿ� TMP �ؽ�Ʈ
    public GameObject endCursor;        // ��� ������ �� ������ ȭ��ǥ ������

    [Header("��� ������")]
    public DialogueLine[] lines;        // ��� �迭 (**Inspector���� �Է�**)

    [Header("���� �� �̸�")]
    public string nextSceneName;        // ��� ��� ���� �� �̵��� �� �̸�

    [Header("Ÿ�ڱ� ȿ�� �ӵ�")]
    public float typeSpeed = 0.05f;     // ���� �ϳ� ��¸��� ������ �ð�

    // ���� ���� ���� ����
    private int currentIndex = 0;       // ���� ��� ���� ��� �ε���
    private bool isDialogueActive = false; // ��ȭâ�� Ȱ��ȭ ��������
    private bool isTyping = false;      // ���� Ÿ�� ȿ�� ���� ������
    private Coroutine typingCoroutine;  // ���� ���� �ڷ�ƾ �����

    void Start() {
        // ���� �� ��ȭâ �Ѱ�, endCursor�� ����
        if (dialoguePanel != null) dialoguePanel.SetActive(true);
        if (endCursor != null) endCursor.SetActive(false);

        // ù �� ���
        if (lines.Length > 0) {
            ShowLine();
            isDialogueActive = true;
        }
    }

    void Update() {
        if (isDialogueActive && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))) {
            if (isTyping)
                SkipTyping();   // ��� ���� -> ��ü ���� �ٷ� ���
            else
                NextLine();     // ��� �Ϸ� -> ���� �ٷ� �̵�
        }
    }

    // ���� �ε����� ��� ���
    void ShowLine() {
        HideEndCursor();
        if (currentIndex < lines.Length) {
            // ȭ�� �̸� ���
            if (nameText != null)
                nameText.text = lines[currentIndex].speakerName;

            // ��� ��� ���� (Ÿ�� ȿ��)
            if (dialogueText != null) {
                if (typingCoroutine != null)
                    StopCoroutine(typingCoroutine); // ���� �ڷ�ƾ �ߴ�

                typingCoroutine = StartCoroutine(TypeLine(lines[currentIndex].text));
            }
        } else {
            // ��� ��簡 ������ ���� ������ �̵�
            if (!string.IsNullOrEmpty(nextSceneName))
                SceneManager.LoadScene(nextSceneName);
            else {
                // �� �̸��� �� ��������� �׳� ��ȭâ�� ����
                if (dialoguePanel != null)
                    dialoguePanel.SetActive(false);
            }

            isDialogueActive = false;
        }
    }

    // Ÿ�ڱ� ȿ��
    IEnumerator TypeLine(string line) {
        isTyping = true;

        // �ʱ�ȭ
        dialogueText.text = "";
        dialogueText.maxVisibleCharacters = 0;
        dialogueText.text = line;
        dialogueText.ForceMeshUpdate();

        int totalChars = dialogueText.textInfo.characterCount;

        // �� ���ھ� ���������� �����ֱ�
        for (int i = 0; i < totalChars; i++) {
            dialogueText.maxVisibleCharacters = i + 1;
            yield return new WaitForSeconds(typeSpeed);
        }

        isTyping = false;
        ShowEndCursor();
    }

    // Ÿ�� ȿ�� ��ŵ
    void SkipTyping() {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        dialogueText.ForceMeshUpdate();
        dialogueText.maxVisibleCharacters = dialogueText.textInfo.characterCount;

        isTyping = false;
        ShowEndCursor();
    }

    // ���� ���� �̵�
    void NextLine() {
        currentIndex++;
        ShowLine();
    }

    // ȭ��ǥ ������ ǥ��
    void ShowEndCursor() {
        if (endCursor != null) endCursor.SetActive(true);
    }

    // ȭ��ǥ ������ �����
    void HideEndCursor() {
        if (endCursor != null) endCursor.SetActive(false);
    }
}