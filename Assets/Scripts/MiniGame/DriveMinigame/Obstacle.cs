using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;       
    [SerializeField] private float destroyY = -6f;

    private bool isMoving = false;

    private void Start()
    {
        DriveGameManager.Instance.OnPlay += StartMoving;
        DriveGameManager.Instance.OnResume += StartMoving;
        DriveGameManager.Instance.OnPause += StopMoving;
        DriveGameManager.Instance.OnLose += StopMoving;
        DriveGameManager.Instance.OnReplay += DestroyObstacle;
        DriveGameManager.Instance.OnWin += StopMoving;


        isMoving = true;
    }

    private void Update()
    {
        if (!isMoving) return;

        transform.Translate(Vector3.down * moveSpeed * Time.deltaTime);

        if (transform.position.y <= destroyY)
        {
            Destroy(gameObject);
        }
    }

    private void StartMoving()
    {
        isMoving = true;
    }

    private void StopMoving()
    {
        isMoving = false;
    }

    private void DestroyObstacle()
    {
        Destroy(gameObject) ;
    }

    private void OnDestroy()
    {
        if (DriveGameManager.Instance == null) return;

        DriveGameManager.Instance.OnPlay -= StartMoving;
        DriveGameManager.Instance.OnResume -= StartMoving;
        DriveGameManager.Instance.OnPause -= StopMoving;
        DriveGameManager.Instance.OnLose -= StopMoving;
        DriveGameManager.Instance.OnReplay -= DestroyObstacle;
    }
}
