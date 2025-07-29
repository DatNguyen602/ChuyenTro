using UnityEngine;

[System.Serializable]
public class RoomInfor
{
    public int price;
    public int locationScore;
    public Vector2Int size = new Vector2Int(1, 1);

    public static RoomInfor GenerateRandomRoom()
    {
        RoomInfor room = new RoomInfor();

        room.price = Random.Range(300, 1001);
        room.locationScore = Random.Range(50, 101);
        room.size = new Vector2Int(Random.Range(3, 12), Random.Range(3, 12));

        return room;
    }

    public override string ToString()
    {
        return
            $"<align=center><b><size=52><color=#4CAF50>Room Information</color></size></b></align>\n\n" +

            $"<b><color=#ffffff>• Price:</color></b>     <color=#FFD700>${price}</color>\n" +
            $"<b><color=#ffffff>• Location:</color></b>  <color=#00BFFF>{locationScore}/100</color>\n" +
            $"<b><color=#ffffff>• Size:</color></b>      <color=#FFA07A>{size.x}m x {size.y}m</color>";
    }
}
