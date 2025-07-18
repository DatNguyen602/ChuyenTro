using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ItemShapeData", menuName = "Scriptable Objects/ItemShapeData")]
public class ItemShapeData : ScriptableObject
{
    public Sprite itemSprite; // The sprite representing the item shape
    public Vector2 boxSize; // The size of the bounding box for the item shape
    public List<Vector2Int> occupiedCells; // List of grid cells occupied by the item shape

}
