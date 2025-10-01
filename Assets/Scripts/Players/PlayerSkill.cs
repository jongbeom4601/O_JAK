using UnityEngine;

public class PlayerSkill : MonoBehaviour, IAllySkill {
    private int remainingUses;
    private PlayerInput ownerInput;
    private ActionType skillType;

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
    }

    public void UseSkill(GameObject caster) {
        if (remainingUses <= 0) {
            Debug.Log("남은 횟수 없음!");
            return;
        }
        var interaction = caster.GetComponent<PlayerInteraction>();
        var pm = caster.GetComponent<PlayerMovement>();
        var input = caster.GetComponent<PlayerInput>();
        if (interaction == null || pm == null) return;
        if (interaction.TryAction(input.LastDir, skillType)) {
            remainingUses--; // 스킬 동작 성공 시에만 차감
            Debug.Log($"스킬 사용! 남은 횟수: {remainingUses}/{ownerInput.Config.MaxUses}");
        } else {
            Debug.Log("스킬 실패 또는 단순 이동 → 횟수 유지");
        }
    }
}