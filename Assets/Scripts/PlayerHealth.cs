
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private float invincibilityTime = 1f;

    [Header("Hearts UI")]
    [SerializeField] private Image[] hearts;
    [SerializeField] private Sprite fullHeart;
    [SerializeField] private Sprite emptyHeart;

    [Header("Player Visual")]
    [SerializeField] private SpriteRenderer playerSprite;
    [SerializeField] private float flashInterval = 0.1f;

    [Header("Game Over")]
    [SerializeField] private GameObject gameOverPanel;

    private int currentHealth;
    private float nextDamageTime;
    private bool isDead;

    private Color normalPlayerColor;
    private Color[] originalHeartColors;

    private void Awake()
    {
        currentHealth = maxHealth;

        if (playerSprite != null)
            normalPlayerColor = playerSprite.color;

        originalHeartColors = new Color[hearts.Length];

        for (int i = 0; i < hearts.Length; i++)
        {
            if (hearts[i] != null)
                originalHeartColors[i] = hearts[i].color;
        }

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        UpdateHearts();
    }

    public void TakeDamage(int damage)
    {
        if (isDead || Time.time < nextDamageTime)
            return;

        currentHealth = Mathf.Max(
            0, currentHealth - damage
        );

        nextDamageTime =
            Time.time + invincibilityTime;

        Debug.Log("Player HP: " + currentHealth);

        UpdateHearts();

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(FlashPlayer());
        }
    }

    private void UpdateHearts()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (hearts[i] == null)
                continue;

            bool isFull = i < currentHealth;

            if (isFull && fullHeart != null)
            {
                hearts[i].sprite = fullHeart;
            }
            else if (!isFull && emptyHeart != null)
            {
                hearts[i].sprite = emptyHeart;
            }

            if (isFull || emptyHeart != null)
            {
                hearts[i].color =
                    originalHeartColors[i];
            }
            else
            {
                Color color = originalHeartColors[i];
                color.a = 0.25f;
                hearts[i].color = color;
            }
        }
    }

    private IEnumerator FlashPlayer()
    {
        if (playerSprite == null)
            yield break;

        while (Time.time < nextDamageTime && !isDead)
        {
            playerSprite.color = Color.red;

            yield return new WaitForSeconds(
                flashInterval
            );

            playerSprite.color = normalPlayerColor;

            yield return new WaitForSeconds(
                flashInterval
            );
        }

        playerSprite.color = normalPlayerColor;
    }

    private void Die()
    {
        isDead = true;

        Debug.Log("GAME OVER");

        if (playerSprite != null)
            playerSprite.color = Color.red;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        Time.timeScale = 0f;
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().name
        );
    }

    [ContextMenu("Test Damage")]
    private void TestDamage()
    {
        TakeDamage(1);
    }
}
