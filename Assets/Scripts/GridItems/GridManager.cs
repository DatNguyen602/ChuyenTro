using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class GridManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static GridManager instance { get; private set; }

    public GameObject gridItemPrefab, itemTemplate;
    [SerializeField] private List<GridItem> gridItems = new List<GridItem>();
    [SerializeField] private Transform gridParent;
    [SerializeField] private ScrollRect scrollRect;

    [SerializeField] private GraphicRaycaster raycaster;
    [SerializeField] private EventSystem eventSystem;

    private List<GameObject> gridItemObjects = new List<GameObject>();

    private bool isHovered = false;

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
    }

    private GameObject selectedObject;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    void Start()
    {
        
        for( int i = 0; i < gridItems.Count; i++)
        {
            GamePlayManager.instance.RendererList(gridItems[i], gridParent, i < gridParent.childCount ? gridParent.GetChild(i).gameObject : null);
        }
        for (int i =0;i<10;i++)
        {
            GameObject item = Instantiate(itemTemplate);
            item.SetActive(false);
            gridItemObjects.Add(item);
        }
    }

    private GameObject itemSelected, itemUISelected;
    private GridItem itemSelectedData;
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PointerEventData pointerData = new PointerEventData(eventSystem)
            {
                position = Input.mousePosition
            };

            List<RaycastResult> results = new List<RaycastResult>();
            raycaster.Raycast(pointerData, results);
            if (results.Count > 0)
            {
                if (itemUISelected != null)
                {
                    itemUISelected.GetComponent<Image>().color = Color.white;//new Color32(56, 56, 56, 115);
                }
                GridItemUI gridItemUI = (results[0]).gameObject.GetComponent<GridItemUI>();
                if (gridItemUI == null)
                {
                    Debug.Log(results[0].gameObject.name);
                    return;
                }
                itemUISelected = results[0].gameObject;
                itemUISelected.GetComponent<Image>().color = Color.red;
                itemSelectedData = gridItemUI.gridItem;
                if (scrollRect != null)
                {
                    scrollRect.enabled = false;
                }
            }
        }
        else if(Input.GetMouseButtonUp(0))
        {
            if(itemUISelected != null&& itemSelected!=null)
            {
                itemUISelected.GetComponent<Image>().color = Color.white;//new Color32(56, 56, 56, 115);
            }
            itemSelectedData = null;
            itemSelected = null;
            if (scrollRect != null)
            {
                scrollRect.enabled = true;
            }
        }
        else
        {
            if (!isHovered && itemSelectedData)
            {
                if (!itemSelected)
                {
                    itemSelected = GetItemObject();
                    itemSelected.SetActive(true);
                    itemSelected.GetComponentInChildren<SpriteRenderer>().sprite = itemSelectedData.spriteTemp;

                    itemSelected.transform.GetChild(0).localScale = Vector3.one * itemSelectedData.spriteScale;

                    itemSelected.transform.GetChild(0).localPosition = itemSelectedData.spriteOffsetPercent;

                    Destroy(itemUISelected);//.SetActive(false);
                    gridItems.Remove(itemSelectedData);
                    GridItemObject gridItemObject = itemSelected.GetComponent<GridItemObject>() ?? itemSelected.AddComponent<GridItemObject>();
                    gridItemObject.gridItem = itemSelectedData;
                    gridItemObject.OnSelect = (GameObject obj) =>
                    {
                        if (selectedObject != null)
                        {
                            if (obj != selectedObject)
                            {
                                selectedObject.GetComponentInChildren<SpriteRenderer>().color = Color.white;
                                selectedObject = obj;
                                selectedObject.GetComponentInChildren<SpriteRenderer>().color = Color.green;
                            }
                            else
                            {
                                selectedObject = null;
                                obj.GetComponentInChildren<SpriteRenderer>().color = Color.white;
                            }
                        }
                        else
                        {
                            selectedObject = obj;
                            obj.GetComponentInChildren<SpriteRenderer>().color = Color.green;
                        }
                        ManagerUI.instance.itemControlBtns.SetActive(selectedObject);
                    };
                    DragDropManager.instance.SetDrag(itemSelected);
                    Vector3 inputPosition;
                    bool released = DragDropManager.instance.GetInput(out inputPosition);
                    itemSelected.transform.position = inputPosition;
                    DragDropManager.instance.EndDrag = (GameObject obj, bool canDrop) =>
                    {
                        Debug.Log("End Drag");
                        if (!canDrop)
                        {
                            if (itemUISelected != null)
                            {
                                itemUISelected.GetComponent<Image>().color = Color.white;//new Color32(56, 56, 56, 115);
                            }
                            gridItems.Add(obj.GetComponent<GridItemObject>().gridItem);
                            GamePlayManager.instance.RendererList(obj.GetComponent<GridItemObject>().gridItem, gridParent);
                            obj.SetActive(false);
                            itemSelectedData = null;
                            itemSelected = null;
                            if (scrollRect != null)
                            {
                                scrollRect.enabled = true;
                            }
                        }
                        DragDropManager.instance.EndDrag = null;
                        return canDrop;
                    };

                    itemUISelected = null;
                    //RendererList();
                }
            }
        }
    }

    private GameObject GetItemObject()
    {
        foreach (GameObject item in gridItemObjects)
        {
            if (!item.activeSelf)
            {
                item.SetActive(true);
                item.transform.localRotation = Quaternion.Euler(0, 0, 0);
                return item;
            }
        }
        GameObject newItem = Instantiate(itemTemplate, gridParent);
        gridItemObjects.Add(newItem);
        return newItem;

    }
   
    public void retunObjtoList()
    {
        if(selectedObject != null)
        {
            Container.Instance.SetState(selectedObject.transform.position, selectedObject.GetComponent<GridItemObject>().gridItem.occupiedCellsStart, false);
            gridItems.Add(selectedObject.GetComponent<GridItemObject>().gridItem);
            GamePlayManager.instance.RendererList(selectedObject.GetComponent<GridItemObject>().gridItem, gridParent);
            selectedObject.SetActive(false);
            selectedObject = null;
            itemSelectedData = null;
            itemSelected = null;

            ManagerUI.instance.itemControlBtns.SetActive(selectedObject);
        }
    }
    public void RotateSelectedClockwise()
    {
        RotateSelectedItem(1);
    }

    public void RotateSelectedCounterClockwise()
    {
        RotateSelectedItem(-1);
    }

    public void RotateSelectedItem(int direction)
    {
        if (selectedObject == null)
        {
            Debug.LogWarning("Không có object được chọn.");
            return;
        }

        GridItemObject gridItemObj = selectedObject.GetComponent<GridItemObject>();
        if (gridItemObj == null || gridItemObj.gridItem == null)
        {
            Debug.LogWarning("Không tìm thấy GridItemObject.");
            return;
        }

        GridItem gridItem = gridItemObj.gridItem;

        // Xoay các cell quanh tâm (0,0)
        List<Vector2Int> oldCells = new List<Vector2Int>(gridItem.occupiedCellsStart);
        List<Vector2Int> newCells = new List<Vector2Int>();
        foreach (Vector2Int cell in oldCells)
        {
            if (direction > 0)
                newCells.Add(new Vector2Int(-cell.y, cell.x)); // Xoay thuận
            else
                newCells.Add(new Vector2Int(cell.y, -cell.x)); // Xoay ngược
        }

        // Lấy vị trí trong grid
        Vector2 pos = selectedObject.transform.position;
        Vector2Int gridPos = Container.Instance.WorldToCell(pos);

        // Xóa state cũ tạm thời
        Container.Instance.SetState(pos, oldCells, false);

        // Kiểm tra state mới
        if (!Container.Instance.CheckState(pos, newCells))
        {
            Debug.Log("Không thể xoay vì va chạm.");
            Container.Instance.SetState(pos, oldCells, true); // Khôi phục lại
            return;
        }

        // Cập nhật occupiedCells
        gridItem.occupiedCellsStart = newCells;

        // Xoay sprite (góc mới theo từng lần 90 độ)
        float currentZ = selectedObject.transform.localEulerAngles.z;
        float newZ = currentZ + 90f* direction;
        selectedObject.transform.localRotation = Quaternion.Euler(0, 0, newZ);

        // Đặt lại state mới
        Container.Instance.SetState(pos, newCells, true);

        Debug.Log("Xoay thành công.");
    }


}

