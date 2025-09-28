using UnityEngine;
using UnityEngine.UI;

public class GeonwooSkill : MonoBehaviour, IAllySkill
{

    [Header("��ų ����")]
    public int maxUses = 3;   // �ִ� ��� ���� Ƚ��
    private int remainingUses; // ���� Ƚ��

    [Header("UI ����")]
    public Text usesText;
    public Animator anim;

    [Header("ĳ���� �ִϸ��̼�")]
    public Animator casterAnim; // ĳ���� �ִϸ�����
    public Animator allyAnim;

    void Awake()
    {
        remainingUses = maxUses; // ���� �� Ǯ�� ä��
    }

    // ���������� ui ����
    void OnEnable()
    {
        UpdateUI();
    }

    // ���� ����
    void Start()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        if (usesText != null)
            usesText.text = $"{remainingUses}";
    }

    public void UseOnSelf(GameObject caster)
    {
        if (remainingUses <= 0)
        {
            anim.SetTrigger("Pulse"); // �ִϸ��̼� Ʈ����
            Debug.Log("���� Ƚ�� ����!");
            return;
        }

        var pm = caster.GetComponent<PlayerMovement>();
        if (!pm) return;

        Vector2 playerPos = pm.transform.position;
        Vector2 targetPos = playerPos + pm.LastDir;

        Collider2D[] hits = Physics2D.OverlapCircleAll(targetPos, 0.1f);
        Collider2D chosen = null;

        // 1����: Breakable
        foreach (var h in hits)
        {
            if (h.CompareTag("Obstacle_Breakable"))
            {
                chosen = h;
                break;
            }
        }

        // Breakable ������ ù ��° �浹ü�� ����
        if (chosen == null && hits.Length > 0)
            chosen = hits[0];

        if (chosen != null)
        {
            IInteractable interactable = chosen.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactable.Interact(caster, pm.LastDir);
                remainingUses--;
                UpdateUI();
                anim.SetTrigger("Pulse");
                casterAnim.SetTrigger("Skill");
                Debug.Log($"��ų ���! ���� Ƚ��: {remainingUses}/{maxUses}");
            }
        }
    }

    public void UseOnAlly(GameObject caster, GameObject ally)
    {
        if (remainingUses <= 0)
        {
            Debug.Log("���� Ƚ�� ����!");
            return;
        }

        var pm = ally.GetComponent<PlayerMovement>();
        if (!pm) return;

        Vector2 playerPos = pm.transform.position;
        Vector2 targetPos = playerPos + pm.LastDir;

        Collider2D[] hits = Physics2D.OverlapCircleAll(targetPos, 0.1f);
        Collider2D chosen = null;

        // 1����: Breakable
        foreach (var h in hits)
        {
            if (h.CompareTag("Obstacle_Breakable"))
            {
                chosen = h;
                break;
            }
        }

        // Breakable ������ ù ��° �浹ü�� ����
        if (chosen == null && hits.Length > 0)
            chosen = hits[0];

        if (chosen != null)
        {
            IInteractable interactable = chosen.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactable.Interact(caster, pm.LastDir);
                remainingUses--;
                UpdateUI();
                anim.SetTrigger("Pulse");
                casterAnim.SetTrigger("Skill");
                Debug.Log($"��ų ���! ���� Ƚ��: {remainingUses}/{maxUses}");
            }
        }
    }

    
}
