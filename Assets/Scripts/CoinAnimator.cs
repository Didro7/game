
using UnityEngine;

public class CoinAnimator : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite[] coinFrames;
    [SerializeField] private float framesPerSecond = 8f;

    private float timer;

    private void Update()
    {
        if (coinFrames == null || coinFrames.Length == 0)
            return;

        timer += Time.deltaTime;

        int frame = (int)(timer * framesPerSecond)
                    % coinFrames.Length;

        spriteRenderer.sprite = coinFrames[frame];
    }
}
