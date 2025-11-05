using UnityEngine;
using UnityEngine.UI;

public class PlayerSkill : MonoBehaviour, IAllySkill {
    private int remainingUses;
    private PlayerInput ownerInput;
    private ActionType skillType;

    [Header("애니메이션")]
    [SerializeField] private Animator animator;

    [Header("UI")]
    [SerializeField] private Text usesText;
    [SerializeField] private Animator anim;

    void Awake() {
        ownerInput = GetComponent<PlayerInput>();
        if (ownerInput == null || ownerInput.Config == null) {
            Debug.LogError($"{name}: PlayerConfig 연결 안 됨!");
            return;
        }
        switch (ownerInput.Config.Type) {
            case PlayerType.Geonwoo:
                skillType = ActionType.BreakOnly;
                break;
            case PlayerType.Jiknyeo:
                skillType = ActionType.JumpOnly;
                break;
            default:
                skillType = ActionType.MoveOrInteract; // fallback
                break;
        }
        remainingUses = ownerInput.Config.MaxUses;
        UpdateUI();
    }

    public void UseSkill(GameObject caster, GameObject target) {
        if (remainingUses <= 0) {
            Debug.Log("남은 횟수 없음!");
            return;
        }
        var interaction = target.GetComponent<PlayerInteraction>();
        var pm = target.GetComponent<PlayerMovement>();
        var input = target.GetComponent<PlayerInput>();
        if (interaction == null || pm == null) return;
        if (interaction.TryAction(input.LastDir, skillType)) {
            remainingUses--; // 스킬 동작 성공 시에만 차감
            Debug.Log($"스킬 사용! 남은 횟수: {remainingUses}/{ownerInput.Config.MaxUses}");
            UpdateUI(); // UI 갱신
            anim.SetTrigger("Pulse"); // 애니메이션 트리거

            string motionTrigger = (skillType == ActionType.BreakOnly) ? "Punch" : "Jump";
            GameObject who = (caster == target) ? caster : target;
            var whoAnim = who ? who.GetComponentInChildren<Animator>(true) : null;
            if (whoAnim)
            {
                whoAnim.ResetTrigger(motionTrigger); // 중복 방지(선택)
                whoAnim.SetTrigger(motionTrigger);
            }
            else
            {
                Debug.LogWarning($"[{name}] '{who?.name}'에 Animator가 없습니다. '{motionTrigger}' 트리거 불가");
            }
        } else {
            Debug.Log("스킬 실패 또는 단순 이동 → 횟수 유지");
        }
    }

    void UpdateUI() {
        if (usesText != null)
            usesText.text = $"{remainingUses}";
    }
}