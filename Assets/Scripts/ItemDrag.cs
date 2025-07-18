using UnityEngine;
using UnityEngine.EventSystems;

public class ItemDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GridManager gridManager; // Reference to the GridManager
    public ItemShapeData data; // The item shape data to be dragged
    private Vector2Int anchorCell;
    private Vector3 dragOffset;

    void Awake()
    {
        if (data != null && data.itemSprite != null) // Kiểm tra data và sprite
        {
            GetComponent<SpriteRenderer>().sprite = data.itemSprite; // Gán sprite từ ItemShapeData
            GetComponent<BoxCollider2D>().size = data.boxSize; // Gán kích thước box
        }
        else
        {
            Debug.LogError("Data hoặc itemSprite trong ItemShapeData bị null!");
        }
        if (gridManager == null)
        {
            gridManager = GameObject.Find("GridManager").GetComponent<GridManager>(); // Tự động gán nếu null
        }
    }

    public void OnBeginDrag(PointerEventData e)
    {
        dragOffset = transform.position - Camera.main.ScreenToWorldPoint(e.position);
        dragOffset.z = 0; // Ensure z is zero for 2D
        if (GetComponent<SpriteRenderer>().sprite == null)
        {
            Debug.LogError("Sprite không được gán khi bắt đầu kéo!");
        }
    }

    public void OnDrag(PointerEventData e)
    {
        Vector3 world = Camera.main.ScreenToWorldPoint(e.position) + dragOffset;
        world.z = 0; // Ensure z is zero for 2D
        transform.position = world;
        anchorCell = gridManager.WorldToGrid(transform.position); // Sử dụng WorldToGrid từ GridManager
        HighlightCells(anchorCell); // Highlight cells based on the current anchor position
    }

    public void OnEndDrag(PointerEventData e)
    {
        anchorCell = gridManager.WorldToGrid(transform.position);
        if (gridManager.CanPlace(data, anchorCell)) // Kiểm tra có thể đặt không
        {
            gridManager.Place(data, anchorCell); // Place the item in the grid
            // Snap to grid with Y adjusted (from bottom to top)
            transform.position = new Vector3(anchorCell.x, (gridManager.gridHeight - 1 - anchorCell.y), 0) * gridManager.cellSize;
        }
        else
        {
            // Snap back to original position or a default position
            transform.position = new Vector3(0, 0, 0); // Đặt lại vị trí ban đầu
        }
    }

    private void HighlightCells(Vector2Int anchor)
    {
        foreach (Transform cell in gridManager.transform)
            cell.GetComponent<SpriteRenderer>().color = Color.white; // Reset all cells to white

        foreach (var local in data.occupiedCells)
        {
            var pos = anchor + local;
            if (pos.x >= 0 && pos.x < gridManager.gridWidth && pos.y >= 0 && pos.y < gridManager.gridHeight)
            {
                int index = pos.x + pos.y * gridManager.gridWidth; // Sử dụng gridWidth thay vì rows
                var cell = gridManager.transform.GetChild(index).GetComponent<SpriteRenderer>();
                cell.color = gridManager.CanPlace(data, anchor) ? Color.green : Color.red; // Highlight valid cells in green, invalid in red
            }
        }
    }
}