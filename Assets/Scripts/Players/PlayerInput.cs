using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

[RequireComponent(typeof(PlayerInteraction))]
public class PlayerInput : MonoBehaviour
{
    [Header("플레이어 config")]
    [SerializeField] private PlayerConfig config;

    [Header("시각 반전(좌/우)")]
    public Transform visual;
    private float baseScaleX = 1f;

    [SerializeField] private float holdThreshold = 1f;
    private float pressedAt = -1f;
    private bool ready = false;

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
    }

    void Update()
    {
        if (config == null) return;

        HandleMoveInput();
        HandleSkillInput();
    }

    private void HandleMoveInput()
    {
        Vector2 dir = GetInputDirection();
        if (dir != Vector2.zero)
        {
            LastDir = dir;
            interaction.TryAction(dir);
        }
    }

    private void HandleSkillInput()
    {
        if (skill == null) return;

        if (Input.GetKeyDown(config.SkillKey))
        {
            pressedAt = Time.time;
            ready = false;
        }
        if (pressedAt > 0 && Input.GetKey(config.SkillKey))
        {
            float held = Time.time - pressedAt;
            if (held >= holdThreshold)
                ready = true;
        }
        if (Input.GetKeyUp(config.SkillKey) && pressedAt > 0)
        {
            float held = Time.time - pressedAt;
            pressedAt = -1f;

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
        }
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
}