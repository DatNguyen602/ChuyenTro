using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JoyStick : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Joystick joystick;
    private Rigidbody2D rb;
    public List<RoomInfo> roomList = new List<RoomInfo>();
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
            SceneManager.LoadScene("PlayScene");
            Container.Instance.size=roomList[0].size;
        }
        else if (collision.gameObject.tag == ("Room"))
        {
            SceneManager.LoadScene("PlayScene");
            Container.Instance.size = roomList[1].size;
        }
    }
}
