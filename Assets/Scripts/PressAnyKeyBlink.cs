using UnityEngine;
using UnityEngine.UI;

public class PressAnyKeyBlink : MonoBehaviour
{
    public Text text;           // UI Text 연결
    public float blinkSpeed = 2f; // 2면 대략 2초 주기

    void Update()
    {
        if (!text) return;

        float alpha = (Mathf.Sin(Time.unscaledTime * blinkSpeed) + 1f) * 0.5f; // 0~1
        var c = text.color;
        c.a = alpha;
        text.color = c;
    }
}
