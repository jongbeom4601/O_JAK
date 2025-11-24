using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public static class InputGates
{
    public static bool DialogueOpen;
}


public class FirstLoadDialogueGate : MonoBehaviour
{
    [Header("씬 페이드")]
    public UIFader screenFader;         // ← 검은 Image에 붙은 UIFader
    public float sceneFadeIn = 0.4f;    // 시작: 검정 → 투명
    public float sceneFadeOut = 0.4f;   // 끝: 투명 → 검정
    private bool isEnding = false;

    [Header("노출 정책: 같은 씬 '재시작'만 스킵")]
    [Tooltip("같은 씬을 즉시 다시 로드하면 스킵, 다른 씬 갔다가 돌아오면 다시 노출")]
    public bool skipOnSameSceneReload = true;

    // 이 플레이 세션에서 마지막으로 '대사를 보여준' 씬 인덱스
    private static int s_lastShownSceneIndex = -1;

    [Header("대사 중 배경 디머")]
    [Tooltip("풀스크린 검정 Image에 CanvasGroup를 붙여 할당 (초기 Alpha=0)")]
    public CanvasGroup backgroundDimmer;
    [Range(0f, 1f)] public float dimAlpha = 0.6f;
    public float dimFadeIn = 0.25f;
    public float dimFadeOut = 0.2f;

    [Header("시작 지연")]
    [Tooltip("첫 대사가 나오기 전 대기 시간(초)")]
    public float firstLineDelay = 0f;

    [Header("UI 연결")]
    public GameObject dialoguePanel;    // 대화창 패널
    public TMP_Text nameText;           // 화자 이름 표시용 TMP 텍스트
    public TMP_Text dialogueText;       // 대사 표시용 TMP 텍스트
    public GameObject endCursor;        // 대사 끝났을 때 나오는 화살표 아이콘

    [Header("초상화")]
    public Image portraitImage;         // 초상 이미지 슬롯
    public float portraitFade = 0.15f;  // 페이드 시간(초)

    [Header("대사 데이터")]
    public DialogueLine[] lines;        // 대사 배열 (**Inspector에서 입력**)

    [Header("다음 씬 이름 (스킵 시 이동할 경우에만 사용)")]
    public string nextSceneName;        // 필요 없으면 비워두기

    [Header("타자기 효과 속도")]
    public float typeSpeed = 0.05f;     // 글자 하나 출력마다 딜레이 시간


    public CanvasGroup postDialogueUI1;
    public CanvasGroup postDialogueUI2;
    // 내부 상태
    private int currentIndex = 0;
    private bool isDialogueActive = false;
    private bool isTyping = false;
    private Coroutine typingCoroutine;
    private Coroutine portraitFadeCoroutine;

    void Start()
    {
        //  이 오브젝트가 속한 씬 인덱스(애디티브 안전)
        int thisSceneIndex = gameObject.scene.buildIndex;

        //  StageIntroRunner의 기록을 참고해서 "같은 씬 재시작"이면 스킵
        if (skipOnSameSceneReload && StageIntroRunner.WasThisSceneShown(thisSceneIndex))
        {
            SkipAllAndProceed();
            return;
        }

        StartCoroutine(CoStartDialogue());
    }


