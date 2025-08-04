using UnityEngine;

[CreateAssetMenu(fileName = "RoomInfo", menuName = "ScriptableObjects/RoomInfo")]
public class RoomInfo : ScriptableObject
{
    public string roomName;
    public int price;
    public int width;
    public int height;

    public override string ToString()
    {
        return $"Tên phòng: {roomName}\nGiá: {price}k\nKích thước: {width} x {height} mét";
    }
}
