using System.Collections.Generic;
using UnityEngine;

// Handles item drag-and-drop
public class DraggableItem : MonoBehaviour
{
    public ItemData itemData; // Item data
    public bool isPlaced = false; // Is item placed?
    public Color normalColor = Color.white; // Default color
    public Color dragColor = Color.yellow; // Drag color
    public Color validColor = Color.green; // Valid placement color
    public Color invalidColor = Color.red; // Invalid placement color
    private Vector2Int currentGridPosition; // Current grid position
    private Vector3 originalPosition; // Initial position
    private bool isDragging = false; // Is dragging?
    private int rotationState = 0; // Rotation state (0-3)
    private GridManager gridManager; // Grid manager reference
    private Camera mainCamera; // Main camera
    private SpriteRenderer spriteRenderer; // Sprite renderer

    // Initialize item
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }
        if (itemData.sprite != null)
        {
            spriteRenderer.sprite = itemData.sprite;
        }
        else
        {
            Debug.LogWarning($"No sprite assigned for {itemData.itemName}, using default sprite");
        }
        gridManager = FindObjectOfType<GridManager>();
        mainCamera = Camera.main;
        originalPosition = transform.position;
        if (GetComponent<Collider2D>() == null)
        {
            BoxCollider2D collider = gameObject.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(itemData.size.x * 0.9f, itemData.size.y * 0.9f);
        }
        UpdateVisual();
        Debug.Log($"Created {name} with size {itemData.size}, cells: {string.Join(", ", itemData.relativeCells)}");
    }

    // Start dragging
    void OnMouseDown()
    {
        if (gridManager == null) return;
        isDragging = true;
        if (isPlaced)
        {
            gridManager.RemoveItem(this);
        }
        transform.position = new Vector3(transform.position.x, transform.position.y, -1);
        gridManager.SetDraggedItem(this);
        UpdateVisual();
    }

    // Drag item
    void OnMouseDrag()
    {
        if (!isDragging || gridManager == null) return;
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = -1;
        transform.position = mouseWorldPos;
        ShowPlacementPreview();
    }

    // Place or snap back
    void OnMouseUp()
    {
        if (!isDragging || gridManager == null) return;
        isDragging = false;
        Vector2Int gridPos = gridManager.WorldToGridPosition(transform.position);
        if (gridManager.CanPlaceItemAt(gridPos, GetRotatedCells()))
        {
            gridManager.PlaceItem(this, gridPos);
            transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        }
        else
        {
            transform.position = originalPosition;
            isPlaced = false;
            Debug.Log($"Cannot place {name} at {gridPos}");
        }
        gridManager.ClearAllHighlights();
        gridManager.SetDraggedItem(null);
        UpdateVisual();
    }

    // Show placement preview
    void ShowPlacementPreview()
    {
        if (gridManager == null) return;
        Vector2Int gridPos = gridManager.WorldToGridPosition(transform.position);
        gridManager.ClearAllHighlights();
        gridManager.HighlightCells(gridPos, GetRotatedCells(), true);
        spriteRenderer.color = gridManager.CanPlaceItemAt(gridPos, GetRotatedCells()) ? validColor : invalidColor;
    }

    // Rotate item
    public void Rotate()
    {
        rotationState = (rotationState + 1) % 4;
        transform.Rotate(0, 0, 90);
        Vector2Int newSize = new Vector2Int(itemData.size.y, itemData.size.x);
        itemData.size = newSize;
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        collider.size = new Vector2(itemData.size.x * 0.9f, itemData.size.y * 0.9f);
    }

    // Rotate back
    public void RotateBack()
    {
        rotationState = (rotationState + 3) % 4;
        transform.Rotate(0, 0, -90);
        Vector2Int newSize = new Vector2Int(itemData.size.y, itemData.size.x);
        itemData.size = newSize;
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        collider.size = new Vector2(itemData.size.x * 0.9f, itemData.size.y * 0.9f);
    }

    // Update item color
    void UpdateVisual()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = isDragging ? (gridManager.CanPlaceItemAt(gridManager.WorldToGridPosition(transform.position), GetRotatedCells()) ? validColor : invalidColor) : normalColor;
        }
    }

    // Get item data
    public ItemData GetItemData()
    {
        return itemData;
    }

    // Set item data
    public void SetItemData(ItemData data)
    {
        itemData = data;
        if (spriteRenderer != null && itemData.sprite != null)
        {
            spriteRenderer.sprite = itemData.sprite;
        }
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider != null)
        {
            collider.size = new Vector2(itemData.size.x * 0.9f, itemData.size.y * 0.9f);
        }
    }

    // Get item size
    public Vector2Int GetItemSize()
    {
        return itemData.size;
    }

    // Get grid position
    public Vector2Int GetGridPosition()
    {
        return currentGridPosition;
    }

    // Set grid position
    public void SetGridPosition(Vector2Int position)
    {
        currentGridPosition = position;
    }

    // Get rotation state
    public int GetRotationState()
    {
        return rotationState;
    }

    // Set rotation state
    public void SetRotationState(int state)
    {
        while (rotationState != state)
        {
            Rotate();
        }
    }

    // Get original position
    public Vector3 GetOriginalPosition()
    {
        return originalPosition;
    }

    // Get rotated cells
    public List<Vector2Int> GetRotatedCells()
    {
        List<Vector2Int> rotatedCells = new List<Vector2Int>();
        foreach (Vector2Int cell in itemData.relativeCells)
        {
            switch (rotationState)
            {
                case 0: rotatedCells.Add(cell); break;
                case 1: rotatedCells.Add(new Vector2Int(-cell.y, cell.x)); break;
                case 2: rotatedCells.Add(new Vector2Int(-cell.x, -cell.y)); break;
                case 3: rotatedCells.Add(new Vector2Int(cell.y, -cell.x)); break;
            }
        }
        return rotatedCells;
    }
}