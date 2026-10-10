using UnityEngine;

/// <summary>
/// Frame-based pixel-art animator for PedaLi.
/// Assign sliced Sprite arrays in the Inspector. Uses Point-filtered sprites for crisp pixels.
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(SpriteRenderer))]
public class PedaLiSpriteAnimator : MonoBehaviour
{
    public enum AnimationState { Idle, Run, Jump, Grab, Climb, Fall }

    [System.Serializable]
    public class SpriteAnimation
    {
        public string label = "Animation";
        public Sprite[] frames;
        [Min(1f)] public float framesPerSecond = 8f;
        public bool loop = true;
    }

    [Header("Animation clips — assign sliced frames in order")]
    [SerializeField] private SpriteAnimation idle = new SpriteAnimation { label = "Idle", framesPerSecond = 5f };
    [SerializeField] private SpriteAnimation run = new SpriteAnimation { label = "Run", framesPerSecond = 10f };
    [SerializeField] private SpriteAnimation jump = new SpriteAnimation { label = "Jump", framesPerSecond = 8f, loop = false };
    [SerializeField] private SpriteAnimation grab = new SpriteAnimation { label = "Grab", framesPerSecond = 5f };
    [SerializeField] private SpriteAnimation climb = new SpriteAnimation { label = "Climb", framesPerSecond = 8f };
    [SerializeField] private SpriteAnimation fall = new SpriteAnimation { label = "Fall", framesPerSecond = 6f };

    [Header("Playback")]
    [SerializeField] private AnimationState startingState = AnimationState.Idle;
    [SerializeField] private bool useUnscaledTime;

    private SpriteRenderer spriteRenderer;
    private AnimationState currentState;
    private SpriteAnimation currentAnimation;
    private int frameIndex;
    private float frameTimer;

    public AnimationState CurrentState => currentState;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        Play(startingState, true);
    }

    private void Update()
    {
        if (currentAnimation == null || currentAnimation.frames == null || currentAnimation.frames.Length == 0)
            return;

        float dt = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
        frameTimer += dt;
        float frameDuration = 1f / Mathf.Max(1f, currentAnimation.framesPerSecond);

        while (frameTimer >= frameDuration)
        {
            frameTimer -= frameDuration;
            if (frameIndex < currentAnimation.frames.Length - 1)
                frameIndex++;
            else if (currentAnimation.loop)
                frameIndex = 0;
            else
                frameIndex = currentAnimation.frames.Length - 1;

            ApplyFrame();
        }
    }

    public void Play(AnimationState state) => Play(state, false);

    public void Play(AnimationState state, bool forceRestart)
    {
        if (!forceRestart && currentState == state) return;

        currentState = state;
        currentAnimation = GetAnimation(state);
        frameIndex = 0;
        frameTimer = 0f;
        ApplyFrame();
    }

    private SpriteAnimation GetAnimation(AnimationState state)
    {
        switch (state)
        {
            case AnimationState.Run: return run;
            case AnimationState.Jump: return jump;
            case AnimationState.Grab: return grab;
            case AnimationState.Climb: return climb;
            case AnimationState.Fall: return fall;
            default: return idle;
        }
    }

    private void ApplyFrame()
    {
        if (spriteRenderer == null || currentAnimation == null ||
            currentAnimation.frames == null || currentAnimation.frames.Length == 0)
            return;

        Sprite frame = currentAnimation.frames[Mathf.Clamp(frameIndex, 0, currentAnimation.frames.Length - 1)];
        if (frame != null) spriteRenderer.sprite = frame;
    }
}
