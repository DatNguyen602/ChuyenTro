using TMPro;
using UnityEngine;

public class ManagerUI : MonoBehaviour
{
    public static ManagerUI instance { get; private set; }

    public TextMeshProUGUI roomDataInfor;

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
