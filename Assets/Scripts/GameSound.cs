
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class GameSounds : MonoBehaviour
{
    public static GameSounds Instance { get; private set; }

    [SerializeField] private AudioClip coinSound;

    private AudioSource audioSource;

    private void Awake()
    {
        Instance = this;
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayCoin()
    {
        if (coinSound != null)
        {
            audioSource.PlayOneShot(coinSound);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
