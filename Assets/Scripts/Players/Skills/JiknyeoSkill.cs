using UnityEngine;
using UnityEngine.UI;

public class JiknyeoSkill : MonoBehaviour, IAllySkill
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

        Collider2D hit = Physics2D.OverlapCircle(targetPos, 0.1f);

        if (hit != null && hit.CompareTag("Obstacle_JumpHole"))
        {
            IInteractable interactable = hit.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactable.Interact(caster, pm.LastDir);
                remainingUses--; // 성공 시 횟수 차감
                Debug.Log($"스킬 사용! 남은 횟수: {remainingUses}/{maxUses}");
                UpdateUI(); // UI 갱신
                anim.SetTrigger("Pulse"); // 애니메이션 트리거
                casterAnim.SetTrigger("Jump");
                return;
            }
        }
    }

    public void UseOnAlly(GameObject caster, GameObject ally)
    {

        if (remainingUses <= 0)
        {
            anim.SetTrigger("Pulse"); // 애니메이션 트리거
            Debug.Log("남은 횟수 없음!");
            return;
        }

        var pm = ally.GetComponent<PlayerMovement>();
        if (!pm) return;

        Vector2 playerPos = pm.transform.position;
        Vector2 targetPos = playerPos + pm.LastDir;

        Collider2D hit = Physics2D.OverlapCircle(targetPos, 0.1f);

        if (hit != null && hit.CompareTag("Obstacle_JumpHole"))
        {
            IInteractable interactable = hit.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactable.Interact(ally, pm.LastDir);
                remainingUses--; // 성공 시 횟수 차감
                Debug.Log($"스킬 사용! 남은 횟수: {remainingUses}/{maxUses}");
                UpdateUI(); // UI 갱신
                anim.SetTrigger("Pulse"); // 애니메이션 트리거
                allyAnim.SetTrigger("Jump");
                return;
            }
        }
    }


}
