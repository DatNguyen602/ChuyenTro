using System.Collections.Generic;
using TMPro;
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
            GridItemUI gridItemUI = (results[0]).gameObject.GetComponent<GridItemUI>();
            if (gridItemUI == null)
            {
                Debug.Log(results[0].gameObject.name);
                return;
            }
            itemUISelected = results[0].gameObject;
            Debug.Log(gridItemUI.gridItem.name);
            itemUISelected.GetComponent<Image>().color = Color.red;
            itemSelectedData = gridItemUI.gridItem;
            if(scrollRect != null)
            {
                scrollRect.enabled = false;
            }
        }
        else if(Input.GetMouseButtonUp(0))
        {
            if(itemUISelected != null)
            {
                itemUISelected.GetComponent<Image>().color = new Color32(56, 56, 56, 115);
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
                    itemSelected.transform.GetChild(0).localScale = new Vector2(itemSelectedData.spriteScale, itemSelectedData.spriteScale);
                    gridItems.Remove(itemSelectedData);
                    itemUISelected = null;
                    RendererList();
                }
            }
            if(itemSelected && itemSelectedData)
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePos.z = 0; // nếu game 2D
                itemSelected.transform.position = mousePos;
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