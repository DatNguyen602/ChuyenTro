using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GamePlayManager : MonoBehaviour
{
    public static GamePlayManager instance;

    [SerializeField] private List<GameObject> gameObjectsToManage;
    [SerializeField] private long startMoney = 3000000;
    [SerializeField] private List<GridItem> gridItems = new List<GridItem>();

    private float timerInPlay = 0f;
    public float TimerInPlay
    {
        get { return timerInPlay; }
        set { timerInPlay = value; }
    }
    private long money = 0;
    public long Money
    {
        get { return money; }
        set { money = value; }
    }

    private RoomData roomSelected;
    public RoomData RoomSelected
    {
        get { return roomSelected; }
        set { roomSelected = value; }
    }

    public GamePhase gamePhase = GamePhase.FindingRoom;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void LoadingScene(string sceneName)
    {
        AsyncOperation scene = SceneManager.LoadSceneAsync(sceneName);
        if(scene != null)
        {
            scene.allowSceneActivation = false;
            while (!scene.isDone)
            {
                if (scene.progress >= 0.9f)
                {
                    scene.allowSceneActivation = true;
                    return;
                }
            }
        }
    }

    public enum GamePhase
    {
        FindingRoom,     // Tìm phòng trọ
        Negotiating,     // Đàm phán giá
        Moving,          // Chuyển đồ
        RoomSetup,       // Sắp xếp đồ đạc trong phòng
        LivingSummary    // Tổng kết sau khi ổn định
    }
}
