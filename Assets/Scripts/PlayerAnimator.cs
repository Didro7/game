
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Rigidbody2D playerBody;

    [SerializeField] private Sprite[] idleFrames;
    [SerializeField] private Sprite[] runFrames;
    [SerializeField] private Sprite[] jumpFrames;

    [SerializeField] private float framesPerSecond = 8f;

    private float timer;

    private void Update()
    {
        Sprite[] frames = idleFrames;

        if (Mathf.Abs(playerBody.linearVelocity.y) > 0.4f)
            frames = jumpFrames;
        else if (Mathf.Abs(playerBody.linearVelocity.x) > 0.1f)
            frames = runFrames;

        if (frames == null || frames.Length == 0)
            return;

        timer += Time.deltaTime;

        int frame = (int)(timer * framesPerSecond)
                    % frames.Length;

        spriteRenderer.sprite = frames[frame];
    }
}
