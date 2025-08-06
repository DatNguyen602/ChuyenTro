using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class JoyStick : MonoBehaviour
{
    //code singleton
    public static JoyStick instance;
    public float moveSpeed = 5f;
    public Joystick joystick;
    private Rigidbody2D rb;
    public List<RoomInfo> roomList = new List<RoomInfo>();
    public GameObject panel;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI priceText;
    public TextMeshProUGUI sizeText;
    public Button buyButton;
    private RoomInfo currentRoom;
    public GameObject Shop;
    public GameObject container;
    public GameObject ItemCanvas;
    public void Show(RoomInfo room)
    {
        currentRoom = room;

        nameText.text = "Tên phòng: " + room.roomName;
        priceText.text = "Giá: " + room.price.ToString();
        sizeText.text = $"Kích thước: {room.size} ";

        panel.SetActive(true); // bật panel
    }
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    private void Update()
    {
        Move();
    }

    private void Move()
    {
        Vector2 direction = new Vector2(joystick.Horizontal, joystick.Vertical).normalized;
        rb.linearVelocity = direction * moveSpeed;

        // Optional: Flip player theo hướng
        if (direction.x != 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Sign(direction.x) * Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Xử lý va chạm với các đối tượng khác nếu cần
        if (collision.gameObject.tag == ("Tro"))
        {
            Show(roomList[0]);
        }
        else if (collision.gameObject.tag == ("Shop"))
        {
            Shop.SetActive(true); // Hiển thị cửa hàng khi va chạm với đối tượng có tag "Shop"
            BuyAndSell.instance.RenderBuyList(); // Hiển thị danh sách mua bán trong cửa hàng
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        // Xử lý khi rời khỏi va chạm với các đối tượng khác nếu cần
        if (collision.gameObject.tag == ("Tro"))
        {
            panel.SetActive(false); // ẩn panel khi không còn va chạm
        }
        else if (collision.gameObject.tag == ("Shop"))
        {
            Shop.SetActive(false); // ẩn cửa hàng khi không còn va chạm
        }
    }
    public void BuyRoom()
    {
        if (currentRoom != null)
        {
            TempRoomData.selectedRoom = currentRoom;

            for (int i = 0; i < GridManager.instance.gridItems.Count; i++)
            {
                GamePlayManager.instance.RendererList(
                    GridManager.instance.gridItems[i],
                    GridManager.instance.gridParent, i <
                    GridManager.instance.gridParent.childCount ? GridManager.instance.gridParent.GetChild(i).gameObject : null);
            }
            container.SetActive(true);
            ItemCanvas.SetActive(true); // Hiển thị canvas chứa item
            GamePlayManager.instance.virtualCamera.Follow = Container.Instance.transform;
            panel.SetActive(false); // ẩn panel sau khi mua phòng
            joystick.gameObject.SetActive(false); // ẩn nhân vật sau khi mua phòng
            GridManager.instance.RecheckRoomItem(); // Hiển thị các item trong grid
        }
    }
    public void TurnOffContainer()
    {
        GamePlayManager.instance.virtualCamera.Follow = gameObject.transform;
        container.SetActive(false); // ẩn container
        ItemCanvas.SetActive(false); // ẩn canvas chứa item
        foreach(var item in GridManager.instance.gridItemObjects)
        {
            item.gameObject.SetActive(false); // ẩn tất cả các item trong grid
        }
        joystick.gameObject.SetActive(true); // hiện lại nhân vật
    }
    public void TurnOffShop()
    {
        Shop.SetActive(false); // ẩn cửa hàng
        joystick.gameObject.SetActive(true); // hiện lại nhân vật
    }
}
