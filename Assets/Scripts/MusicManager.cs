
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance
    {
        get;
        private set;
    }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource sourceA;
    [SerializeField] private AudioSource sourceB;

    [Header("Music")]
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip levelMusic;

    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer gameMixer;

    [Header("Settings")]
    [SerializeField] private float fadeDuration = 1f;

    private AudioSource currentSource;
    private Coroutine fadeCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (sourceA == null || sourceB == null)
        {
            Debug.LogError(
                "MusicManager AudioSources missing!"
            );
            return;
        }

        sourceA.loop = true;
        sourceB.loop = true;

        sourceA.playOnAwake = false;
        sourceB.playOnAwake = false;

        sourceA.spatialBlend = 0f;
        sourceB.spatialBlend = 0f;

        sourceA.volume = 0f;
        sourceB.volume = 0f;
    }

    private void Start()
    {
        if (gameMixer == null)
        {
            Debug.LogError("GameMixer is missing!");
            return;
        }

        SaveData data = SaveSystem.Load();

        SetMixerVolume("MusicVol", data.musicVolume);
        SetMixerVolume("SFXVol", data.sfxVolume);
    }

    private void SetMixerVolume(string parameter, float value)
    {
        float db =
            Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20f;

        if (!gameMixer.SetFloat(parameter, db))
        {
            Debug.LogWarning(
                "Mixer parameter not found: " + parameter
            );
        }
    }

    public void PlayMenuMusic()
    {
        PlayTrack(menuMusic);
    }

    public void PlayLevelMusic()
    {
        PlayTrack(levelMusic);
    }

    private void PlayTrack(AudioClip clip)
    {
        if (clip == null || sourceA == null || sourceB == null)
            return;

        if (currentSource != null &&
            currentSource.clip == clip &&
            currentSource.isPlaying)
        {
            return;
        }

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        AudioSource next =
            currentSource == sourceA ? sourceB : sourceA;

        if (next.isPlaying)
            next.Stop();

        next.clip = clip;
        next.volume = 0f;
        next.Play();

        AudioSource previous = currentSource;
        currentSource = next;

        fadeCoroutine = StartCoroutine(
            Crossfade(previous, next)
        );
    }

    private IEnumerator Crossfade(
        AudioSource previous,
        AudioSource next)
    {
        float duration = Mathf.Max(0.01f, fadeDuration);

        float previousVolume =
            previous != null && previous.isPlaying
            ? previous.volume
            : 0f;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(
                elapsed / duration
            );

            if (previous != null)
            {
                previous.volume =
                    Mathf.Lerp(previousVolume, 0f, t);
            }

            next.volume = t;

            yield return null;
        }

        if (previous != null)
        {
            previous.Stop();
            previous.volume = 0f;
        }

        next.volume = 1f;
        fadeCoroutine = null;
    }
}
