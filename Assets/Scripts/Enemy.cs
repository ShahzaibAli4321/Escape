//using UnityEngine;
//using UnityEngine.SceneManagement;

//public class Enemy : MonoBehaviour
//{
//    [Header("References")]
//    public Transform player;
//    public Animator animator;

//    [Header("Sound")]
//    public AudioSource audioSource;     // The AudioSource component
//    public AudioClip firstClip;         // Plays once
//    public AudioClip loopingClip;       // Plays in loop after first clip
//    private bool firstClipPlayed = false;

//    [Header("Detection Settings")]
//    public float detectionRadius = 40f;   // how far the enemy can "see"
//    [Range(0, 360)]
//    public float viewAngle = 90f;         // FOV angle
//    public float reactionDelay = 1.5f;    // wait before chasing
//    public LayerMask obstructionMask;     // assign walls/obstacles here

//    [Header("Chase Settings")]
//    public float chaseSpeed = 7f;
//    public float rotationSpeed = 5f;
//    public float catchDistance = 1f;

//    [Header("Ground Settings")]
//    public LayerMask groundLayer;           // Assign your ground layer here
//    public float groundOffset = 0.1f;       // Small offset to avoid clipping

//    private bool playerSpotted = false;
//    public bool isChasing = false;
//    private float chaseTimer = 0f;

//    void Start()
//    {
//        audioSource.loop = false; // first sound should not loop
//        audioSource.clip = firstClip;
//    }

//    void Update()
//    {
//        KeepEnemyGrounded();
//        animator.SetBool("isChasing", isChasing);

//        if (player == null) return;

//        float distance = Vector3.Distance(transform.position, player.position);

//        if (!isChasing)
//        {
//            if (CanSeePlayer(distance))
//            {
//                if (!playerSpotted)
//                {
//                    Debug.Log("Enemy spotted the player! Waiting before chasing...");
//                    playerSpotted = true;
//                    chaseTimer = reactionDelay;
//                }

//                // Count down before chasing
//                if (playerSpotted)
//                {
//                    chaseTimer -= Time.deltaTime;
//                    if (chaseTimer <= 0f)
//                    {
//                        isChasing = true;
//                        audioSource.Play();
//                        Debug.Log("Enemy starts chasing!");
//                    }
//                }
//            }
//            else
//            {
//                // Reset if player leaves detection range or FOV
//                playerSpotted = false;
//                isChasing = false;

//            }
//        }

//        if (isChasing)
//        {
//            ChasePlayer();
//            if (!audioSource.isPlaying && !firstClipPlayed)
//            {
//                firstClipPlayed = true;

//                audioSource.clip = loopingClip;
//                audioSource.loop = true;    // this one loops
//                audioSource.Play();
//            }

//            // Check for Game Over
//            if (distance <= catchDistance)
//            {
//                Debug.Log("Game Over! Enemy caught the player.");
//                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
//            }

//            // Optional: give up if player is too far away
//            if (distance > detectionRadius * 1.5f)
//            {
//                Debug.Log("Enemy gave up chasing.");
//                isChasing = false;
//                playerSpotted = false;

//                audioSource.Stop();
//                audioSource.clip = firstClip;
//                audioSource.loop = false;
//                firstClipPlayed = false;
//            }
//        }
//    }

//    private bool CanSeePlayer(float distance)
//    {
//        if (distance > detectionRadius) return false;

//        // Check FOV angle
//        Vector3 directionToPlayer = (player.position - transform.position).normalized;
//        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

//        if (angleToPlayer > viewAngle / 2f) return false;

//        // Check if there's an obstacle between enemy and player
//        if (Physics.Raycast(transform.position + Vector3.up, directionToPlayer, distance, obstructionMask))
//        {
//            return false; // blocked by wall
//        }

//        return true;
//    }

//    private void ChasePlayer()
//    {
//        Vector3 direction = (player.position - transform.position).normalized;
//        direction.y = 0;

//        transform.position += direction * chaseSpeed * Time.deltaTime;

//        if (direction != Vector3.zero)
//        {
//            Quaternion targetRotation = Quaternion.LookRotation(direction);
//            targetRotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);
//            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
//        }
//    }

//    private void KeepEnemyGrounded()
//    {
//        RaycastHit hit;
//        // Cast a ray from slightly above the dog, down to the ground
//        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hit, 5f, groundLayer))
//        {
//            Vector3 pos = transform.position;
//            pos.y = hit.point.y + groundOffset; // snap to ground
//            transform.position = pos;
//        }
//    }
//}


using UnityEngine;
using UnityEngine.SceneManagement;

