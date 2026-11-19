
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public SaveData Data { get; private set; }

    private int levelCoins = 0;

    public void CoinCollected(int amount)
    {
        if (amount <= 0 || Data == null)
            return;

        levelCoins += amount;
        Data.totalCoins += amount;

        SaveSystem.Save(Data);

        Debug.Log(
            "Level coins: " + levelCoins +
            " | Total coins: " + Data.totalCoins
        );
    }


    private void Awake()
    {
        Data = SaveSystem.Load();

        AudioListener.volume =
            Mathf.Clamp01(Data.musicVolume);
    }

    public void CompleteLevel(
        int levelNumber,
        int totalLevels)
    {
        if (levelNumber < 1 ||
            levelNumber > totalLevels)
        {
            Debug.LogWarning("Invalid completed level");
            return;
        }

        int nextLevel = levelNumber + 1;

        if (nextLevel <= totalLevels &&
            !Data.unlockedLevels.Contains(nextLevel))
        {
            Data.unlockedLevels.Add(nextLevel);
        }

        SaveSystem.Save(Data);

        Debug.Log(
            "Level completed: " + levelNumber
        );
    }

    public void SetMusicVolume(float volume)
    {
        Data.musicVolume = Mathf.Clamp01(volume);
        SaveSystem.Save(Data);
    }

    public void SetVibration(bool enabled)
    {
        Data.vibration = enabled;
        SaveSystem.Save(Data);
    }

    private void OnApplicationPause(bool paused)
    {
        if (paused && Data != null)
        {
            SaveSystem.Save(Data);
        }
    }
}
