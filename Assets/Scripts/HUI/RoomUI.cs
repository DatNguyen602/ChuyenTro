using TMPro;
using UnityEngine;

[System.Serializable]
public class RoomUI : MonoBehaviour
{
    [SerializeField] private GameObject informationPanel;
    [SerializeField] private TextMeshProUGUI priceTMP;

    public void ShowRoomInfor(float price)
    {
        if (!informationPanel.activeSelf)
            informationPanel.SetActive(true);

        priceTMP.text = $"Giá: {price:N0} VND";

    }


}
