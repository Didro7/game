
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class GoblinPatrol : MonoBehaviour
{
    private enum State
    {
        Patrol,
        Chase,
        WindUp,
        Cooldown
    }

    [Header("Objects")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform leftPoint;
    [SerializeField] private Transform rightPoint;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private LayerMask wallMask;

    [Header("Animation")]
    [SerializeField] private Sprite idleSprite;
    [SerializeField] private Sprite[] walkFrames;
    [SerializeField] private Sprite attackSprite;
    [SerializeField] private float framesPerSecond = 6f;
    [SerializeField] private bool artFacesRight = true;

    [Header("Movement")]
    [SerializeField] private float speed = 1.5f;
    [SerializeField] private float chaseSpeed = 2.3f;
    [SerializeField] private float edgeMargin = 0.1f;
    [SerializeField] private float stopDistance = 0.15f;

    [Header("Vision")]
    [SerializeField] private float viewRange = 5f;
    [SerializeField] private float viewAngle = 120f;
    [SerializeField] private float sightInterval = 0.15f;
    [SerializeField] private float loseTime = 1.5f;

    [Header("Attack")]
    [SerializeField] private float attackRange = 1.2f;
    [SerializeField] private float attackHeight = 1f;
    [SerializeField] private float windUpTime = 0.6f;
    [SerializeField] private float cooldownTime = 1.2f;
    [SerializeField] private int damage = 1;

    private Rigidbody2D rb;
    private State currentState;

    private bool movingRight = true;
    private bool seesPlayer;

    private float stateTimer;
    private float sightTimer;
    private float lostTimer;
    private float animationTimer;

    private Color normalColor;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (spriteRenderer != null)
            normalColor = spriteRenderer.color;
    }

    private void Start()
    {
        ChangeState(State.Patrol);
    }

    private void Update()
    {
        if (player == null)
            return;

        stateTimer += Time.deltaTime;
        sightTimer -= Time.deltaTime;
        animationTimer += Time.deltaTime;


        if (sightTimer <= 0f)
        {
            sightTimer = sightInterval;
            seesPlayer = CanSeePlayer();
        }

        switch (currentState)
        {
            case State.Patrol:

                if (seesPlayer && IsOnScreen())
                {
                    ChangeState(State.Chase);
                }

                break;

            case State.Chase:

                if (seesPlayer)
                    lostTimer = 0f;
                else
                    lostTimer += Time.deltaTime;

                if (seesPlayer &&
                    IsNearPlayer() &&
                    IsOnScreen())
                {
                    ChangeState(State.WindUp);
                }
                else if (lostTimer >= loseTime)
                {
                    ChangeState(State.Patrol);
                }

                break;

            case State.WindUp:

                if (stateTimer >= windUpTime)
                {
                    if (IsNearPlayer() &&
                        IsOnScreen() &&
                        CanSeePlayer())
                    {
                        PlayerHealth health =
                            player.GetComponent<PlayerHealth>();

                        if (health != null)
                            health.TakeDamage(damage);
                    }

                    ChangeState(State.Cooldown);
                }

                break;

            case State.Cooldown:

                if (stateTimer >= cooldownTime)
                {
                    ChangeState(seesPlayer
                        ? State.Chase
                        : State.Patrol);
                }

                break;
        }

        if ((currentState == State.Chase ||
             currentState == State.WindUp) && player != null)
        {
            float dx = player.position.x - transform.position.x;
            if (Mathf.Abs(dx) > 0.05f)
                movingRight = dx > 0f;
        }

        UpdateAnimation();
    }

    private void FixedUpdate()
    {
        if (leftPoint == null || rightPoint == null)
            return;

        float minX = Mathf.Min(
            leftPoint.position.x,
            rightPoint.position.x
        );

        float maxX = Mathf.Max(
            leftPoint.position.x,
            rightPoint.position.x
        );

        float horizontalVelocity = 0f;

        if (currentState == State.Patrol)
        {
            if (movingRight &&
                rb.position.x >= maxX - edgeMargin)
            {
                movingRight = false;
            }
            else if (!movingRight &&
                     rb.position.x <= minX + edgeMargin)
            {
                movingRight = true;
            }

            horizontalVelocity =
                movingRight ? speed : -speed;
        }

        if (currentState == State.Chase &&
            player != null)
        {
            float difference =
                player.position.x - rb.position.x;

            if (Mathf.Abs(difference) > stopDistance)
            {
                movingRight = difference > 0f;

                float direction =
                    movingRight ? 1f : -1f;

                float nextX = rb.position.x +
                    direction * chaseSpeed *
                    Time.fixedDeltaTime;

                if (nextX >= minX + edgeMargin &&
                    nextX <= maxX - edgeMargin)
                {
                    horizontalVelocity =
                        direction * chaseSpeed;
                }
            }
        }

        rb.linearVelocity = new Vector2(
            horizontalVelocity,
            rb.linearVelocity.y
        );

        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = artFacesRight
                ? !movingRight
                : movingRight;
        }
    }

    private bool CanSeePlayer()
    {
        Vector2 toPlayer =
            player.position - transform.position;

        float distance = toPlayer.magnitude;

        if (distance > viewRange ||
            distance < 0.01f)
            return false;

        Vector2 lookingDirection =
            movingRight ? Vector2.right : Vector2.left;

        float angle = Vector2.Angle(
            lookingDirection,
            toPlayer
        );

        if (angle > viewAngle / 2f && !IsNearPlayer())
            return false;

        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            toPlayer.normalized,
            distance,
            wallMask
        );

        return hit.collider == null;
    }

    private bool IsNearPlayer()
    {
        float dx = Mathf.Abs(
            player.position.x - transform.position.x);

        float dy = Mathf.Abs(
            player.position.y - transform.position.y);

        return dx <= attackRange &&
               dy <= attackHeight;
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

    private void ChangeState(State next)
    {
        currentState = next;
        stateTimer = 0f;
        animationTimer = 0f;

        if (next == State.Chase)
            lostTimer = 0f;

        Debug.Log("Goblin State: " + currentState);
    }

    private void UpdateAnimation()
    {
        if (spriteRenderer == null)
            return;

        spriteRenderer.color =
            currentState == State.WindUp
            ? new Color(1f, 0.3f, 0.3f)
            : normalColor;

        Sprite nextSprite = idleSprite;

        if (currentState == State.Patrol ||
            currentState == State.Chase)
        {
            if (walkFrames != null &&
                walkFrames.Length > 0)
            {
                int frame = (int)(
                    animationTimer * framesPerSecond
                ) % walkFrames.Length;

                nextSprite = walkFrames[frame];
            }
        }
        else if (currentState == State.WindUp)
        {
            nextSprite = attackSprite;
        }

        if (nextSprite != null)
            spriteRenderer.sprite = nextSprite;
    }
}
