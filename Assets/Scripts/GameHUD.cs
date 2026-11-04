
using UnityEngine;
using TMPro;

public class GameHUD : MonoBehaviour
{
    [SerializeField] private TMP_Text coinText;
    [SerializeField] private GameObject[] hearts;

    private int coins = 0;
    private int health;

    private void Start()
    {
        health = hearts.Length;
        UpdateUI();
    }

    public void AddCoin()
    {
        coins++;
        UpdateUI();
    }

    public void TakeDamage()
    {
        health = Mathf.Max(0, health - 1);
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (coinText != null)
            coinText.text = "COINS: " + coins;

        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].SetActive(i < health);
        }
    }
}