    void Update()
    {
        if (isDialogueActive && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))
        {
            if (isTyping) SkipTyping();
            else NextLine();
        }
    }

    IEnumerator CoStartDialogue()
    {
        InputGates.DialogueOpen = true;
        // 시작 페이드: 검정 → 투명
        if (screenFader)
        {
            screenFader.SetVisible(true);
            yield return screenFader.FadeOut(sceneFadeIn);
        }

        if (firstLineDelay > 0f)
            yield return new WaitForSecondsRealtime(firstLineDelay);

        // 배경 디밍: 0 → dimAlpha
        if (backgroundDimmer)
        {
            backgroundDimmer.gameObject.SetActive(true);
            backgroundDimmer.blocksRaycasts = true;
            backgroundDimmer.interactable = false;
            backgroundDimmer.alpha = 0f;
            yield return CoFade(backgroundDimmer, 0f, dimAlpha, dimFadeIn);
        }

        if (dialoguePanel) dialoguePanel.SetActive(true);
        if (endCursor) endCursor.SetActive(false);

        if (lines != null && lines.Length > 0)
        {
            ShowLine();
            isDialogueActive = true;
        }
        else
        {
            isDialogueActive = false;
            if (!isEnding) StartCoroutine(CoEndScene());
        }
    }

    void ShowLine()
    {
        HideEndCursor();

        if (currentIndex < lines.Length)
        {
            var line = lines[currentIndex];

            if (nameText) nameText.text = line.speakerName;

            if (portraitImage)
            {
                StopCoroutineSafely(ref portraitFadeCoroutine);
                if (line.portrait)
                {
                    portraitImage.sprite = line.portrait;

                    var rt = portraitImage.rectTransform;
                    rt.localScale = new Vector3(
                        (line.flipX ? -1f : 1f) * Mathf.Abs(rt.localScale.x),
                        rt.localScale.y,
                        rt.localScale.z
                    );

                    portraitImage.gameObject.SetActive(true);
                    StartPortraitFade(1f, portraitFade);
                }
                else
                {
                    StartPortraitFade(0f, portraitFade, disableOnDone: true);
                }
            }

            if (dialogueText)
            {
                if (typingCoroutine != null) StopCoroutine(typingCoroutine);
                typingCoroutine = StartCoroutine(TypeLine(line.text));
            }
        }
        else
        {
            isDialogueActive = false;
            if (dialoguePanel) dialoguePanel.SetActive(false);

            if (!isEnding) StartCoroutine(CoEndScene());
        }
    }

    IEnumerator CoEndScene()
    {
        if (isEnding) yield break;
        isEnding = true;

        // 디머 해제
        if (backgroundDimmer)
        {
            yield return CoFade(backgroundDimmer, backgroundDimmer.alpha, 0f, dimFadeOut);
            backgroundDimmer.blocksRaycasts = false;
            backgroundDimmer.gameObject.SetActive(false);
        }

        if (postDialogueUI1 != null)
        {
            postDialogueUI1.gameObject.SetActive(true);
        }

        if (postDialogueUI2 != null)
        {
            postDialogueUI2.gameObject.SetActive(true);
        }

        // 화면 페이드아웃
        if (screenFader)
        {
            screenFader.SetVisible(true);
            yield return screenFader.FadeIn(sceneFadeOut);
        }
        InputGates.DialogueOpen = false;
        // 로고: 대사 끝 알림
        FindObjectOfType<StageIntroRunner>()?.NotifyDialogueFinished();

        // 필요하다면 다음 씬 이동 (지금은 주석 권장)
        // if (!string.IsNullOrEmpty(nextSceneName))
        //     SceneManager.LoadScene(nextSceneName);
    }

    IEnumerator TypeLine(string line)
    {
        isTyping = true;

        dialogueText.text = "";
        dialogueText.maxVisibleCharacters = 0;
        dialogueText.text = line;
        dialogueText.ForceMeshUpdate();

        int totalChars = dialogueText.textInfo.characterCount;

        for (int i = 0; i < totalChars; i++)
        {
            dialogueText.maxVisibleCharacters = i + 1;
            yield return new WaitForSeconds(typeSpeed);
        }

        isTyping = false;
        ShowEndCursor();
    }

    void SkipTyping()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        dialogueText.ForceMeshUpdate();
        dialogueText.maxVisibleCharacters = dialogueText.textInfo.characterCount;

        isTyping = false;
        ShowEndCursor();
    }

    void NextLine()
    {
        currentIndex++;
        ShowLine();
    }

    void ShowEndCursor()
    {
        if (endCursor) endCursor.SetActive(true);
    }
    void HideEndCursor()
    {
        if (endCursor) endCursor.SetActive(false);
    }

    void StartPortraitFade(float target, float dur, bool disableOnDone = false)
    {
        if (!portraitImage) return;
        StopCoroutineSafely(ref portraitFadeCoroutine);
        portraitFadeCoroutine = StartCoroutine(CoPortraitFade(target, dur, disableOnDone));
    }

    IEnumerator CoPortraitFade(float target, float dur, bool disableOnDone)
    {
        var cg = portraitImage.GetComponent<CanvasGroup>();
        if (cg == null) cg = portraitImage.gameObject.AddComponent<CanvasGroup>();

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
            portraitImage.gameObject.SetActive(false);
    }

    void StopCoroutineSafely(ref Coroutine co)
    {
        if (co != null)
        {
            StopCoroutine(co);
            co = null;
        }
    }

    static IEnumerator CoFade(CanvasGroup cg, float from, float to, float dur)
    {
        float t = 0f;
        while (t < dur)
        {
            t += Time.unscaledDeltaTime;
            float k = t / dur;
            cg.alpha = Mathf.Lerp(from, to, EaseOutQuad(k));
            yield return null;
        }
        cg.alpha = to;
    }

    static float EaseOutQuad(float x) => 1f - (1f - x) * (1f - x);

    // 같은 씬 재시작 시: 대사/디머 스킵하고 바로 진행(원래 네 흐름에 맞게 수정해 써)
    void SkipAllAndProceed()
    {
        if (backgroundDimmer)
        {
            backgroundDimmer.alpha = 0f;
            backgroundDimmer.blocksRaycasts = false;
            backgroundDimmer.gameObject.SetActive(false);
        }
        if (dialoguePanel) dialoguePanel.SetActive(false);

        // 재시작 스킵 시 보통은 '이 씬에서 바로 플레이'를 원할 거라
        // 다음 씬 이동은 기본 비활성. 필요하면 아래 주석 해제.
        // if (!string.IsNullOrEmpty(nextSceneName))
        //     SceneManager.LoadScene(nextSceneName);

        // 로고를 대사 없이 바로 띄우고 싶다면:
        FindObjectOfType<StageIntroRunner>()?.NotifyDialogueFinished();
    }
}
