using UnityEngine;

public class JoyStick : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Joystick joystick;
    private Rigidbody2D rb;

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
}
