using UnityEngine;
using System.Collections.Generic;

// Manages grid and items, Singleton
public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; } // Singleton instance
    public List<ItemData> items; // List of items
    public int gridWidth = 6; // Grid width
    public int gridHeight = 8; // Grid height
    public float cellSize = 64f; // World cell size (pixels)
    private GridCell[,] gridCells; // Grid array
    private List<DraggableItem> placedItems = new List<DraggableItem>(); // Placed items
    private Stack<List<(DraggableItem, Vector2Int, int)>> undoStack = new Stack<List<(DraggableItem, Vector2Int, int)>>(); // Undo stack
    private DraggableItem draggedItem; // Current dragged item

    // Set up Singleton
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Initialize grid and items
    void Start()
    {
        CreateGrid();
        CreateItems();
    }

    // Create grid
    void CreateGrid()
    {
        gridCells = new GridCell[gridWidth, gridHeight];
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector3 worldPos = new Vector3(x * cellSize, y * cellSize, 0);
                GameObject cellObj = new GameObject($"Cell_{x}_{y}");
                cellObj.transform.position = worldPos;
                cellObj.transform.parent = transform;
                GridCell cell = cellObj.AddComponent<GridCell>();
                gridCells[x, y] = cell;
            }
        }
        Debug.Log($"Created grid {gridWidth}x{gridHeight}");
    }

    // Create items
    void CreateItems()
    {
        float offsetY = 0;
        foreach (ItemData itemData in items)
        {
            GameObject itemObj = new GameObject(itemData.itemName);
            itemObj.transform.position = new Vector3(-5 * cellSize, offsetY, 0);
            DraggableItem item = itemObj.AddComponent<DraggableItem>();
            item.SetItemData(itemData);
            offsetY += itemData.size.y * cellSize + 32f;
            Debug.Log($"Created item {itemData.itemName} at (-5 * {cellSize}, {offsetY})");
        }
    }

    // Handle input
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && draggedItem != null)
        {
            RotateItem(draggedItem);
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            UndoLastAction();
        }
    }

    // Convert world to grid position
    public Vector2Int WorldToGridPosition(Vector3 worldPosition)
    {
        int x = Mathf.RoundToInt(worldPosition.x / cellSize);
        int y = Mathf.RoundToInt(worldPosition.y / cellSize);
        return new Vector2Int(x, y);
    }

    // Convert grid to world position
    public Vector3 GridToWorldPosition(Vector2Int gridPosition)
    {
        return new Vector3(gridPosition.x * cellSize, gridPosition.y * cellSize, 0);
    }

    // Check if position is valid
    public bool IsValidGridPosition(Vector2Int position)
    {
        return position.x >= 0 && position.x < gridWidth && position.y >= 0 && position.y < gridHeight;
    }

    // Check if item can be placed
    public bool CanPlaceItemAt(Vector2Int startPos, List<Vector2Int> relativeCells)
    {
        foreach (Vector2Int cell in relativeCells)
        {
            Vector2Int checkPos = startPos + cell;
            if (!IsValidGridPosition(checkPos) || gridCells[checkPos.x, checkPos.y].isOccupied)
            {
                Debug.LogWarning($"Cannot place at {checkPos}: Out of bounds or occupied");
                return false;
            }
        }
        Debug.Log($"Can place at {startPos}");
        return true;
    }

    // Place item on grid
    public void PlaceItem(DraggableItem item, Vector2Int startPos)
    {
        SaveStateForUndo();
        List<Vector2Int> relativeCells = item.GetRotatedCells();
        foreach (Vector2Int cell in relativeCells)
        {
            Vector2Int cellPos = startPos + cell;
            gridCells[cellPos.x, cellPos.y].SetOccupied(true, item);
            Debug.Log($"Occupied cell {cellPos}");
        }
        item.SetGridPosition(startPos);
        Vector3 worldPos = GridToWorldPosition(startPos);
        Vector2 itemCenter = new Vector2((item.GetItemSize().x - 1) * cellSize * 0.5f, (item.GetItemSize().y - 1) * cellSize * 0.5f);
        item.transform.position = worldPos + new Vector3(itemCenter.x, itemCenter.y, 0);
        item.isPlaced = true;
        if (!placedItems.Contains(item))
        {
            placedItems.Add(item);
        }
        Debug.Log($"Placed {item.name} at {startPos}");
    }

    // Remove item from grid
    public void RemoveItem(DraggableItem item)
    {
        if (!item.isPlaced) return;
        List<Vector2Int> relativeCells = item.GetRotatedCells();
        Vector2Int startPos = item.GetGridPosition();
        foreach (Vector2Int cell in relativeCells)
        {
            Vector2Int cellPos = startPos + cell;
            if (IsValidGridPosition(cellPos))
            {
                gridCells[cellPos.x, cellPos.y].SetOccupied(false);
                Debug.Log($"Cleared cell {cellPos}");
            }
        }
        item.isPlaced = false;
        placedItems.Remove(item);
        Debug.Log($"Removed {item.name} from grid");
    }

    // Highlight cells
    public void HighlightCells(Vector2Int startPos, List<Vector2Int> relativeCells, bool highlight)
    {
        bool canPlace = CanPlaceItemAt(startPos, relativeCells);
        foreach (Vector2Int cell in relativeCells)
        {
            Vector2Int cellPos = startPos + cell;
            if (IsValidGridPosition(cellPos))
            {
                gridCells[cellPos.x, cellPos.y].SetHighlight(highlight, canPlace);
                if (highlight)
                    Debug.Log($"Highlighting cell {cellPos}: {(canPlace ? "Valid" : "Invalid")}");
            }
        }
    }

    // Clear all highlights
    public void ClearAllHighlights()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                gridCells[x, y].SetHighlight(false);
            }
        }
    }

    // Get cell at position
    public GridCell GetCell(Vector2Int gridPos)
    {
        return IsValidGridPosition(gridPos) ? gridCells[gridPos.x, gridPos.y] : null;
    }

    // Rotate item
    void RotateItem(DraggableItem item)
    {
        SaveStateForUndo();
        Vector2Int oldPos = item.GetGridPosition();
        bool wasPlaced = item.isPlaced;
        if (wasPlaced)
            RemoveItem(item);
        item.Rotate();
        List<Vector2Int> newCells = item.GetRotatedCells();
        if (!wasPlaced || CanPlaceItemAt(oldPos, newCells))
        {
            if (wasPlaced)
                PlaceItem(item, oldPos);
        }
        else
        {
            item.RotateBack();
            if (wasPlaced)
                PlaceItem(item, oldPos);
            Debug.Log($"Cannot rotate {item.name}: Not enough space!");
        }
    }

    // Save state for undo
    void SaveStateForUndo()
    {
        List<(DraggableItem, Vector2Int, int)> state = new List<(DraggableItem, Vector2Int, int)>();
        foreach (DraggableItem item in placedItems)
        {
            state.Add((item, item.GetGridPosition(), item.GetRotationState()));
        }
        undoStack.Push(state);
    }

    // Revert to previous state
    void UndoLastAction()
    {
        if (undoStack.Count == 0)
        {
            Debug.Log("No actions to undo!");
            return;
        }
        List<(DraggableItem, Vector2Int, int)> state = undoStack.Pop();
        foreach (DraggableItem item in placedItems)
        {
            RemoveItem(item);
            item.transform.position = item.GetOriginalPosition();
            item.isPlaced = false;
        }
        placedItems.Clear();
        foreach (var (item, pos, rotation) in state)
        {
            item.SetRotationState(rotation);
            PlaceItem(item, pos);
        }
        Debug.Log("Undo successful!");
    }

    // Set current dragged item
    public void SetDraggedItem(DraggableItem item)
    {
        draggedItem = item;
    }
}