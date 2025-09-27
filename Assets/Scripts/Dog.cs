using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Dog : MonoBehaviour
{
    [Header("Target to Follow")]
    public Transform player;
    private NavMeshAgent agent;

    public GameObject CheckpointUI;
    public Texture2D cursorTexture;

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

    public static bool isFollowing = false;       // Starts following after trigger
    private Vector3 wanderTarget;
    private float stateTimer;
    private bool isIdle;

    private GameSaveManager saveManager;
    public AudioSource DogBark;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = comfortDistance; // Dog keeps this much distance from player

        // find the GameSaveManager in the scene
        saveManager = FindAnyObjectByType<GameSaveManager>();
        DogBark.enabled = false;
        CheckpointUI.SetActive(false);

        if (isFollowing && GetComponent<Collider>().enabled)
        {
            GetComponent<Collider>().enabled = false;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && DogBark.enabled)
        {
            DogBark.Play();
        }

        if (!isFollowing || player == null)
        {
            return;
        }

        if (isFollowing && GetComponent<Collider>().enabled)
        {
            GetComponent<Collider>().enabled = false;
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
    }

    private void FollowPlayer()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        // Scale speed with distance
        float t = Mathf.InverseLerp(comfortDistance, followDistance, distance);
        float currentSpeed = Mathf.Lerp(minFollowSpeed, maxFollowSpeed, t);

        agent.speed = currentSpeed;
        agent.SetDestination(player.position);

        animator.SetFloat("DogSpeed", currentSpeed);
    }

    private void WanderAroundPlayer()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > followDistance)
        {
            FollowPlayer();
            return;
        }

        if (distance < stopDistance)
        {
            agent.ResetPath(); // stop moving
            animator.SetFloat("DogSpeed", 0.0f);
            return;
        }

        stateTimer -= Time.deltaTime;

        if (!hasWanderTarget || stateTimer <= 0f)
        {
            if (Random.value < idleChance)
            {
                isIdle = true;
                stateTimer = Random.Range(minIdleTime, maxIdleTime);
                animator.SetFloat("DogSpeed", 0.0f);
                agent.ResetPath();
                return;
            }

            // Pick a new wander point on the NavMesh
            Vector2 randomCircle = Random.insideUnitCircle * wanderRadius;
            Vector3 target = player.position + new Vector3(randomCircle.x, 0, randomCircle.y);

            NavMeshHit hit;
            if (NavMesh.SamplePosition(target, out hit, 2f, NavMesh.AllAreas))
            {
                wanderTarget = hit.position;
                agent.SetDestination(wanderTarget);
                agent.speed = wanderSpeed;
                animator.SetFloat("DogSpeed", wanderSpeed);
            }

            hasWanderTarget = true;
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isFollowing = true;
            saveManager.Save("checkpoint");
            Debug.Log("Checkpoint reached! Game saved.");
            GetComponent<Collider>().enabled = false; // disables the trigger after use
            CheckpointUI.SetActive(true);
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            // set the custom cursor again
            Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
        }
    }
}
