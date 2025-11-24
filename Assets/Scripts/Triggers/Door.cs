using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    public int Priority => 100;

    [Header("애니메이터")]
    public Animator animator;

    [Header("문 매니저")]
    public DoorManager doorManager;

    [Header("문 효과음")]
    public AudioClip openSound;       // 문 열릴 때 소리
    public AudioClip lockedSound;     // 열쇠 없을 때 소리 (선택)
    public AudioSource audioSource;  // 오디오 소스 (없으면 자동 추가)

    public void Interact(GameObject interactor, Vector2 direction)
    {
        var input = interactor.GetComponent<PlayerInput>();
        var movement = interactor.GetComponent<PlayerMovement>();

        if (input != null && movement != null)
        {
            if (input.Config.hasKey)
            {
                input.Config.hasKey = false; //  열쇠 사용
                animator.SetTrigger("Open");
                movement.MoveTo(transform.position);

                //  입력 잠금
                input.SetInputLocked(true);

                //  문 열리는 소리 재생
                PlaySound(openSound);

                // 문 매니저 알림
                if (doorManager != null)
                    doorManager.DoorOpened();
            }
            else
            {
                Debug.Log("열쇠가 필요합니다!");
                //  열쇠 없을 때 소리
                PlaySound(lockedSound);
            }
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip == null) return;

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0f; // 2D 사운드 (거리 무시)
        }

        audioSource.PlayOneShot(clip);
    }
}
