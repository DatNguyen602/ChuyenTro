using UnityEngine;

[CreateAssetMenu(fileName = "RoomInfo", menuName = "Scriptable Objects/RoomInfo")]
public class RoomInfo : ScriptableObject
{
    [Header("Th�ng tin ph�ng")]
    public string tenPhong;
    public int giaPhong;
    public int chieuDai;
    public int chieuRong;
}
