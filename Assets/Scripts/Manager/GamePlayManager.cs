using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GamePlayManager : MonoBehaviour
{
    public static GamePlayManager instance;

    [SerializeField] private RoomPicker roomPicker;

    [SerializeField] private GameObject gridContainer;

    [SerializeField] private GameObject itemsCanvas;

    [SerializeField] private List<GameObject> gameObjectsToManage;

    [SerializeField] private List<GridItem> gridItems = new List<GridItem>();
    public GameObject gridItemPrefab;
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

    private RoomInfor roomInfor;
    public long Money
    {
        get => money;
        set => money = System.Math.Max(0, value);
    }

    public int Stress { get => stress; set { stress = Mathf.Clamp(value, 0, 10); } }
    public int Emotion { get => emotion; set { emotion = Mathf.Clamp(value, 0, 10); } }
    public int Comfort { get => comfort; set { comfort = Mathf.Clamp(value, 0, 100); } }
    public int Relation { get => relation; set { relation = Mathf.Clamp(value, 0, 100); } }

    

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
        roomPicker.OnTake += OnTakeRoom;
    }


    private void OnTakeRoom(RoomInfor roomInfor)
    {
        this.roomInfor = roomInfor;
        gridContainer.SetActive(true);
        itemsCanvas.SetActive(true);
        
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
    public GameObject RendererList(GridItem i, Transform Parent, GameObject item = null)
    {
        float pixelsPerUnit = Mathf.Max(i.sprite.rect.width, i.sprite.rect.height);
        i.spriteTemp = Sprite.Create(i.sprite.texture, new Rect(i.sprite.rect.x, i.sprite.rect.y, i.sprite.rect.width, i.sprite.rect.height),
                            new Vector2(0.5f, 0.5f),
                            pixelsPerUnit: pixelsPerUnit);
        if (!item) item = Instantiate(gridItemPrefab, Parent);
        item.SetActive(true);
        item.transform.GetChild(0).GetComponent<Image>().sprite = i.spriteTemp;
        item.GetComponentInChildren<TextMeshProUGUI>().text = i.name;
        i.occupiedCellsStart = new List<Vector2Int>(i.occupiedCells);

        GridItemUI gridItemUI = item.GetComponent<GridItemUI>() ?? item.AddComponent<GridItemUI>();
        gridItemUI.gridItem = i;
        return item;
    }
}
