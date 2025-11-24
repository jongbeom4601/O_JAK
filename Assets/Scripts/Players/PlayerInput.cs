using UnityEngine;

[RequireComponent(typeof(PlayerInteraction))]
public class PlayerInput : MonoBehaviour
{
    [Header("플레이어 config")]
    [SerializeField] private PlayerConfig config;
    private Animator animator;

    [Header("스킬 게이지 (SpriteRenderer)")]
    [SerializeField] private SpriteRenderer gaugeBack;
    [SerializeField] private SpriteRenderer gaugeFill;

    [Tooltip("센터에서 양쪽으로 균등하게 채움")]
    [SerializeField] private bool centerFillMode = true;

    [Tooltip("왼쪽을 고정점으로 채움 (centerFillMode=false 때만 사용)")]
    [SerializeField] private bool pinLeftEdge = false;

    [SerializeField] private float holdThreshold = 0.4f;
    [SerializeField] private float cooldown = 0.1f;

    private float pressedAt = -1f;
    private float lastUseTime = -999f;
    private bool ready = false;

    public AudioSource audioSource;
    public AudioClip footstepClip;

    private static readonly Color kGaugeNormal = Color.white;
    private static readonly Color kGaugeFull = Color.green;

    private Vector3 _fillBaseScale = Vector3.one;
    private Vector3 _fillBaseLocalPos = Vector3.zero;

    private PlayerInteraction interaction;
    private IAllySkill skill;

    public Vector2 LastDir { get; private set; } = Vector2.zero;
    public PlayerConfig Config => config;

    //  추가: 입력 잠금 상태
    public bool InputLocked { get; private set; } = false;

    void Awake()
    {
        interaction = GetComponent<PlayerInteraction>();
        skill = GetComponent<IAllySkill>();
        animator = config.Animator;

        if (config == null)
            Debug.LogError($"{name}에 PlayerConfig가 할당되지 않았습니다!");

        if (gaugeFill != null)
        {
            _fillBaseScale = gaugeFill.transform.localScale;
            _fillBaseLocalPos = gaugeFill.transform.localPosition;
        }

        HideGauge();
        SetAnimatorDir(Vector2.down);
    }

    void Update()
    {
        //  대화 중이면 입력 처리 안 함
        if (InputGates.DialogueOpen) return;
        if (config == null) return;

        HandleMoveInput();
        HandleSkillInput();
        UpdateHoldGauge();
    }

    private void HandleMoveInput()
    {
        Vector2 dir = GetInputDirection();
        if (dir == Vector2.zero) return;
        if (InputLocked) return;

        if (pressedAt > 0f)
        {
            CancelCharge();
            return;
        }

        LastDir = dir;
        if (footstepClip && audioSource)
            audioSource.PlayOneShot(footstepClip);

        SetAnimatorDir(dir);
        interaction.TryAction(dir);
    }

    private void HandleSkillInput()
    {
        if (skill == null) return;

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

            skill.UseSkill(gameObject, target);
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

        SetGaugeColor(ratio >= 1f ? kGaugeFull : kGaugeNormal);
        if (ratio >= 1f) ready = true;

        SetGauge(ratio);
    }

    private Vector2 GetInputDirection()
    {
        if (Input.GetKeyDown(config.UpKey)) return Vector2.up;
        if (Input.GetKeyDown(config.DownKey)) return Vector2.down;
        if (Input.GetKeyDown(config.LeftKey)) return Vector2.left;
        if (Input.GetKeyDown(config.RightKey)) return Vector2.right;
        return Vector2.zero;
    }

    void SetAnimatorDir(Vector2 dir)
    {
        if (animator == null) return;
        int d = 0;
        if (dir == Vector2.up) d = 3;
        else if (dir == Vector2.down) d = 2;
        else if (dir == Vector2.left) d = 1;
        else if (dir == Vector2.right) d = 0;
        animator.SetFloat("Dir", d);
    }

    // ===== SpriteRenderer 기반 게이지 제어 =====
    void ShowGauge()
    {
        if (gaugeBack) gaugeBack.enabled = true;
        if (gaugeFill) gaugeFill.enabled = true;
    }

    void HideGauge()
    {
        if (gaugeBack) gaugeBack.enabled = false;
        if (gaugeFill) gaugeFill.enabled = false;
    }

    void SetGaugeColor(Color c)
    {
        if (gaugeFill) gaugeFill.color = c;
    }

    void SetGauge(float t01)
    {
        if (!gaugeFill) return;
        t01 = Mathf.Clamp01(t01);

        if (centerFillMode)
        {
            float newX = Mathf.Max(0f, _fillBaseScale.x * t01);
            gaugeFill.transform.localScale = new Vector3(newX, _fillBaseScale.y, _fillBaseScale.z);
            gaugeFill.transform.localPosition = _fillBaseLocalPos;
        }
        else
        {
            float newX = Mathf.Max(0f, _fillBaseScale.x * t01);
            gaugeFill.transform.localScale = new Vector3(newX, _fillBaseScale.y, _fillBaseScale.z);

            if (pinLeftEdge)
            {
                float delta = (_fillBaseScale.x - newX) * 0.5f;
                gaugeFill.transform.localPosition = new Vector3(
                    _fillBaseLocalPos.x - delta,
                    _fillBaseLocalPos.y,
                    _fillBaseLocalPos.z
                );
            }
            else
            {
                gaugeFill.transform.localPosition = _fillBaseLocalPos;
            }
        }
    }

    void CancelCharge()
    {
        pressedAt = -1f;
        ready = false;
        HideGauge();
        SetGauge(0f);
    }

    //  외부에서 잠금/해제 제어용 메서드
    public void SetInputLocked(bool locked)
    {
        InputLocked = locked;
    }
}
