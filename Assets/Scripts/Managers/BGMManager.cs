using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class BGMManager : MonoBehaviour
{
    public static BGMManager Instance;

    public AudioSource audioSource;
    public float fadeDuration = 1.0f;

    private AudioClip currentClip;

    void Awake()
    {
        // 싱글톤 유지
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 씬 이름에 따라 자동으로 브금 결정
        var newClip = GetBGMForScene(scene.name);

        // 브금이 없으면 정지
        if (newClip == null)
        {
            StopBGM();
            return;
        }

        // 같은 클립이면 그대로 유지
        if (currentClip == newClip) return;

        // 다르면 교체
        StartCoroutine(FadeToNewClip(newClip));
    }

    private AudioClip GetBGMForScene(string sceneName)
    {
        // 여기에 씬별 브금 지정
        switch (sceneName)
        {
            case "TitleScene": return Resources.Load<AudioClip>("BGM/mainsoundtrack1");
            case "Story1Scene": return Resources.Load<AudioClip>("BGM/start_soundtrack1");
            case "Stage1Scene": return Resources.Load<AudioClip>("BGM/mainsoundtrack2");
            case "Stage2Scene": return Resources.Load<AudioClip>("BGM/mainsoundtrack2");
            case "Stage3Scene": return Resources.Load<AudioClip>("BGM/mainsoundtrack2");
            case "EndingScene": return Resources.Load<AudioClip>("BGM/ending_soundtrack");
            default: return null;
        }
    }

    private IEnumerator FadeToNewClip(AudioClip newClip)
    {
        if (audioSource.isPlaying)
        {
            // 페이드 아웃
            float startVol = audioSource.volume;
            for (float t = 0; t < fadeDuration; t += Time.unscaledDeltaTime)
            {
                audioSource.volume = Mathf.Lerp(startVol, 0, t / fadeDuration);
                yield return null;
            }
            audioSource.volume = 0;
            audioSource.Stop();
        }

        // 새 클립으로 교체
        audioSource.clip = newClip;
        audioSource.Play();
        currentClip = newClip;

        // 페이드 인
        float targetVol = 1f;
        for (float t = 0; t < fadeDuration; t += Time.unscaledDeltaTime)
        {
            audioSource.volume = Mathf.Lerp(0, targetVol, t / fadeDuration);
            yield return null;
        }
        audioSource.volume = targetVol;
    }

    public void StopBGM()
    {
        audioSource.Stop();
        currentClip = null;
    }

    public void PlayBGM(AudioClip clip)
    {
        if (clip == null || clip == currentClip) return;
        StartCoroutine(FadeToNewClip(clip));
    }
}
