
using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private GameObject gameOverPanel;

    private float remainingTime;
    private bool isRunning;

    public void Begin(float seconds)
    {
        remainingTime = Mathf.Max(0f, seconds);
        isRunning = true;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        UpdateTimerText();
    }

    private void Update()
    {
        if (!isRunning)
            return;

        remainingTime -= Time.deltaTime;

        if (remainingTime <= 0f)
        {
            remainingTime = 0f;
            isRunning = false;

            UpdateTimerText();
            TimeUp();
            return;
        }

        UpdateTimerText();
    }

    private void UpdateTimerText()
    {
        if (timerText == null)
            return;

        int totalSeconds =
            Mathf.CeilToInt(remainingTime);

        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;

        timerText.text =
            $"{minutes:00}:{seconds:00}";
    }

    private void TimeUp()
    {
        Debug.Log("TIME IS UP!");

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        Time.timeScale = 0f;
    }

    public void StopTimer()
    {
        isRunning = false;
    }
}
