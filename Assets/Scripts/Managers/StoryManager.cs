using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

[System.Serializable]

public class DialogueLine
{
    public string speakerName;          // 화자 이름
    [TextArea(3, 5)]
    public string text;                 // 대사
    public Sprite portrait;             //해당 대사에 쓸 초상(표정)
    public bool flipX;                  // 좌우반전이 필요 시
}

public class StoryManager : MonoBehaviour
{
    [Header("씬 페이드")]
    public UIFader screenFader;         // ← 방금 만든 검은 Image에 있는 UIFader
    public float sceneFadeIn = 0.4f;    // 시작 시: 검은 화면 → 투명
    public float sceneFadeOut = 0.4f;   // 끝날 때: 투명 → 검은 화면
    private bool isEnding = false;

    [Header("시작 지연")]
    [Tooltip("첫 대사가 나오기 전 대기 시간(초)")]
    public float firstLineDelay = 0f;

    [Header("UI 연결")]
    public GameObject dialoguePanel;    // 대화창 패널
    public TMP_Text nameText;           // 화자 이름 표시용 TMP 텍스트
    public TMP_Text dialogueText;       // 대사 표시용 TMP 텍스트
    public GameObject endCursor;        // 대사 끝났을 때 나오는 화살표 아이콘

    [Header("초상화")]
    public Image portraitImage;         // ← 초상 이미지 슬롯
    public float portraitFade = 0.15f;  // ← 페이드 시간(초)

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
    private Coroutine portraitFadeCoroutine;

    void Start()
    {
        StartCoroutine(CoStartDialogue());
    }


