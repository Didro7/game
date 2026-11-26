
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class GameSfxManager : MonoBehaviour
{
    public static GameSfxManager Instance
    {
        get;
        private set;
    }

    [Header("Audio Source")]
    [SerializeField] private AudioSource sfxSource;

    [Header("Player")]
    [SerializeField] private AudioClip jump;
    [SerializeField] private AudioClip hit;
    [SerializeField] private AudioClip pickup;
    [SerializeField] private AudioClip death;

    [Header("Enemies")]
    [SerializeField] private AudioClip goblinWarning;
    [SerializeField] private AudioClip ghostWarning;
    [SerializeField] private AudioClip orcWarning;

    [Header("UI and Results")]
    [SerializeField] private AudioClip click;
    [SerializeField] private AudioClip win;
    [SerializeField] private AudioClip lose;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        if (sfxSource == null)
            sfxSource = GetComponent<AudioSource>();
    }

    private void PlaySound(
        AudioClip clip,
        bool variation = false)
    {
        if (clip == null || sfxSource == null)
            return;

        sfxSource.pitch = variation
            ? Random.Range(0.92f, 1.08f)
            : 1f;

        sfxSource.PlayOneShot(clip);
    }

    public void PlayJump() =>
        PlaySound(jump, true);

    public void PlayHit() =>
        PlaySound(hit, true);

    public void PlayPickup() =>
        PlaySound(pickup, true);

    public void PlayDeath() =>
        PlaySound(death);

    public void PlayGoblinWarning() =>
        PlaySound(goblinWarning);

    public void PlayGhostWarning() =>
        PlaySound(ghostWarning);

    public void PlayOrcWarning() =>
        PlaySound(orcWarning);

    public void PlayClick() =>
        PlaySound(click, true);

    public void PlayWin() =>
        PlaySound(win);

    public void PlayLose() =>
        PlaySound(lose);
}
