using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private RoomUI roomUI;
    public RoomUI RoomUI => roomUI;

    [SerializeField] private MoveUI moveUI;
    public MoveUI MoveUI => moveUI;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void HideAllUI()
    {
        roomUI.HideUI();
        moveUI.HideUI();
    }    
    public bool IsUIBlock()
    {
        if(roomUI.IsBlock() || moveUI.IsBlock())
        {
            return true;
        }  
        return false;
    }    
}
