using UnityEngine;

[RequireComponent(typeof(PlayerInteraction))]
public class PlayerInput : MonoBehaviour {
    [Header("�÷��̾� ����")]
    [SerializeField] private PlayerConfig config;  // Ű ��Ʈ, maxUses �� ����

    private PlayerInteraction interaction;
    private IAllySkill skill;

    void Awake() {
        interaction = GetComponent<PlayerInteraction>();
        skill = GetComponent<IAllySkill>();

        if (config == null) {
            Debug.LogError($"{name}�� PlayerConfig�� �Ҵ���� �ʾҽ��ϴ�!");
        }
    }

    void Update() {
        if (config == null) return;

        HandleMoveInput();
        HandleSkillInput();
    }

    private void HandleMoveInput() {
        Vector2 dir = GetInputDirection();
        if (dir != Vector2.zero) {
            interaction.TryAction(dir);
        }
    }

    // ���߿� ����
    private void HandleSkillInput() {
        if (skill == null) return;

        if (Input.GetKeyDown(config.SkillKey)) {
            skill.UseOnSelf(gameObject);
        }
    }

    private Vector2 GetInputDirection() {
        if (Input.GetKeyDown(config.UpKey)) return Vector2.up;
        if (Input.GetKeyDown(config.DownKey)) return Vector2.down;
        if (Input.GetKeyDown(config.LeftKey)) return Vector2.left;
        if (Input.GetKeyDown(config.RightKey)) return Vector2.right;

        return Vector2.zero;
    }
}