public class Enemy : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Animator animator;

    [Header("Sound")]
    public AudioSource audioSource;     // The AudioSource component
    public AudioClip firstClip;         // Plays once
    public AudioClip loopingClip;       // Plays in loop after first clip
    private bool firstClipPlayed = false;
    private static Enemy soundOwner = null; // The enemy currently making sound


    [Header("Detection Settings")]
    public float detectionRadius = 40f;   // how far the enemy can "see"
    [Range(0, 360)]
    public float viewAngle = 90f;         // FOV angle
    public float reactionDelay = 1.5f;    // wait before chasing
    public LayerMask obstructionMask;

    [Header("Chase Settings")]
    public float chaseSpeed = 7f;
    public float rotationSpeed = 5f;
    public float catchDistance = 1f;
    public float innerChaseRadius = 20f;  // Inner lock-on chase radius
    public float reDetectionDelay = 3f;   // Wait before detecting again

    [Header("Ground Settings")]
    public LayerMask groundLayer;
    public float groundOffset = 0.1f;

    private bool playerSpotted = false;
    public bool isChasing = false;
    private bool inInnerChase = false;
    private float chaseTimer = 0f;
    private float reDetectionTimer = 0f;

    void Start()
    {
        audioSource.loop = false;
        audioSource.clip = firstClip;
    }

    void Update()
    {
        KeepEnemyGrounded();
        animator.SetBool("isChasing", isChasing);

        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (reDetectionTimer > 0f)
        {
            reDetectionTimer -= Time.deltaTime;

            // When cooldown finishes, reset the state so detection works again
            if (reDetectionTimer <= 0f)
            {
                playerSpotted = false;
                isChasing = false;
                inInnerChase = false;
            }
            return; // Still ignore the player during cooldown
        }


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

                // Countdown before chasing
                if (playerSpotted)
                {
                    chaseTimer -= Time.deltaTime;
                    if (chaseTimer <= 0f)
                    {
                        isChasing = true;
                        inInnerChase = false;
                        Debug.Log("Enemy starts chasing (outer chase mode)!");

                        if (soundOwner == null)
                        {
                            audioSource.Play(); // play first clip
                            soundOwner = this;  // claim ownership
                        }
                    }
                }
            }
            else
            {
                playerSpotted = false;
                isChasing = false;
                inInnerChase = false;
                soundOwner = null;
            }
        }

        if (isChasing)
        {
            ChasePlayer();

            // Switch audio from firstClip → loopingClip
            if (!audioSource.isPlaying && !firstClipPlayed && soundOwner == this)
            {
                firstClipPlayed = true;
                audioSource.clip = loopingClip;
                audioSource.loop = true;
                audioSource.Play();
            }
            else if (!audioSource.isPlaying && !firstClipPlayed && soundOwner != this)
            {
                firstClipPlayed = true;
                audioSource.clip = loopingClip;
                audioSource.loop = true;
                audioSource.Play();
                soundOwner = this;
            }

            // Game Over
            if (distance <= catchDistance)
            {
                Debug.Log("Game Over! Enemy caught the player.");
                FindAnyObjectByType<EnemyJumpscare>().TriggerJumpscare(transform);
            }

            // Switch into inner chase if within 20 units
            if (!inInnerChase && distance <= innerChaseRadius)
            {
                inInnerChase = true;
                Debug.Log("Enemy locked into inner chase mode!");
            }

            // Stop conditions
            if (!inInnerChase && distance > detectionRadius)
            {
                // Outer chase, give up if past 40
                Debug.Log("Enemy gave up chasing (player escaped past 40).");
                StopChase();
            }
            else if (inInnerChase && distance > innerChaseRadius)
            {
                // Inner chase, give up if past 20
                Debug.Log("Enemy lost player in inner chase. Cooling down...");
                StopChase();
                reDetectionTimer = reDetectionDelay; // wait before detecting again
            }
        }
    }

    private void StopChase()
    {
        isChasing = false;
        playerSpotted = false;
        inInnerChase = false;

        if (soundOwner == this) // only release if this enemy owned it
        {
            audioSource.Stop();
            audioSource.clip = firstClip;
            audioSource.loop = false;
            firstClipPlayed = false;
            soundOwner = null; // free up for others
        }
    }

    private bool CanSeePlayer(float distance)
    {
        if (distance > detectionRadius) return false;

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        if (angleToPlayer > viewAngle / 2f) return false;

        if (Physics.Raycast(transform.position + Vector3.up, directionToPlayer, distance, obstructionMask))
        {
            return false;
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
        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hit, 5f, groundLayer))
        {
            Vector3 pos = transform.position;
            pos.y = hit.point.y + groundOffset;
            transform.position = pos;
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Detection radius (outer chase)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // Inner chase radius
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, innerChaseRadius);

        // View angle lines
        Gizmos.color = Color.blue;
        Vector3 rightBoundary = Quaternion.Euler(0, viewAngle / 2f, 0) * transform.forward;
        Vector3 leftBoundary = Quaternion.Euler(0, -viewAngle / 2f, 0) * transform.forward;
        Gizmos.DrawRay(transform.position, rightBoundary * detectionRadius);
        Gizmos.DrawRay(transform.position, leftBoundary * detectionRadius);
    }
}
