using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class StageIntroRunner : MonoBehaviour
{
    [SerializeField] private StageIntroUI introUI;
    [SerializeField] private StageIntroData data;

    [SerializeField] private bool waitForDialogueEnd = true;
    [SerializeField] private float delayBeforeShow = 0.75f;
    [SerializeField] private bool useRealtimeWait = false;

    // 직전에 "로고를 보여줬던" 씬 인덱스 기록 (같은 씬 재시작 시 스킵)
    private static int s_lastIntroSceneIndex = -1;

    //  조회용 공개 API
    public static int LastIntroSceneIndex => s_lastIntroSceneIndex;
    public static bool WasThisSceneShown(int sceneIndex) => s_lastIntroSceneIndex == sceneIndex;
    public static void MarkShownFor(int sceneIndex) => s_lastIntroSceneIndex = sceneIndex;

    private bool _dialogueFinished = false;

    void Start()
    {
        StartCoroutine(CoMaybeShow());
    }

    private IEnumerator CoMaybeShow()
    {
        if (!introUI || !data) yield break;

        //  이 컴포넌트가 속한 씬 인덱스(애디티브 안전)
        int current = gameObject.scene.buildIndex;

        // 씬이 바뀐 경우에만 로고 표시 (재시작 = 같은 씬 -> 스킵)
        if (WasThisSceneShown(current)) yield break;

        if (waitForDialogueEnd)
            yield return new WaitUntil(() => _dialogueFinished);

        if (delayBeforeShow > 0f)
        {
            if (useRealtimeWait) yield return new WaitForSecondsRealtime(delayBeforeShow);
            else yield return new WaitForSeconds(delayBeforeShow);
        }

        if (this && introUI)
        {
            introUI.Show(data);
            MarkShownFor(current); //  이번 씬을 "보여줌"으로 기록
        }
    }

    /// <summary>대사(또는 컷신) 종료 시 외부에서 호출</summary>
    public void NotifyDialogueFinished()
    {
        _dialogueFinished = true;
    }
}
