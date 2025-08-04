using UnityEngine;

[CreateAssetMenu(fileName = "NewRoomInfo", menuName = "ScriptableObjects/RoomInfo")]
public class RoomInfo : ScriptableObject
{
    public string roomName;
    public int price;
    public Vector2Int size;
}
