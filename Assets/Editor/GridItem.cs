using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "GridItem", menuName = "Grid/GridItem")]
public class GridItem : ScriptableObject
{
    public string itemName;
    public string id;
    public Sprite sprite;
    public float spriteScale;
    public Vector2 spriteOffsetPercent;
    public List<Vector2Int> occupiedCells;
}
