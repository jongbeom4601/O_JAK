using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [Header("�̵� ����")]
    public float gridSize = 1f;
    public float moveDuration = 0.1f;

    [Header("�Է� Ű ����")]
    public KeyCode upKey = KeyCode.W;
    public KeyCode downKey = KeyCode.S;
    public KeyCode leftKey = KeyCode.A;
    public KeyCode rightKey = KeyCode.D;

    [Header("UI ����")]
    public TMPro.TextMeshProUGUI moveCountText;
    public TMPro.TextMeshProUGUI inputCountText;

    [Header("ĳ���� �ִϸ��̼�")]
    public Animator characterAnim; // ĳ���� �ִϸ�����

    [Header("�ð� ����(��/��)")]
    public Transform visual;       // SpriteRenderer�� �޸� �ڽ�(����)
    private float baseScaleX = 1f; // ���� ������ X(+)

    private bool isMoving = false;
    private bool inputLocked = false;

    private int moveCount = 0;
    private int inputCount = 0;
    public bool hasKey = false; // ����

    public bool IsMoving => isMoving;
    public bool IsInputLocked => inputLocked;

    public Vector2 LastDir { get; private set; } = Vector2.down; // �⺻�� �ƹ��ų�


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

        // �ش� ĭ�� �ִ� ��� �浹ü ��������
        Collider2D[] hits = Physics2D.OverlapCircleAll(targetPos, 0.1f);

        if (hits.Length > 0)
        {
            Collider2D chosen = null;

            // 1����: �ڽ�
            foreach (var h in hits)
            {
                if (h.CompareTag("Obstacle_Box"))
                {
                    chosen = h;
                    break;
                }
            }

            // 2����: ����
            if (chosen == null)
            {
                foreach (var h in hits)
                {
                    if (h.CompareTag("Key"))
                    {
                        chosen = h;
                        break;
                    }
                }
            }

            // 3����: �μ��� �� (��ų �ʿ�)
            if (chosen == null)
            {
                foreach (var h in hits)
                {
                    if (h.CompareTag("Obstacle_Breakable"))
                    {
                        Debug.Log("��ų�� ��ȣ�ۿ��ؾ� �ϴ� ��ֹ��Դϴ�.");
                        return; // �׳� ���� (Interact ȣ�� X)
                    }
                }
            }

            // 4����: ����Ȧ (��ų �ʿ�)
            if (chosen == null)
            {
                foreach (var h in hits)
                {
                    if (h.CompareTag("Obstacle_JumpHole"))
                    {
                        Debug.Log("��ų�� ��ȣ�ۿ��ؾ� �ϴ� ��ֹ��Դϴ�.");
                        return; // �׳� ���� (Interact ȣ�� X)
                    }
                }
            }

            // 5����: ������ (���� ����)
            if (chosen == null)
            {
                foreach (var h in hits)
                {
                    if (h.CompareTag("Cloud")) continue;

                    IInteractable interactable = h.GetComponent<IInteractable>();
                    if (interactable != null)
                    {
                        chosen = h;
                        break;
                    }
                }
            }

            // 6����: ���� (������)
            if (chosen == null)
            {
                foreach (var h in hits)
                {
                    if (h.CompareTag("Cloud"))
                    {
                        chosen = h;
                        break;
                    }
                }
            }

            // ���������� ���õ� ������Ʈ�� ��ȣ�ۿ�
            if (chosen != null)
            {
                IInteractable interactable = chosen.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    if (chosen.CompareTag("Obstacle_Box"))
                    {
                        characterAnim.SetTrigger("Push"); // �ڽ� �б� �ִϸ��̼�
                    }

                    interactable.Interact(gameObject, dir);
                    return;
                }
            }
        }

        // �̵� ������ ��� (�ƹ� �浹ü ����)
        if (hits.Length == 0)
        {
            StartCoroutine(Move(transform, targetPos));
        }
    }


    IEnumerator Move(Transform obj, Vector2 target)
    {
        isMoving = true;

        Vector2 start = obj.position;
        float elapsed = 0f;

        characterAnim.SetTrigger("Move"); // ĳ���� �ִϸ��̼�
        while (elapsed < moveDuration)
        {
            
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / moveDuration);

            // ���� (EaseOutQuad)
            float easedT = 1f - (1f - t) * (1f - t);

            obj.position = Vector2.Lerp(start, target, easedT);
            yield return null;
        }

        // Move() �ڷ�ƾ ���κп� �߰�
        obj.position = target;
        isMoving = false;

        if (obj == transform)
        {
            moveCount++;
            UpdateMoveUI();

            // �̵� �Ϸ� �� ������ �ڵ� ȹ�� üũ
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 0.1f);
            foreach (var h in hits)
            {
                if (h.CompareTag("Item_Key"))
                {
                    var interactable = h.GetComponent<IInteractable>();
                    if (interactable != null)
                        interactable.Interact(gameObject, Vector2.zero);
                }
            }
        }

    }

    // JumpHole ���� �޼���... ����ȭ �� �����ؾ� ��
    IEnumerator Jump(Transform obj, Vector2 target)
    {
        isMoving = true;

        Vector2 start = obj.position;
        float elapsed = 0f;

        float JumpDuration = moveDuration *2; // ���� �ð��� ���� ����

        while (elapsed < moveDuration)
        {

            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / moveDuration);

            // ���� (EaseOutQuad)
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

    // JumpHole���� ���
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
                break; // ù ��°�� ó��
            }
        }
    }

    void SetFacingRight()
    {
        var s = visual.localScale;
        s.x = -baseScaleX; // ��û��� ������ = -1
        visual.localScale = s;

    }

    // ����: ������ +1 ����
    void SetFacingLeft()
    {
        var s = visual.localScale;
        s.x = baseScaleX; // ���� = +1
        visual.localScale = s;
    }

    void UpdateInputUI()
    {
        if (inputCountText != null)
            inputCountText.text = $"�Է� Ƚ��: {inputCount}";
    }

    void UpdateMoveUI()
    {
        if (moveCountText != null)
            moveCountText.text = $"�̵� Ƚ��: {moveCount}";
    }

    public void AcquireKey()
    {
        hasKey = true;
        Debug.Log(" ���� ȹ��!");
    }

    public void UseKey()
    {
        hasKey = false;
        Debug.Log(" ���� ���!");
    }

    public bool HasKey()
    {
        return hasKey;
    }

    public void TeleportTo(Vector2 targetPos)
    {
        transform.position = targetPos;
    }

    public void LockInput() => inputLocked = true;   //  ��� �޼���
    public void UnlockInput() => inputLocked = false;  //  ���� �޼���
}



