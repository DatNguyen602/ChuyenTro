using UnityEngine;

public class CarController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float minX = -2f;
    [SerializeField] private float maxX = 2f;
    [SerializeField] private float tiltAngle = 15f; 
    [SerializeField] private float tiltSmooth = 10f; 

    private Quaternion targetRotation;

    private bool isStop ;


    private void Start()
    {
        DriveGameManager.Instance.OnPlay += HandlePlay;
        DriveGameManager.Instance.OnLose += HandleLose;
        DriveGameManager.Instance.OnPause += HandlePause;
        DriveGameManager.Instance.OnResume += HandleResume;
        DriveGameManager.Instance.OnReplay += HandleReplay;

        isStop = true;
        targetRotation = transform.rotation;
    }

    private void Update()
    {
        if (isStop) return;
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.position.x < Screen.width / 2f)
            {
                MoveLeft();
                targetRotation = Quaternion.Euler(0, 0, tiltAngle); 
            }
            else
            {
                MoveRight();
                targetRotation = Quaternion.Euler(0, 0, -tiltAngle);
            }
        }
        else
        {
            targetRotation = Quaternion.Euler(0, 0, 0);
        }

        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * tiltSmooth);
    }

    void MoveLeft()
    {
        Vector3 newPos = transform.position + Vector3.left * moveSpeed * Time.deltaTime;

        if (newPos.x > minX)
        {
            transform.position = newPos;
        }
    }

    void MoveRight()
    {
        Vector3 newPos = transform.position + Vector3.right * moveSpeed * Time.deltaTime;

        if (newPos.x < maxX)
        {
            transform.position = newPos;
        }
    }

    void HandlePlay() => isStop = false;
    void HandleLose() => isStop = true;
    void HandlePause() => isStop = true;
    void HandleResume() => isStop = false;
    void HandleReplay()
    {
        transform.position = new Vector3(0f, transform.position.y, transform.position.z);
        transform.rotation = Quaternion.identity;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Obstacle"))
        {
            isStop = true;
            DriveGameManager.Instance.Lose();
        }
    }

}
