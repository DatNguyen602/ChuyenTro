using UnityEngine;

// Manages grid cell state and visuals
public class GridCell : MonoBehaviour
{
    public Vector2Int gridPosition; // Grid coordinates
    public bool isOccupied = false; // Is cell occupied?
    public DraggableItem currentItem; // Item in cell
    public Color normalColor = Color.white; // Default color
    public Color highlightColor = Color.yellow; // Ideal cell color
    public Color validPlaceColor = Color.green; // Valid placement color
    public Color invalidPlaceColor = Color.red; // Invalid placement color
    private SpriteRenderer spriteRenderer; // Sprite renderer

    // Initialize sprite and position
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = CreateSquareSprite();
        }
        string[] parts = name.Split('_');
        gridPosition = new Vector2Int(int.Parse(parts[1]), int.Parse(parts[2]));
        UpdateVisual();
    }

    // Create 64x64 pixel sprite
    Sprite CreateSquareSprite()
    {
        Texture2D texture = new Texture2D(64, 64);
        Color[] colors = new Color[64 * 64];
        for (int i = 0; i < colors.Length; i++)
        {
            int x = i % 64;
            int y = i / 64;
            colors[i] = (x < 2 || x >= 62 || y < 2 || y >= 62) ? Color.black : Color.white;
        }
        texture.SetPixels(colors);
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f), 64f);
    }

    // Set cell occupied state
    public void SetOccupied(bool occupied, DraggableItem item = null)
    {
        isOccupied = occupied;
        currentItem = item;
        UpdateVisual();
    }

    // Highlight cell
    public void SetHighlight(bool highlight, bool canPlace = true)
    {
        if (spriteRenderer != null)
        {
            if (highlight)
            {
                spriteRenderer.color = gridPosition == Vector2Int.zero ? highlightColor : (isOccupied ? invalidPlaceColor : (canPlace ? validPlaceColor : invalidPlaceColor));
            }
            else
            {
                UpdateVisual();
            }
        }
    }

    // Update cell color
    private void UpdateVisual()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = gridPosition == Vector2Int.zero && !isOccupied ? highlightColor : (isOccupied ? invalidPlaceColor : normalColor);
        }
    }
}