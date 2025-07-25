using System.Collections.Generic;
using UnityEngine;

public class HexTile : MonoBehaviour
{
    public Vector2Int axialCoord;
    public HexTileVisual HexTileVisual { get; private set; }
    
    [SerializeField]private List<HexTile> neighbors = new();
    [SerializeField] private TileContent content;

    private void Awake()
    {
        HexTileVisual = GetComponent<HexTileVisual>();
        ClearContent();
    }

    public void Init(Vector2Int coord)
    {
        axialCoord = coord;
        neighbors.Clear();
    }
    public void AddNeighbors(HexTile tile)
    {
        neighbors.Add(tile);
    }

    public bool Isneighbor(HexTile tile)
    {
        return neighbors.Contains(tile);
    }    

    public List<HexTile> GetNeighbors()
    {
        return neighbors;
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
        }

    }

    public void Interact()
    {
        if(content != null) 
        content.OnInteract();
    }    


}
