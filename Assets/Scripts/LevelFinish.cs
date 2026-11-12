using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelFinish : MonoBehaviour
{
    public enum FinishAction
    {
        LoadNextLevel,
        ShowVictoryPanel
    }

    [Header("Finish Settings")]
    [SerializeField] private FinishAction finishAction = FinishAction.LoadNextLevel;
    [SerializeField] private string nextSceneName = "Level2";
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private string menuSceneName = "MainMenu";

    private bool finished;

    private void Start()
    {
        if (victoryPanel != null)
            victoryPanel.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (finished || other.GetComponentInParent<PlayerController>() == null)
            return;

        finished = true;
        Debug.Log("Level finished: " + SceneManager.GetActiveScene().name);

        if (finishAction == FinishAction.LoadNextLevel)
        {
            if (string.IsNullOrWhiteSpace(nextSceneName))
            {
                Debug.LogWarning("LevelFinish: Next Scene Name is empty.", this);
                finished = false;
                return;
            }

            Time.timeScale = 1f;
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            if (victoryPanel == null)
            {
                Debug.LogWarning("LevelFinish: Victory Panel is not assigned.", this);
                finished = false;
                return;
            }

            victoryPanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(menuSceneName);
    }
}
