using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerInteraction))]
public class PlayerInput : MonoBehaviour
{
    [Header("플레이어 config")]
    [SerializeField] private PlayerConfig config;

    [Header("시각 반전(좌/우)")]
    public Transform visual;
    private float baseScaleX = 1f;

    [Header("스킬 UI")]
    [SerializeField] private float holdThreshold = 0.4f;
    [SerializeField] private float cooldown = 0.1f;
    private float pressedAt = -1f;
    private float lastUseTime = -999f;
    private bool ready = false;
    public Image holdGauge;                // 게이지 Image (Type=Filled)
    public CanvasGroup gaugeRoot;          // 게이지 루트(CanvasGroup, 페이드용)
    // 게이지 색상 (가득 차면 초록색)
    private static readonly Color kGaugeNormal = Color.white;
    private static readonly Color kGaugeFull = Color.green;

    private PlayerInteraction interaction;
    private IAllySkill skill;

    public Vector2 LastDir { get; private set; } = Vector2.zero;
    public PlayerConfig Config => config;

    void Awake()
    {
        interaction = GetComponent<PlayerInteraction>();
        skill = GetComponent<IAllySkill>();
        if (config == null)
            Debug.LogError($"{name}에 PlayerConfig가 할당되지 않았습니다!");
        if (visual != null)
            baseScaleX = Mathf.Abs(visual.localScale.x);

        if (holdGauge != null)
        {
            holdGauge.type = Image.Type.Filled;
            holdGauge.fillMethod = Image.FillMethod.Horizontal;
            holdGauge.fillOrigin = (int)Image.OriginHorizontal.Left;
            holdGauge.fillAmount = 0f;
            holdGauge.color = kGaugeNormal;
        }
        HideGauge();
    }

    void Update()
    {
        if (config == null) return;

        HandleMoveInput();
        HandleSkillInput();
        UpdateHoldGauge();
    }

    private void HandleMoveInput()
    {
        Vector2 dir = GetInputDirection();
        if (dir == Vector2.zero) return;

        if (pressedAt > 0f)
        {
            CancelCharge();
            return;
        }

        LastDir = dir;
        interaction.TryAction(dir);
    }

    private void HandleSkillInput()
    {
        if (skill == null) return;
        // 쿨다운 중이면 입력 무시
        if (Time.time - lastUseTime < cooldown)
        {
            if (pressedAt > 0f) CancelCharge();
            return;
        }

        if (Input.GetKeyDown(config.SkillKey))
        {
            pressedAt = Time.time;
            ready = false;
            ShowGauge();
            SetGauge(0f);
            SetGaugeColor(kGaugeNormal);
        }
        if (pressedAt > 0f && Input.GetKey(config.SkillKey))
        {
            float held = Time.time - pressedAt;
            if (held >= holdThreshold)
                ready = true;
        }
        if (Input.GetKeyUp(config.SkillKey) && pressedAt > 0)
        {
            float held = Time.time - pressedAt;
            bool shouldCastToPartner = ready || held >= holdThreshold;

            var target = (shouldCastToPartner && config.Partner != null) 
                ? config.Partner
                : gameObject;
            /*
            if (ready || held >= holdThreshold)
            {
                // 홀드 완료 → 아군 대상으로
                // partner 연결해둔 거 쓰면 됨
                if (config.Partner != null) skill.UseSkill(config.Partner);
                else skill.UseSkill(gameObject);
            }
            else
            { // 탭 → 자기 자신 대상
                skill.UseSkill(gameObject);
            }
            */

            skill.UseSkill(target);
            lastUseTime = Time.time;

            pressedAt = -1f;
            ready = false;
            HideGauge();
            SetGauge(0f);
            SetGaugeColor(kGaugeNormal);
        }
    }

    private void UpdateHoldGauge()
    {
        if (pressedAt < 0f) return;

        float held = Mathf.Max(0f, Time.time - pressedAt);
        float ratio = holdThreshold > 0f ? Mathf.Clamp01(held / holdThreshold) : 1f;

        if (ratio >= 1f)
        {
            ready = true;
            SetGaugeColor(kGaugeFull);
        }
        else
        {
            SetGaugeColor(kGaugeNormal);
        }
        SetGauge(ratio);
    }

    private Vector2 GetInputDirection()
    {
        if (Input.GetKeyDown(config.UpKey)) return Vector2.up;
        if (Input.GetKeyDown(config.DownKey)) return Vector2.down;
        if (Input.GetKeyDown(config.LeftKey)) { SetFacingLeft();  return Vector2.left; }
        if (Input.GetKeyDown(config.RightKey)) { SetFacingRight();  return Vector2.right; }

        return Vector2.zero;
    }

    void SetFacingRight()
    {
        var s = visual.localScale;
        s.x = -baseScaleX;
        visual.localScale = s;
    }

    void SetFacingLeft()
    {
        var s = visual.localScale;
        s.x = baseScaleX;
        visual.localScale = s;
    }
    
    void ShowGauge()
    {
        if (gaugeRoot)
        {
            gaugeRoot.alpha = 1f;
            gaugeRoot.interactable = false;
            gaugeRoot.blocksRaycasts = false;
        }
        if (holdGauge) holdGauge.enabled = true;
    }

    void HideGauge()
    {
        if (gaugeRoot)
        {
            gaugeRoot.alpha = 0f;
            gaugeRoot.interactable = false;
            gaugeRoot.blocksRaycasts = false;
        }
        if (holdGauge) holdGauge.enabled = false;
    }

    void SetGaugeColor(Color c)
    {
        if (holdGauge) holdGauge.color = c;
    }

    void SetGauge(float t01)
    {
        if (holdGauge) holdGauge.fillAmount = Mathf.Clamp01(t01);
    }

    void CancelCharge()
    {
        pressedAt = -1f;
        ready = false;
        HideGauge();
        SetGauge(0f);
    }
}