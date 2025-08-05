using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] obstaclePrefabs; 
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private float minX = -2f;
    [SerializeField] private float maxX = 2f;
    [SerializeField] private float spawnY = 6f;

    private float timer;
    private bool isSpawning = false;

    private void Start()
    {
        DriveGameManager.Instance.OnPlay += StartSpawning;
        DriveGameManager.Instance.OnResume += StartSpawning;
        DriveGameManager.Instance.OnPause += StopSpawning;
        DriveGameManager.Instance.OnLose += StopSpawning;
        DriveGameManager.Instance.OnReplay += ResetSpawner;
        DriveGameManager.Instance.OnWin += StopSpawning;

        isSpawning = false;
    }
    private void Update()
    {
        if (!isSpawning) return;
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnObstacle();
            timer = 0f;
        }
    }

    void SpawnObstacle()
    {
        if(obstaclePrefabs.Length == 0) return;
        float spawnX = Random.Range(minX, maxX);
        Vector3 spawnPos = new Vector3(spawnX, spawnY, 0f);

        int randomIndex = Random.Range(0, obstaclePrefabs.Length);
        GameObject prefabToSpawn = obstaclePrefabs[randomIndex];

        Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);
    }

    private void StartSpawning()
    {
        isSpawning = true;
    }

    private void StopSpawning()
    {
        isSpawning = false;
    }

    private void ResetSpawner()
    {
        timer = 0f;
    }

    private void OnDestroy()
    {
        if (DriveGameManager.Instance == null) return;

        DriveGameManager.Instance.OnPlay -= StartSpawning;
        DriveGameManager.Instance.OnResume -= StartSpawning;
        DriveGameManager.Instance.OnPause -= StopSpawning;
        DriveGameManager.Instance.OnLose -= StopSpawning;
        DriveGameManager.Instance.OnReplay -= ResetSpawner;
    }
}
