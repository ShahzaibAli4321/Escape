using UnityEngine;

public class DOgSoundManager : MonoBehaviour
{
    public AudioSource dogAudio;
    public AudioSource whistle;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            whistle.Play();
            dogAudio.Play();
        }
    }
}
