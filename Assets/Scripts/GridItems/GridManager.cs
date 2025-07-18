using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class GridManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject gridItemPrefab, itemTemplate;
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

    void Start()
    {
        RendererList();
        for(int i =0;i<10;i++)
        {
            GameObject item = Instantiate(itemTemplate);
            item.SetActive(false);
            gridItemObjects.Add(item);
        }
    }

    /* ----- biến cần có ----- */
    private GridItem draggingData;      // dữ liệu item đang kéo
    private GameObject spawnedObj;        // obj đã spawn (nếu có)
    private bool isDragging = false;
    /* ------------------------ */

    private void Update()
    {
        /* Mouse Down: bắt đầu chọn item trong UI */
        if (Input.GetMouseButtonDown(0))
        {
            PointerEventData pd = new PointerEventData(eventSystem) { position = Input.mousePosition };
            var hits = new List<RaycastResult>();
            raycaster.Raycast(pd, hits);

            GridItemUI hitUI = hits.Count > 0 ? hits[0].gameObject.GetComponent<GridItemUI>() : null;
            if (hitUI)
            {
                draggingData = hitUI.gridItem;
                isDragging = true;
                hitUI.GetComponent<Image>().color = Color.red;   // highlight
                if (scrollRect) scrollRect.enabled = false;
            }
        }

        /* Mouse Hold: chuột rời UI → spawn obj (nhưng cứ giữ dữ liệu trong list) */
        if (Input.GetMouseButton(0) && isDragging && spawnedObj == null && !isHovered)
        {
            spawnedObj = GetItemObject();
            spawnedObj.SetActive(true);
            spawnedObj.GetComponentInChildren<SpriteRenderer>().sprite = draggingData.spriteTemp;
            spawnedObj.transform.GetChild(0).localScale = Vector2.one * draggingData.spriteScale;

            var gio = spawnedObj.AddComponent<GridItemObject>();
            gio.gridItem = draggingData;

            DragDropManager.instance.SetDrag(spawnedObj);
        }

        /* Mouse Up: quyết định commit hay hủy */
        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            if (isHovered)
            {
                /* ===== HỦY KÉO: thả lại UI ===== */
                if (spawnedObj) spawnedObj.SetActive(false);  // trả về pool
                                                              // <<< CHỈ 2 DÒNG then chốt >>>
                if (!gridItems.Contains(draggingData))
                    gridItems.Add(draggingData);              // ➊ add lại
                RendererList();                               // ➋ vẽ lại
            }
            else
            {
                /* ===== COMMIT: thả ngoài UI ===== */
                gridItems.Remove(draggingData);               // xóa vĩnh viễn
                RendererList();
            }

            /* --- reset --- */
            draggingData = null;
            spawnedObj = null;
            isDragging = false;
            if (scrollRect) scrollRect.enabled = true;
        }
    }


    private GameObject GetItemObject()
    {
        foreach (GameObject item in gridItemObjects)
        {
            if (!item.activeSelf)
            {
                item.SetActive(true);
                return item;
            }
        }
        GameObject newItem = Instantiate(itemTemplate, gridParent);
        gridItemObjects.Add(newItem);
        return newItem;
    }

    private void RendererList()
    {
        for (int i = 0; i < gridParent.childCount; i++)
        {
            Destroy(gridParent.GetChild(i).gameObject);
        }
        foreach (GridItem i in gridItems)
        {
            float pixelsPerUnit = Mathf.Max(i.sprite.rect.width, i.sprite.rect.height);
            Debug.Log(i.name + ": " + pixelsPerUnit);
            i.spriteTemp = Sprite.Create(i.sprite.texture, new Rect(i.sprite.rect.x, i.sprite.rect.y, i.sprite.rect.width, i.sprite.rect.height),
                              new Vector2(0.5f, 0.5f),
                              pixelsPerUnit: pixelsPerUnit);
            GameObject item = Instantiate(gridItemPrefab, gridParent);
            item.transform.GetChild(0).GetComponent<Image>().sprite = i.spriteTemp;
            item.GetComponentInChildren<TextMeshProUGUI>().text = i.name;

            GridItemUI gridItemUI = item.AddComponent<GridItemUI>();
            gridItemUI.gridItem = i;
        }
    }
}

public class GridItemUI : MonoBehaviour
{
    public GridItem gridItem;
}

public class GridItemObject : MonoBehaviour
{
    public GridItem gridItem;
}