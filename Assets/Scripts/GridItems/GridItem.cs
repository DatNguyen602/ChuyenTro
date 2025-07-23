using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GridItem", menuName = "Grid/GridItem")]
public class GridItem : ScriptableObject
{
    public string itemName;
    public string id;
    public Sprite sprite, spriteTemp;
    public float spriteScale;
    public Vector2 spriteOffsetPercent;
    public List<Vector2Int> occupiedCells;

    public List<Vector2Int> occupiedCellsStart;
    public int price;
    public float brokenRate; // 0.0 - 1.0
}
