using UnityEngine;

public class MirrorAnime : MonoBehaviour
{
    [SerializeField] Transform visual; // À§ÀÇ Visual
    float baseScaleX;

    void Awake() => baseScaleX = visual.localScale.x;

    public void Face(float dirX)
    {
        if (Mathf.Abs(dirX) < 0.01f) return;
        float sign = dirX < 0 ? -1f : 1f;
        Vector3 s = visual.localScale;
        s.x = Mathf.Abs(baseScaleX) * sign;
        visual.localScale = s;
    }
}
