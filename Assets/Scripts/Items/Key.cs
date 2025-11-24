using UnityEngine;


//public class Key : MonoBehaviour, IInteractable {
//    public int Priority => 40; // 3순위

//    public void Interact(GameObject interactor, Vector2 direction) {
//        var input = interactor.GetComponent<PlayerInput>();
//        var movement = interactor.GetComponent<PlayerMovement>();

//        if (input != null && movement != null) {
//            if (!input.Config.hasKey) {
//                input.Config.hasKey = true;
//                Debug.Log("열쇠 획득!");
//                Destroy(gameObject);
//            }

//            movement.MoveTo(transform.position);
//        }
//    }
//}


public class Key : MonoBehaviour, IOnEnter {

    [Header("획득 효과음")]
    public AudioClip pickupSound;       // 재생할 효과음 클립
    public AudioSource audioSource;     // 효과음을 재생할 AudioSource (없으면 자동 생성)
    public SpriteRenderer spriteRenderer;  //  이미지 투명화용

    public void OnEnter(GameObject interactor, Vector2 dir) {
        var input = interactor.GetComponent<PlayerInput>();

        if (!input.Config.hasKey) {
            input.Config.hasKey = true;
            Debug.Log("열쇠 획득!");
            PlayPickupSound();
            //input.Config.Animator.SetTrigger("Pick");
            //  이미지 바로 숨기기
            if (spriteRenderer != null)
                spriteRenderer.enabled = false;

            //  충돌 방지 (다시 주워지는 것 방지)
            var collider = GetComponent<Collider2D>();
            if (collider != null)
                collider.enabled = false;

            Destroy(gameObject, pickupSound.length);
        }
    }

    private void PlayPickupSound()
    {
        if (pickupSound == null)
            return;

        // AudioSource가 없다면 임시로 생성
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 1f; // 3D 사운드 (원거리 감쇠 적용)
        }

        audioSource.PlayOneShot(pickupSound);
    }
}