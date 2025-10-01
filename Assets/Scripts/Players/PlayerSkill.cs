using UnityEngine;

public class PlayerSkill : MonoBehaviour, IAllySkill {
    private int remainingUses;
    private PlayerInput ownerInput;
    private ActionType skillType;

    void Awake() {
        ownerInput = GetComponent<PlayerInput>();
        if (ownerInput == null || ownerInput.Config == null) {
            Debug.LogError($"{name}: PlayerConfig ���� �� ��!");
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
            Debug.Log("���� Ƚ�� ����!");
            return;
        }
        var interaction = caster.GetComponent<PlayerInteraction>();
        var pm = caster.GetComponent<PlayerMovement>();
        var input = caster.GetComponent<PlayerInput>();
        if (interaction == null || pm == null) return;
        if (interaction.TryAction(input.LastDir, skillType)) {
            remainingUses--; // ��ų ���� ���� �ÿ��� ����
            Debug.Log($"��ų ���! ���� Ƚ��: {remainingUses}/{ownerInput.Config.MaxUses}");
        } else {
            Debug.Log("��ų ���� �Ǵ� �ܼ� �̵� �� Ƚ�� ����");
        }
    }
}