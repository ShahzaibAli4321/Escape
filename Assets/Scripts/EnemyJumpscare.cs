using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class EnemyJumpscare : MonoBehaviour
{
    [Header("References")]
    public Transform playerCamera;       // The player's camera
    public Transform jumpscarePoint;     // Empty object in front of the camera
    public AudioSource audioSource;      // For jumpscare sound
    public AudioClip scareSound;         // Scary scream
    public Image flashImage;             // UI Image covering screen (transparent by default)

    [Header("Settings")]
    public float delayBeforeGameOver = 2f;
    public float shakeDuration = 0.5f;
    public float shakeMagnitude = 0.3f;
    public float flashDuration = 0.2f;

    public void TriggerJumpscare(Transform enemy)
    {
        StartCoroutine(JumpscareSequence(enemy));
    }

    private IEnumerator JumpscareSequence(Transform enemy)
    {
        // Disable enemy logic
        MonoBehaviour[] scripts = enemy.GetComponents<MonoBehaviour>();
        foreach (var script in scripts)
        {
            if (script != this) // make sure not to disable the jumpscare manager
                script.enabled = false;
        }

        Animator anim = enemy.GetComponent<Animator>();
        if (anim) anim.enabled = false;

        UnityEngine.AI.NavMeshAgent agent = enemy.GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent) agent.enabled = false;

        // Stop player movement
        GameObject player = GameObject.FindWithTag("Player");

        // Disable movement script
        var moveScript = player.GetComponent<FirstPersonMovement>();
        if (moveScript) moveScript.enabled = false;

        // Disable camera look script
        var lookScript = player.GetComponentInChildren<FirstPersonCamera>();
        if (lookScript) lookScript.enabled = false;

        // Disable animator (if player has one)
        Animator playerAnim = player.GetComponent<Animator>();
        if (playerAnim) playerAnim.enabled = false;

        // Snap enemy in front of camera
        enemy.position = jumpscarePoint.position;
        enemy.LookAt(playerCamera);
        enemy.Rotate(-50f, 0f, 0f); // tilt head upwards (negative X rotates up)

        // Play scream
        if (scareSound != null)
            audioSource.PlayOneShot(scareSound);

        // Flash screen red
        if (flashImage != null)
        {
            flashImage.color = new Color(1, 0, 0, 1);
            yield return new WaitForSeconds(flashDuration);
            flashImage.color = new Color(1, 0, 0, 0);
        }

        // Camera shake
        yield return StartCoroutine(ShakeCamera());

        // Wait before game over
        yield return new WaitForSeconds(delayBeforeGameOver);

        // Reload scene or call game over
        SceneManager.LoadScene(0);
    }

    private IEnumerator ShakeCamera()
    {
        Vector3 originalPos = playerCamera.localPosition;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;

            playerCamera.localPosition = originalPos + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        playerCamera.localPosition = originalPos;
    }
}
