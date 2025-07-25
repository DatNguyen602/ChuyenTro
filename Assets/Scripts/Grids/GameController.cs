using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    [SerializeField] private GridInteractHandler gridInteractHandler;
    public GridInteractHandler GridInteractHandler => gridInteractHandler;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }    
        else
        {
            Destroy(this.gameObject);
        }    
    }

    private void Update()
    {
        
        GridInteractHandler.TileInteract();
            
    }

}
