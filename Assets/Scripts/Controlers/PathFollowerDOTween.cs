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

    [Header("Gizmo")]
    public Color gizmoPathColor = Color.red;

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

        SetupStart();
    }

    private void OnEnable()
    {
        try
        {
            SetupStart();
        }
        catch { }
    }

    public void RePLay()
    {
        if (sequence != null && sequence.IsActive())
        {
            sequence.Restart();
            sequence.Play();
        }
        else
        {
            Start();
        }
    }

    private void SetupStart()
    {
        var wpList = pathCreator.waypoints;
        Vector3[] pts = wpList
            .Select(wp => pathCreator.transform.TransformPoint(wp.position))
            .ToArray();
        GamePlayManager.instance.Player.transform.position = wpList[1].position - Vector3.one;
        GamePlayManager.instance.Player.SetActive(false);
        GamePlayManager.instance.Player.GetComponent<JoyStick>().joystick.gameObject.SetActive(false);
        float totalLen = 0f;
        for (int i = 1; i < pts.Length; i++)
            totalLen += Vector3.Distance(pts[i - 1], pts[i]);

        transform.position = pts[0];

        sequence = DOTween.Sequence();
        //sequence.Pause();
        float targetZoom = Camera.main.orthographicSize;
        GamePlayManager.instance.virtualCamera.Lens.OrthographicSize = 5;

        //sequence.Join(Camera.main.DOOrthoSize(targetZoom, totalDuration).SetEase(Ease.OutQuad));

        for (int i = 1; i < pts.Length; i++)
        {
            if (i == 1) sequence.AppendCallback(() => transform.GetChild(0).gameObject.SetActive(true));
            Vector3 from = pts[i - 1];
            Vector3 to = pts[i];
            float segLen = Vector3.Distance(from, to);
            float segTime = totalDuration * (segLen / totalLen);

            Vector3 dir = (to - from).normalized;
            float angle = Mathf.Atan2((to - from).y, (to - from).x) * Mathf.Rad2Deg;
            sequence.Append(transform.DOMove(to, segTime).SetEase(Ease.Linear))
                    .Join(transform.DORotate(new Vector3(0, 0, angle), 0.5f).SetEase(Ease.OutQuad));

            if (i == pts.Length - 1)
            {
                sequence.AppendCallback(() => transform.GetChild(0).gameObject.SetActive(false));
            }

            float pause = 0;
            if (wpList[i].pauseDuration.from >= 0 && wpList[i].pauseDuration.to > 0)
            {
                if (wpList[i].pauseDuration.from < wpList[i].pauseDuration.to)
                {
                    pause = Random.Range(wpList[i].pauseDuration.from, wpList[i].pauseDuration.to);
                }
                else if (wpList[i].pauseDuration.from == wpList[i].pauseDuration.to)
                {
                    pause = wpList[i].pauseDuration.from;
                }
            }

            if (pause > 0f)
            {
                sequence.AppendCallback(() => isPausedAtWaypoint = true).
                    AppendInterval(pause / 2).AppendCallback(() => isPausedAtWaypoint = false);
                if (i == 1)
                {
                    sequence.AppendCallback(() =>
                    {
                        GamePlayManager.instance.Player.SetActive(true);
                    });
                    sequence.Join(DOTween.To(
                            () => GamePlayManager.instance.virtualCamera.Lens.OrthographicSize,
                            x => GamePlayManager.instance.virtualCamera.Lens.OrthographicSize = x,
                            targetZoom,
                            segTime
                        ).SetEase(Ease.InOutSine)
                    );

                    sequence.AppendCallback(() =>
                    {
                        GamePlayManager.instance.Player.GetComponent<JoyStick>().joystick.gameObject.SetActive(true);
                    });
                }
                sequence.AppendCallback(() => isPausedAtWaypoint = true).
                    AppendInterval(pause / 2).AppendCallback(() => isPausedAtWaypoint = false);
            }
            else
            {
                if (i == 1)
                {
                    sequence.AppendCallback(() =>
                    {
                        GamePlayManager.instance.Player.SetActive(true);
                    });
                    //sequence.Join(Camera.main.DOOrthoSize(targetZoom, totalDuration).SetEase(Ease.InOutSine));
                }
            }
        }
    }

    public void ChangeMap()
    {
        var wpList = pathCreator.waypoints;
        Vector3[] pts = wpList
            .Select(wp => pathCreator.transform.TransformPoint(wp.position))
            .ToArray();
        GamePlayManager.instance.Player.transform.position = wpList[1].position - Vector3.one;
        GamePlayManager.instance.Player.SetActive(false);
        GamePlayManager.instance.Player.GetComponent<JoyStick>().joystick.gameObject.SetActive(false);
        float totalLen = 0f;
        for (int i = 1; i < pts.Length; i++)
            totalLen += Vector3.Distance(pts[i - 1], pts[i]);

        transform.position = pts[0];

        sequence = DOTween.Sequence();
        float targetZoom = Camera.main.orthographicSize;

        for (int i = 1; i < pts.Length; i++)
        {
            if (i == 1) sequence.AppendCallback(() => transform.GetChild(0).gameObject.SetActive(true));
            Vector3 from = pts[i - 1];
            Vector3 to = pts[i];
            float segLen = Vector3.Distance(from, to);
            float segTime = totalDuration * (segLen / totalLen);

            Vector3 dir = (to - from).normalized;
            float angle = Mathf.Atan2((to - from).y, (to - from).x) * Mathf.Rad2Deg;
            sequence.Append(transform.DOMove(to, segTime).SetEase(Ease.Linear))
                    .Join(transform.DORotate(new Vector3(0, 0, angle), 0.5f).SetEase(Ease.OutQuad));

            if (i == pts.Length - 1)
            {
                sequence.AppendCallback(() => transform.GetChild(0).gameObject.SetActive(false));
            }

            float pause = 0;
            if (wpList[i].pauseDuration.from >= 0 && wpList[i].pauseDuration.to > 0)
            {
                if (wpList[i].pauseDuration.from < wpList[i].pauseDuration.to)
                {
                    pause = Random.Range(wpList[i].pauseDuration.from, wpList[i].pauseDuration.to);
                }
                else if (wpList[i].pauseDuration.from == wpList[i].pauseDuration.to)
                {
                    pause = wpList[i].pauseDuration.from;
                }
            }

            if (pause > 0f)
            {
                sequence.AppendCallback(() => isPausedAtWaypoint = true).
                    AppendInterval(pause).AppendCallback(() => isPausedAtWaypoint = false);
            }
        }
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
