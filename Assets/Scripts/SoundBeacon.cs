using UnityEngine;
using UnityEngine.AI;

public class SoundBeacon : MonoBehaviour
{
    public Transform player;      // Drag Player here
    public Transform dog;         // Drag Dog here
    public float desiredDistance = 5f;   // Distance beacon keeps from player
    public float updateRate = 0.5f;      // How often NavMesh target updates
    private AudioLowPassFilter lowPass;
    public LayerMask wallMask;           // set to your "Walls" layer

    private NavMeshAgent agent;
    private AudioSource beaconAudio;
    private float timer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        beaconAudio = GetComponent<AudioSource>();
        // Add a low-pass filter if not already present
        lowPass = gameObject.AddComponent<AudioLowPassFilter>();
        lowPass.cutoffFrequency = 22000; // no effect initially
    }

    void Update()
    {
        // Start the beacon sound
        if (beaconAudio != null && Input.GetKeyDown(KeyCode.F) && !beaconAudio.isPlaying)
        {
            beaconAudio.Play();
        }

        // Recalculate beacon position toward dog every "updateRate" seconds
        timer += Time.deltaTime;
        if (timer >= updateRate)
        {
            timer = 0f;

            // Direction from player to dog
            Vector3 direction = (dog.position - player.position).normalized;

            // Ideal position is desiredDistance units toward the dog
            Vector3 targetPos = player.position + direction * desiredDistance;

            // Use NavMeshAgent to pathfind
            agent.SetDestination(targetPos);
        }

        // === Occlusion Check ===
        Vector3 toPlayer = player.position - transform.position;
        if (Physics.Raycast(transform.position, toPlayer.normalized, out RaycastHit hit, toPlayer.magnitude, wallMask))
        {
            // Wall in the way → muffled
            lowPass.cutoffFrequency = 1000;
            beaconAudio.volume = 0.5f;
        }
        else
        {
            // Clear line of sight → normal
            lowPass.cutoffFrequency = 22000;
            beaconAudio.volume = 1f;
        }
    }

    // Called from DogTrigger
    public void DisableBeacon()
    {
        if (beaconAudio != null && beaconAudio.isPlaying)
            beaconAudio.Stop();

        agent.isStopped = true;
        enabled = false; // disable this script
    }
}
