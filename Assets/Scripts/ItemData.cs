using UnityEngine;
using System.Collections.Generic;

// ScriptableObject for item properties
[CreateAssetMenu(fileName = "ItemData", menuName = "ScriptableObjects/ItemData", order = 1)]
public class ItemData : ScriptableObject
{
    public string itemName; // Item name
    public Vector2Int size; // Grid size (cells)
    public List<Vector2Int> relativeCells; // Occupied cells
    public Sprite sprite; // Sprite for item
}