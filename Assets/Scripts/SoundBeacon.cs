using UnityEngine;
using UnityEngine.AI;

public class SoundBeacon : MonoBehaviour
{
    public Transform player;      // Drag Player here
    public Transform dog;         // Drag Dog here
    public float desiredDistance = 5f;   // Distance beacon keeps from player
    public float updateRate = 0.5f;      // How often NavMesh target updates

    private NavMeshAgent agent;
    private AudioSource beaconAudio;
    private float timer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        beaconAudio = GetComponent<AudioSource>();
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
