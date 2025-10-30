using UnityEngine;

[CreateAssetMenu(menuName = "Game/Stage Intro Data", fileName = "StageIntroData")]
public class StageIntroData : ScriptableObject
{
    [Header("표시 콘텐츠")]
    [SerializeField] private string title = "사막";
    [SerializeField] private string subtitle = "";    // 선택
    [SerializeField] private Sprite icon;             // 선택
    [SerializeField] private Color themeColor = Color.yellow;

    [Header("타이밍(sec)")]
    [Min(0f)] public float fadeIn = 0.35f;
    [Min(0f)] public float hold = 0.8f;
    [Min(0f)] public float fadeOut = 0.35f;

    [Header("사운드(선택)")]
    public AudioClip sfx;

    public string Title => title;
    public string Subtitle => subtitle;
    public Sprite Icon => icon;
    public Color ThemeColor => themeColor;
}
