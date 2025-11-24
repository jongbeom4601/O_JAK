// SceneVisitTracker.cs
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneVisitTracker
{
    public static int PrevActiveSceneIndex { get; private set; } = -1;
    private static bool _hooked;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AutoHook() => Hook();

    public static void Hook()
    {
        if (_hooked) return;
        _hooked = true;

        SceneManager.activeSceneChanged += (prev, next) =>
        {
            PrevActiveSceneIndex = prev.IsValid() ? prev.buildIndex : -1;
        };
    }

    public static void EnsureHooked() => Hook();

    /// <summary>
    /// 주어진 sceneIndex가 '직전에 활성화되어 있던 씬'과 같으면 즉시 재시작으로 간주.
    /// </summary>
    public static bool IsImmediateReloadOf(int sceneIndex)
    {
        return sceneIndex == PrevActiveSceneIndex;
    }
}
