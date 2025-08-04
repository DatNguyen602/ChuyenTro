using UnityEngine;

public class TrashSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] trashPrefabs;
    [SerializeField] private Vector2 spawnCenter;
    [SerializeField] private Vector2 spawnSize = new Vector2(10f, 6f);

    private int currentOrder = 0;

    

    public void SpawnTrash( int  spawnCount )
    {
        for (int i = 0; i < spawnCount; i++)
        {
            SpawnTrashObject();
        }
    }
    private void SpawnTrashObject()
    {
        GameObject prefab = trashPrefabs[Random.Range(0, trashPrefabs.Length)];

        float x = Random.Range(spawnCenter.x - spawnSize.x / 2, spawnCenter.x + spawnSize.x / 2);
        float y = Random.Range(spawnCenter.y - spawnSize.y / 2, spawnCenter.y + spawnSize.y / 2);
        Vector2 spawnPos = new Vector2(x, y);

        float randomRotation = Random.Range(0f, 360f);
        Quaternion rotation = Quaternion.Euler(0f, 0f, randomRotation);

        GameObject newTrash = Instantiate(prefab, spawnPos, rotation);

        SpriteRenderer sr = newTrash.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sortingOrder = currentOrder;
            currentOrder++;
        }
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;

        Gizmos.DrawWireCube(spawnCenter, spawnSize);
    }

}
