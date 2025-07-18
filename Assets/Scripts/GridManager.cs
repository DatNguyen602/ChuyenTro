using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int gridWidth = 6;  // Width of the grid
    public int gridHeight = 8; // Height of the grid
    public GameObject cellPrefab; // Prefab for the grid cells
    public float cellSize = 1.0f; // Size of each cell
    private GameObject[,] gridCells; // 2D array to hold the grid cells
    private ItemShapeData[,] gridData; // 2D array to hold the grid data

    void Awake()
    {
        // Initialize the grid cells and data arrays
        gridCells = new GameObject[gridWidth, gridHeight];
        gridData = new ItemShapeData[gridWidth, gridHeight];

        // Create the grid cells, adjust Y to increase from bottom to top
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                GameObject cell = Instantiate(cellPrefab, transform);
                cell.transform.position = new Vector3(x * cellSize, (gridHeight - 1 - y) * cellSize, 0);
                gridCells[x, y] = cell;
            }
        }
    }

    public bool CanPlace(ItemShapeData itemShape, Vector2Int anchor)
    {
        // Check if the item shape can be placed at the specified position
        foreach (var local in itemShape.occupiedCells)
        {
            var pos = anchor + local;
            if (pos.x < 0 || pos.x >= gridWidth || pos.y < 0 || pos.y >= gridHeight || gridData[pos.x, pos.y] != null)
            {
                return false; // Out of bounds or cell already occupied
            }
        }
        return true; // Item can be placed
    }

    public void Place(ItemShapeData item, Vector2Int anchor)
    {
        foreach (var local in item.occupiedCells)
        {
            var pos = anchor + local;
            gridData[pos.x, pos.y] = item; // Store the item shape in the grid data
            // Highlight the placed cell
            gridCells[pos.x, pos.y].GetComponent<SpriteRenderer>().color = Color.red; // Mark as occupied
        }
    }

    public Vector2Int WorldToGrid(Vector3 worldPosition)
    {
        return new Vector2Int(
            Mathf.FloorToInt(worldPosition.x / cellSize),
            gridHeight - 1 - Mathf.FloorToInt(worldPosition.y / cellSize) // Adjust Y from bottom to top
        );
    }
}