public class GridItemUI : MonoBehaviour
{
    public GridItem gridItem;
}

public class GridItemObject : MonoBehaviour, IPointerClickHandler,IPointerDownHandler
{
    public GridItem gridItem;
    public bool canDrop;
    public delegate void ActionSelect(GameObject gameObject);
    public ActionSelect OnSelect;
    private Vector3 startPosition, currentMousePos;
    public void OnPointerClick(PointerEventData eventData)
    {
        //if(OnSelect != null)
        //{
        //    OnSelect?.Invoke(gameObject);
        //}
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        startPosition = transform.position;
        DragDropManager.instance.GetInput(out currentMousePos);
        DragDropManager.instance.SetDrag(gameObject);
        Container.Instance.SetState(
                new Vector2Int(Mathf.FloorToInt(startPosition.x), Mathf.FloorToInt(startPosition.y)),
                gridItem.occupiedCellsStart, false);

        DragDropManager.instance.EndDrag =
            (objDrag, canDrop) =>
            {
                Vector3 mousePos;
                DragDropManager.instance.GetInput(out mousePos);
                if (Vector3.Distance(mousePos, currentMousePos) < 5f)
                {
                    canDrop = false;
                }
                if (!canDrop)
                {
                    Container.Instance.SetState(
                        new Vector2Int(Mathf.FloorToInt(startPosition.x), Mathf.FloorToInt(startPosition.y)),
                        gridItem.occupiedCellsStart);
                    transform.position = startPosition;
                }
                OnSelect?.Invoke(gameObject);
                return canDrop;
            };
    }
}