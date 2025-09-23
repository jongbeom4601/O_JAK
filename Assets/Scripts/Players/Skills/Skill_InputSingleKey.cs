using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SkillInputSingleKey : MonoBehaviour
{
    [Header("입력")]
    public KeyCode skillKey = KeyCode.Q;
    [Tooltip("이 시간(초) 이상 누르면 '준비 완료' 상태")]
    public float holdThreshold = 1f;

    [Header("참조")]
    public MonoBehaviour skillBehaviour;   // IAllySkill 구현 컴포넌트
    public GameObject partner;             // 상대 플레이어
    public Image holdGauge;                // 게이지 Image (Type=Filled)
    public CanvasGroup gaugeRoot;          // 게이지 루트(CanvasGroup, 페이드용)

    [Header("플레이어 이동키 참조(캔슬에 사용)")]
    public PlayerMovement ownerMovement;
    private bool canCancelUp, canCancelDown, canCancelLeft, canCancelRight; // 홀드 시작 시점 스냅샷

    [Header("쿨다운")]
    public float cooldown = 0.3f;

    [Header("UI")]
    [Tooltip("홀드 시작 후 이 시간이 지나면 게이지를 보여줌(탭 시 플리커 방지)")]
    public float gaugeShowDelay = 0.3f;
    public float fadeDuration = 0.2f;
    public Color fillColor = Color.white;
    public Color readyColor = Color.green;

    private IAllySkill _skill;
    private float _pressedAt = -1f;
    private float _lastUseTime = -999f;
    private bool _ready = false;       // 임계치 도달 여부(다 찬 상태)
    private bool _canceled = false;    // 홀드 중 방향키로 캔슬 여부
    private bool _gaugeShown = false;  // 현재 게이지가 보이는 중인지
    private Coroutine _fadeCo;

    void Awake()
    {
        _skill = skillBehaviour as IAllySkill;
        if (_skill == null)
            Debug.LogError("[SkillInputSingleKey] skillBehaviour는 IAllySkill 구현이 필요합니다.");

        // 게이지 기본 설정: 좌->우로 차오르게
        if (holdGauge != null)
        {
            holdGauge.type = Image.Type.Filled;
            holdGauge.fillMethod = Image.FillMethod.Horizontal;
            holdGauge.fillOrigin = (int)Image.OriginHorizontal.Left;
            holdGauge.fillAmount = 0f;
            holdGauge.color = fillColor;
        }

        // 시작은 숨김
        SetGaugeVisible(false, instant: true);
    }

    void Update()
    {
        // 쿨다운 중이면 종료
        if (Time.time - _lastUseTime < cooldown) { HideAndResetGauge(); return; }

        //  캔슬 상태에서 스킬 키 계속 누르고 있으면 아무 것도 안 함
        if (_canceled && Input.GetKey(skillKey))
            return;

        //  스킬 키를 떼면 캔슬 잠금 해제
        if (_canceled && Input.GetKeyUp(skillKey))
        {
            _canceled = false;
            return;
        }

        // 캔슬 잠금: 키를 뗄 때까지 아무 동작 안 함
        if (_canceled && Input.GetKey(skillKey)) return;
        if (_canceled && Input.GetKeyUp(skillKey)) { _canceled = false; return; }


        // 쿨다운 동안 입력 무시 + UI 숨김
        if (Time.time - _lastUseTime < cooldown)
        {
            HideAndResetGauge();
            return;
        }

        // KeyDown: 타임스탬프만 기록(탭/홀드 아직 모름)
        if (Input.GetKeyDown(skillKey))
        {
            _pressedAt = Time.time;
            _ready = false;
            _canceled = false;
            _gaugeShown = false;

            if (holdGauge != null)
            {
                holdGauge.fillAmount = 0f;
                holdGauge.color = fillColor;
            }
            SetGaugeVisible(false, instant: true); // 탭이면 안 보여야 함

            // "그 순간 이미 눌려 있던" 방향키는 캔슬 후보에서 제외
            // 조작이 부드러워야해!!!!
            if (ownerMovement != null)
            {
                canCancelUp = !Input.GetKey(ownerMovement.upKey);
                canCancelDown = !Input.GetKey(ownerMovement.downKey);
                canCancelLeft = !Input.GetKey(ownerMovement.leftKey);
                canCancelRight = !Input.GetKey(ownerMovement.rightKey);
            }
            else
            {
                // fallback (원하면 삭제해도 됨)
                canCancelUp = !Input.GetKey(KeyCode.W);
                canCancelDown = !Input.GetKey(KeyCode.S);
                canCancelLeft = !Input.GetKey(KeyCode.A);
                canCancelRight = !Input.GetKey(KeyCode.D);
            }
        }

        // Key 유지 중: 홀드 처리
        if (_pressedAt > 0f && Input.GetKey(skillKey))
        {
            // 방향키 입력으로 캔슬
            if (IsMoveCancelTriggered())
            {
                _canceled = true;          // 취소 잠금
                _pressedAt = -1f;          // 이번 홀드 종료
                HideAndResetGauge(keepCanceled: true);
                return;
            }

            float held = Time.time - _pressedAt;

            // 지연 후 처음 보일 때 0에서 시작
            if (held >= gaugeShowDelay && !_gaugeShown)
            {
                SetGaugeVisible(true);
                _gaugeShown = true;
                if (holdGauge != null) holdGauge.fillAmount = 0f; // ← 0에서 시작
            }

            // 게이지 채우기 (표시용 진행률로)
            if (_gaugeShown && holdGauge != null)
            {
                float fill01 = GetDisplayFill(held);
                holdGauge.fillAmount = fill01;

                // 준비 완료 판정은 '실제 held'로 별도 계산 (보여주는 것과 분리)
                bool readyNow = held >= holdThreshold;
                holdGauge.color = readyNow ? readyColor : fillColor;
                _ready = readyNow; // 키를 뗄 때(_ready)로 분기
            }
        }

        // KeyUp: 분기(캔슬/홀드완료/탭)
        if (Input.GetKeyUp(skillKey) && _pressedAt > 0f)
        {
            float held = Time.time - _pressedAt;
            _pressedAt = -1f;

            if (_canceled)
            {
                // 아무 것도 하지 않음
                HideAndResetGauge();
                return;
            }

            if (_ready || held >= holdThreshold)
            {
                // 홀드 완료: 키를 뗀 시점에 상대에게 시전
                if (partner != null) _skill.UseOnAlly(gameObject, partner);
                else _skill.UseOnSelf(gameObject); // 파트너 없으면 자기 시전
            }
            else
            {
                // 탭: 자기 시전
                _skill.UseOnSelf(gameObject);
            }

            _lastUseTime = Time.time;
            HideAndResetGauge();
        }
    }

    // 방향키 입력 감지(키보드 축/화살표)
    private bool IsMoveCancelTriggered()
    {
        if (ownerMovement != null)
        {
            if (canCancelUp && Input.GetKeyDown(ownerMovement.upKey)) return true;
            if (canCancelDown && Input.GetKeyDown(ownerMovement.downKey)) return true;
            if (canCancelLeft && Input.GetKeyDown(ownerMovement.leftKey)) return true;
            if (canCancelRight && Input.GetKeyDown(ownerMovement.rightKey)) return true;
            return false;
        }

        // fallback (원하면 삭제)
        if (canCancelUp && Input.GetKeyDown(KeyCode.W)) return true;
        if (canCancelDown && Input.GetKeyDown(KeyCode.S)) return true;
        if (canCancelLeft && Input.GetKeyDown(KeyCode.A)) return true;
        if (canCancelRight && Input.GetKeyDown(KeyCode.D)) return true;
        return false;
    }

    // === UI 유틸 ===
    private void SetGaugeVisible(bool visible, bool instant = false)
    {
        //  항상 Image.enabled도 함께 제어
        if (holdGauge != null) holdGauge.enabled = visible;

        if (gaugeRoot == null)
            return;

        if (_fadeCo != null) StopCoroutine(_fadeCo);

        if (instant || fadeDuration <= 0f)
        {
            gaugeRoot.alpha = visible ? 1f : 0f;
            gaugeRoot.interactable = visible;
            gaugeRoot.blocksRaycasts = visible;
        }
        else
        {
            _fadeCo = StartCoroutine(FadeGauge(visible ? 1f : 0f));
        }
    }

    private IEnumerator FadeGauge(float target)
    {
        float start = gaugeRoot.alpha;
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            gaugeRoot.alpha = Mathf.Lerp(start, target, t / fadeDuration);
            yield return null;
        }
        gaugeRoot.alpha = target;
        bool on = target > 0.99f;
        gaugeRoot.interactable = on;
        gaugeRoot.blocksRaycasts = on;
        _fadeCo = null;
    }

    private void HideAndResetGauge(bool keepCanceled = false)
    {
        if (holdGauge != null)
        {
            holdGauge.fillAmount = 0f;
            holdGauge.color = fillColor;
            //  gaugeRoot가 없을 때만 직접 끔
            if (gaugeRoot == null)
                holdGauge.enabled = false;
        }
        SetGaugeVisible(false);
        _gaugeShown = false;
        _ready = false;
        if (!keepCanceled)
            _canceled = false;   // ← 기본은 초기화, '캔슬 시'에는 유지
    }

    // 헬퍼: 표시용 진행률(0~1)
    // gaugeShowDelay 이후 0에서 시작해 holdThreshold에서 1이 되게 매핑
    float GetDisplayFill(float held)
    {
        // 안전장치: delay가 임계보다 크거나 같으면 기존 방식으로
        if (holdThreshold <= gaugeShowDelay)
            return Mathf.Clamp01(held / Mathf.Max(0.0001f, holdThreshold));

        float visibleHeld = Mathf.Max(0f, held - gaugeShowDelay);
        float denom = holdThreshold - gaugeShowDelay;
        return Mathf.Clamp01(visibleHeld / Mathf.Max(0.0001f, denom));
    }

}
