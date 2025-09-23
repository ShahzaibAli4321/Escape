using UnityEngine;

public class Dog : MonoBehaviour
{
    [Header("Target to Follow")]
    public Transform player;

    [Header("Animatior")]
    public Animator animator;

    [Header("Dynamic Speed Settings")]
    public float minFollowSpeed = 1f;    // Speed when very close
    public float maxFollowSpeed = 8f;    // Speed when far away
    public float speedSmoothing = 5f;    // How smoothly the speed changes

    [Header("Avoidance Settings")]
    public float stopDistance = 1.5f;   // Dog stays at least this far away from player

    [Header("Follow Settings")]
    public float rotationSpeed = 10f;       // How smoothly the dog turns
    public float followDistance = 6f;       // Max distance before the dog runs to the player
    public float comfortDistance = 3f;      // Comfortable range where dog can wander

    [Header("Wander Settings")]
    public float wanderRadius = 2f;         // Radius around the player to wander
    public float wanderSpeed = 1f;          // Wandering speed
    public float wanderChangeInterval = 2f; // How often to pick a new wander spot
    private bool hasWanderTarget = false;   // whether the dog has a wander target
    public float idleChance = 0.4f;         // Chance to go idle instead of moving
    public float minIdleTime = 1f;          // Minimum idle duration
    public float maxIdleTime = 3f;          // Maximum idle duration

    [Header("Ground Settings")]
    public LayerMask groundLayer;           // Assign your ground layer here
    public float groundOffset = 0.1f;       // Small offset to avoid clipping

    private bool isFollowing = false;       // Starts following after trigger
    private Vector3 wanderTarget;
    private float stateTimer;
    private bool isIdle;

    private GameSaveManager saveManager;
    void Start()
    {
        // find the GameSaveManager in the scene
        saveManager = FindAnyObjectByType<GameSaveManager>();
    }

    void Update()
    {
        if (!isFollowing || player == null)
        {
            KeepDogGrounded();
            return;
        }

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > followDistance)
        {
            // FOLLOW mode
            FollowPlayer();
        }
        else
        {
            // WANDER or IDLE mode
            if (isIdle)
            {
                HandleIdle();
            }
            else
            {
                WanderAroundPlayer();
            }
        }

        // Always keep the dog on the ground
        KeepDogGrounded();
    }

    private void FollowPlayer()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        // Scale speed with distance
        float t = Mathf.InverseLerp(comfortDistance, followDistance, distance);
        float currentSpeed = Mathf.Lerp(minFollowSpeed, maxFollowSpeed, t);
        animator.SetFloat("DogSpeed", currentSpeed);

        // Only move if farther than stopDistance
        if (distance > stopDistance)
        {
            // Calculate direction (ignore vertical component to stop tilting)
            Vector3 direction = (player.position - transform.position).normalized;
            direction.y = 0;

            // Position to move toward (keeps dog at comfortDistance away)
            Vector3 movePosition = player.position - direction * comfortDistance;

            // Move Dog
            transform.position = Vector3.MoveTowards(transform.position, movePosition, currentSpeed * Time.deltaTime);

            // Rotate dog to face movement direction
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
    }

    private void WanderAroundPlayer()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        // If dog is too far, switch to follow
        if (distance > followDistance)
        {
            FollowPlayer();
            return;
        }

        // If too close, just stop and face the player (respect stopDistance)
        if (distance < stopDistance)
        {
            Vector3 faceDir = (player.position - transform.position).normalized;
            faceDir.y = 0;
            if (faceDir != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(faceDir);
                targetRotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            animator.SetFloat("DogSpeed", 0.0f);
            return;
        }

        // Countdown the state timer
        stateTimer -= Time.deltaTime;

        // If no wander target or timer expired, pick new behavior
        if (!hasWanderTarget || stateTimer <= 0f)
        {
            if (Random.value < idleChance)
            {
                // Switch to idle
                isIdle = true;
                stateTimer = Random.Range(minIdleTime, maxIdleTime);
                animator.SetFloat("DogSpeed", 0.0f);
                return;
            }

            // Pick a new wander point around the player
            Vector2 randomCircle = Random.insideUnitCircle * wanderRadius;
            wanderTarget = player.position + new Vector3(randomCircle.x, 0, randomCircle.y);

            hasWanderTarget = true;
            stateTimer = wanderChangeInterval;
        }

        // Move toward wander target
        Vector3 direction = (wanderTarget - transform.position).normalized;
        direction.y = 0;

        transform.position = Vector3.MoveTowards(transform.position, wanderTarget, wanderSpeed * Time.deltaTime);
        animator.SetFloat("DogSpeed", wanderSpeed);

        // Rotate only on Y axis
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            targetRotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // If reached wander target, clear it so a new one is chosen later
        if (Vector3.Distance(transform.position, wanderTarget) < 0.5f)
        {
            hasWanderTarget = false;
            stateTimer = wanderChangeInterval;
        }
    }

    private void HandleIdle()
    {
        stateTimer -= Time.deltaTime;
        if (stateTimer <= 0)
        {
            isIdle = false; // Back to wandering
        }
        animator.SetFloat("DogSpeed", 0.0f);
    }

    private void KeepDogGrounded()
    {
        RaycastHit hit;
        // Cast a ray from slightly above the dog, down to the ground
        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hit, 5f, groundLayer))
        {
            Vector3 pos = transform.position;
            pos.y = hit.point.y + groundOffset; // snap to ground
            transform.position = pos;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isFollowing = true;
            saveManager.Save("checkpoint");
            Debug.Log("Checkpoint reached! Game saved.");
            GetComponent<Collider>().enabled = false; // disables the trigger after use
        }
    }
}
