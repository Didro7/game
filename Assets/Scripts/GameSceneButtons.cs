
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneButtons : MonoBehaviour
{
    [Header("Scenes")]
    [SerializeField]
    private string menuSceneName = "MainMenu";

    public void RestartLevel()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("Game");
    }

    public void ReturnToMenu()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(menuSceneName);
    }
}
