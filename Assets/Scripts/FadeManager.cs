using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class FadeManager : MonoBehaviour
{
    public static FadeManager Instance;

    [Header("Settings")]
    public Image fadeImage;                // 풀스크린 검은 Image
    public float defaultDuration = 1.0f;   // 기본 페이드 시간
    public AnimationCurve ease = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public bool useUnscaledTime = true;    // 타임스케일 영향 없이

    bool isFading = false;
    float currentAlpha = 1f; // 시작을 1로 하면 첫 씬에서 FadeIn 가능

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (!fadeImage)
            Debug.LogWarning("[FadeManager] fadeImage가 비어있음. 풀스크린 Image를 연결하세요.");

        SetAlpha(currentAlpha);
    }

    void SetAlpha(float a)
    {
        if (!fadeImage) return;
        var c = fadeImage.color;
        c.a = a;
        fadeImage.color = c;
        currentAlpha = a;

        // 알파가 0이면 입력 통과, 0보다 크면 입력 차단
        fadeImage.raycastTarget = (a > 0.001f);
    }

    float DeltaTime => useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

    IEnumerator FadeRoutine(float from, float to, float duration)
    {
        isFading = true;
        float t = 0f;

        while (t < duration)
        {
            t += DeltaTime;
            float p = Mathf.Clamp01(t / duration);
            float e = ease != null ? ease.Evaluate(p) : p;
            SetAlpha(Mathf.Lerp(from, to, e));
            yield return null;
        }

        SetAlpha(to);
        isFading = false;
    }

    // --- Public API ---

    /// <summary> 까만 화면에서 밝아지기 </summary>
    public void FadeIn(float duration = -1f)
    {
        if (duration <= 0f) duration = defaultDuration;
        StopAllCoroutines();
        StartCoroutine(FadeRoutine(currentAlpha, 0f, duration));
    }

    /// <summary> 화면을 까맣게 </summary>
    public void FadeOut(float duration = -1f)
    {
        if (duration <= 0f) duration = defaultDuration;
        StopAllCoroutines();
        StartCoroutine(FadeRoutine(currentAlpha, 1f, duration));
    }

    /// <summary> 페이드 아웃 후 씬 로드, 로드 후 페이드 인 </summary>
    public void FadeOutInToScene(string sceneName, float outDur = -1f, float inDur = -1f)
    {
        if (outDur <= 0f) outDur = defaultDuration;
        if (inDur <= 0f) inDur = defaultDuration;

        StopAllCoroutines();
        StartCoroutine(FadeOutInLoadRoutine(sceneName, outDur, inDur));
    }

    IEnumerator FadeOutInLoadRoutine(string sceneName, float outDur, float inDur)
    {
        // Out
        yield return FadeRoutine(currentAlpha, 1f, outDur);

        // 씬 로드 (원한다면 LoadSceneMode.Additive로 바꿔도 됨)
        yield return SceneManager.LoadSceneAsync(sceneName);

        // 바로 깜빡임 방지용 한 프레임 대기
        yield return null;

        // In
        yield return FadeRoutine(1f, 0f, inDur);
    }

    /// <summary> 원하는 알파로 즉시 설정(0~1) </summary>
    public void SetInstantAlpha(float a) => SetAlpha(Mathf.Clamp01(a));

    public bool IsFading => isFading;
}
