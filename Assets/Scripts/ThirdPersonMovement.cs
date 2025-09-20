using UnityEngine;
using UnityEngine.UI;

public class ThirdPersonMovement : MonoBehaviour
{
    [Header("Movement")]
    public CharacterController controller;
    public Transform cameraTransform;
    float speed = 0.0f;
    public float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;

    [Header("Animation")]
    public Animator animator;

    [Header("Scoring")]
    public Text scoreText;
    public AudioSource coinAudio;

    private int score = 0;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    [Header("Jump Settings")]
    float maxJumpHeight = 20.0f;
    float maxJumpTime = 1.0f;

    private float gravity;
    private float groundedGravity = -0.05f;
    private float initialJumpVelocity;
    private Vector3 velocity;
    private bool isGrounded;
    private bool isJumping;

    void Start()
    {
        CalculateJumpVariables();
        UpdateScoreUI();
    }

    void Update()
    {
        HandleMovement();
        HandleGroundCheck();
        HandleGravity();
        HandleJump();
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

            // Calculate rotation relative to camera
            float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg
                                + cameraTransform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle,
                                                ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            // Move relative to camera
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            speed = animator.GetFloat("Speed") * 10;
            Vector3 move = moveDir.normalized * speed;
            move.y = velocity.y; // apply gravity/jump
            controller.Move(move * Time.deltaTime);
        }
        else
        {
            animator.SetFloat("Speed", 0f, 0.02f, Time.deltaTime);
            Vector3 move = new Vector3(0f, velocity.y, 0f);
            controller.Move(move * Time.deltaTime);
        }
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
            {
                // Jump finished
                isJumping = false;
            }

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
            velocity.y = groundedGravity; // small downward push keeps controller grounded
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.CompareTag("Coin"))
        {
            score++;
            coinAudio.Play();
            UpdateScoreUI();
            Destroy(hit.gameObject);
        }
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = score.ToString();
    }
}
