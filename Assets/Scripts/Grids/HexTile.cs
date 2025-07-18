using System.Collections.Generic;
using UnityEngine;

public class HexTile : MonoBehaviour
{
    public Vector2Int axialCoord;
    private List<HexTile> neighbors = new();
    [SerializeField] private TileContent content;

    private void Awake()
    {
        ClearContent();
    }

    public void Init(Vector2Int coord)
    {
        axialCoord = coord;
        neighbors.Clear();
    }
    public void AddNeibors(HexTile tile)
    {
        neighbors.Add(tile);
    }
    public void SetContent(TileContent obj)
    {
        content = obj;
        obj.Setparent(this);
    }

    private void ClearContent()
    {
        if (content != null)
        {
            content.Setparent(null);
            content = null;
            Debug.Log("h");
        }

    }

    void OnMouseDown()
    {
        if (content != null)
        {
            content.OnInteract();
        }
    }

}
