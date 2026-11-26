
using UnityEngine;
using UnityEngine.Audio;

public class PauseAudio : MonoBehaviour
{
    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer gameMixer;

    [Header("Snapshots")]
    [SerializeField] private AudioMixerSnapshot normalSnapshot;
    [SerializeField] private AudioMixerSnapshot pausedSnapshot;

    private void Start()
    {
        if (gameMixer != null)
        {
            gameMixer.updateMode =
                AudioMixerUpdateMode.UnscaledTime;
        }

        if (normalSnapshot != null)
            normalSnapshot.TransitionTo(0f);
    }

    public void SetPaused(bool isPaused)
    {
        if (isPaused)
        {
            if (pausedSnapshot != null)
                pausedSnapshot.TransitionTo(0.3f);
        }
        else
        {
            if (normalSnapshot != null)
                normalSnapshot.TransitionTo(0.3f);
        }
    }

    private void OnDisable()
    {
        if (normalSnapshot != null)
            normalSnapshot.TransitionTo(0f);
    }
}
