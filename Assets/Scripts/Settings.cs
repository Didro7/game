
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class Settings : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    [Header("Mixer")]
    [SerializeField] private AudioMixer gameMixer;

    private SaveData data;

    private void Start()
    {
        ApplySavedVolumes();

        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    public void OpenSettings()
    {
        ApplySavedVolumes();

        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    public void ApplySavedVolumes()
    {
        data = SaveSystem.Load();

        if (musicSlider != null)
            musicSlider.SetValueWithoutNotify(
                data.musicVolume
            );

        if (sfxSlider != null)
            sfxSlider.SetValueWithoutNotify(
                data.sfxVolume
            );

        SetMixerVolume("MusicVol", data.musicVolume);
        SetMixerVolume("SFXVol", data.sfxVolume);
    }

    public void ChangeMusicVolume(float value)
    {
        if (data == null)
            data = SaveSystem.Load();

        data.musicVolume = Mathf.Clamp01(value);

        SetMixerVolume(
            "MusicVol",
            data.musicVolume
        );

        SaveSystem.Save(data);
    }

    public void ChangeSfxVolume(float value)
    {
        if (data == null)
            data = SaveSystem.Load();

        data.sfxVolume = Mathf.Clamp01(value);

        SetMixerVolume(
            "SFXVol",
            data.sfxVolume
        );

        SaveSystem.Save(data);
    }

    private void SetMixerVolume(
        string parameter,
        float value)
    {
        if (gameMixer == null)
            return;

        float db = Mathf.Log10(
            Mathf.Max(value, 0.0001f)
        ) * 20f;

        gameMixer.SetFloat(parameter, db);
    }
}
