using System.Collections;
using TMPro;
using UnityEngine;

public class DartGameController : MonoBehaviour
{
    [SerializeField] private GameObject redDot;
    [SerializeField] private DartBoard dartBoard;
    [SerializeField] private GameObject dart;
    [SerializeField] private TextMeshProUGUI scoreTMP;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float zoomSize = 3f;
    [SerializeField] private float zoomDuration = 0.15f;

    [SerializeField] private float speed = 2f;

    private Vector3 dartStartPos;
    private float dartboardRadius = 2.29f;
    private Vector2 center ;
    private bool isPlaying = true;
    private bool selectingX = true;
    private Vector2 dartTarget = Vector2.zero;
    private Vector3 moveDir;

    void Start()
    {
        dartStartPos = dart.transform.position;
        dartboardRadius = dartBoard.BoardRadius;
        moveDir = selectingX ? Vector3.right : Vector3.up;
        redDot.transform.position = dartBoard.transform.position;
        center = dartBoard.transform.position;

    }

    void Update()
    {
        if (!isPlaying) return;

        redDot.transform.position += moveDir * speed * Time.deltaTime;

        Vector2 currentPos = redDot.transform.position;
        float distance = (selectingX)? Mathf.Abs(currentPos.x - center.x): Mathf.Abs(currentPos.y - center.y);

        if (distance > dartboardRadius)
        {
            moveDir *= -1;
        }

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
                Debug.Log("ném phi tiêu: " + dartTarget);
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

        Vector3 direction = (targetPos - dartStartPos).normalized;
        dart.transform.right = direction;

        float journey = 0f;
        float duration = 0.2f;

        while (journey < duration)
        {
            journey += Time.deltaTime;
            float t = journey / duration;
            dart.transform.position = Vector3.Lerp(dartStartPos, targetPos, t);
            yield return null;
        }
        dart.transform.position = targetPos;

        StartCoroutine(CameraZoomEffect(targetPos));


        float score = dartBoard.GetScore(targetPos);

        StartCoroutine(ShowScore(score));

        
    }

    private IEnumerator ShowScore(float score)
    {
        scoreTMP.text = score.ToString();
        scoreTMP.gameObject.SetActive(true);

        float scaleUpTime = 0.1f;
        float maxScale = 2f;
        float t = 0f;
        while (t < scaleUpTime)
        {
            t += Time.deltaTime;
            float s = Mathf.Lerp(1f, maxScale, t / scaleUpTime);
            scoreTMP.transform.localScale = new Vector3(s, s, s);
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        float scaleDownTime = 0.1f;
        t = 0f;
        while (t < scaleDownTime)
        {
            t += Time.deltaTime;
            float s = Mathf.Lerp(maxScale, 1f, t / scaleDownTime);
            scoreTMP.transform.localScale = new Vector3(s, s, s);
            yield return null;
        }
        scoreTMP.gameObject.SetActive(false);

        yield return new WaitForSeconds(0.5f);

        redDot.transform.position = dartBoard.transform.position;
        redDot.SetActive(true);
        dart.transform.position = dartStartPos;
        selectingX = true;
        isPlaying = true;
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
            mainCamera.transform.position = Vector3.Lerp(originalPos, new Vector3(focusPoint.x, focusPoint.y, originalPos.z), lerpT);
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        t = 0f;
        while (t < zoomDuration)
        {
            t += Time.deltaTime;
            float lerpT = t / zoomDuration;
            mainCamera.orthographicSize = Mathf.Lerp(zoomSize, originalSize, lerpT);
            mainCamera.transform.position = Vector3.Lerp(new Vector3(focusPoint.x, focusPoint.y, originalPos.z), originalPos, lerpT);
            yield return null;
        }

        mainCamera.orthographicSize = originalSize;
        mainCamera.transform.position = originalPos;
    }

}
