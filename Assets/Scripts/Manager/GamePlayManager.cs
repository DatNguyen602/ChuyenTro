using UnityEngine;

public class GamePlayManager : MonoBehaviour
{
    public static GamePlayManager instance;



    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
