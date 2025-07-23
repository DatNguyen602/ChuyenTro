using UnityEngine;

[System.Serializable]
public class RoomData
{
    public string roomName;
    public long minPrice, maxPrice, price;
    public Sprite roomImage;
    public District district;
    public Vector2Int size = Vector2Int.one;
    [Range(0f, 1f)]
    public float bargainSuccessRate = 0.15f;
    public int bargainDiscountAmount = 100000;

    public enum District
    {
        DongDa,
        HaiBaTrung,
        CauGiay,
        BaDinh,
        HoanKiem,
        ThanhXuan,
        HoangMai,
        TayHo,
        LongBien,
        NamTuLiem,
        BacTuLiem,
        HaDong
    }
}
