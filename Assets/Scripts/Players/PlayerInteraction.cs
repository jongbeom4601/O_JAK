using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86.Avx;

public enum ActionType {
    MoveOrInteract, // �⺻ �Է�
    JumpOnly,       // ���� ��ų
    BreakOnly       // �߿� ��ų   
}

[RequireComponent(typeof(PlayerMovement))]
public class PlayerInteraction : MonoBehaviour {


    public LayerMask preMoveMask; // ����/������ ���̾�� ����
    public LayerMask onEnterMask;
    private PlayerMovement movement;

    private Vector2Int _lastCell;
    private bool _hadCell;
    private List<Component> _lastCellObjects = new();

    void Awake() {
        movement = GetComponent<PlayerMovement>();
    }

    void Update() {
        if (movement.IsMoving) return;

        Vector2 pos = transform.position;
        Vector2Int cell = new Vector2Int(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y));

        if (!_hadCell) {
            _hadCell = true;
            _lastCell = cell;
            ProcessEnter(cell);
            CacheCellObjects(cell);
            return;
        }
        if (cell != _lastCell) {
            // ���� ĭ�� �ֵ鿡 Exit
            ProcessExit(_lastCell);
            // �� ĭ�� Enter
            ProcessEnter(cell);
            // ĳ�� ����
            CacheCellObjects(cell);
            _lastCell = cell;
        } else {
            // ���� ĭ�̸� Stay
            ProcessStay(cell);
        }
    }

    public bool TryAction(Vector2 dir, ActionType actionType = ActionType.MoveOrInteract) {
        Vector2 targetPos = (Vector2)transform.position + dir;  // �÷��̾ ���� ������ ��ǥ
        var hits = Physics2D.OverlapCircleAll(targetPos, 0.1f, preMoveMask);

        // ������Ʈ Ȯ��
        List<IInteractable> candidates = new();
        foreach (var h in hits) {
            if (h.TryGetComponent<IInteractable>(out var obj)) {
                Debug.Log($"�ĺ� �߰�: {h.gameObject.name}, Priority={obj.Priority}");
                candidates.Add(obj);
            }
        } // CanInteractWith(obj, actionType)

        // �켱������ ���� ���� ������Ʈ ����
        IInteractable best = candidates.OrderByDescending(o => o.Priority).FirstOrDefault();
        if (best != null) {
            var comp = best as MonoBehaviour;
            if (comp != null)
            {
                Debug.Log($"TryAction: ���õ� ��� = {comp.gameObject.name} (Priority={best.Priority})");
            }
            // best�� CanInteractWith �˻�
            if (CanInteractWith(best, actionType))
            {
                best.Interact(gameObject, dir);

                // ��ų �Ҹ� ����
                if (actionType == ActionType.BreakOnly || actionType == ActionType.JumpOnly)
                {
                    return true;
                }
            }
            return false;
        }

        // ������Ʈ�ε� �ɷ����� ��
        // ����, ��
        if (hits.Length > 0) {
            return false;
        }
        // ������Ʈ ���� �� �̵�
        if (actionType == ActionType.MoveOrInteract) {
            movement.MoveTo(targetPos, dir);
            return false;
        }
        return false;
    }

    private bool CanInteractWith(IInteractable obj, ActionType type) {
        switch (type) {
            case ActionType.JumpOnly:
                return obj is JumpHole; // ���� ��ų�� ���� ����Ȧ ���
            case ActionType.BreakOnly:
                return obj is Breakable; // �߿� ��ų�� ���� �μ� �� �ִ� �� ���
            case ActionType.MoveOrInteract:
                // �Ϲ� �̵��� ���� ����Ȧ/�μ� ���� ����
                if (obj is JumpHole) return false;
                if (obj is Breakable) return false;
                return true;
            default:
                return true;
        }
    }

    void ProcessEnter(Vector2Int cell) {
        foreach (var hit in OverlapAt(cell))
            if (hit.TryGetComponent<IOnEnter>(out var enter))
                enter.OnEnter(gameObject, Vector2.zero);
    }

    void ProcessStay(Vector2Int cell) {
        foreach (var hit in OverlapAt(cell))
            if (hit.TryGetComponent<IOnStay>(out var stay))
                stay.OnStay(gameObject);
    }

    void ProcessExit(Vector2Int prevCell) {
        // ���� ĭ�� �ִ� �ֵ� �������� Exit ȣ��
        foreach (var comp in _lastCellObjects)
            if (comp != null && comp.TryGetComponent<IOnExit>(out var exit))
                exit.OnExit(gameObject);
    }

    void CacheCellObjects(Vector2Int cell) {
        _lastCellObjects.Clear();
        foreach (var hit in OverlapAt(cell))
            _lastCellObjects.Add(hit);
    }

    Collider2D[] OverlapAt(Vector2Int cell) {
        Vector2 p = new(cell.x, cell.y);
        return Physics2D.OverlapCircleAll(p, 0.12f, onEnterMask);
    }
}