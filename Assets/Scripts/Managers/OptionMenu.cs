using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;
using System.Collections.Generic;

public class OptionMenu : MonoBehaviour
{
    [Header("UI Elements")]
    public Toggle fullscreenToggle;
    public TMP_Dropdown resolutionDropdown;
    public TMP_Dropdown framerateDropdown;

    [Header("Audio UI Elements")]
    public Button soundOnButton;
    public Button soundOffButton;
    public Slider bgmSlider;
    public Slider sfxSlider;

    [Header("Audio Mixer")]
    public AudioMixer audioMixer;

    private const string MASTER_VOL = "MasterVolume";
    private const string BGM_VOL = "BGMVolume";
    private const string SFX_VOL = "SFXVolume";

    private Vector2Int[] allowedResolutions = new Vector2Int[]
    {
        new Vector2Int(1280, 720),
        new Vector2Int(1600, 900),
        new Vector2Int(1920, 1080),
        new Vector2Int(2560, 1440),
        new Vector2Int(3840, 2160)
    };

    private int[] frameOptions = { 30, 60, 120, 144, -1 }; // -1 = Unlimited

    void Start()
    {
        SetupDisplayOptions();
        SetupAudioOptions();
    }

    // -------------------------------
    // 디스플레이 관련 설정
    // -------------------------------
    void SetupDisplayOptions()
    {
        if (fullscreenToggle != null)
        {
            fullscreenToggle.isOn = Screen.fullScreen;
            fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
        }

        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < allowedResolutions.Length; i++)
        {
            string option = allowedResolutions[i].x + " x " + allowedResolutions[i].y;
            options.Add(option);

            if (allowedResolutions[i].x == Screen.currentResolution.width &&
                allowedResolutions[i].y == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
        resolutionDropdown.onValueChanged.AddListener(SetResolution);

        framerateDropdown.ClearOptions();
        List<string> frameStrings = new List<string>();
        int currentFrameIndex = 0;

        for (int i = 0; i < frameOptions.Length; i++)
        {
            string label = (frameOptions[i] == -1) ? "Unlimited" : frameOptions[i] + " FPS";
            frameStrings.Add(label);

            if (frameOptions[i] == Application.targetFrameRate ||
                (frameOptions[i] == -1 && Application.targetFrameRate == -1))
            {
                currentFrameIndex = i;
            }
        }

        framerateDropdown.AddOptions(frameStrings);
        framerateDropdown.value = currentFrameIndex;
        framerateDropdown.RefreshShownValue();
        framerateDropdown.onValueChanged.AddListener(SetFramerate);
    }

    // -------------------------------
    // 오디오 관련 설정
    // -------------------------------
    void SetupAudioOptions()
    {
        if (audioMixer == null)
        {
            Debug.LogWarning("AudioMixer가 연결되지 않았습니다. 오디오 슬라이더는 작동하지 않습니다.");
            return;
        }

        if (bgmSlider)
        {
            bgmSlider.onValueChanged.AddListener(SetBGMVolume);
            audioMixer.GetFloat(BGM_VOL, out float bgmVal);
            bgmSlider.value = Mathf.Pow(10f, bgmVal / 20f);
        }

        if (sfxSlider)
        {
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
            audioMixer.GetFloat(SFX_VOL, out float sfxVal);
            sfxSlider.value = Mathf.Pow(10f, sfxVal / 20f);
        }

        if (soundOnButton) soundOnButton.onClick.AddListener(TurnSoundOn);
        if (soundOffButton) soundOffButton.onClick.AddListener(TurnSoundOff);

        // 초기 상태: 현재 볼륨이 -80dB 이하면 “꺼짐”으로 간주
        audioMixer.GetFloat(MASTER_VOL, out float masterVal);
        bool isMuted = masterVal <= -80f;
        UpdateSoundButtons(!isMuted);
    }

    // -------------------------------
    // 디스플레이 제어
    // -------------------------------
    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SetResolution(int resolutionIndex)
    {
        Vector2Int res = allowedResolutions[resolutionIndex];
        Screen.SetResolution(res.x, res.y, Screen.fullScreen);
    }

    public void SetFramerate(int frameIndex)
    {
        int target = frameOptions[frameIndex];
        Application.targetFrameRate = (target == -1) ? -1 : target;
    }

    // -------------------------------
    // 오디오 제어
    // -------------------------------
    public void SetBGMVolume(float value)
    {
        float dB = Mathf.Log10(Mathf.Max(value, 0.001f)) * 20f;
        audioMixer.SetFloat(BGM_VOL, dB);
    }

    public void SetSFXVolume(float value)
    {
        float dB = Mathf.Log10(Mathf.Max(value, 0.001f)) * 20f;
        audioMixer.SetFloat(SFX_VOL, dB);
    }

    public void TurnSoundOn()
    {
        audioMixer.SetFloat(MASTER_VOL, 0f); // 정상 볼륨
        UpdateSoundButtons(true);
    }

    public void TurnSoundOff()
    {
        audioMixer.SetFloat(MASTER_VOL, -80f); // 완전 음소거
        UpdateSoundButtons(false);
    }

    private void UpdateSoundButtons(bool soundOn)
    {
        if (soundOnButton) soundOnButton.gameObject.SetActive(!soundOn);
        if (soundOffButton) soundOffButton.gameObject.SetActive(soundOn);
    }
}
