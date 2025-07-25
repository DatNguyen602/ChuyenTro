using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MoveUI : MonoBehaviour
{
    [SerializeField] private GameObject container;
    [SerializeField] private Button moveBtn;

    private GraphicRaycaster raycaster;
    private EventSystem eventSystem;


    private void Awake()
    {
        if (moveBtn != null)
        {
            moveBtn.onClick.AddListener(OnMoveBtnClick);
        }
        else
        {
            Debug.LogWarning("Move button is not assigned in MoveUI.");
        }
    }
    private void Start()
    {
        raycaster = GetComponent<GraphicRaycaster>();
        eventSystem = FindFirstObjectByType<EventSystem>();
    }

    private void OnMoveBtnClick()
    {
        if (GameController.Instance != null && GameController.Instance.GridInteractHandler != null)
        {
            GameController.Instance.GridInteractHandler.MoveToSelectedTile();

            if (container != null)
                container.SetActive(false);
        }
        else
        {
            Debug.LogWarning("PlayerController or MoveHandler is null.");
        }
    }

    public void ShowUI()
    {
        if (container != null)
            container.SetActive(true);
    }

    public void HideUI()
    {
        if (container != null)
            container.SetActive(false);
    }

    public bool IsBlock()
    {
        if (!container.activeSelf || raycaster == null || eventSystem == null)
            return false;

        PointerEventData pointerData = new PointerEventData(eventSystem);

        pointerData.position = InputReader.Instance.currentTouchPos;
        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerData, results);

        return results.Count > 0;
    }    
}
