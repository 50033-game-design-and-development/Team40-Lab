using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MarioMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float jumpForce = 7f;
    [SerializeField] bool allowJump = true;

    Rigidbody2D rb;
    bool grounded = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 2f;
    }

    void Update()
    {
        float move = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(move * moveSpeed, rb.linearVelocity.y);

        if (move != 0)
            transform.localScale = new Vector3(Mathf.Sign(move) * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);

        if (allowJump && grounded && Input.GetKeyDown(KeyCode.Space))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            grounded = false;
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.contacts[0].normal.y > 0.5f)
            grounded = true;
    }
}
