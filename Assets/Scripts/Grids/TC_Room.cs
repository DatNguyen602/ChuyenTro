using UnityEngine;

public class TC_Room : TileContent
{
    [SerializeField] private float price;

    public override void OnInteract()
    {
        UIManager.Instance.RoomUI.ShowRoomInfor(price);
    }
}
