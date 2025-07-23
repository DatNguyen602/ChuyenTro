using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartLoading : MonoBehaviour
{
    [SerializeField] private string sceneName = "MainMenu";
    [SerializeField] private float delay = 2f;
    [SerializeField] private Slider loadingBar;
    private float timer;
    private AsyncOperation sceneManager;
    
    void Start()
    {
        timer = delay;
        if (sceneName != "")
        {
            sceneManager = SceneManager.LoadSceneAsync(sceneName);
            sceneManager.allowSceneActivation = false;
        }
    }

    void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            if (loadingBar != null)
            {
                loadingBar.value = 1 - (timer / delay);
            }
        }
        else
        {
            if (sceneManager != null && !sceneManager.isDone)
            {
                sceneManager.allowSceneActivation = true;
            }
        }
    }
}