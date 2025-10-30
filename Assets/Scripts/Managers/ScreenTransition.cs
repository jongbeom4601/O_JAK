using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class ScreenTransition : MonoBehaviour
{
    public enum Style { SoftDim, Letterbox, Iris }

    [Header("Refs")]
    [SerializeField] CanvasGroup root;    // 전환 전체 루트
    [SerializeField] Image dimmer;        // 화면 어둡게(SoftDim용)
    [SerializeField] Image logo;          // 로고(없어도 됨)
    [SerializeField] RectTransform barTop, barBottom; // Letterbox용
    [SerializeField] Image iris;          // Iris용 (Image Type=Filled, Radial360)

    [Header("Timings (sec, unscaled)")]
    [SerializeField] float inDur = 0.4f;   // 등장
    [SerializeField] float hold = 0.10f;   // 유지
    [SerializeField] float outDur = 0.18f;   // 퇴장

    [Header("Looks")]
    [SerializeField] float dimAlpha = 0.8f;  // SoftDim 목표 알파
    [SerializeField] Vector2 letterboxBarSize = new Vector2(0, 240f); // 바 높이
    [SerializeField] Sprite placeholderSprite;
    [SerializeField] Color logoTint = Color.white;

    void Awake()
    {
        if (!root) root = GetComponentInChildren<CanvasGroup>(true);
        if (logo)
        {
            //logo.sprite = placeholderSprite;
            logo.color = logoTint;
            logo.rectTransform.localScale = Vector3.one * 0.9f;
            logo.enabled = false;
        }
        if (dimmer)
        {
            var c = dimmer.color; c.a = 0f; dimmer.color = c;
            dimmer.enabled = false;
        }
        if (iris)
        {
            iris.type = Image.Type.Filled;
            iris.fillMethod = Image.FillMethod.Radial360;
            iris.fillOrigin = (int)Image.Origin360.Top; // 아무거나 ok
            iris.fillAmount = 0f; // 0=안 보임
            iris.enabled = false;
        }
        if (barTop) barTop.sizeDelta = new Vector2(0, 0);
        if (barBottom) barBottom.sizeDelta = new Vector2(0, 0);
        SetActive(false);
    }

    public void SetSprite(Sprite s, Color? tint = null)
    {
        if (!logo) return;
        logo.sprite = s ? s : placeholderSprite;
        logo.color = tint ?? logoTint;
    }

    public void PlayAndLoad(Style style, string sceneName)
    {
        StartCoroutine(CoPlayAndLoad(style, sceneName));
    }

    IEnumerator CoPlayAndLoad(Style style, string sceneName)
    {
        SetActive(true);

        switch (style)
        {
            case Style.SoftDim:
                yield return CoSoftDimIn();
                break;
            case Style.Letterbox:
                yield return CoLetterboxIn();
                break;
            case Style.Iris:
                yield return CoIrisIn();
                break;
        }

        yield return WaitUnscaled(hold);

        if (!string.IsNullOrEmpty(sceneName))
            SceneManager.LoadScene(sceneName);

        switch (style)
        {
            case Style.SoftDim:
                yield return CoSoftDimOut();
                break;
            case Style.Letterbox:
                yield return CoLetterboxOut();
                break;
            case Style.Iris:
                yield return CoIrisOut();
                break;
        }

        SetActive(false);
    }

    // ---------- SoftDim ----------
    IEnumerator CoSoftDimIn()
    {
        // 딤머(배경 어둡게 만드는 UI Image)와 로고(Image)가 비활성 상태면 먼저 보이도록 켠다
        if (dimmer) dimmer.enabled = true;
        if (logo) logo.enabled = true;
        // t: 경과 시간(초), c: 딤머 색상(알파만 바꿔서 투명도 조절)
        float t = 0;
        var c = dimmer ? dimmer.color : Color.black;
        // inDur(연출 시간) 동안 매 프레임 보간
        while (t < inDur)
        {
            // timeScale의 영향(일시정지 등)과 무관하게 흐르게 하려고 unscaledDeltaTime 사용
            t += Time.unscaledDeltaTime;
            // k: 0→1로 증가하는 보간 인자에 EaseOutQuad를 적용해 초반 빠르고 후반 느리게
            float k = EaseOutQuad(t / inDur);
            // 딤머의 알파를 0 → dimAlpha 로 보간해서 점점 어둡게
            if (dimmer)
            {
                c.a = Mathf.Lerp(0f, dimAlpha, k);
                dimmer.color = c;
            }
            // 로고의 스케일을 0.9 → 1.0으로 보간해 살짝 줌인(팝업) 느낌
            if (logo)
                logo.rectTransform.localScale = Vector3.Lerp(Vector3.one * 0.7f, Vector3.one, k);
            // 다음 프레임까지 대기
            yield return null;
        }
        // 루프가 끝난 뒤 최종 상태를 한 번 더 고정해 오차 제거
        if (dimmer)
        {
            c.a = dimAlpha;
            dimmer.color = c;
        }
        if (logo)
            logo.rectTransform.localScale = Vector3.one;
    }

    IEnumerator CoSoftDimOut()
    {
        float t = 0; var c = dimmer ? dimmer.color : Color.black;
        while (t < outDur)
        {
            t += Time.unscaledDeltaTime;
            float k = EaseInQuad(t / outDur);
            if (dimmer) { c.a = Mathf.Lerp(dimAlpha, 0f, k); dimmer.color = c; }
            if (logo) logo.rectTransform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 0.95f, k);
            yield return null;
        }
        if (dimmer) { c.a = 0f; dimmer.color = c; dimmer.enabled = false; }
        if (logo) logo.enabled = false;
    }

    // ---------- Letterbox ----------
    IEnumerator CoLetterboxIn()
    {
        if (!barTop || !barBottom) yield break;
        float t = 0; var sz = letterboxBarSize;
        while (t < inDur)
        {
            t += Time.unscaledDeltaTime;
            float k = EaseOutCubic(t / inDur);
            float h = Mathf.Lerp(0f, sz.y, k);
            barTop.sizeDelta = new Vector2(0, h);
            barBottom.sizeDelta = new Vector2(0, h);
            yield return null;
        }
    }
    IEnumerator CoLetterboxOut()
    {
        if (!barTop || !barBottom) yield break;
        float t = 0; var sz = letterboxBarSize;
        while (t < outDur)
        {
            t += Time.unscaledDeltaTime;
            float k = EaseInCubic(t / outDur);
            float h = Mathf.Lerp(sz.y, 0f, k);
            barTop.sizeDelta = new Vector2(0, h);
            barBottom.sizeDelta = new Vector2(0, h);
            yield return null;
        }
    }

    // ---------- Iris (Radial) ----------
    IEnumerator CoIrisIn()
    {
        if (!iris) yield break;
        iris.enabled = true; iris.fillAmount = 0f;
        float t = 0;
        while (t < inDur)
        {
            t += Time.unscaledDeltaTime;
            float k = EaseOutCubic(t / inDur);
            iris.fillAmount = Mathf.Lerp(0f, 1f, k); // 원형이 커짐
            yield return null;
        }
        iris.fillAmount = 1f;
    }
    IEnumerator CoIrisOut()
    {
        if (!iris) yield break;
        float t = 0;
        while (t < outDur)
        {
            t += Time.unscaledDeltaTime;
            float k = EaseInCubic(t / outDur);
            iris.fillAmount = Mathf.Lerp(1f, 0f, k);
            yield return null;
        }
        iris.fillAmount = 0f; iris.enabled = false;
    }

    // ---------- Utils ----------
    void SetActive(bool on)
    {
        if (!root) return;
        root.alpha = on ? 1f : 0f;
        root.blocksRaycasts = on;
        root.interactable = false;
        gameObject.SetActive(true);
    }
    IEnumerator WaitUnscaled(float dur) { float t = 0; while (t < dur) { t += Time.unscaledDeltaTime; yield return null; } }
    float EaseOutQuad(float x) => 1 - (1 - x) * (1 - x);
    float EaseInQuad(float x) => x * x;
    float EaseOutCubic(float x) => 1 - Mathf.Pow(1 - x, 3);
    float EaseInCubic(float x) => x * x * x;
}
