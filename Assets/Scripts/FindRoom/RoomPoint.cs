using UnityEngine;
using UnityEngine.EventSystems;

public class RoomPoint : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private RoomData roomData;
    public RoomData RoomData
    {
        get => roomData;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"Clicked on room: {roomData?.roomName}");
        GamePlayManager.instance.RoomSelected = roomData;
        if (ManagerUI.instance != null && roomData != null)
        {
            ManagerUI.instance.roomDataInfor.text =
                $"<size=120%><b><color=#00bb00>{roomData.roomName}</color></b></size>\n\n" +
                $"<b><color=#888888>Giá thuê:</color></b> <color=#ff6600>{roomData.minPrice:N0} VND</color> - <color=#ff6600>{roomData.maxPrice:N0} VND</color>\n" +
                $"<b><color=#888888>Khu vực:</color></b> {roomData.district}\n" +
                $"<b><color=#888888>Kích thước:</color></b> {roomData.size.x} <color=#22bb22>x</color> {roomData.size.y} m²\n" +
                $"<b><color=#888888>Tỷ lệ đàm phán thành công:</color></b> <color=#3399ff>{roomData.bargainSuccessRate * 100:F0}%</color>\n" +
                $"<b><color=#888888>Lượng giảm giá:</color></b> <color=#009933>{roomData.bargainDiscountAmount:N0} VND</color>";
        }
    }
}
