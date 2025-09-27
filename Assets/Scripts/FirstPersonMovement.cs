using UnityEngine;
using UnityEngine.UI;

public class FirstPersonMovement : MonoBehaviour
{
    [Header("Movement")]
    public CharacterController controller;
    public Transform cameraTransform;
    public float moveSpeed = 0.0f;
    public float turnSmoothTime = 0.1f;

    private Vector3 velocity;
    private bool isGrounded;
    private bool isJumping;

    public AudioSource Whistle;
    public Light light;

    [Header("Animation")]
    public Animator animator;
    public GameObject Prologue;
    public Texture2D cursorTexture;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.05f;
    public LayerMask groundMask;

    [Header("Jump Settings")]
    public float maxJumpHeight = 20.0f;
    public float maxJumpTime = 0.8f;

    private float gravity;
    private float groundedGravity = -0.05f;
    private float initialJumpVelocity;

    void Start()
    {
        CalculateJumpVariables();

        Prologue.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        // set the custom cursor again
        Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);

        light.enabled = false;
    }

    void Update()
    {
        HandleMovement();
        HandleGroundCheck();
        HandleGravity();
        HandleCrouch();
        
        if(!animator.GetBool("isCrouching"))
        {
            HandleJump();
        }

        if(Input.GetKeyDown(KeyCode.F))
        {
            Whistle.Play();
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            light.enabled = !light.enabled;
        }
    }

    private void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 inputDirection = new Vector3(horizontal, 0f, vertical).normalized;

        if (inputDirection.magnitude >= 0.1f)
        {
            // Animation speed parameter (0 idle → 1 moving)
            animator.SetFloat("Speed", inputDirection.magnitude, 0.02f, Time.deltaTime);

            // Movement relative to where the camera is looking
            Vector3 moveDir = cameraTransform.right * inputDirection.x
                            + cameraTransform.forward * inputDirection.z;

            // Ignore vertical tilt from camera
            moveDir.y = 0f;

            if (animator.GetBool("isCrouching"))
            {
                moveSpeed = animator.GetFloat("Speed") * 5;
            }
            else
            {
                moveSpeed = animator.GetFloat("Speed") * 10;
            }

            controller.Move(moveDir.normalized * moveSpeed * Time.deltaTime);
        }
        else
        {
            animator.SetFloat("Speed", 0f, 0.02f, Time.deltaTime);
            moveSpeed = animator.GetFloat("Speed") * 10;
        }

        // Always apply vertical motion (gravity/jump)
        controller.Move(velocity * Time.deltaTime);
    }

    private void HandleGroundCheck()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    }

    private void CalculateJumpVariables()
    {
        float timeToApex = maxJumpTime / 2f;
        gravity = (-2 * maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        initialJumpVelocity = (2 * maxJumpHeight) / timeToApex;
    }

    private void HandleJump()
    {
        if (isGrounded)
        {
            if (isJumping)
                isJumping = false;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                isJumping = true;
                velocity.y = initialJumpVelocity;
            }
        }
    }

    private void HandleGravity()
    {
        if (isGrounded && !isJumping)
        {
            velocity.y = groundedGravity;
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }
    }

    private void HandleCrouch()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            animator.SetBool("isCrouching", true);
            controller.height = 3.0f;
            controller.center = new Vector3(0.0f, 1.4f, 0.0f);
        }
        else
        {
            animator.SetBool("isCrouching", false);
            controller.height = 3.7f;
            controller.center = new Vector3(0.0f, 1.88f, 0.0f);
        }
    }
}
