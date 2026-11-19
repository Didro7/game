
using UnityEngine;

public class GhostEnemy : MonoBehaviour
{
    private enum State
    {
        Idle,
        Aim,
        Fire,
        Cooldown
    }

    [Header("Objects")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform muzzle;
    [SerializeField] private EnemyProjectile projectilePrefab;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private bool artFacesRight = true;
    [SerializeField] private Sprite idleSprite;
    [SerializeField] private Sprite attackSprite;
    [SerializeField] private LayerMask wallMask;


    [Header("Enemy Data")]
    [SerializeField] private EnemyStats stats;

    private float detectionRange => stats.detectionRange;
    private float aimTime => stats.aimTime;
    private float fireTime => stats.fireTime;
    private float cooldownTime => stats.cooldownTime;
    private float projectileSpeed => stats.projectileSpeed;
    private int damage => stats.damage;

    private State state;
    private float stateTimer;
    private bool hasFired;
    private Vector2 aimDirection;
    private Color normalColor;

    private void Awake()
    {

        if (stats == null)
        {
            Debug.LogError("Ghost Stats is not assigned!", this);
            enabled = false;
            return;
        }

        if (spriteRenderer != null)
            normalColor = spriteRenderer.color;
    }

    private void Start()
    {
        ChangeState(State.Idle);
    }

    private void Update()
    {
        if (player == null || muzzle == null)
            return;

        stateTimer += Time.deltaTime;

        switch (state)
        {
            case State.Idle:
                if (CanSeePlayer())
                {
                    aimDirection =
                        (player.position - muzzle.position)
                        .normalized;

                    ChangeState(State.Aim);
                }
                break;

            case State.Aim:
                if (!IsOnScreen())
                {
                    ChangeState(State.Cooldown);
                }
                else if (stateTimer >= aimTime)
                {
                    ChangeState(State.Fire);
                }
                break;

            case State.Fire:
                if (!hasFired)
                {
                    hasFired = true;

                    // Aim direction was locked at the start of Aim.
                    // Do not cancel an already-telegraphed shot just
                    // because the player moved during the warning.
                    if (IsOnScreen())
                        Shoot();
                }

                if (stateTimer >= fireTime)
                    ChangeState(State.Cooldown);
                break;

            case State.Cooldown:
                if (stateTimer >= cooldownTime)
                    ChangeState(State.Idle);
                break;
        }

        UpdateFacing();
    }

    private void UpdateFacing()
    {
        if (spriteRenderer == null || player == null)
            return;

     
        float dx = (state == State.Aim || state == State.Fire)
            ? aimDirection.x
            : player.position.x - transform.position.x;

        if (Mathf.Abs(dx) < 0.05f)
            return;

        spriteRenderer.flipX = artFacesRight ? dx < 0f : dx > 0f;
    }

    private bool CanSeePlayer()
    {
        if (!IsOnScreen())
            return false;

        Vector2 toPlayer =
            player.position - muzzle.position;

        float distance = toPlayer.magnitude;

        if (distance > detectionRange ||
            distance < 0.01f)
            return false;

        RaycastHit2D wall = Physics2D.Raycast(
            muzzle.position,
            toPlayer.normalized,
            distance,
            wallMask
        );

        return wall.collider == null;
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

    private void Shoot()
    {
        if (projectilePrefab == null)
        {
            Debug.LogWarning("Ghost: Projectile Prefab is not assigned.", this);
            return;
        }

        EnemyProjectile projectile = Instantiate(
            projectilePrefab,
            muzzle.position,
            Quaternion.identity
        );

        projectile.Initialize(
            aimDirection,
            projectileSpeed,
            damage
        );
    }

    private void ChangeState(State next)
    {
        state = next;
        stateTimer = 0f;

        if (next == State.Fire)
            hasFired = false;

        if (spriteRenderer != null)
        {
            spriteRenderer.color =
                next == State.Aim
                ? Color.red
                : normalColor;

            if (next == State.Aim ||
                next == State.Fire)
            {
                if (attackSprite != null)
                    spriteRenderer.sprite = attackSprite;
            }
            else if (idleSprite != null)
            {
                spriteRenderer.sprite = idleSprite;
            }
        }

        Debug.Log("Ghost State: " + state);
    }

    public void SetPlayer(Transform target)
    {
        player = target;
    }

}
