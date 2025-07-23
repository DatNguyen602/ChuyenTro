using UnityEngine;

public class CameraZoomAndPan : MonoBehaviour
{
    [Header("Zoom Settings")]
    public float zoomSpeed = 0.01f;
    public float minZoom = 3f;
    public float maxZoom = 8f;

    [Header("Pan Settings")]
    public float panSpeed = 0.01f;
    public Vector2 minPosition = new Vector2(-10f, -10f);
    public Vector2 maxPosition = new Vector2(10f, 10f);

    [SerializeField] private Camera cam;
    private Vector3 lastPanPosition;
    private bool isPanning;

    private Vector3 startCameraPos;
    private float startCameraSize;

    void Start()
    {
        //cam = GetComponent<Camera>();
        if (cam == null) Debug.LogError("Script phải gắn lên Camera!");
        else
        {
            startCameraPos = cam.transform.position;
            startCameraSize = cam.orthographicSize;
        }
    }

    void Update()
    {
//#if UNITY_EDITOR
//        // Dùng chuột để test trong Editor
//        HandleMousePan();
//        HandleMouseZoom();
//#else
        HandleTouch();
//#endif
    }

    void HandleTouch()
    {
        if (Input.touchCount == 1)
        {
            // PAN bằng 1 ngón
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Moved)
            {
                Vector3 delta = cam.ScreenToWorldPoint(touch.position - touch.deltaPosition) - cam.ScreenToWorldPoint(touch.position);
                Vector3 newPos = cam.transform.position + delta;

                cam.transform.position = ClampCameraPosition(newPos);
            }
        }
        else if (Input.touchCount == 2)
        {
            // ZOOM bằng 2 ngón
            Touch t0 = Input.GetTouch(0);
            Touch t1 = Input.GetTouch(1);

            Vector2 t0Prev = t0.position - t0.deltaPosition;
            Vector2 t1Prev = t1.position - t1.deltaPosition;

            float prevMag = (t0Prev - t1Prev).magnitude;
            float currentMag = (t0.position - t1.position).magnitude;
            float diff = prevMag - currentMag;

            cam.orthographicSize += diff * zoomSpeed;
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
        }
    }

    void HandleMouseZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            cam.orthographicSize -= scroll * 5f;
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
        }
    }

    void HandleMousePan()
    {
        if (Input.GetMouseButtonDown(0))
        {
            lastPanPosition = Input.mousePosition;
            isPanning = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isPanning = false;
        }

        if (isPanning && Input.GetMouseButton(0))
        {
            Vector3 delta = cam.ScreenToWorldPoint(lastPanPosition) - cam.ScreenToWorldPoint(Input.mousePosition);
            cam.transform.position = ClampCameraPosition(cam.transform.position + delta);
            lastPanPosition = Input.mousePosition;
        }
    }

    Vector3 ClampCameraPosition(Vector3 pos)
    {
        pos.x = Mathf.Clamp(pos.x, minPosition.x, maxPosition.x);
        pos.y = Mathf.Clamp(pos.y, minPosition.y, maxPosition.y);
        return pos;
    }

    private void OnDisable()
    {
        if(cam != null)
        {
            cam.transform.position = startCameraPos;
            cam.orthographicSize = startCameraSize;
        }
    }
}
