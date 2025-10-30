using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class StageIntroUI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private CanvasGroup root;       // 패널 루트
    [SerializeField] private Image bg;               // 살짝 틴트
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI subtitle;
    [SerializeField] private Image icon;             // 선택
    [SerializeField] private AudioSource audioSrc;   // 선택(SFX)

    [Header("옵션")]
    [SerializeField] private bool allowSkip = true;  // 아무 키로 스킵

    void Awake()
    {
        if (root != null) { root.alpha = 0f; root.interactable = false; root.blocksRaycasts = false; }
        if (bg != null) bg.enabled = true;
        gameObject.SetActive(false);
    }

    public Coroutine Show(StageIntroData data)
    {
        if (data == null) return null;
        gameObject.SetActive(true);

        // 콘텐츠 바인딩
        if (title) title.text = data.Title;
        if (subtitle)
        {
            subtitle.text = string.IsNullOrEmpty(data.Subtitle) ? "" : data.Subtitle;
            subtitle.gameObject.SetActive(!string.IsNullOrEmpty(data.Subtitle));
        }
        if (icon)
        {
            icon.enabled = data.Icon != null;
            icon.sprite = data.Icon;
        }
        if (bg) bg.color = new Color(data.ThemeColor.r, data.ThemeColor.g, data.ThemeColor.b, 0.2f);
        if (title) title.color = data.ThemeColor;
        if (subtitle) subtitle.color = data.ThemeColor;

        // SFX
        if (audioSrc && data.sfx) { audioSrc.clip = data.sfx; audioSrc.Play(); }

        return StartCoroutine(CoShow(data));
    }

    IEnumerator CoShow(StageIntroData d)
    {
        // 페이드 인
        yield return Fade(0f, 1f, d.fadeIn);

        // 유지(스킵 가능)
        float t = 0f;
        bool skipped = false;
        while (t < d.hold)
        {
            t += Time.unscaledDeltaTime;
            if (allowSkip && AnyKeyDown()) { skipped = true; break; }
            yield return null;
        }

        // 페이드 아웃(스킵일 때 즉시 0.1초로 빠르게)
        float outDur = skipped ? Mathf.Min(0.1f, d.fadeOut) : d.fadeOut;
        yield return Fade(root.alpha, 0f, outDur);

        gameObject.SetActive(false);
    }

    IEnumerator Fade(float a, float b, float dur)
    {
        if (!root || dur <= 0f) { if (root) root.alpha = b; yield break; }
        float t = 0f;
        while (t < dur)
        {
            t += Time.unscaledDeltaTime; // ★ 타임스케일 무시
            root.alpha = Mathf.Lerp(a, b, t / dur);
            yield return null;
        }
        root.alpha = b;
    }

    bool AnyKeyDown()
    {
        // UI 스킵용(마우스/키보드 아무 입력)
        return Input.anyKeyDown;
    }
}
