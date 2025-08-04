using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEditor.Recorder.OutputPath;
using static UnityEngine.Rendering.DebugUI;

public class JoyStick : MonoBehaviour
{
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
    public void Show(RoomInfo room)
    {
        currentRoom = room;

        nameText.text = "Tên phòng: " + room.roomName;
        priceText.text = "Giá: " + room.price.ToString();
        sizeText.text = $"Kích thước: {room.size} ";

        panel.SetActive(true); // bật panel
    }
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
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
            SceneManager.LoadScene("PlayScene");
        }
    }
}
