using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ManagerUI : MonoBehaviour
{
    public static ManagerUI instance { get; private set; }

    public TextMeshProUGUI roomDataInfor;
    public GameObject itemControlBtns;
    public Button Rote1, Rote2;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
