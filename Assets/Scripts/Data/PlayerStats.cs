
using UnityEngine;

[CreateAssetMenu(
    fileName = "PlayerStats",
    menuName = "Coin Adventure/Player Stats"
)]
public class PlayerStats : ScriptableObject
{
    [Header("Health")]
    public int maxHealth = 3;
    public float invincibilityTime = 1f;
    public float flashInterval = 0.1f;

    [Header("Movement")]
    public float moveSpeed;
    public float jumpForce;
}
