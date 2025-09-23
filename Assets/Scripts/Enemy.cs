using UnityEngine;
using UnityEngine.SceneManagement;

public class Enemy : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Animator animator;

    [Header("Detection Settings")]
    public float detectionRadius = 40f;   // how far the enemy can "see"
    [Range(0, 360)]
    public float viewAngle = 90f;         // FOV angle
    public float reactionDelay = 1.5f;    // wait before chasing
    public LayerMask obstructionMask;     // assign walls/obstacles here

    [Header("Chase Settings")]
    public float chaseSpeed = 7f;
    public float rotationSpeed = 5f;
    public float catchDistance = 1f;

    [Header("Ground Settings")]
    public LayerMask groundLayer;           // Assign your ground layer here
    public float groundOffset = 0.1f;       // Small offset to avoid clipping

    private bool playerSpotted = false;
    public bool isChasing = false;
    private float chaseTimer = 0f;

    void Update()
    {
        KeepEnemyGrounded();
        animator.SetBool("isChasing", isChasing);

        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (!isChasing)
        {
            if (CanSeePlayer(distance))
            {
                if (!playerSpotted)
                {
                    Debug.Log("Enemy spotted the player! Waiting before chasing...");
                    playerSpotted = true;
                    chaseTimer = reactionDelay;
                }

                // Count down before chasing
                if (playerSpotted)
                {
                    chaseTimer -= Time.deltaTime;
                    if (chaseTimer <= 0f)
                    {
                        isChasing = true;
                        Debug.Log("Enemy starts chasing!");
                    }
                }
            }
            else
            {
                // Reset if player leaves detection range or FOV
                playerSpotted = false;
                isChasing = false;
            }
        }

        if (isChasing)
        {
            ChasePlayer();

            // Check for Game Over
            if (distance <= catchDistance)
            {
                Debug.Log("Game Over! Enemy caught the player.");
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }

            // Optional: give up if player is too far away
            if (distance > detectionRadius * 1.5f)
            {
                Debug.Log("Enemy gave up chasing.");
                isChasing = false;
                playerSpotted = false;
            }
        }
    }

    private bool CanSeePlayer(float distance)
    {
        if (distance > detectionRadius) return false;

        // Check FOV angle
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        if (angleToPlayer > viewAngle / 2f) return false;

        // Check if there's an obstacle between enemy and player
        if (Physics.Raycast(transform.position + Vector3.up, directionToPlayer, distance, obstructionMask))
        {
            return false; // blocked by wall
        }

        return true;
    }

    private void ChasePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0;

        transform.position += direction * chaseSpeed * Time.deltaTime;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            targetRotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void KeepEnemyGrounded()
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
}
