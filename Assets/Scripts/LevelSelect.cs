
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelSelect : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject levelSelectPanel;
    [SerializeField] private GameObject resetConfirmPanel;

    [Header("Level Buttons")]
    [SerializeField] private Button level1Button;
    [SerializeField] private Button level2Button;
    [SerializeField] private Button level3Button;

    private void Start()
    {
        levelSelectPanel.SetActive(false);
        resetConfirmPanel.SetActive(false);
    }

    public void OpenLevelSelect()
    {
        levelSelectPanel.SetActive(true);
        RefreshButtons();
    }

    public void CloseLevelSelect()
    {
        levelSelectPanel.SetActive(false);
    }

    private void RefreshButtons()
    {
        SaveData data = SaveSystem.Load();

        UpdateButton(level1Button, 1, data);
        UpdateButton(level2Button, 2, data);
    }

    private void UpdateButton(

        Button button,

        int levelNumber,
        SaveData data)
    {
        if (button == null) return;
        bool unlocked =
            data.unlockedLevels.Contains(levelNumber);

        button.interactable = unlocked;

        TMP_Text label =
            button.GetComponentInChildren<TMP_Text>(true);

        if (label != null)
        {
            label.text = unlocked
                ? "LEVEL " + levelNumber
                : "LEVEL " + levelNumber + " (LOCKED)";
        }

    }

    public void PlayLevel1()
    {
        StartLevel(1);
    }

    public void PlayLevel2()
    {
        StartLevel(2);
    }

    public void PlayLevel3()
    {
        StartLevel(3);
    }

    private void StartLevel(int levelNumber)
    {
        SaveData data = SaveSystem.Load();

        if (!data.unlockedLevels.Contains(levelNumber))
        {
            Debug.LogWarning("Level is locked!");
            return;
        }

        GameSession.levelIndex = levelNumber - 1;

        Time.timeScale = 1f;
        SceneManager.LoadScene("Game");
    }

    public void AskReset()
    {
        resetConfirmPanel.SetActive(true);
    }

    public void CancelReset()
    {
        resetConfirmPanel.SetActive(false);
    }

    public void ConfirmReset()
    {
        if (!SaveSystem.Delete())
            return;

        GameSession.levelIndex = 0;

        PlayerPrefs.DeleteKey("volume");
        PlayerPrefs.DeleteKey("vibration");
        PlayerPrefs.Save();

        resetConfirmPanel.SetActive(false);

        if (levelSelectPanel.activeSelf)
            RefreshButtons();

        Debug.Log("Progress reset!");
    }
}
