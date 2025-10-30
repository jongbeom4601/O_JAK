using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class StageIntroRunner : MonoBehaviour
{
    [SerializeField] private StageIntroUI introUI;
    [SerializeField] private StageIntroData data;

    // 로고 표시 전 대기 시간(초). 0이면 즉시 표시
    [SerializeField] private float delayBeforeShow = 0.75f;

    // 타임스케일 0이어도 기다리려면 체크 (Realtime 대기)
    [SerializeField] private bool useRealtimeWait = false;

    // 직전에 "로고를 보여줬던" 씬 인덱스 기록
    private static int s_lastIntroSceneIndex = -1;

    void Start()
    {
        StartCoroutine(CoMaybeShow());
    }

    private IEnumerator CoMaybeShow()
    {
        if (!introUI || !data) yield break;

        int current = SceneManager.GetActiveScene().buildIndex;

        // 씬이 바뀐 경우에만 로고 표시 (재시작 = 같은 씬 -> 스킵)
        bool shouldShow = (current != s_lastIntroSceneIndex);
        if (!shouldShow) yield break;

        // 표시 전 대기
        if (delayBeforeShow > 0f)
        {
            if (useRealtimeWait)
                yield return new WaitForSecondsRealtime(delayBeforeShow);
            else
                yield return new WaitForSeconds(delayBeforeShow);
        }

        // 대기 중 파괴되었을 수 있으니 최종 체크
        if (this && introUI)
        {
            introUI.Show(data);
            s_lastIntroSceneIndex = current; // 이번 씬을 "보여줌"으로 기록
        }
    }
}
