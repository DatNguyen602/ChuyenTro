using UnityEngine;

public class EndlessRoadTextureScroller : MonoBehaviour
{
    [SerializeField] private float scrollSpeed = 0.1f; 
    [SerializeField] private Renderer roadRenderer;  

    private Vector2 currentOffset = Vector2.zero;
    private bool isScrolling = false;


    private void Start()
    {
        DriveGameManager.Instance.OnPlay += StartScrolling;
        DriveGameManager.Instance.OnResume += StartScrolling;
        DriveGameManager.Instance.OnPause += StopScrolling;
        DriveGameManager.Instance.OnLose += StopScrolling;
        DriveGameManager.Instance.OnReplay += ResetScrolling;
        DriveGameManager.Instance.OnWin += StopScrolling;


        isScrolling = false;
    }

    private void Update()
    {
        if (!isScrolling) return;

        currentOffset.y += scrollSpeed * Time.deltaTime;

        roadRenderer.material.mainTextureOffset = currentOffset;
    }

    private void StartScrolling()
    {
        isScrolling = true;
    }

    private void StopScrolling()
    {
        isScrolling = false;
    }

    private void ResetScrolling()
    {
        currentOffset = Vector2.zero;
        roadRenderer.material.mainTextureOffset = currentOffset;
    }

    private void OnDestroy()
    {
        if (DriveGameManager.Instance == null) return;
        DriveGameManager.Instance.OnPlay -= StartScrolling;
        DriveGameManager.Instance.OnResume -= StartScrolling;
        DriveGameManager.Instance.OnPause -= StopScrolling;
        DriveGameManager.Instance.OnLose -= StopScrolling;
        DriveGameManager.Instance.OnReplay -= ResetScrolling;
    }
}
