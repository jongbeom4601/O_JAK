using UnityEngine;

public class Breakable : MonoBehaviour, IInteractable
{
    public int Priority => 50; // 2순위

    [Header("파괴 효과음")]
    public AudioClip breakSound;       // 파괴 시 재생할 효과음
    public AudioSource audioSource;    // AudioSource (없으면 자동 추가)
    public SpriteRenderer spriteRenderer; // 벽 이미지

    public void Interact(GameObject interactor, Vector2 direction)
    {
        Debug.Log("BreakableWall: 파괴됨!");
        PlayBreakSound();

        // 이미지 즉시 숨기기
        if (spriteRenderer != null)
            spriteRenderer.enabled = false;

        // 충돌 비활성화 (중복 파괴 방지)
        var collider = GetComponent<Collider2D>();
        if (collider != null)
            collider.enabled = false;

        // 사운드 재생 끝나면 오브젝트 삭제
        float delay = (breakSound != null) ? breakSound.length : 0f;
        Destroy(gameObject, delay);
    }

    private void PlayBreakSound()
    {
        if (breakSound == null) return;

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0f; // 2D 사운드
        }

        audioSource.PlayOneShot(breakSound);
    }
}
