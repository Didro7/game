
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class OrcEnemy : MonoBehaviour
{
    private enum State
    {
        Idle,
        WindUp,
        Dash,
        Stunned,
        Cooldown
    }

    [Header("Objects")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform leftLimit;
    [SerializeField] private Transform rightLimit;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Sprites")]
    [SerializeField] private Sprite idleSprite;
    [SerializeField] private Sprite[] walkFrames;
    [SerializeField] private Sprite attackSprite;
    [SerializeField] private Sprite hurtSprite;
    [SerializeField] private bool artFacesRight = true;


    [Header("Enemy Data")]
    [SerializeField] private EnemyStats stats;
    private float detectionRange => stats.detectionRange;
    private float detectionHeight => stats.detectionHeight;
    private float windUpTime => stats.windUpTime;
    private float dashSpeed => stats.dashSpeed;
    private float dashDuration => stats.dashDuration;
    private float stunTime => stats.stunTime;
    private float cooldownTime => stats.cooldownTime;
    private float edgeMargin => stats.edgeMargin;
    private int damage => stats.damage;
    private float animationSpeed => stats.animationSpeed;

    private Rigidbody2D rb;
    private State currentState;
    private float stateTimer;
    private int direction = 1;
    private Color normalColor;

    private void Awake()
    {

        if (stats == null)
        {
            Debug.LogError("Orc Stats is not assigned!", this);
            enabled = false;
            return;
        }

        rb = GetComponent<Rigidbody2D>();

        if (spriteRenderer != null)
            normalColor = spriteRenderer.color;
    }

    private void Start()
    {
        ChangeState(State.Idle);
    }

    private void Update()
    {
        stateTimer += Time.deltaTime;

       
        if (currentState == State.Idle && player != null)
            direction = player.position.x >= transform.position.x ? 1 : -1;

        switch (currentState)
        {
            case State.Idle:
                if (PlayerIsNear() && IsOnScreen())
                    ChangeState(State.WindUp);
                break;

            case State.WindUp:
                if (!IsOnScreen())
                {
                    ChangeState(State.Cooldown);
                }
                else if (stateTimer >= windUpTime)
                {
                    ChangeState(State.Dash);
                }
                break;

            case State.Dash:
                if (stateTimer >= dashDuration ||
                    !IsOnScreen())
                {
                    ChangeState(State.Stunned);
                }
                break;

            case State.Stunned:
                if (stateTimer >= stunTime)
                    ChangeState(State.Cooldown);
                break;

            case State.Cooldown:
                if (stateTimer >= cooldownTime)
                    ChangeState(State.Idle);
                break;
        }

        UpdateAnimation();
    }

    private void FixedUpdate()
    {
        float horizontalSpeed = 0f;

        if (currentState == State.Dash)
        {
            if (leftLimit == null ||
                rightLimit == null)
            {
                ChangeState(State.Stunned);
            }
            else
            {
                float minX = Mathf.Min(
                    leftLimit.position.x,
                    rightLimit.position.x
                ) + edgeMargin;

                float maxX = Mathf.Max(
                    leftLimit.position.x,
                    rightLimit.position.x
                ) - edgeMargin;

                float nextX = rb.position.x +
                    direction * dashSpeed *
                    Time.fixedDeltaTime;

                if (nextX < minX || nextX > maxX)
                {
                    ChangeState(State.Stunned);
                }
                else
                {
                    horizontalSpeed =
                        direction * dashSpeed;
                }
            }
        }

        rb.linearVelocity = new Vector2(
            horizontalSpeed,
            rb.linearVelocity.y
        );
    }

    private bool PlayerIsNear()
    {
        if (player == null)
            return false;

        float dx = Mathf.Abs(
            player.position.x - transform.position.x
        );

        float dy = Mathf.Abs(
            player.position.y - transform.position.y
        );

        return dx <= detectionRange &&
               dy <= detectionHeight;
    }

    private bool IsOnScreen()
    {
        Camera cam = Camera.main;

        if (cam == null)
            return false;

        Vector3 point =
            cam.WorldToViewportPoint(transform.position);

        return point.z > 0f &&
               point.x >= 0f &&
               point.x <= 1f &&
               point.y >= 0f &&
               point.y <= 1f;
    }

    private void OnCollisionEnter2D(
        Collision2D collision)
    {
        TryDamagePlayer(collision.collider);
    }

    private void OnCollisionStay2D(
        Collision2D collision)
    {
        TryDamagePlayer(collision.collider);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryDamagePlayer(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TryDamagePlayer(other);
    }

    private void TryDamagePlayer(Collider2D other)
    {
        if (currentState != State.Dash)
            return;

        PlayerHealth health =
            other.GetComponentInParent<PlayerHealth>();

        if (health != null)
        {
            health.TakeDamage(damage);
            ChangeState(State.Stunned);
        }
    }

    private void ChangeState(State next)
    {
        currentState = next;
        stateTimer = 0f;

        Debug.Log("Orc State: " + currentState);
    }

    private void UpdateAnimation()
    {
        if (spriteRenderer == null)
            return;

        spriteRenderer.flipX = artFacesRight
            ? direction < 0
            : direction > 0;

        spriteRenderer.color =
            currentState == State.WindUp
            ? new Color(1f, 0.3f, 0.3f)
            : normalColor;

        Sprite nextSprite = idleSprite;

        if (currentState == State.WindUp)
        {
            nextSprite = idleSprite;
        }
        else if (currentState == State.Dash)
        {
            nextSprite = attackSprite;
        }
        else if (currentState == State.Stunned)
        {
            if (hurtSprite != null)
                nextSprite = hurtSprite;
        }

        if (nextSprite != null)
            spriteRenderer.sprite = nextSprite;
    }

    public void SetPlayer(Transform target)
    {
        player = target;
    }

}
