
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelExit : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private LevelLoader levelLoader;
    [SerializeField] private GameTimer gameTimer;
    [SerializeField] private SaveManager saveManager;

    [Header("Victory UI")]
    [SerializeField] private GameObject completePanel;
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private GameObject nextButton;

    private bool completed;

    private void Start()
    {
        if (completePanel != null)
            completePanel.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (completed || player == null)
            return;

        bool isPlayer =
            other.transform == player ||
            other.transform.IsChildOf(player);

        if (!isPlayer)
            return;

        if (levelLoader == null ||
            levelLoader.CurrentLevel == null ||
            saveManager == null)
        {
            Debug.LogError("Finish references missing!");
            return;
        }

        LevelData level = levelLoader.CurrentLevel;

        if (level.goal != LevelGoal.ReachExit)
            return;

        completed = true;

        GameSfxManager.Instance?.PlayWin();


        if (gameTimer != null)
            gameTimer.StopTimer();

        saveManager.CompleteLevel(
            level.levelNumber,
            levelLoader.LevelCount
        );

        bool lastLevel =
            levelLoader.CurrentIndex + 1 >=
            levelLoader.LevelCount;

        if (resultText != null)
        {
            resultText.text = lastLevel
                ? "ALL LEVELS COMPLETED!"
                : "LEVEL COMPLETED!";
        }

        if (nextButton != null)
            nextButton.SetActive(!lastLevel);

        if (completePanel != null)
            completePanel.SetActive(true);

        Time.timeScale = 0f;

        Debug.Log("LEVEL COMPLETED!");
    }


    public void NextLevel()
    {
        if (!completed || levelLoader == null)
            return;

        int nextIndex = levelLoader.CurrentIndex + 1;

        if (nextIndex >= levelLoader.LevelCount)
        {
            Debug.Log("All levels completed!");
            return;
        }

        GameSession.levelIndex = nextIndex;

        Debug.Log(
            "NEXT LEVEL: Loading level index " +
            GameSession.levelIndex
        );

        Time.timeScale = 1f;

        SceneManager.LoadScene("Game");
    }

}
