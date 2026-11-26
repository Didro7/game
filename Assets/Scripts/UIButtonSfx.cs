
using UnityEngine;
using UnityEngine.UI;

public class UIButtonSfx : MonoBehaviour
{
    private void Start()
    {
        Button[] buttons =
            GetComponentsInChildren<Button>(true);

        foreach (Button button in buttons)
        {
            button.onClick.AddListener(PlayClick);
        }
    }

    private void PlayClick()
    {
        GameSfxManager.Instance?.PlayClick();
    }
}
