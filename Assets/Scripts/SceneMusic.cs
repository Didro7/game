
using UnityEngine;

public class SceneMusic : MonoBehaviour
{
    public enum MusicType
    {
        Menu,
        Level
    }

    [SerializeField] private MusicType musicType;

    private void Start()
    {
        if (MusicManager.Instance == null)
        {
            Debug.LogWarning("MusicManager not found!");
            return;
        }

        if (musicType == MusicType.Menu)
        {
            MusicManager.Instance.PlayMenuMusic();
        }
        else
        {
            MusicManager.Instance.PlayLevelMusic();
        }
    }
}
