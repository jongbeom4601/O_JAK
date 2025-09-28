using UnityEngine;
using UnityEngine.UI;

public class GeonwooSkill : MonoBehaviour, IAllySkill
{

    [Header("스킬 제한")]
    public int maxUses = 3;   // 최대 사용 가능 횟수
    private int remainingUses; // 남은 횟수

    [Header("UI 연결")]
    public Text usesText;
    public Animator anim;

    [Header("캐릭터 애니메이션")]
    public Animator casterAnim; // 캐릭터 애니메이터
    public Animator allyAnim;

    void Awake()
    {
        remainingUses = maxUses; // 시작 시 풀로 채움
    }

    // 안전빵으로 ui 갱신
    void OnEnable()
    {
        UpdateUI();
    }

    // 위와 동일
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
            anim.SetTrigger("Pulse"); // 애니메이션 트리거
            Debug.Log("남은 횟수 없음!");
            return;
        }

        var pm = caster.GetComponent<PlayerMovement>();
        if (!pm) return;

        Vector2 playerPos = pm.transform.position;
        Vector2 targetPos = playerPos + pm.LastDir;

        Collider2D[] hits = Physics2D.OverlapCircleAll(targetPos, 0.1f);
        Collider2D chosen = null;

        // 1순위: Breakable
        foreach (var h in hits)
        {
            if (h.CompareTag("Obstacle_Breakable"))
            {
                chosen = h;
                break;
            }
        }

        // Breakable 없으면 첫 번째 충돌체라도 선택
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
                Debug.Log($"스킬 사용! 남은 횟수: {remainingUses}/{maxUses}");
            }
        }
    }

    public void UseOnAlly(GameObject caster, GameObject ally)
    {
        if (remainingUses <= 0)
        {
            Debug.Log("남은 횟수 없음!");
            return;
        }

        var pm = ally.GetComponent<PlayerMovement>();
        if (!pm) return;

        Vector2 playerPos = pm.transform.position;
        Vector2 targetPos = playerPos + pm.LastDir;

        Collider2D[] hits = Physics2D.OverlapCircleAll(targetPos, 0.1f);
        Collider2D chosen = null;

        // 1순위: Breakable
        foreach (var h in hits)
        {
            if (h.CompareTag("Obstacle_Breakable"))
            {
                chosen = h;
                break;
            }
        }

        // Breakable 없으면 첫 번째 충돌체라도 선택
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
                Debug.Log($"스킬 사용! 남은 횟수: {remainingUses}/{maxUses}");
            }
        }
    }

    
}
