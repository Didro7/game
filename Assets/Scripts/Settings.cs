
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [Header("Settings UI")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Slider volumeSlider;

    private SaveData data;

    private void Start()
    {
        data = SaveSystem.Load();

        AudioListener.volume =
            Mathf.Clamp01(data.musicVolume);

        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    public void OpenSettings()
    {
        data = SaveSystem.Load();

        if (volumeSlider != null)
        {
            volumeSlider.SetValueWithoutNotify(
                Mathf.Clamp01(data.musicVolume)
            );
        }

        AudioListener.volume =
            Mathf.Clamp01(data.musicVolume);

        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    public void ChangeVolume(float value)
    {
        if (data == null)
            data = SaveSystem.Load();

        data.musicVolume = Mathf.Clamp01(value);

        AudioListener.volume = data.musicVolume;

        SaveSystem.Save(data);

        Debug.Log(
            "Volume saved: " + data.musicVolume
        );
    }
}
