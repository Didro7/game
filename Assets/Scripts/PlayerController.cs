
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 11f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private SpriteRenderer playerSprite;

    private Rigidbody2D rb;
    private float moveDirection;

    private float keyboardDirection;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (playerSprite == null)
        {
            playerSprite = GetComponentInChildren<SpriteRenderer>();
        }
    }

    private void Update()
    {
        keyboardDirection = 0f;

        if (Input.GetKey(KeyCode.A) ||
            Input.GetKey(KeyCode.LeftArrow))
        {
            keyboardDirection -= 1f;
        }

        if (Input.GetKey(KeyCode.D) ||
            Input.GetKey(KeyCode.RightArrow))
        {
            keyboardDirection += 1f;
        }

        if (Input.GetKeyDown(KeyCode.Space) ||
            Input.GetKeyDown(KeyCode.W) ||
            Input.GetKeyDown(KeyCode.UpArrow))
        {
            Jump();
        }
    }

    private void FixedUpdate()
    {
        float horizontalInput = keyboardDirection != 0f
            ? keyboardDirection
            : moveDirection;

        rb.linearVelocity = new Vector2(
            horizontalInput * moveSpeed,
            rb.linearVelocity.y
        );

        if (playerSprite != null)
        {
            if (horizontalInput < 0f)
                playerSprite.flipX = true;
            else if (horizontalInput > 0f)
                playerSprite.flipX = false;
        }
    }

    public void LeftDown()
    {
        moveDirection = -1f;
    }

    public void LeftUp()
    {
        if (moveDirection < 0f)
            moveDirection = 0f;
    }

    public void RightDown()
    {
        moveDirection = 1f;
    }

    public void RightUp()
    {
        if (moveDirection > 0f)
            moveDirection = 0f;
    }

    public void Jump()
    {
        Debug.Log("Jump button pressed!");

        if (groundCheck == null)
        {
            Debug.LogWarning("GroundCheck is missing!");
            return;
        }

        bool isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            0.25f,
            groundLayer
        );

        Debug.Log("Is Grounded: " + isGrounded);

        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                jumpForce
            );
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(
            groundCheck.position,
            0.25f
        );
    }
}
