using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[System.Serializable]
public class RoomUI : MonoBehaviour
{
    [SerializeField] private GameObject informationPanel;
    [SerializeField] private TextMeshProUGUI priceTMP;

    private GraphicRaycaster raycaster;
    private EventSystem eventSystem;

    private void Start()
    {
        raycaster = GetComponent<GraphicRaycaster>();
        eventSystem = FindFirstObjectByType<EventSystem>();
    }

    public void ShowRoomInfor(float price)
    {
        if (!informationPanel.activeSelf)
            informationPanel.SetActive(true);

        priceTMP.text = $"Giá: {price:N0} VND";

    }
    public void HideUI()
    {
        informationPanel.SetActive(false);
    }

    public bool IsBlock()
    {
        if (!informationPanel.activeSelf || raycaster == null || eventSystem == null)
            return false;

        PointerEventData pointerData = new PointerEventData(eventSystem);

        pointerData.position = InputReader.Instance.currentTouchPos;
        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerData, results);
       
        return results.Count > 0;
    }

}
