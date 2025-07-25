using GoogleMobileAds.Api;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GamePlayManager : MonoBehaviour
{
    public static GamePlayManager instance;

    [SerializeField] private List<GameObject> gameObjectsToManage;

    [SerializeField] private List<GridItem> gridItems = new List<GridItem>();

    private float timerInPlay = 0f;
    public float TimerInPlay
    {
        get { return timerInPlay; }
        set { timerInPlay = value; }
    }
    [Header("Player Stats")]
    [SerializeField] private long money =3000000;
    [SerializeField] private int stress = 0;   // 0 –10
    [SerializeField] private int emotion = 5;   // 0 –10
    [SerializeField] private int comfort = 20;  // 0 –100
    [SerializeField] private int relation = 0;  // 0 –100 (tuỳ NPC)

    /*–  2.  Thuộc tính public (đọc/ghi)  –*/
    public long Money
    {
        get => money;
        set => money = System.Math.Max(0, value);
    }

    public int Stress { get => stress; set { stress = Mathf.Clamp(value, 0, 10); } }
    public int Emotion { get => emotion; set { emotion = Mathf.Clamp(value, 0, 10); } }
    public int Comfort { get => comfort; set { comfort = Mathf.Clamp(value, 0, 100); } }
    public int Relation { get => relation; set { relation = Mathf.Clamp(value, 0, 100); } }

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

    private void Start()
    {
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
    public void AddMoney(long amount) => Money += amount;
    public void AddStress(int amount) => Stress += amount;
    public void AddEmotion(int amount) => Emotion += amount;
    public void AddComfort(int amount) => Comfort += amount;
    public void AddRelation(int amount) => Relation += amount;
}
