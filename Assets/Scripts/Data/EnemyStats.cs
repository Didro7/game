using UnityEngine;

[CreateAssetMenu(
    fileName = "EnemyStats",
    menuName = "Coin Adventure/Enemy Stats"
)]
public class EnemyStats : ScriptableObject
{
    [Header("Movement")]
    public float patrolSpeed;
    public float chaseSpeed;

    [Header("Detection")]
    public float viewRange;
    public float viewAngle;

    [Header("Attack")]
    public float attackRange;
    public float attackHeight;
    public float windUpTime;
    public float cooldownTime;
    public int damage;

    [Header("Additional Settings")]
    public float edgeMargin = 0.1f;
    public float stopDistance = 0.15f;
    public float sightInterval = 0.15f;
    public float loseTime = 1.5f;

    [Header("Animation")]
    public float framesPerSecond = 6f;


    [Header("Ghost and Orc Detection")]
    public float detectionRange;
    public float detectionHeight;

    [Header("Ghost Shooting")]
    public float aimTime;
    public float fireTime;
    public float projectileSpeed;

    [Header("Orc Dash")]
    public float dashSpeed;
    public float dashDuration;
    public float stunTime;
    public float animationSpeed;


}
