
using UnityEngine;

public class CoinCollectible : MonoBehaviour
{
    [SerializeField] private GameHUD hud;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        hud.AddCoin();
        GameSounds.Instance?.PlayCoin();
        Destroy(gameObject);
    }
}
