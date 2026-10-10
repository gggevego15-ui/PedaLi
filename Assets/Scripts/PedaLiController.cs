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

    private CharacterController characterController;
    private float horizontalInput;
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
        depthZ = transform.position.z;
    }

    private void Update()
    {
        ReadKeyboardForEditor();
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
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) horizontalInput = -1f;
        else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) horizontalInput = 1f;
        else horizontalInput = 0f;

        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.E))
            grabHeld = true;
        else if (!MobileGrabIsHeld())
            grabHeld = false;

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            Jump();
    }

    // Mobile UI calls these methods from PedaLiMobileButton.
    public void SetMoveLeft(bool held) { if (held) horizontalInput = -1f; else if (horizontalInput < 0f) horizontalInput = 0f; }
    public void SetMoveRight(bool held) { if (held) horizontalInput = 1f; else if (horizontalInput > 0f) horizontalInput = 0f; }
    public void SetGrab(bool held) { grabHeld = held; }
    public void Jump() { jumpQueued = true; }

    private bool MobileGrabIsHeld() { return grabHeld; }

    private void UpdateAnimator()
    {
        if (animator == null) return;
        animator.SetBool(IsMoving, Mathf.Abs(horizontalInput) > 0.01f);
        animator.SetBool(IsJumping, !characterController.isGrounded && !isGrabbing);
        animator.SetBool(IsGrabbing, isGrabbing);
        animator.SetFloat(VerticalSpeed, verticalVelocity);
    }

    private void OnDrawGizmosSelected()
    {
        if (ledgeCheck == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(ledgeCheck.position, ledgeCheckRadius);
    }
}
