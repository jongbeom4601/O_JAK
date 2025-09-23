using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [Header("이동 설정")]
    public float gridSize = 1f;
    public float moveDuration = 0.1f;

    [Header("입력 키 설정")]
    public KeyCode upKey = KeyCode.W;
    public KeyCode downKey = KeyCode.S;
    public KeyCode leftKey = KeyCode.A;
    public KeyCode rightKey = KeyCode.D;

    [Header("UI 연결")]
    public TMPro.TextMeshProUGUI moveCountText;
    public TMPro.TextMeshProUGUI inputCountText;

    [Header("캐릭터 애니메이션")]
    public Animator characterAnim; // 캐릭터 애니메이터

    [Header("시각 반전(좌/우)")]
    public Transform visual;       // SpriteRenderer가 달린 자식(권장)
    private float baseScaleX = 1f; // 기준 스케일 X(+)

    private bool isMoving = false;
    private bool inputLocked = false;

    private int moveCount = 0;
    private int inputCount = 0;
    public bool hasKey = false; // 열쇠

    public bool IsMoving => isMoving;
    public bool IsInputLocked => inputLocked;

    public Vector2 LastDir { get; private set; } = Vector2.down; // 기본값 아무거나


    void Update()
    {
        if (inputLocked || isMoving) return;

        Vector2 input = GetInputDirection();
        if (input != Vector2.zero)
        {
            inputCount++;
            UpdateInputUI();

            TryMoveOrInteract(input);
        }
    }

    Vector2 GetInputDirection()
    {
        if (Input.GetKeyDown(upKey)) { LastDir = Vector2.up; return Vector2.up; }
        if (Input.GetKeyDown(downKey)) { LastDir = Vector2.down; return Vector2.down; }
        if (Input.GetKeyDown(leftKey)) { LastDir = Vector2.left; SetFacingLeft(); return Vector2.left; }
        if (Input.GetKeyDown(rightKey)) { LastDir = Vector2.right; SetFacingRight(); return Vector2.right; }
        return Vector2.zero;
    }

    void TryMoveOrInteract(Vector2 dir)
    {
        Vector2 playerPos = transform.position;
        Vector2 targetPos = playerPos + dir;

        Collider2D hit = Physics2D.OverlapCircle(targetPos, 0.1f);
        if (hit != null)
        {
            // 견우, 직녀 스킬 상호작용 장애물은 제외
            if (hit.CompareTag("Obstacle_JumpHole") || hit.CompareTag("Obstacle_Breakable"))
            {
                Debug.Log("스킬로 상호작용해야하는 장애물입니다.");
                return; // 그냥 멈춤
            }

            IInteractable interactable = hit.GetComponent<IInteractable>();
            if (interactable != null)
            {
                if (hit.CompareTag("Obstacle_Box"))
                    {
                    characterAnim.SetTrigger("Push"); // 캐릭터 애니메이션
                }
                interactable.Interact(gameObject, dir);
                return;
            }
        }

        // 이동 가능한 경우 (hit == null)
        if (hit == null)
        {
            StartCoroutine(Move(transform, targetPos));
        }
    }

    IEnumerator Move(Transform obj, Vector2 target)
    {
        isMoving = true;

        Vector2 start = obj.position;
        float elapsed = 0f;

        characterAnim.SetTrigger("Move"); // 캐릭터 애니메이션
        while (elapsed < moveDuration)
        {
            
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / moveDuration);

            // 감속 (EaseOutQuad)
            float easedT = 1f - (1f - t) * (1f - t);

            obj.position = Vector2.Lerp(start, target, easedT);
            yield return null;
        }

        obj.position = target;
        isMoving = false;

        if (obj == transform)
        {
            moveCount++;
            UpdateMoveUI();
        }
    }
    
    // JumpHole 전용 메서드... 최적화 시 수정해야 함
    IEnumerator Jump(Transform obj, Vector2 target)
    {
        isMoving = true;

        Vector2 start = obj.position;
        float elapsed = 0f;

        float JumpDuration = moveDuration *2; // 점프 시간만 따로 설정

        while (elapsed < moveDuration)
        {

            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / moveDuration);

            // 감속 (EaseOutQuad)
            float easedT = 1f - (1f - t) * (1f - t);

            obj.position = Vector2.Lerp(start, target, easedT);
            yield return null;
        }

        obj.position = target;
        isMoving = false;

    }
    public void JumpTo(Vector2 targetPos)
    {
        if (!isMoving)
        {
            StartCoroutine(Jump(transform, targetPos));
        }
    }

    public void MoveTo(Vector2 targetPos)
    {
        if (!isMoving)
        {
            StartCoroutine(Move(transform, targetPos));
        }
    }

    // JumpHole에서 사용
    public void TryInteractOnly(Vector2 dir)
    {

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 0.1f);

        foreach (var hit in hits)
        {
            if (hit.gameObject == gameObject) continue;

            IInteractable interactable = hit.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactable.Interact(gameObject, dir);
                break; // 첫 번째만 처리
            }
        }
    }

    void SetFacingRight()
    {
        var s = visual.localScale;
        s.x = -baseScaleX; // 요청대로 오른쪽 = -1
        visual.localScale = s;

    }

    // 왼쪽: 스케일 +1 유지
    void SetFacingLeft()
    {
        var s = visual.localScale;
        s.x = baseScaleX; // 왼쪽 = +1
        visual.localScale = s;
    }

    void UpdateInputUI()
    {
        if (inputCountText != null)
            inputCountText.text = $"입력 횟수: {inputCount}";
    }

    void UpdateMoveUI()
    {
        if (moveCountText != null)
            moveCountText.text = $"이동 횟수: {moveCount}";
    }

    public void AcquireKey()
    {
        hasKey = true;
        Debug.Log(" 열쇠 획득!");
    }

    public void UseKey()
    {
        hasKey = false;
        Debug.Log(" 열쇠 사용!");
    }

    public bool HasKey()
    {
        return hasKey;
    }

    public void TeleportTo(Vector2 targetPos)
    {
        transform.position = targetPos;
    }

    public void LockInput() => inputLocked = true;   //  잠금 메서드
    public void UnlockInput() => inputLocked = false;  //  해제 메서드
}



