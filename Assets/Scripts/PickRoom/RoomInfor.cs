using UnityEngine;

[System.Serializable]
public class RoomInfor
{
    public int price;
    public int locationScore;

    public static RoomInfor GenerateRandomRoom()
    {
        RoomInfor room = new RoomInfor();

        room.price = Random.Range(300, 1001);
        room.locationScore = Random.Range(50, 101);

        return room;
    }

    public override string ToString()
    {
        return $"Room Information:\n" +
               $"- Price: ${price}\n" +
               $"- Location Score: {locationScore}/100";
    }
}
