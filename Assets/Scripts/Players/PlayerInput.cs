using UnityEngine;

[RequireComponent(typeof(PlayerInteraction))]
public class PlayerInput : MonoBehaviour {
    [Header("플레이어 설정")]
    [SerializeField] private PlayerConfig config;  // 키 세트, maxUses 등 관리

    private PlayerInteraction interaction;
    private IAllySkill skill;

    void Awake() {
        interaction = GetComponent<PlayerInteraction>();
        skill = GetComponent<IAllySkill>();

        if (config == null) {
            Debug.LogError($"{name}에 PlayerConfig가 할당되지 않았습니다!");
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

    // 나중에 수정
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
