using UnityEngine;

/// <summary>
/// Starter controller for PedaLi: a pixel-art character moving through a 3D climbing world.
/// Add a CharacterController and Animator to the same GameObject.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class PedaLiController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float gravity = -22f;
    [SerializeField] private float jumpSpeed = 8f;
    [SerializeField] private float climbSpeed = 3.2f;
    [SerializeField] private float depthZ;

    [Header("Ledge grab (optional)")]
    [SerializeField] private Transform ledgeCheck;
    [SerializeField] private float ledgeCheckRadius = 0.28f;
    [SerializeField] private LayerMask ledgeLayer;
    [SerializeField] private bool requireLedgeToGrab = true;

    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private PedaLiSpriteAnimator pixelAnimator;

    private CharacterController characterController;
    private float horizontalInput;
    private float keyboardHorizontalInput;
    private bool mobileLeftHeld;
    private bool mobileRightHeld;
    private bool mobileGrabHeld;
    private bool keyboardGrabHeld;
    private bool grabHeld;
    private bool jumpQueued;
    private float verticalVelocity;
    private bool isGrabbing;

    private static readonly int IsMoving = Animator.StringToHash("IsMoving");
    private static readonly int IsJumping = Animator.StringToHash("IsJumping");
    private static readonly int IsGrabbing = Animator.StringToHash("IsGrabbing");
    private static readonly int VerticalSpeed = Animator.StringToHash("VerticalSpeed");

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        if (animator == null) animator = GetComponent<Animator>();
        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (pixelAnimator == null) pixelAnimator = GetComponentInChildren<PedaLiSpriteAnimator>();
        depthZ = transform.position.z;
    }

    private void Update()
    {
        ReadKeyboardForEditor();
        horizontalInput = mobileLeftHeld == mobileRightHeld ? keyboardHorizontalInput : (mobileLeftHeld ? -1f : 1f);
        grabHeld = mobileGrabHeld || keyboardGrabHeld;
        bool nearLedge = !requireLedgeToGrab || (ledgeCheck != null &&
            Physics.CheckSphere(ledgeCheck.position, ledgeCheckRadius, ledgeLayer));

        isGrabbing = grabHeld && nearLedge;

        if (isGrabbing)
        {
            verticalVelocity = 0f;
            Vector3 climb = new Vector3(horizontalInput * moveSpeed, climbSpeed, 0f);
            characterController.Move(climb * Time.deltaTime);
        }
        else
        {
            if (characterController.isGrounded && verticalVelocity < 0f)
                verticalVelocity = -2f;

            if (jumpQueued && (characterController.isGrounded || nearLedge))
                verticalVelocity = jumpSpeed;

            verticalVelocity += gravity * Time.deltaTime;
            Vector3 movement = new Vector3(horizontalInput * moveSpeed, verticalVelocity, 0f);
            characterController.Move(movement * Time.deltaTime);
        }

        jumpQueued = false;
        Vector3 position = transform.position;
        position.z = depthZ;
        transform.position = position;

        if (spriteRenderer != null && Mathf.Abs(horizontalInput) > 0.01f)
            spriteRenderer.flipX = horizontalInput < 0f;

        UpdateAnimator();
    }

    private void ReadKeyboardForEditor()
    {
        // Keyboard controls make it possible to test in the Unity Editor.
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) keyboardHorizontalInput = -1f;
        else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) keyboardHorizontalInput = 1f;
        else keyboardHorizontalInput = 0f;

        keyboardGrabHeld = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.E);

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            Jump();
    }

    // Mobile UI calls these methods from PedaLiMobileButton.
    public void SetMoveLeft(bool held) { mobileLeftHeld = held; }
    public void SetMoveRight(bool held) { mobileRightHeld = held; }
    public void SetGrab(bool held) { mobileGrabHeld = held; }
    public void Jump() { jumpQueued = true; }


    private void UpdateAnimator()
    {
        if (animator == null) return;
        animator.SetBool(IsMoving, Mathf.Abs(horizontalInput) > 0.01f);
        animator.SetBool(IsJumping, !characterController.isGrounded && !isGrabbing);
        animator.SetBool(IsGrabbing, isGrabbing);
        animator.SetFloat(VerticalSpeed, verticalVelocity);

        if (pixelAnimator != null)
        {
            PedaLiSpriteAnimator.AnimationState state;
            if (isGrabbing)
                state = Mathf.Abs(horizontalInput) > 0.01f ? PedaLiSpriteAnimator.AnimationState.Climb : PedaLiSpriteAnimator.AnimationState.Grab;
            else if (!characterController.isGrounded)
                state = verticalVelocity > 0f ? PedaLiSpriteAnimator.AnimationState.Jump : PedaLiSpriteAnimator.AnimationState.Fall;
            else if (Mathf.Abs(horizontalInput) > 0.01f)
                state = PedaLiSpriteAnimator.AnimationState.Run;
            else
                state = PedaLiSpriteAnimator.AnimationState.Idle;

            pixelAnimator.Play(state);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (ledgeCheck == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(ledgeCheck.position, ledgeCheckRadius);
    }
}
