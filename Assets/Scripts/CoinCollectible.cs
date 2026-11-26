
using UnityEngine;

public class CoinCollectible : MonoBehaviour
{
    [SerializeField] private GameHUD hud;

    private bool collected = false;

    private void Awake()
    {
        if (hud == null)
        {
            hud = FindFirstObjectByType<GameHUD>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected)
            return;

        if (!other.CompareTag("Player"))
            return;

        collected = true;

        if (hud == null)
        {
            hud = FindFirstObjectByType<GameHUD>();
        }

        if (hud != null)
        {
            hud.AddCoin();
        }
        else
        {
            Debug.LogWarning(
                "GameHUD not found! Coin: " + gameObject.name
            );
        }

        GameSounds.Instance?.PlayCoin();

        Destroy(gameObject);
    }
}
