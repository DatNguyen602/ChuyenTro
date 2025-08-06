using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TrashGameController : MonoBehaviour
{

    [SerializeField] private TrashSpawner trashSpawner;
    [SerializeField] private TouchDragTrash touchDragTrash;
    [SerializeField] private TrashUI trashUI;

    [SerializeField] private int spawnCount;
    private int currentCount;

    [SerializeField] private int timeLimits;
    private float timer;
    private int lastDisplayedTime = -1;



    
    private void Start()
    {
        timer = timeLimits;
        currentCount =spawnCount;
        trashUI.SetTimeText(timeLimits);
        trashSpawner.SpawnTrash(spawnCount);
        touchDragTrash.OnDestroyTrash += OnTrashDestroy;
    }

    private void Update()
    {
        if (currentCount == 0 || timer<=0f ) return;

        timer -= Time.deltaTime;

        int displayedTime = Mathf.CeilToInt(timer);
        if (displayedTime != lastDisplayedTime)
        {
            lastDisplayedTime = displayedTime;
            trashUI.SetTimeText(displayedTime);
        }

        if (timer <= 0f)
        {
            trashUI.ShowTimeOutVisual();
            touchDragTrash.enabled = false;
            StartCoroutine(ReturnToMapScene());
        }
    }

    private void OnTrashDestroy()
    {
        currentCount--;
        if (currentCount == 0)
        {
            trashUI.ShowDoneVisual();
            touchDragTrash.enabled=false;
            StartCoroutine(ReturnToMapScene());
        }

    }
    private IEnumerator ReturnToMapScene()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("Map");
    }
}
