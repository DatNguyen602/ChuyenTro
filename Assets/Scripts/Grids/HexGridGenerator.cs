#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
using UnityEngine;

public class HexGridGenerator : MonoBehaviour
{
    [Header("Grid Settings")]
    public int gridSize = 8;
    public float hexSize = 1f;

    [Header("Prefabs")]
    public GameObject hexTilePrefab;

    private Dictionary<Vector2Int, HexTile> hexMap = new();

    private Vector2Int[] directions = new Vector2Int[] {
        new(+1,  0),  // East
        new(+1, -1),  // Southeast
        new( 0, -1),  // Southwest
        new(-1,  0),  // West
        new(-1, +1),  // Northwest
        new( 0, +1)   // Northeast
    };

    public void GenerateHexGrid()
    {
        ClearExisting();
        int radius = Mathf.Max(gridSize, gridSize) / 2;
        float hexWidth = hexSize * Mathf.Sqrt(3f);
        float hexHeight = hexSize * 2f;

        for (int q = -radius; q <= radius; q++)
        {
            for (int r = -radius; r <= radius; r++)
            {
                int s = -q - r;
                if (Mathf.Abs(s) > radius) continue;

                Vector2Int axialCoord = new(q, r);
                Vector3 worldPos = AxialToWorld(q, r);

                GameObject hexGO = Instantiate(hexTilePrefab, worldPos, Quaternion.identity, transform);
                hexGO.name = $"Hex_{q}_{r}";

                HexTile tile = hexGO.GetComponent<HexTile>();
                tile.Init(axialCoord);
                hexMap[axialCoord] = tile;
            }
        }
        AssignNeighbors();
    }

    private void ClearExisting()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
        hexMap.Clear();
    }


    private void AssignNeighbors()
    {
        foreach (var cell in hexMap)
        {
            HexTile tile = cell.Value;
            foreach (var dir in directions)
            {
                Vector2Int neighborCoord = tile.axialCoord + dir;
                if (hexMap.TryGetValue(neighborCoord, out HexTile neighbor))
                {
                    tile.AddNeibors(neighbor);
                }
            }
        }
    }

    Vector3 AxialToWorld(int q, int r)
    {
        float x = hexSize * Mathf.Sqrt(3) * (q + r / 2f);
        float y = hexSize * 1.5f * r;
        return new Vector3(x, y, 0);
    }

}
