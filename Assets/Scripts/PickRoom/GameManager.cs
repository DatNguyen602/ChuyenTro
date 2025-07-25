using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private RoomInfor roomInfor;

    [SerializeField] private RoomPicker roomPicker;

    [SerializeField] private TimeLine timeLine;

    [SerializeField] private PlayerStats playerStats;


    [SerializeField] private EventGenerator eventGenerator;


    private void Start()
    {

        roomPicker.OnTake += OnTakeRoom;
        timeLine.OnTimePass += OnTimeAdvanced;
    }


    private void OnTakeRoom(RoomInfor roomInfor)
    {
        this.roomInfor = roomInfor;
        eventGenerator.ShowChoices();
        eventGenerator.GenEvent(playerStats, roomInfor, timeLine);
    }

    private void OnTimeAdvanced(int t)
    {
        eventGenerator.GenEvent(playerStats, roomInfor, timeLine);
    }
}
