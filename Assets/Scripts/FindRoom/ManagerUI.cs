using TMPro;
using UnityEngine;

public class ManagerUI : MonoBehaviour
{
    public static ManagerUI instance { get; private set; }

    public TextMeshProUGUI roomDataInfor;
    public GameObject itemControlBtns;
    public GameObject GridItem;
    public GameObject Shop;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void TurnOffShop()
    {
        Shop.SetActive(false);
        GridItem.SetActive(true);
    }
    public void TurnOnShop()
    {
        Shop.SetActive(true);
        GridItem.SetActive(false);
    }
}
