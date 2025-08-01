using System.Linq;
using UnityEngine;
using DG.Tweening;

public class PathFollowerDOTween : MonoBehaviour
{
    [Header("Path Creator Reference")]
    public DOTweenPathCreator pathCreator;

    [Header("DOTween Settings")]
    public float totalDuration = 5f;
    public bool loop = true;
    public bool pingPong = false;
    public PathType pathType = PathType.CatmullRom;
    public Color gizmoPathColor = Color.green;

    private Sequence sequence;

    private void Start()
    {
        if (pathCreator == null || pathCreator.waypoints.Count < 2)
        {
            Debug.LogWarning("PathFollowerDOTween: Thiếu path hoặc < 2 waypoints.");
            return;
        }

        // Tạo mảng vị trí world
        var wpList = pathCreator.waypoints;
        Vector3[] pts = wpList
            .Select(wp => pathCreator.transform.TransformPoint(wp.position))
            .ToArray();

        // Tính tổng độ dài path
        float totalLen = 0f;
        for (int i = 1; i < pts.Length; i++)
            totalLen += Vector3.Distance(pts[i - 1], pts[i]);

        // Đặt start
        transform.position = pts[0];

        // Xây Sequence
        sequence = DOTween.Sequence();
        for (int i = 1; i < pts.Length; i++)
        {
            float segLen = Vector3.Distance(pts[i - 1], pts[i]);
            float segTime = totalDuration * (segLen / totalLen);

            // Move tới điểm i
            sequence.Append(transform.DOMove(pts[i], segTime).SetEase(Ease.Linear));

            // Pause tại điểm i
            float pause = wpList[i].pauseDuration;
            if (pause > 0f)
                sequence.AppendInterval(pause);
        }

        // Thiết lập loop
        if (loop)
            sequence.SetLoops(-1, pingPong ? LoopType.Yoyo : LoopType.Restart);

        sequence.Play();
    }

    private void OnDisable()
    {
        sequence?.Kill();
    }

    private void OnDrawGizmos()
    {
        if (pathCreator == null) return;
        Gizmos.color = gizmoPathColor;
        var t = pathCreator.transform;

        for (int i = 0; i < pathCreator.waypoints.Count - 1; i++)
        {
            Vector3 a = t.TransformPoint(pathCreator.waypoints[i].position);
            Vector3 b = t.TransformPoint(pathCreator.waypoints[i + 1].position);
            Gizmos.DrawLine(a, b);
        }
    }
}
