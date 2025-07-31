using System;
using UnityEngine;

public class DriveGameManager : MonoBehaviour
{
    public static DriveGameManager Instance { get; private set; }

    public event Action OnPlay;
    public event Action OnLose;
    public event Action OnPause;
    public event Action OnResume;
    public event Action OnReplay;


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

    public void Play()
    {
        OnPlay?.Invoke();
    }    

    public void Lose()
    {
        OnLose?.Invoke();
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
}
