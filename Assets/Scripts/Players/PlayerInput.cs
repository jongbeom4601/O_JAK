using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

[RequireComponent(typeof(PlayerInteraction))]
public class PlayerInput : MonoBehaviour
{
    [Header("�÷��̾� config")]
    [SerializeField] private PlayerConfig config;

    [Header("�ð� ����(��/��)")]
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
            Debug.LogError($"{name}�� PlayerConfig�� �Ҵ���� �ʾҽ��ϴ�!");
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
                // Ȧ�� �Ϸ� �� �Ʊ� �������
                // partner �����ص� �� ���� ��
                if (config.Partner != null) skill.UseSkill(config.Partner);
                else skill.UseSkill(gameObject);
            }
            else
            { // �� �� �ڱ� �ڽ� ���
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