    void Update()
    {
        if (isDialogueActive && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))
        {
            if (isTyping)
                SkipTyping();   // 출력 도중 -> 전체 문장 바로 출력
            else
                NextLine();     // 출력 완료 -> 다음 줄로 이동
        }
    }

    IEnumerator CoStartDialogue()
    {
        // (선택) 지연 동안 패널을 숨기고 싶으면 주석 해제
        // if (dialoguePanel) dialoguePanel.SetActive(false);

        // 지연: 타임스케일 영향을 받기 싫으면 Realtime 유지
        if (screenFader)
        {
            screenFader.SetVisible(true);             // 시작은 검은 화면
            yield return screenFader.FadeOut(sceneFadeIn); // 검정 → 투명
        }

        if (firstLineDelay > 0f)
            yield return new WaitForSecondsRealtime(firstLineDelay);

        if (dialoguePanel != null) dialoguePanel.SetActive(true);
        if (endCursor != null) endCursor.SetActive(false);

        if (lines.Length > 0)
        {
            ShowLine();
            isDialogueActive = true;
        }
    }


    // 현재 인덱스의 대사 출력
    void ShowLine()
    {
        HideEndCursor();

        if (currentIndex < lines.Length)
        {
            var line = lines[currentIndex];

            // 화자 이름
            if (nameText) nameText.text = line.speakerName;

            // 초상 세팅
            if (portraitImage)
            {
                // 스프라이트가 있으면 교체하고 보이게, 없으면 숨김
                if (line.portrait)
                {
                    StopCoroutineSafely(ref portraitFadeCoroutine);
                    portraitImage.sprite = line.portrait;

                    // 좌우 반전이 필요하면 RectTransform 스케일로 처리
                    if (line.flipX)
                    {
                        var rt = portraitImage.rectTransform;
                        rt.localScale = new Vector3(-Mathf.Abs(rt.localScale.x), rt.localScale.y, rt.localScale.z);
                    }
                    else
                    {
                        var rt = portraitImage.rectTransform;
                        rt.localScale = new Vector3(Mathf.Abs(rt.localScale.x), rt.localScale.y, rt.localScale.z);
                    }

                    // 부드럽게 페이드 인
                    portraitImage.gameObject.SetActive(true);
                    StartPortraitFade(1f, portraitFade);
                }
                else
                {
                    // 이 줄은 초상 없이 진행
                    StartPortraitFade(0f, portraitFade, disableOnDone: true);
                }
            }

            // 대사 타자 효과
            if (dialogueText)
            {
                if (typingCoroutine != null) StopCoroutine(typingCoroutine);
                typingCoroutine = StartCoroutine(TypeLine(line.text));
            }
        }
        else
        {
            // 대사 종료 처리: 여기서는 씬 로드 금지!
            isDialogueActive = false;

            // (선택) UI 정리 정도만
            if (dialoguePanel) dialoguePanel.SetActive(false);

            // 페이드 아웃 코루틴만 호출
            if (!isEnding) StartCoroutine(CoEndScene());
        }
    }

    // ... CoEndScene() 수정
    IEnumerator CoEndScene()
    {
        if (isEnding) yield break;
        isEnding = true;

        // 페이더가 있다면: 투명 → 검정
        if (screenFader)
        {
            screenFader.gameObject.SetActive(true);
            screenFader.SetAlpha(0f);          // ← 시작값 0으로 세팅(메서드 추가 필요)
            yield return screenFader.FadeIn(sceneFadeOut);
        }

        // 페이드 끝난 뒤에만 씬 로드
        if (!string.IsNullOrEmpty(nextSceneName))
            SceneManager.LoadScene(nextSceneName);
    }

    // 타자기 효과
    IEnumerator TypeLine(string line)
    {
        isTyping = true;

        // 초기화
        dialogueText.text = "";
        dialogueText.maxVisibleCharacters = 0;
        dialogueText.text = line;
        dialogueText.ForceMeshUpdate();

        int totalChars = dialogueText.textInfo.characterCount;

        // 한 글자씩 순차적으로 보여주기
        for (int i = 0; i < totalChars; i++)
        {
            dialogueText.maxVisibleCharacters = i + 1;
            yield return new WaitForSeconds(typeSpeed);
        }

        isTyping = false;
        ShowEndCursor();
    }

    // 타자 효과 스킵
    void SkipTyping()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        dialogueText.ForceMeshUpdate();
        dialogueText.maxVisibleCharacters = dialogueText.textInfo.characterCount;

        isTyping = false;
        ShowEndCursor();
    }

    // 다음 대사로 이동
    void NextLine()
    {
        currentIndex++;
        ShowLine();
    }

    // 화살표 아이콘 표시
    void ShowEndCursor()
    {
        if (endCursor != null) endCursor.SetActive(true);
    }

    // 화살표 아이콘 숨기기
    void HideEndCursor()
    {
        if (endCursor != null) endCursor.SetActive(false);
    }
    //------------ 캐릭터 이미지 ---------------------
    void StartPortraitFade(float target, float dur, bool disableOnDone = false)
    {
        if (!portraitImage) return;
        StopCoroutineSafely(ref portraitFadeCoroutine);
        portraitFadeCoroutine = StartCoroutine(CoPortraitFade(target, dur, disableOnDone));
    }

    IEnumerator CoPortraitFade(float target, float dur, bool disableOnDone)
    {
        // CanvasGroup가 있으면 그걸 우선 사용
        var cg = portraitImage.GetComponent<CanvasGroup>();
        if (cg == null)
        {
            cg = portraitImage.gameObject.AddComponent<CanvasGroup>();
        }

        float t = 0f;
        float start = cg.alpha;
        while (t < dur)
        {
            t += Time.unscaledDeltaTime;
            cg.alpha = Mathf.Lerp(start, target, t / dur);
            yield return null;
        }
        cg.alpha = target;

        if (disableOnDone && target <= 0f)
        {
            portraitImage.gameObject.SetActive(false);
        }
    }

    void StopCoroutineSafely(ref Coroutine co)
    {
        if (co != null)
        {
            StopCoroutine(co);
            co = null;
        }
    }
}