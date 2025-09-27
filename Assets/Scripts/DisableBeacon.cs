using UnityEngine;

public class DisableBeacon : MonoBehaviour
{
    private Dog dog;
    private SoundBeacon beacon;
    private AudioSource dogAudio;

    void Start()
    {
        // Get references once
        dog = FindAnyObjectByType<Dog>();
        beacon = FindAnyObjectByType<SoundBeacon>();

        if (dog != null)
            dogAudio = dog.DogBark;

        // Disable dog's AudioSource at start
        if (dogAudio != null)
            dogAudio.enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Disable the beacon
            if (beacon != null)
                beacon.DisableBeacon();

            // Re-enable dog's AudioSource (bark logic in other script now works)
            if (dogAudio != null)
                dogAudio.enabled = true;
        }
    }
}
