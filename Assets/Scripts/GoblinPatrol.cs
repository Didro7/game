
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
    [SerializeField] private bool artFacesRight = true;




    [Header("Enemy Data")]
    [SerializeField] private EnemyStats stats;
    private float speed => stats.patrolSpeed;
    private float chaseSpeed => stats.chaseSpeed;
    private float edgeMargin => stats.edgeMargin;
    private float stopDistance => stats.stopDistance;
    private float viewRange => stats.viewRange;
    private float viewAngle => stats.viewAngle;
    private float sightInterval => stats.sightInterval;
    private float loseTime => stats.loseTime;
    private float attackRange => stats.attackRange;
    private float attackHeight => stats.attackHeight;
    private float windUpTime => stats.windUpTime;
    private float cooldownTime => stats.cooldownTime;
    private int damage => stats.damage;
    private float framesPerSecond => stats.framesPerSecond;

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

        if (stats == null)
        {
            Debug.LogError("Goblin Stats is not assigned!", this);
            enabled = false;
            return;
        }

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
                    if (player != null && IsNearPlayer())
                    {
                        PlayerHealth health =
                            player.GetComponent<PlayerHealth>();

                        if (health == null)
                            health = player.GetComponentInParent<PlayerHealth>();

                        if (health == null)
                            health = player.GetComponentInChildren<PlayerHealth>(true);

                        if (health != null)
                        {
                            health.TakeDamage(damage);
                            Debug.Log("Goblin called TakeDamage!");
                        }
                        else
                        {
                            Debug.LogWarning(
                                "PlayerHealth not found in Player hierarchy!"
                            );
                        }

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

        if (next == State.WindUp)
        {
            GameSfxManager.Instance?.PlayGoblinWarning();
        }

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

    public void SetPlayer(Transform target)
    {
        player = target;
        Debug.Log("Goblin Player assigned: " + target.name);
    }

}
