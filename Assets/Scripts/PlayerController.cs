
using UnityEngine;

public class PlayerController2D : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 11f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private SpriteRenderer playerSprite;

    private Rigidbody2D rb;
    private float moveDirection;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (playerSprite == null)
        {
            playerSprite = GetComponentInChildren<SpriteRenderer>();
        }
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(
            moveDirection * moveSpeed,
            rb.linearVelocity.y
        );

        // Разворачиваем только спрайт, а не Player
        if (playerSprite != null)
        {
            if (moveDirection < 0f)
                playerSprite.flipX = true;
            else if (moveDirection > 0f)
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
