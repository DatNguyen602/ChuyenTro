using UnityEditor;
using UnityEngine;

public class TileContent : MonoBehaviour
{
    [SerializeField] private HexTile tileParent;

    private void Start()
    {
        Vector2 origin = this.transform.position;

        Collider2D hitCol = Physics2D.OverlapPoint(origin);

        if (hitCol != null)
        {
            HexTile hex = hitCol.GetComponent<HexTile>();
            if (hex != null)
            {

                hex.SetContent(this);

                this.transform.position = hex.transform.position;

                return;
            }
        }

        Destroy(this.gameObject);
    }

    public void Setparent(HexTile tile)
    {
        tileParent = tile;
    }
    public HexTile Getparent()
    {
        return tileParent;
    }
    public virtual void OnInteract() { }
}
