using System.Collections.Generic;
using UnityEngine;

public static class TempRoomData
{
    public static RoomInfo selectedRoom;
    public static bool isSetPos;
    public static Vector3 userPosition;

    public static List<ItemInf> itemList = new List<ItemInf>();
}

public class ItemInf
{
    public GridItem item;
    public Vector2 pos;
    public int dir;
    public ItemInf(GridItem item, Vector2 pos, int dir)
    {
        this.item = item;
        this.pos = pos;
        this.dir = dir;
    }
}