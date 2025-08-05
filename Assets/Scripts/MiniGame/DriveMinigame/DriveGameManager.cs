using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DriveGameManager : MonoBehaviour
{
    public static DriveGameManager Instance { get; private set; }

    public event Action OnPlay;
    public event Action OnLose;
    public event Action OnPause;
    public event Action OnResume;
    public event Action OnReplay;
    public event Action OnWin;
    public event Action<float> OnTimeUpdate;

    [SerializeField] private float gameDuration = 20f;
    public float GameDuration=>gameDuration;
    private float timer;
    private bool isPlaying = false;


    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }    
        else
        {
            Destroy(gameObject);
        }    
    }

    private void Update()
    {
        if (isPlaying)
        {
            timer -= Time.deltaTime;
            
            if (timer <= 0f)
            {
                timer= 0f;
                Win();
            }

            OnTimeUpdate?.Invoke(timer);
        }
    }

    public void Win()
    {
        isPlaying = false;
        OnWin?.Invoke();
        StartCoroutine(ReturnToMapScene());
    }
    public void Play()
    {
        timer =gameDuration;
        isPlaying = true;
        OnPlay?.Invoke();
    }    

    public void Lose()
    {
        isPlaying = false;
        OnLose?.Invoke();
        StartCoroutine(ReturnToMapScene());
    }    

    public void Pause()
    {
        OnPause?.Invoke();
    }

    public void Resume()
    {
        OnResume?.Invoke();
    }

    public void Replay()
    {
        OnReplay?.Invoke();
    }

    private IEnumerator ReturnToMapScene()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("Map");
    }
}
