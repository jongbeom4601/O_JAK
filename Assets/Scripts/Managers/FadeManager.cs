using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class FadeManager : MonoBehaviour
{
    public static FadeManager Instance;

    [Header("Settings")]
    public Image fadeImage;                // Ǯ��ũ�� ���� Image
    public float defaultDuration = 1.0f;   // �⺻ ���̵� �ð�
    public AnimationCurve ease = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public bool useUnscaledTime = true;    // Ÿ�ӽ����� ���� ����

    bool isFading = false;
    float currentAlpha = 1f; // ������ 1�� �ϸ� ù ������ FadeIn ����

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (!fadeImage)
            Debug.LogWarning("[FadeManager] fadeImage�� �������. Ǯ��ũ�� Image�� �����ϼ���.");

        SetAlpha(currentAlpha);
    }

    void SetAlpha(float a)
    {
        if (!fadeImage) return;
        var c = fadeImage.color;
        c.a = a;
        fadeImage.color = c;
        currentAlpha = a;

        // ���İ� 0�̸� �Է� ���, 0���� ũ�� �Է� ����
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

    /// <summary> � ȭ�鿡�� ������� </summary>
    public void FadeIn(float duration = -1f)
    {
        if (duration <= 0f) duration = defaultDuration;
        StopAllCoroutines();
        StartCoroutine(FadeRoutine(currentAlpha, 0f, duration));
    }

    /// <summary> ȭ���� ��İ� </summary>
    public void FadeOut(float duration = -1f)
    {
        if (duration <= 0f) duration = defaultDuration;
        StopAllCoroutines();
        StartCoroutine(FadeRoutine(currentAlpha, 1f, duration));
    }

    /// <summary> ���̵� �ƿ� �� �� �ε�, �ε� �� ���̵� �� </summary>
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

        // �� �ε� (���Ѵٸ� LoadSceneMode.Additive�� �ٲ㵵 ��)
        yield return SceneManager.LoadSceneAsync(sceneName);

        // �ٷ� ������ ������ �� ������ ���
        yield return null;

        // In
        yield return FadeRoutine(1f, 0f, inDur);
    }

    /// <summary> ���ϴ� ���ķ� ��� ����(0~1) </summary>
    public void SetInstantAlpha(float a) => SetAlpha(Mathf.Clamp01(a));

    public bool IsFading => isFading;
}
