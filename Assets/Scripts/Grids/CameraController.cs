using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 0.01f;
    [SerializeField] private float minX = -100f, maxX = 100f;
    [SerializeField] private float minY = -100f, maxY = 100f;

    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        HandleDragFromInputReader();
    }

    void HandleDragFromInputReader()
    {
        var input = InputReader.Instance;
        if (input == null || cam == null) return;

        if (input.CurrentState == InputReader.InputState.Dragging)
        {
            Vector2 moveDelta = input.DragDelta;
            if (moveDelta == Vector2.zero) return;

            Vector3 worldDelta = cam.ScreenToWorldPoint(new Vector3(-moveDelta.x, -moveDelta.y, cam.nearClipPlane))
                               - cam.ScreenToWorldPoint(new Vector3(0, 0, cam.nearClipPlane));

            cam.transform.position += worldDelta * moveSpeed;

            cam.transform.position = new Vector3(
                Mathf.Clamp(cam.transform.position.x, minX, maxX),
                Mathf.Clamp(cam.transform.position.y, minY, maxY),
                cam.transform.position.z
            );
        }
    }
}
