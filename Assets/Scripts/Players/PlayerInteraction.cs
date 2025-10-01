using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86.Avx;

public enum ActionType {
    MoveOrInteract, // 기본 입력
    JumpOnly,       // 직녀 스킬
    BreakOnly       // 견우 스킬   
}

[RequireComponent(typeof(PlayerMovement))]
public class PlayerInteraction : MonoBehaviour {


    public LayerMask preMoveMask; // 구름/아이템 레이어는 제외
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
            // 지난 칸의 애들에 Exit
            ProcessExit(_lastCell);
            // 새 칸의 Enter
            ProcessEnter(cell);
            // 캐시 갱신
            CacheCellObjects(cell);
            _lastCell = cell;
        } else {
            // 같은 칸이면 Stay
            ProcessStay(cell);
        }
    }

    public bool TryAction(Vector2 dir, ActionType actionType = ActionType.MoveOrInteract) {
        Vector2 targetPos = (Vector2)transform.position + dir;  // 플레이어가 보는 방향의 좌표
        var hits = Physics2D.OverlapCircleAll(targetPos, 0.1f, preMoveMask);

        // 오브젝트 확인
        List<IInteractable> candidates = new();
        foreach (var h in hits) {
            if (h.TryGetComponent<IInteractable>(out var obj)) {
                Debug.Log($"후보 발견: {h.gameObject.name}, Priority={obj.Priority}");
                candidates.Add(obj);
            }
        } // CanInteractWith(obj, actionType)

        // 우선순위가 가장 높은 오브젝트 선택
        IInteractable best = candidates.OrderByDescending(o => o.Priority).FirstOrDefault();
        if (best != null) {
            var comp = best as MonoBehaviour;
            if (comp != null)
            {
                Debug.Log($"TryAction: 선택된 대상 = {comp.gameObject.name} (Priority={best.Priority})");
            }
            // best만 CanInteractWith 검사
            if (CanInteractWith(best, actionType))
            {
                best.Interact(gameObject, dir);

                // 스킬 소모 판정
                if (actionType == ActionType.BreakOnly || actionType == ActionType.JumpOnly)
                {
                    return true;
                }
            }
            return false;
        }

        // 오브젝트인데 걸러졌을 때
        // 바위, 물
        if (hits.Length > 0) {
            return false;
        }
        // 오브젝트 없을 시 이동
        if (actionType == ActionType.MoveOrInteract) {
            movement.MoveTo(targetPos, dir);
            return false;
        }
        return false;
    }

    private bool CanInteractWith(IInteractable obj, ActionType type) {
        switch (type) {
            case ActionType.JumpOnly:
                return obj is JumpHole; // 직녀 스킬일 때만 점프홀 허용
            case ActionType.BreakOnly:
                return obj is Breakable; // 견우 스킬일 때만 부술 수 있는 벽 허용
            case ActionType.MoveOrInteract:
                // 일반 이동일 때는 점프홀/부술 벽은 막음
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
        // 지난 칸에 있던 애들 기준으로 Exit 호출
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