using System.Linq;
using UnityEngine;
using DG.Tweening;

public class PathFollowerDOTween : MonoBehaviour
{
    [Header("Path Creator Reference")]
    public DOTweenPathCreator pathCreator;

    [Header("DOTween Settings")]
    public float totalDuration = 5f;
    public PathType pathType = PathType.CatmullRom;

    [Header("Loop Settings")]
    public bool loop = true;
    public bool pingPong = false;
    [Tooltip("Thời gian dừng trước khi bắt đầu lại vòng tiếp theo")]
    public float loopDelay = 1f;

    //[Header("Rotation Settings")]
    //[Tooltip("Thời gian xoay về hướng waypoint trước khi di chuyển")]
    //public float rotationDuration = 0.2f;

    [Header("Gizmo")]
    public Color gizmoPathColor = Color.green;

    private Sequence sequence;

    private bool isPausedAtWaypoint = false;
    public bool IsPausedAtWaypoint => isPausedAtWaypoint;

    private void Start()
    {
        if (pathCreator == null || pathCreator.waypoints.Count < 2)
        {
            Debug.LogWarning("PathFollowerDOTween: Thiếu path hoặc < 2 waypoints.");
            return;
        }

        // Tạo mảng world‐points
        var wpList = pathCreator.waypoints;
        Vector3[] pts = wpList
            .Select(wp => pathCreator.transform.TransformPoint(wp.position))
            .ToArray();

        // Tính tổng độ dài path
        float totalLen = 0f;
        for (int i = 1; i < pts.Length; i++)
            totalLen += Vector3.Distance(pts[i - 1], pts[i]);

        // Đặt vị trí bắt đầu
        transform.position = pts[0];

        // Xây Sequence
        sequence = DOTween.Sequence();

        for (int i = 1; i < pts.Length; i++)
        {
            if(i == 1) sequence.AppendCallback(() => transform.GetChild(0).gameObject.SetActive(true));
            Vector3 from = pts[i - 1];
            Vector3 to = pts[i];
            float segLen = Vector3.Distance(from, to);
            float segTime = totalDuration * (segLen / totalLen);

            // 1) Xoay về hướng điểm tiếp theo 2D (xoay quanh Z), thay bằng đoạn sau:
            Vector3 dir = (to - from).normalized;
            float angle = Mathf.Atan2((to - from).y, (to - from).x) * Mathf.Rad2Deg;
            sequence.Append(transform.DOMove(to, segTime).SetEase(Ease.Linear))
                    .Join(transform.DORotate(new Vector3(0, 0, angle), 0.5f).SetEase(Ease.OutQuad));

            if(i == pts.Length - 1)
            {
                sequence.AppendCallback(() => transform.GetChild(0).gameObject.SetActive(false));
            }

            // 3) Pause tại waypoint
            float pause = wpList[i].pauseDuration;
            if (pause > 0f)
                sequence.AppendCallback(() => isPausedAtWaypoint = true).
                    AppendInterval(pause).AppendCallback(() => isPausedAtWaypoint = false); ;
        }

        // 4) Delay giữa các lần lặp, rồi thiết lập loop/ping-pong
        if (loop)
        {
            if (loopDelay > 0f)
                sequence.AppendInterval(loopDelay);

            sequence.SetLoops(-1, pingPong ? LoopType.Yoyo : LoopType.Restart);
        }

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
