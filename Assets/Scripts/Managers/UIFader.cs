using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class UIFader : MonoBehaviour
{
    public float defaultDuration = 0.3f;
    public bool useUnscaledTime = true;

    CanvasGroup cg; Coroutine co;

    void Awake() { cg = GetComponent<CanvasGroup>(); }

    public void SetVisible(bool v)
    {
        cg.alpha = v ? 1f : 0f;
        cg.blocksRaycasts = v; cg.interactable = v;
        gameObject.SetActive(v);
    }

    public Coroutine FadeIn(float? d = null) => FadeTo(1f, d ?? defaultDuration);
    public Coroutine FadeOut(float? d = null) => FadeTo(0f, d ?? defaultDuration);
    public void SetAlpha(float a)
    {
        cg.alpha = a;
    }

    Coroutine FadeTo(float t, float dur)
    {
        if (co != null) StopCoroutine(co);
        if (!gameObject.activeSelf) gameObject.SetActive(true);
        return co = StartCoroutine(Co(t, dur));
    }
    IEnumerator Co(float target, float dur)
    {
        float s = cg.alpha, t = 0f;
        while (t < dur)
        {
            t += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            cg.alpha = Mathf.Lerp(s, target, t / dur);
            yield return null;
        }
        cg.alpha = target;
        bool vis = target >= 0.999f;
        cg.blocksRaycasts = vis; cg.interactable = vis;
        if (!vis) gameObject.SetActive(false);
        co = null;
    }
}
