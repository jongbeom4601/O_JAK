using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class OptionMenu : MonoBehaviour
{
    [Header("UI Elements")]
    public Toggle fullscreenToggle;
    public TMP_Dropdown resolutionDropdown;
    public TMP_Dropdown framerateDropdown;

    // 16:9 해상도 목록 직접 정의
    private Vector2Int[] allowedResolutions = new Vector2Int[]
    {
        new Vector2Int(1280, 720),   // HD
        new Vector2Int(1600, 900),   // HD+
        new Vector2Int(1920, 1080),  // Full HD
        new Vector2Int(2560, 1440),  // QHD
        new Vector2Int(3840, 2160)   // 4K UHD
    };

    private int[] frameOptions = { 30, 60, 120, 144, -1 }; // -1 = Unlimited

    void Start()
    {
        // ----- 전체화면 토글 -----
        if (fullscreenToggle != null)
        {
            fullscreenToggle.isOn = Screen.fullScreen;
            fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
        }

        // ----- 해상도 드롭다운 -----
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

        // ----- 프레임 드롭다운 -----
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
}
