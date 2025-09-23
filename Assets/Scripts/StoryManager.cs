using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

[System.Serializable]

public class DialogueLine {
    public string speakerName;          // 화자 이름
    [TextArea(3, 5)]
    public string text;                 // 대사
}

public class StoryManager : MonoBehaviour {
    [Header("UI 연결")]
    public GameObject dialoguePanel;    // 대화창 패널
    public TMP_Text nameText;           // 화자 이름 표시용 TMP 텍스트
    public TMP_Text dialogueText;       // 대사 표시용 TMP 텍스트
    public GameObject endCursor;        // 대사 끝났을 때 나오는 화살표 아이콘

    [Header("대사 데이터")]
    public DialogueLine[] lines;        // 대사 배열 (**Inspector에서 입력**)

    [Header("다음 씬 이름")]
    public string nextSceneName;        // 모든 대사 종료 후 이동할 씬 이름

    [Header("타자기 효과 속도")]
    public float typeSpeed = 0.05f;     // 글자 하나 출력마다 딜레이 시간

    // 내부 상태 관리 변수
    private int currentIndex = 0;       // 현재 출력 중인 대사 인덱스
    private bool isDialogueActive = false; // 대화창이 활성화 상태인지
    private bool isTyping = false;      // 현재 타자 효과 진행 중인지
    private Coroutine typingCoroutine;  // 실행 중인 코루틴 저장용

    void Start() {
        // 시작 시 대화창 켜고, endCursor는 꺼둠
        if (dialoguePanel != null) dialoguePanel.SetActive(true);
        if (endCursor != null) endCursor.SetActive(false);

        // 첫 줄 출력
        if (lines.Length > 0) {
            ShowLine();
            isDialogueActive = true;
        }
    }

    void Update() {
        if (isDialogueActive && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))) {
            if (isTyping)
                SkipTyping();   // 출력 도중 -> 전체 문장 바로 출력
            else
                NextLine();     // 출력 완료 -> 다음 줄로 이동
        }
    }

    // 현재 인덱스의 대사 출력
    void ShowLine() {
        HideEndCursor();
        if (currentIndex < lines.Length) {
            // 화자 이름 출력
            if (nameText != null)
                nameText.text = lines[currentIndex].speakerName;

            // 대사 출력 시작 (타자 효과)
            if (dialogueText != null) {
                if (typingCoroutine != null)
                    StopCoroutine(typingCoroutine); // 이전 코루틴 중단

                typingCoroutine = StartCoroutine(TypeLine(lines[currentIndex].text));
            }
        } else {
            // 모든 대사가 끝나면 다음 씬으로 이동
            if (!string.IsNullOrEmpty(nextSceneName))
                SceneManager.LoadScene(nextSceneName);
            else {
                // 씬 이름이 안 들어있으면 그냥 대화창만 꺼짐
                if (dialoguePanel != null)
                    dialoguePanel.SetActive(false);
            }

            isDialogueActive = false;
        }
    }

    // 타자기 효과
    IEnumerator TypeLine(string line) {
        isTyping = true;

        // 초기화
        dialogueText.text = "";
        dialogueText.maxVisibleCharacters = 0;
        dialogueText.text = line;
        dialogueText.ForceMeshUpdate();

        int totalChars = dialogueText.textInfo.characterCount;

        // 한 글자씩 순차적으로 보여주기
        for (int i = 0; i < totalChars; i++) {
            dialogueText.maxVisibleCharacters = i + 1;
            yield return new WaitForSeconds(typeSpeed);
        }

        isTyping = false;
        ShowEndCursor();
    }

    // 타자 효과 스킵
    void SkipTyping() {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        dialogueText.ForceMeshUpdate();
        dialogueText.maxVisibleCharacters = dialogueText.textInfo.characterCount;

        isTyping = false;
        ShowEndCursor();
    }

    // 다음 대사로 이동
    void NextLine() {
        currentIndex++;
        ShowLine();
    }

    // 화살표 아이콘 표시
    void ShowEndCursor() {
        if (endCursor != null) endCursor.SetActive(true);
    }

    // 화살표 아이콘 숨기기
    void HideEndCursor() {
        if (endCursor != null) endCursor.SetActive(false);
    }
}