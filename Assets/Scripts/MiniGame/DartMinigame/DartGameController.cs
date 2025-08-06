using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DartGameController : MonoBehaviour
{
    [Header("Dot & Dart")]
    [SerializeField] private GameObject redDot;
    [SerializeField] private GameObject dart;
    [SerializeField] private DartBoard dartBoard;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI scoreTMP;
    [SerializeField] private TextMeshProUGUI totalScoreTMP;
    [SerializeField] private TextMeshProUGUI throwsTMP;
    [SerializeField] private TextMeshProUGUI targetScoreTMP;
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private TextMeshProUGUI resultTMP;

    [Header("Camera & Effects")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float zoomSize = 3f;
    [SerializeField] private float zoomDuration = 0.15f;

    [Header("Game Settings")]
    [SerializeField] private int targetScore = 50;
    [SerializeField] private int maxThrows = 3;
    [SerializeField] private float speed = 2f;

    private Vector3 dartStartPos;
    private float dartboardRadius;
    private Vector2 center;
    private bool isPlaying = true;
    private bool selectingX = true;
    private Vector2 dartTarget;
    private Vector3 moveDir;

    private int currentScore = 0;
    private int throwsCount = 0;

    private void Start()
    {
        dartStartPos = dart.transform.position;
        dartboardRadius = dartBoard.BoardRadius;
        center = dartBoard.transform.position;
        moveDir = Vector3.right;
        redDot.transform.position = center;

        targetScoreTMP.text = $"Target: {targetScore}";
        UpdateTotalScoreUI();
        UpdateThrowsUI();

        resultPanel.SetActive(false);
    }

    private void Update()
    {
        if (!isPlaying) return;

        redDot.transform.position += moveDir * speed * Time.deltaTime;

        Vector2 currentPos = redDot.transform.position;
        float distance = selectingX
            ? Mathf.Abs(currentPos.x - center.x)
            : Mathf.Abs(currentPos.y - center.y);

        if (distance > dartboardRadius)
            moveDir *= -1;

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            if (selectingX)
            {
                dartTarget.x = redDot.transform.position.x;
                selectingX = false;
                moveDir = Vector3.up;
            }
            else
            {
                dartTarget.y = redDot.transform.position.y;
                StartCoroutine(DartThrowingCoroutine());
                moveDir = Vector3.right;
            }
        }
    }

    private IEnumerator DartThrowingCoroutine()
    {
        isPlaying = false;
        redDot.SetActive(false);

        Vector3 targetPos = new Vector3(dartTarget.x, dartTarget.y, dartStartPos.z);
        dart.transform.right = (targetPos - dartStartPos).normalized;

        float journey = 0f, duration = 0.2f;
        while (journey < duration)
        {
            journey += Time.deltaTime;
            float t = journey / duration;
            dart.transform.position = Vector3.Lerp(dartStartPos, targetPos, t);
            yield return null;
        }
        dart.transform.position = targetPos;

        yield return CameraZoomEffect(targetPos);

        float score = dartBoard.GetScore(targetPos);
        StartCoroutine(ShowScore(score));
    }

    private IEnumerator ShowScore(float score)
    {
        scoreTMP.text = score.ToString();
        scoreTMP.gameObject.SetActive(true);

        currentScore += Mathf.RoundToInt(score);
        throwsCount++;
        UpdateTotalScoreUI();
        UpdateThrowsUI();

        // Scale up
        float scaleUpTime = 0.1f, maxScale = 2f, t = 0f;
        while (t < scaleUpTime)
        {
            t += Time.deltaTime;
            float s = Mathf.Lerp(1f, maxScale, t / scaleUpTime);
            scoreTMP.transform.localScale = Vector3.one * s;
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);

        // Scale down
        float scaleDownTime = 0.1f; t = 0f;
        while (t < scaleDownTime)
        {
            t += Time.deltaTime;
            float s = Mathf.Lerp(maxScale, 1f, t / scaleDownTime);
            scoreTMP.transform.localScale = Vector3.one * s;
            yield return null;
        }
        scoreTMP.gameObject.SetActive(false);

        yield return new WaitForSeconds(0.5f);

        if (throwsCount < maxThrows)
            ResetForNextThrow();
        else
            StartCoroutine(ShowResult());
    }

    private void UpdateTotalScoreUI()
    {
        totalScoreTMP.text = $"Total: {currentScore}";
    }

    private void UpdateThrowsUI()
    {
        throwsTMP.text = $"Throw: {throwsCount}/{maxThrows}";
    }

    private void ResetForNextThrow()
    {
        redDot.transform.position = center;
        redDot.SetActive(true);
        dart.transform.position = dartStartPos;
        selectingX = true;
        isPlaying = true;
    }

    private IEnumerator ShowResult()
    {
        yield return new WaitForSeconds(1f);

        resultPanel.SetActive(true);
        bool isWin = currentScore >= targetScore;
        resultTMP.text = isWin ? "You Win!" : "You Lose!";
        isPlaying = false;
        StartCoroutine(ReturnToMapScene());
    }

    private IEnumerator CameraZoomEffect(Vector3 focusPoint)
    {
        float originalSize = mainCamera.orthographicSize;
        Vector3 originalPos = mainCamera.transform.position;

        float t = 0f;
        while (t < zoomDuration)
        {
            t += Time.deltaTime;
            float lerpT = t / zoomDuration;
            mainCamera.orthographicSize = Mathf.Lerp(originalSize, zoomSize, lerpT);
            mainCamera.transform.position = Vector3.Lerp(
                originalPos,
                new Vector3(focusPoint.x, focusPoint.y, originalPos.z),
                lerpT
            );
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        t = 0f;
        while (t < zoomDuration)
        {
            t += Time.deltaTime;
            float lerpT = t / zoomDuration;
            mainCamera.orthographicSize = Mathf.Lerp(zoomSize, originalSize, lerpT);
            mainCamera.transform.position = Vector3.Lerp(
                new Vector3(focusPoint.x, focusPoint.y, originalPos.z),
                originalPos,
                lerpT
            );
            yield return null;
        }

        mainCamera.orthographicSize = originalSize;
        mainCamera.transform.position = originalPos;
    }

    private IEnumerator ReturnToMapScene()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("Map");
    }
}
